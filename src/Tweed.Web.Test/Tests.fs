module Tweed.Web.Tests

open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open NSubstitute
open System
open System.Net
open System.Threading.Tasks
open System.IO
open Xunit

let assertFail msg = Assert.True(false, msg)

let assertFailf format args =
    let msg = sprintf format args
    Assert.True(false, msg)

let next: HttpFunc = Some >> Task.FromResult

module Index =

    [<Fact>]
    let ``GET / returns 200 OK`` () =
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

    [<Fact>]
    let ``indeGetHandler returns Some HttpContext`` () =
        let ctx = Substitute.For<HttpContext>()
        ctx.Response.Body <- new MemoryStream()

        let handler = HttpHandlers.Feed.indexGetHandler

        task {
            let! result = handler next ctx

            match result with
            | Some ctx -> ()
            | None -> assertFail "Unexpected None result"
        }

module Tweed =

    [<Fact>]
    let ``GET /tweed/create returns 200 OK`` () =
        task {
            // -- Arrange
            let webBuilder =
                WebHostBuilder()
                    .ConfigureServices(App.configureServices)
                    .Configure(Action<IApplicationBuilder> App.configureApp)

            let testServer = new TestServer(webBuilder)

            let client = testServer.CreateClient()

            // -- Act
            let! response = client.GetAsync "/tweed/create"

            // -- Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode)
        }

    [<Fact>]
    let ``POST /tweed/create doesn't return 404 NOT FOUND`` () =
        task {
            // -- Arrange
            let webBuilder =
                WebHostBuilder()
                    .ConfigureServices(App.configureServices)
                    .Configure(Action<IApplicationBuilder> App.configureApp)

            let testServer = new TestServer(webBuilder)

            let client = testServer.CreateClient()

            // -- Act
            let! response = client.PostAsync("/tweed/create", null)

            // -- Assert
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode)
        }

    [<Fact>]
    let ``postCreateTweedHandler redirects to /`` () =
        let ctx = Substitute.For<HttpContext>()
        let response = Substitute.For<HttpResponse>()
        ctx.Response.Returns(response) |> ignore

        let documentStore = Substitute.For<Raven.Client.Documents.IDocumentStore>()

        let handler = HttpHandlers.Tweed.postCreateTweedHandler documentStore

        task {
            let! result = handler next ctx

            match result with
            | Some _ -> response.Received().Redirect("/", false)
            | None -> assertFail "Unexpected None result"
        }

// TODO
// [<Fact>]
// let ``POST /tweed/create returns 200 OK`` () =
//     task {
//         // -- Arrange
//         let webBuilder =
//             WebHostBuilder()
//                 .ConfigureServices(App.configureServices)
//                 .Configure(Action<IApplicationBuilder> App.configureApp)

//         // webBuilder.ConfigureLogging(
//         //     fun l -> l.Services.AddSingleton(
//         //         fun sp -> new XUnitLoggerProvider(_testOutputHelper) |> ignore
//         //         )) |> ignore

//         let testServer = new TestServer(webBuilder)

//         let client = testServer.CreateClient()

//         let formVals =
//             [ "Text", "Test" ]
//             |> List.map (fun (x, y) -> new KeyValuePair<string, string>(x, y))

//         let content: FormUrlEncodedContent = new FormUrlEncodedContent(formVals)

//         // -- Act
//         let! response = client.PostAsync("/tweed/create", content)

//         // -- Assert
//         Assert.Equal(HttpStatusCode.OK, response.StatusCode)
//     }
