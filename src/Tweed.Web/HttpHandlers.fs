module Tweed.Web.HttpHandlers

let endpoints documentStore =
    Feed.endpoints documentStore @ Tweed.endpoints documentStore
