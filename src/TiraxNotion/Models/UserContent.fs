namespace TiraxTech.Notion.Models

open System.Collections.Generic
open System.Text.Json.Serialization

type User = {
    Id: string
    Version: int
    Email: string
    ProfilePhoto: string
    OnboardingCompleted: bool
    MobileOnboardingCompleted: bool
    Name: string
}

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

type UserSettings = {
    Fbp: string
    Type: string
    Persona: string
    ClickIds: {| Id: string; Type: string |}
    TeamRole: string
    TimeZone: string
    SignupTime: int64
    PreferredLocale: string
    UsedAndroidApp: bool
    UsedWindowsApp: bool
    StartDayOfWeek: int
    UsedDesktopWebApp: bool
    PreferredLocaleOrigin: string
    SeenCommentSidebarV2: bool
    SeenFileAttachmentIntro: bool
}

type UserSettingItem = {
    Id: string
    Version: int
    Settings: UserSettings
}

type PermissionItem<'T> = {
    Role: string
    Value: 'T
}

type PermissionObjects<'T> = Map<string, PermissionItem<'T>>

type UserContentRecordMap = {
    NotionUser: PermissionObjects<User>
    Space: PermissionObjects<Space>
    UserSettings: PermissionObjects<UserSettingItem>
}

type UserContent = {
    [<JsonPropertyName("recordMap")>] RecordMap: UserContentRecordMap
}
