# Introduction #

A .NET Notion client inspired from https://github.com/axyyu/notion-clear-trash and https://github.com/jamalex/notion-py/.

# How to get Token V2 #

1. Go to notion.so
2. Open developer tools (hit F12)
3. Navigate to the Application tab (may be hidden if the developer window is small)
4. Expand Cookies under the Storage section on the sidebar
5. Click on 'https://www.notion.so' to view all the cookies
6. Copy the value for the key 'token_v2'

# Sample #

```fsharp
open System.Net
open TiraxTech.Notion.Client
open TiraxTech.Notion.Models
open TiraxTech.Http

let getTrash (client: NotionClient) =
    task {
        let! (_, userInfo) = client.LoadUserContent()
        let spaceId = (userInfo.recordMap.space.Keys |> Seq.head)
        printfn "Space ID = %s" spaceId
        let query = { 
            Type = "BlocksInSpace"
            Query = ""
            Filters = {
                IsDeletedOnly = true
                ExcludeTemplates = false
                IsNavigableOnly = true
                RequireEditPermissions = false
                Ancestors = []
                CreatedBy = []
                EditedBy = []
                LastEditedTime = obj()
                CreatedTime = obj()
            }
            Sort = "Relevance"
            Limit = 1000
            SpaceId = spaceId
            Source = "trash"
        }
        let! (status, search) = client.Search(query)
        assert (status = HttpStatusCode.OK)
        let blockIds = search.results
        return [for block in blockIds -> block.id]
    }

let client = NotionClient.createV2 "(YOUR COOKIE)"

let t = task {
    try
        let! blockIds = client |> getTrash
        printfn "Block IDs = %A" blockIds
        let! status = client.DeleteBlocks(blockIds, true)
        printfn "Status = %A" status
    with
    | HttpRequestUnhandled (_,_) as e -> printfn "%A" e
}

t.Wait()
```
