namespace TiraxTech.Notion.Models

open System.Text.Json

type UserPermission = {
    Role: string
    Type: string
    UserId: string
}

type PermissionGroup = {
    Id: string
    Name: string
    UserIds: string list
}

type Space = {
    Id: string
    Version: int
    Name: string
    Permissions: UserPermission list
    PermissionGroup: PermissionGroup list
    Icon: string
    BetaEnabled: bool
    Pages: string list
    DisablePublicAccess: bool
    DisableGuests: bool
    DisableMoveToSpace: bool
    DisableExport: bool
    PlanType: string
    InviteLinkEnabled: bool
}

type PermissionItem<'T> = {
    Role: string
    Value: 'T
}

type PermissionObjects<'T> = Map<string, PermissionItem<'T>>

type RecordMapBase(space: PermissionObjects<Space>, block: JsonElement, collection: JsonElement) =
    member _.Space = space
    member _.Block = block
    member _.Collection = collection

[<NoComparison>]
type RecordMap<'T when 'T :> RecordMapBase> = {
    RecordMap: 'T
}
