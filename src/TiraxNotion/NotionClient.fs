/// Notion client that uses web version of the API
module TiraxTech.NotionClient

open System.Text.Json
open System.Threading.Tasks
open O9d.Json.Formatting
open Http
open TiraxTech.Notion.Models
open System.Net

let private BaseUri = Uri.Https.Host("www.notion.so")
let private ApiBaseUri = BaseUri.ChangePath("/api/v3/")
let private SignedUrlPrefix = BaseUri.ChangePath("/signed")
let private S3UrlPrefix = Uri.Https.Host("s3-us-west-2.amazonaws.com").ChangePath("secure.notion-static.com")
let private S3UrlPrefixEncoded = Uri.Https.Host("s3.us-west-2.amazonaws.com").ChangePath("secure.notion-static.com")

let SnakeCase = JsonSerializerOptions(
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy=JsonSnakeCaseNamingPolicy()
    )
let CamelCase = JsonSerializerOptions(PropertyNameCaseInsensitive = true, PropertyNamingPolicy=JsonNamingPolicy.CamelCase)

let private postRaw headers endpoint data =
    let url = ApiBaseUri.ChangePath endpoint
    let request = HTTP.Post(url.ToSystemUri()).WithHeaders(headers)
    let poster = match data with
                 | Some o -> request.WithJson(o, CamelCase)
                 | None -> request
    poster.Respond()

let private post headers endpoint serializer data =
    task {
        use! res = postRaw headers endpoint data
        return! res |> jsonResponseWithOptions<'Out>(serializer)
    }

let private postWithDefault defaultSerializer headers endpoint serializer data =
    post headers endpoint (serializer |> Option.defaultValue defaultSerializer) data

let private postSnakeCase headers endpoint serializer data = postWithDefault SnakeCase headers endpoint serializer data
let private postCamelCase headers endpoint serializer data = postWithDefault CamelCase headers endpoint serializer data

/// Notion client's context.  All members are subjected to change in the future.
type NotionContext(tokenV2: string) =
    let headers = [| Http.Cookie ("token_v2", tokenV2) |]
    let postDefaultSnake endpoint serializer data = postSnakeCase headers endpoint serializer data
    let postDefaultCamel endpoint serializer data = postCamelCase headers endpoint serializer data

    member _.PostRaw(endpoint, ?data) = postRaw headers endpoint data

    member _.Post<'In,'Out>(endpoint, serializer, ?data: 'In) = post headers endpoint serializer data

    member _.LoadUserContent<'Out>(?serializer) :Task<struct (HttpStatusCode * 'Out)> = postDefaultSnake "loadUserContent" serializer None

    member inline my.LoadUserContent() = my.LoadUserContent<UserContent>()

    member _.Search<'Query,'Out>(query: 'Query, ?serializer) :Task<struct (HttpStatusCode * 'Out)> = postDefaultCamel "search" serializer (Some query)
    member inline my.Search<'Out>(query: SearchQuery, ?serializer) = my.Search<SearchQuery,'Out>(query, ?serializer = serializer)
    
/// <summary>
/// Create NotionContext with V2 token.
/// </summary>
/// <param name="tokenV2"></param>
let createV2 tokenV2 = NotionContext tokenV2
