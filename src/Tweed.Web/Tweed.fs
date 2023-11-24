module Tweed.Web.Tweed

open Microsoft.AspNetCore.Http
open Giraffe
open Tweed.Data
open Tweed.Web.Views

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

let getCreateTweedHandler: HttpHandler =
    let view = Tweed.createTweedView None
    htmlView view
