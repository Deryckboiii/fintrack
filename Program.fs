open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.FileProviders
open System.IO
open Giraffe
open FinTrack
open FinTrack.Storage
open FinTrack.Database

let webApp () =
    choose [
        GET >=> choose [
            route "/" >=> htmlFile "public/index.html"
            route "/transactions" >=> fun next ctx -> task {
                let fromDate = ctx.TryGetQueryStringValue("from")
                let toDate = ctx.TryGetQueryStringValue("to")
                let txs = Database.getAllTransactions ()
                return! json txs next ctx
            }
            route "/summary" >=> fun next ctx ->
                let data = Database.getSummaryByCategory()
                json data next ctx
            route "/summary/month" >=> fun next ctx ->
                let data = Database.getMonthlySummary()
                json data next ctx
            route "/summary/week" >=> fun next ctx ->
                let weekly = Database.getWeeklySummary()
                json weekly next ctx
        ]
        POST >=> choose [
            route "/transactions" >=> fun next ctx -> task {
                let! tx = ctx.BindJsonAsync<Transaction>()
                let saved = Database.addTransaction tx
                return! json saved next ctx
            }
        ]
        DELETE >=> choose [
            routef "/transactions/%s" (fun id -> fun next ctx ->
                if Database.deleteTransaction id then
                    text $"Transaction {id} deleted" next ctx
                else
                    RequestErrors.NOT_FOUND $"Transaction {id} not found" next ctx)
          
        ]
    ]

let configureApp (app: IApplicationBuilder) =
    app.UseStaticFiles(StaticFileOptions(
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "public")),
        RequestPath = PathString("")))

    app.UseGiraffe(webApp())

let configureServices (services: IServiceCollection) =
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main args =
    Database.ensureDatabase()
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .Configure(configureApp)
                .ConfigureServices(configureServices)
                |> ignore)
        .Build()
        .Run()
    0