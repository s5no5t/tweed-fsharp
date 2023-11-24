module Tweed.Web.Tweed

open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.EndpointRouting
open Tweed.Data
open Tweed.Web.Views

module Views =
    open Giraffe.ViewEngine

    let createTweedView text =
        let value =
            match text with
            | Some(t) -> t
            | None -> ""

        [ titlePartial ()
          form
              [ _method "POST"; _upSubmit; _upLayer "parent" ]
              [ label [ _for "text" ] []
                textarea [ _rows "5"; _name "Text"; _value value ] []
                button [ _type "submit" ] [ encodedText "Submit" ] ] ]
        |> layout


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
    let view = Views.createTweedView None
    htmlView view

let endpoints documentStore =
    [ subRoute
          "/tweed"
          [ GET [ route "/create" getCreateTweedHandler ]
            POST [ route "/create" (postCreateTweedHandler documentStore) ] ] ]
