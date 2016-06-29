namespace TromsoFP.Meetup1

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html


// modules are actually compiled to static classes in a .NET context
[<JavaScript>]
module Client = // tell WebSharper to compile the module to JavaScript
    // templatning type provider (google F# type provider for more info)
    // gives access to type safe html templates
    type IndexTemplate = Templating.Template<"index.html">

    // matching functions for advanced pattern matching (active pattern)
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
        // clear any text and cruft defined in the template
        JQuery.Of("#main").Empty().Ignore

        // create reactive, mutable variables
        // reactive variables are observed through views
        let rvInput = Var.Create ""
        let rvList = Var.Create List.Empty : Var<list<string>>

        // if the result is a palindrome or has TromsoFP in it,
        // store it in a reactive list variable
        let addResult  (_ : Dom.Element) (_ : Dom.Event) =
            let v = rvList.Value // read reactive value
            match rvInput.Value with
            | IsPalindrome s -> rvList.Value <- s :: v // append match to list and update reactive variable
            | HasTFP s -> rvList.Value <- s :: v
            | _ -> ()

        // generate text response to matching string patterns
        let actionText t x =
            match x with
            | IsPalindrome s -> t
            | HasTFP s -> t
            | _ -> ""

        // render table rows from list
        let resultRows (lst : list<string>) =
            let n = lst.Length
            List.zip [1..n] lst // enumerate list using zip
            |> List.map (fun (x, y) -> // apply (lambda) function to each element of the list, generating a new list
                tr [ // tr: function for representing and rendering table rows
                    td [text <| string x]
                    td [text y]
                ] :> Doc
            )

        // read input into reactive variables and respond to special cases
        let inputPart =
            div [
                p [ // paragraph
                    text "Enter text:"
                    Doc.Input [on.change addResult] rvInput // input box connected to reactive variable
                    textView <| rvInput.View.Map (actionText " [Press enter]") // reactive response to changes in rvInput
                ]
                hr [] // horizontal rule
                pAttr [ // paragraph with (dyamic/reactive) attribute(s)
                    attr.classDyn <| rvInput.View.Map (actionText "alert alert-danger")
                    ] [
                    text "You enter: "
                    textView <| rvInput.View.Map (fun (x  : string )-> x.ToUpper ())
                ]
                hr []
            ]

        // render reactive result table
        let resultPart =
            div [
                rvList.View
                |> Doc.BindView (fun lst -> // create reactive DOM element by transforming data in a reactive list
                    tableAttr
                        [ attr.``class`` "table table-striped" ] (
                            [
                                tr [
                                    th [text "No."]
                                    th [ text "Result"]
                                ]
                            ]
                            @ resultRows lst // generate list of table rows form lst and append to table
                        )
                    )
            ]

        // initialize the main template
        IndexTemplate.Main.Doc(
            Body = [
                h1 [text "TromsoFP: Meetup 1"]
                inputPart
                h4 [text "Palindromes and such"]
                resultPart
            ]
        )
        |> Doc.RunById "main" // enter reactive event loop, fire and forget
