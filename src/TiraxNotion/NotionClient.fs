/// Notion client that uses web version of the API
module TiraxTech.Notion.Client

open System.Text.Json
open System.Net
open System.Threading.Tasks
open TiraxTech
open TiraxTech.Http
open TiraxTech.Notion.Models
open System.Runtime.InteropServices

let private BaseUri = Uri.Https.Host("www.notion.so")
let private ApiBaseUri = BaseUri.ChangePath("/api/v3/")
let private SignedUrlPrefix = BaseUri.ChangePath("/signed")
let private S3UrlPrefix = Uri.Https.Host("s3-us-west-2.amazonaws.com").ChangePath("secure.notion-static.com")
let private S3UrlPrefixEncoded = Uri.Https.Host("s3.us-west-2.amazonaws.com").ChangePath("secure.notion-static.com")

let private CamelCase = JsonSerializerOptions(PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

let private postRaw headers endpoint serializer data =
    let url = ApiBaseUri.ChangePath endpoint
    let request = HTTP.Post(url.ToSystemUri()).WithHeaders(headers)
    let poster = match data with
                 | Some o -> request.WithJson(o, serializer |> Option.defaultValue CamelCase)
                 | None -> request
    poster.Respond()

let private post headers endpoint serializer data =
    task {
        use! res = postRaw headers endpoint serializer data
        if res.IsSuccessStatusCode then
            return! res |> jsonResponse
        else
            let! content = textResponse res
            return (raise <| HttpRequestUnhandled content)
    }

type NotionClientResult<'Out> = Task<struct (HttpStatusCode * 'Out)>

/// Notion client's context.  All members are subjected to change in the future.
type NotionClient(tokenV2: string) =
    let headers = [| Http.Cookie ("token_v2", tokenV2) |]
    let myPost endpoint serializer data = post headers endpoint serializer data 

    member _.PostRaw(endpoint, ?data) = postRaw headers endpoint data

    member _.Post<'In,'Out>(endpoint, serializer, ?data: 'In) = post headers endpoint serializer data

    member _.LoadUserContent<'Out>() :NotionClientResult<'Out> = myPost "loadUserContent" None None

    member inline my.LoadUserContent() = my.LoadUserContent<UserContent>()

    member _.Search<'Query,'Out>(query: 'Query, ?serializer) :NotionClientResult<'Out> = myPost "search" serializer (Some query)
    member inline my.Search(query: SearchQuery, ?serializer) = my.Search<SearchQuery,SearchResults>(query, ?serializer = serializer)

    member _.DeleteBlocks<'Out>(blockIds, [<Optional; DefaultParameterValue(false)>] permanent: bool, ?serializer) :NotionClientResult<'Out> =
        myPost "deleteBlocks" serializer (Some { BlockIds=blockIds; PermanentlyDelete=permanent })

    member my.DeleteBlocks(blockIds, [<Optional; DefaultParameterValue(false)>] permanent: bool, ?serializer) :Task<HttpStatusCode> =
        task {
            let! struct (status,_) = my.DeleteBlocks<obj>(blockIds, permanent, ?serializer = serializer)
            return status
        }
    
module NotionClient =
    /// <summary>
    /// Create NotionContext with V2 token.
    /// </summary>
    /// <param name="tokenV2"></param>
    let createV2 tokenV2 = NotionClient tokenV2
