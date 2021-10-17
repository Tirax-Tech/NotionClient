module TiraxTech.Http

open System
open System.Threading.Tasks
open System.Net
open System.Net.Http
open System.Net.Http.Json
open System.Text.Json

type HttpStatusCode with
    member me.IsInformational = let code = int(me) in code >= 100 && code < 200
    member me.IsOk = let code = int(me) in code >= 200 && code < 300
    member me.IsRedirection = let code = int(me) in code >= 300 && code < 400
    member me.IsClientError = let code = int(me) in code >= 400 && code < 500
    member me.IsServerError = let code = int(me) in code >= 500 && code < 600

type SameSiteCookie = Strict | Lax | Nothing

type SetCookieBody = {
    Key: string
    Value: string
    Expires: DateTime option
    MaxAge: int option
    Domain: string option
    Path: string option
    Secure: bool
    HttpOnly: bool
    SameSite: SameSiteCookie option
}

type HttpHeaders =
| Cookie of string * string

let private http = HttpClient <| HttpClientHandler(UseCookies=false)

let private setHeader (req: HttpRequestMessage) = function
| Cookie (key, value) -> req.Headers.TryAddWithoutValidation("Cookie", $"{key}={value}") |> ignore

let inline private respondWith ([<InlineIfLambda>] getter: HttpContent -> Task<'Response>) (res: HttpResponseMessage) =
    task {
        use _ = res
        let! text = res.Content |> getter
        return struct (res.StatusCode, text)
    }

let DefaultCamelSerializerOptions = JsonSerializerOptions()

exception HttpRequestUnhandled of HttpStatusCode * string

/// Read content from HttpResponseMessage as JSON and deserialize to 'Response.
// JsonSerializerOptions -> HttpResponseMessage -> Task<struct (HttpStatusCode * 'Response)>
let jsonResponseWithOptions<'Response> (opt: JsonSerializerOptions) (res: HttpResponseMessage) =
    if not res.IsSuccessStatusCode then
        raise <| HttpRequestUnhandled (res.StatusCode, $"Status code is not OK! (%A{res.StatusCode})")
    else
        res |> respondWith (fun content -> content.ReadFromJsonAsync<'Response>(opt))

(* HttpResponseMessage -> Task<struct (HttpStatusCode * 'Response)> *)
let inline jsonResponse<'Response> = jsonResponseWithOptions<'Response> <| DefaultCamelSerializerOptions

(* HttpResponseMessage -> Task<struct (HttpStatusCode * string)> *)
let inline textResponse res = res |> respondWith (fun content -> content.ReadAsStringAsync())

[<NoComparison>]
type HTTP = {
    Method: HttpMethod
    Uri: Uri
    Headers: HttpHeaders seq
    Content: HttpContent option
}
with
    static member Post uri = {
        Method=HttpMethod.Post
        Uri=uri
        Headers=[]
        Content=None
    }
    member me.WithHeaders([<ParamArray>] headers: HttpHeaders[]) = { me with Headers=headers }
    member me.WithJson body = { me with Content=Some (JsonContent.Create body) }
    member my.Respond() =
        let req = HttpRequestMessage(my.Method, my.Uri)
        my.Headers |> Seq.iter (setHeader req)
        if my.Content.IsSome then
            req.Content <- my.Content |> Option.get
        http.SendAsync(req)

    member my.RespondAsJson<'Data>() =
        task {
            use! res= my.Respond()
            return! res |> jsonResponse<'Data>
        }

    member my.RespondAsString() =
        task {
            use! res = my.Respond()
            return! res.Content.ReadAsStringAsync()
        }
