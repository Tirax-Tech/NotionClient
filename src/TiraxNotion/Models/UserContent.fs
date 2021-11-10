namespace TiraxTech.Notion.Models

open System.Collections.Generic
open System.Text.Json.Serialization
open System.Text.Json

type User = {
    Id: string
    Version: int
    Email: string
    ProfilePhoto: string
    OnboardingCompleted: bool
    MobileOnboardingCompleted: bool
    Name: string
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

[<NoComparison>]
type UserContentRecordMap = {
    NotionUser: PermissionObjects<User>
    UserRoot: JsonElement
    UserSettings: PermissionObjects<UserSettingItem>
    Space: PermissionObjects<Space>
    SpaceView: JsonElement
    Block: JsonElement
    Collection: JsonElement
}

[<NoComparison>]
type UserContent = {
    [<JsonPropertyName("recordMap")>] RecordMap: UserContentRecordMap
}
