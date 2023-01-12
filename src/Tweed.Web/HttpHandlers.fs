module Tweed.Web.HttpHandlers

open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.EndpointRouting
open Tweed.Data
open ViewModels

module Index =
    let indexGetHandler =
        let tweedViewModel = { Content = "Some tweed" }
        let indexViewModel = { Tweeds = [ tweedViewModel ] }
        let view = Views.Index.indexGetView indexViewModel
        htmlView view

module Tweed =
    [<CLIMutable>]
    type CreateTweedDto = { Text: string }

    let postCreateTweedHandler documentStore =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            task {
                let! tweedDto = ctx.BindFormAsync<CreateTweedDto>()
                let session = RavenDb.createSession documentStore
                do! session |> Queries.storeTweed tweedDto.Text
                do! RavenDb.saveChangesAsync session

                return! (redirectTo false "/") next ctx
            }

    let getCreateTweedHandler =
        let view = Views.Tweed.createTweedView None
        htmlView view

let endpoints documentStore =
    [ GET [ route "/" Index.indexGetHandler ]
      subRoute
          "/tweed"
          [ GET [ route "/create" Tweed.getCreateTweedHandler ]
            POST [ route "/create" (Tweed.postCreateTweedHandler documentStore) ] ] ]
