module Tweed.Web.Feed

open Giraffe
open Microsoft.AspNetCore.Http
open Tweed.Web.Views

let indexGetHandler: HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let tweedViewModel: ViewModels.TweedViewModel = { Content = "Some tweed" }
        let indexViewModel: ViewModels.IndexViewModel = { Tweeds = [ tweedViewModel ] }
        let view = Index.indexGetView indexViewModel
        htmlView view next ctx
