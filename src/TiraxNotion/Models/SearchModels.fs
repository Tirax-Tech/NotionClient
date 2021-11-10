namespace TiraxTech.Notion.Models

open System.Text.Json

[<NoComparison>]
type SearchQueryFilter = {
    IsDeletedOnly: bool
    ExcludeTemplates: bool
    IsNavigableOnly: bool
    RequireEditPermissions: bool
    Ancestors: string list
    CreatedBy: string list
    EditedBy: string list
    LastEditedTime: obj
    CreatedTime: obj
}

[<NoComparison>]
type SearchQuery = {
    Type: string
    Query: string
    Filters: SearchQueryFilter
    Sort: string
    Limit: int
    SpaceId: string
    Source: string
}

type SearchedDocument = {
    Id: string
    IsNavigable: bool
    Score: double
    SpaceId: string
    Source: string
}

[<NoComparison>]
type SearchResults = {
    Results: SearchedDocument list
    Total: int
    RecordMap: RecordMap<RecordMapBase>
}
