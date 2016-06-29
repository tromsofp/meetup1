namespace TromsoFP.Meetup1

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
        let rvList = Var.Create List.Empty : Var<list<string>>

        let addResult  (_ : Dom.Element) (_ : Dom.Event) =
            let v = rvList.Value
            match rvInput.Value with
            | IsPalindrome s -> rvList.Value <- s :: v
            | HasTFP s -> rvList.Value <- s :: v
            | _ -> ()

        let actionText t x =
            match x with
            | IsPalindrome s -> t
            | HasTFP s -> t
            | _ -> ""

        let resultRows (lst : list<string>) =
            let n = lst.Length
            List.zip [1..n] lst
            |> List.map (fun (x, y) ->
                tr [
                    td [text <| string x]
                    td [text y]
                ] :> Doc
            )

        IndexTemplate.Main.Doc(
            Body = [
                h1 [text "TromsoFP: Meetup 1"]
                p [
                    text "Enter text:"
                    Doc.Input [on.change addResult] rvInput
                    textView <| rvInput.View.Map (actionText " [Press enter]")
                ]
                hr []
                pAttr [
                    attr.classDyn <| rvInput.View.Map (actionText "alert alert-danger")
                    ] [
                    text "You enter: "
                    textView <| rvInput.View.Map (fun (x  : string )-> x.ToUpper ())
                ]
                hr []
                h4 [text "Palindromes and such"]
                div [
                    rvList.View
                    |> Doc.BindView (fun lst ->
                        tableAttr
                            [ attr.``class`` "table table-striped" ] (
                                [
                                    tr [
                                        th [text "#"]
                                        th [ text "Result"]
                                    ]
                                ]
                                @ resultRows lst
                            )
                        )
                ]
            ]
        )
        |> Doc.RunById "main"
