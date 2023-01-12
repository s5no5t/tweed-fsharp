module Tweed.Web.Tests

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open System
open System.Net
open Xunit

[<Fact>]
let ``My test`` () =
    task {
        // -- Arrange
        let webBuilder =
            WebHostBuilder()
                .ConfigureServices(App.configureServices)
                .Configure(Action<IApplicationBuilder> App.configureApp)

        let testServer = new TestServer(webBuilder)

        let client = testServer.CreateClient()

        // -- Act
        let! response = client.GetAsync "/"

        // -- Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode)
    }
