namespace TiraxTech.Notion.Models

open System.Text.Json

type User = {
    id: string
    version: int
    email: string
    profile_photo: string
    onboarding_completed: bool
    mobile_onboarding_completed: bool
    name: string
}

type UserSettings = {
    fbp: string
    ``type``: string
    persona: string
    click_ids: {| id: string; ``type``: string |}
    team_role: string
    time_zone: string
    signup_time: int64
    preferred_locale: string
    used_android_app: bool
    used_windows_app: bool
    start_dayOf_week: int
    used_desktop_web_app: bool
    preferred_locale_origin: string
    seen_comment_sidebar_v2: bool
    seen_file_attachment_intro: bool
}

type UserSettingItem = {
    id: string
    version: int
    settings: UserSettings
}

[<NoComparison>]
type UserContentRecordMap = {
    notion_user: PermissionObjects<User>
    user_root: JsonElement
    user_settings: PermissionObjects<UserSettingItem>
    space_view: JsonElement
    block: JsonElement
    space: PermissionObjects<Space>
    collection: JsonElement
}

[<NoComparison>]
type UserContent = {
    recordMap: UserContentRecordMap
}
