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

// Result classes

type SearchedDocument = {
    id: string
    isNavigable: bool
    score: double
    spaceId: string
    source: string
}

[<NoComparison>]
type SearchRecordMap = {
    block: JsonElement
    space: PermissionObjects<Space>
    collection: JsonElement
}

[<NoComparison>]
type SearchResults = {
    results: SearchedDocument list
    total: int
    recordMap: SearchRecordMap
}
