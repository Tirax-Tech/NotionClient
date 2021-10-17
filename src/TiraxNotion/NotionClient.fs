/// Notion client that uses web version of the API
module TiraxTech.NotionClient

open System.Text.Json
open O9d.Json.Formatting
open Http
open TiraxTech.Notion.Models

let private BaseUri = Uri.Https.Host("www.notion.so")
let private ApiBaseUri = BaseUri.ChangePath("/api/v3/")
let private SignedUrlPrefix = BaseUri.ChangePath("/signed")
let private S3UrlPrefix = Uri.Https.Host("s3-us-west-2.amazonaws.com").ChangePath("secure.notion-static.com")
let private S3UrlPrefixEncoded = Uri.Https.Host("s3.us-west-2.amazonaws.com").ChangePath("secure.notion-static.com")

let private DefaultSerializerOptions = JsonSerializerOptions(
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy=JsonSnakeCaseNamingPolicy()
    )

/// Notion client's context.  All members are subjected to change in the future.
type NotionContext(tokenV2: string) =
    let headers = [| Cookie ("token_v2", tokenV2) |]

    member _.PostRaw(endpoint, ?data) =
        let url = ApiBaseUri.ChangePath endpoint
        let request = HTTP.Post(url.ToSystemUri()).WithHeaders(headers)
        let poster = match data with
                     | Some o -> request.WithJson o
                     | None -> request
        poster.Respond()

    member my.Post<'Data>(endpoint, ?data) =
        task {
            use! res = my.PostRaw(endpoint, ?data = data)
            return! res |> jsonResponseWithOptions<'Data>(DefaultSerializerOptions)
        }

    member inline my.LoadUserContent<'T>() = my.Post<'T>("loadUserContent")
    member inline my.LoadUserContent() = my.LoadUserContent<UserContent>()
    
/// <summary>
/// Create NotionContext with V2 token.
/// </summary>
/// <param name="tokenV2"></param>
let createV2 tokenV2 = NotionContext tokenV2
