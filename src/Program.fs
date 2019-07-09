open System
open TestRunner.Server
open System.Threading

[<EntryPoint>]
let main argv =
    let disposable = Server.Start(port = 8090)
    let rec fn = function
    | "quit" -> disposable.Dispose()
    | _ ->
        let n = Console.ReadLine()
        fn n
    fn "" |> ignore
    
    0 // return an integer exit code
