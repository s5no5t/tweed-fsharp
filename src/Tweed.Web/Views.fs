module Tweed.Web.Views

open Giraffe.ViewEngine

module ViewModels =
    type TweedViewModel = { Content: string }

    type IndexViewModel = { Tweeds: TweedViewModel list }

let _upSubmit = flag "up-submit"
let _upLayer = attr "up-layer"

let layout (content: XmlNode list) =
    html
        []
        [ head
              []
              [ title [] [ encodedText "Tweed.Web" ]
                link [ _rel "stylesheet"; _type "text/css"; _href "/main.css" ] ]
          body [] [ yield! content; a [ _href "/tweed/create" ] [ encodedText "Create Tweed" ] ] ]

let titlePartial () = h1 [] [ encodedText "Tweed.Web" ]
