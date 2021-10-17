module TiraxTech.Option

type Option<'T> with
    member my.Get() = Option.get my

    member inline my.IfSome([<InlineIfLambda>] f) =
        if my.IsSome then
            f <| my.Get()
