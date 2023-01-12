module Tweed.Web.HttpHandlers

open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Giraffe
open Tweed.Data
open ViewModels

module Index =
    let indexGetHandler =
        let tweedViewModel = { Content = "Some tweed" }
        let indexViewModel = { Tweeds = [ tweedViewModel ] }
        let view = Views.Index.indexGetView indexViewModel
        htmlView view

    let handlers = GET >=> indexGetHandler

module Tweed =
    [<CLIMutable>]
    type CreateTweedDto = { Text: string }

    let storeTweedHandler session =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! tweedDto = ctx.BindFormAsync<CreateTweedDto>()
                
                do! session |> Queries.storeTweed tweedDto.Text
                
                return! next ctx
            }

    let tweedPostHandler session = storeTweedHandler session >=> redirectTo false "/"

    let createTweedGetHandler =
        let view = Views.Tweed.createTweedView None
        htmlView view

    let handlers session =
        route "/create"
        >=> choose [ POST >=> tweedPostHandler session; GET >=> createTweedGetHandler ]

module Fallback =
    let notFoundHandler = setStatusCode 404 >=> text "Not Found"


let handler session: HttpHandler =
    choose
        [ subRoute "/tweed" (Tweed.handlers session)
          route "/" >=> Index.handlers
          Fallback.notFoundHandler ]

let sessionHandler documentStore: HttpHandler =
    fun next ctx ->
        task {
            let session = RavenDb.createSession documentStore
            let result = (handler session) next ctx
            do! RavenDb.saveChangesAsync session
            return! result
        }
