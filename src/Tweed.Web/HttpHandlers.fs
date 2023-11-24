module Tweed.Web.HttpHandlers

open Giraffe.EndpointRouting
open Tweed.Web.Feed
open Tweed.Web.Views

let endpoints documentStore =
    [ GET [ route "/" indexGetHandler ]
      subRoute
          "/tweed"
          [ GET [ route "/create" Tweed.getCreateTweedHandler ]
            POST [ route "/create" (Tweed.postCreateTweedHandler documentStore) ] ] ]
