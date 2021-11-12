namespace TiraxTech.Notion.Models

[<NoComparison>]
type DeleteRequest = {
    BlockIds: string seq
    PermanentlyDelete: bool
}
