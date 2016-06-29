namespace UINextApplication2

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

[<JavaScript>]
module Client =
    type IndexTemplate = Templating.Template<"index.html">

    let (|IsPalindrome|_|) s =
        let r = s |> Array.ofSeq |> Array.rev |> Array.fold (fun a s -> a + string s) ""
        if s = r && s.Length > 2
        then Some r
        else None

    let (|HasTFP|_|) (s : string) =
        if s.Contains "TromsoFP"
        then Some s
        else None

    let Main =
        JQuery.Of("#main").Empty().Ignore

        let rvInput = Var.Create ""
        let rvList = Var.Create List.Empty : Var<list<int * string>>

        let doStuff  (_ : Dom.Element) (_ : Dom.Event) =
            match rvInput.Value with
            | IsPalindrome s ->
                let v = rvList.Value
                let (n, _) = if v.Length = 0 then (0, "") else v.[v.Length - 1]
                rvList.Value <- (n+1, s) :: v
            | HasTFP s ->
                let v = rvList.Value
                let (n, _) = v.[v.Length - 1]
                rvList.Value <- (n+1, s) :: v
            | _ -> ()

        IndexTemplate.Main.Doc(
            Body = [
                h1 [text "Hello TromsoFP!"]
                p [
                    text "Enter text:"
                    Doc.Input [on.change doStuff] rvInput
                ]
                pAttr [
                    attr.classDyn <| rvInput.View.Map (fun x ->
                        match x with
                        | IsPalindrome s -> "alert alert-danger"
                        | _ -> ""
                    )
                    ] [
                    text "You enter:"
                    textView <| rvInput.View.Map (fun (x  : string )-> x.ToUpper ())
                ]
                div [
                    rvList.View
                    |> Doc.BindView (fun lst ->
                        tableAttr [
                            attr.``class`` "table table-striped"
                            ] ([
                                tr [th [text "#"]; th [ text "special"]]
                            ]
                            @ (lst |>
                                List.map (fun (x, y) ->
                                    tr [
                                        td [text <| string x]
                                        td [text y]
                                    ] :> Doc
                                )))
                        )
                ]
            ]
        )
        |> Doc.RunById "main"
