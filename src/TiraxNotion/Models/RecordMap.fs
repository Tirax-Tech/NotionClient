namespace TiraxTech.Notion.Models

open System.Text.Json

type UserPermission = {
    role: string
    ``type``: string
    user_id: string
}

type PermissionGroup = {
    id: string
    name: string
    user_ids: string list
}

type Space = {
    id: string
    version: int
    name: string
    permissions: UserPermission list
    permission_group: PermissionGroup list
    icon: string
    beta_enabled: bool
    pages: string list
    disable_public_access: bool
    disable_guests: bool
    disable_move_to_space: bool
    disable_export: bool
    plan_type: string
    invite_link_enabled: bool
}

type PermissionItem<'T> = {
    role: string
    value: 'T
}

type PermissionObjects<'T> = Map<string, PermissionItem<'T>>
