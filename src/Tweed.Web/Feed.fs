module Tweed.Web.Feed

open Giraffe
open Giraffe.EndpointRouting
open Microsoft.AspNetCore.Http
open Tweed.Web.Views

module ViewModels =
    type IndexViewModel =
        { Tweeds: Tweed.ViewModels.TweedViewModel list }

module Views =
    open Giraffe.ViewEngine

    let indexGetView (model: ViewModels.IndexViewModel) =
        [ titlePartial ()
          yield! model.Tweeds |> List.map (fun t -> div [] [ str t.Content ]) ]
        |> layout

let indexGetHandler: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let tweedViewModel: Tweed.ViewModels.TweedViewModel = { Content = "Some tweed" }
        let indexViewModel: ViewModels.IndexViewModel = { Tweeds = [ tweedViewModel ] }
        let view = Views.indexGetView indexViewModel
        htmlView view next ctx

let endpoints documentStore = [ GET [ route "/" indexGetHandler ] ]
