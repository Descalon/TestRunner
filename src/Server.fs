namespace TestRunner

open System
open System.Net
open System.Net.Sockets
open System.Threading

module Server =
    type Socket with
      member socket.AsyncAccept() = Async.FromBeginEnd(socket.BeginAccept, socket.EndAccept)
      member socket.AsyncReceive(buffer:byte[], ?offset, ?count) =
        let offset = defaultArg offset 0
        let count = defaultArg count buffer.Length
        let beginReceive(b,o,c,cb,s) = socket.BeginReceive(b,o,c,SocketFlags.None,cb,s)
        Async.FromBeginEnd(buffer, offset, count, beginReceive, socket.EndReceive)
      member socket.AsyncSend(buffer:byte[], ?offset, ?count) =
        let offset = defaultArg offset 0
        let count = defaultArg count buffer.Length
        let beginSend(b,o,c,cb,s) = socket.BeginSend(b,o,c,SocketFlags.None,cb,s)
        Async.FromBeginEnd(buffer, offset, count, beginSend, socket.EndSend)

    type Server() =
      static member Start(hostname:string, ?port) =
        let ipAddress = Dns.GetHostEntry(hostname).AddressList.[0]
        Server.Start(ipAddress, ?port = port)

      static member Start(?ipAddress, ?port) =
        let ipAddress = defaultArg ipAddress IPAddress.Any
        let port = defaultArg port 80
        let endpoint = IPEndPoint(ipAddress, port)
        let cts = new CancellationTokenSource()
        let listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        listener.Bind(endpoint)
        listener.Listen(int SocketOptionName.MaxConnections)
        printfn "Started listening on port %d" port
        
        let rec loop() = async {
          printfn "Waiting for request ..."
          let! socket = listener.AsyncAccept()
          // Setting ownsSocket to false allows us to later re-use a socket.
          let stream = new NetworkStream(socket, false) 
          printfn "Received request"
          let count = 1028
          let rec fn (acc:byte[]) (stream:NetworkStream) = 
            match stream.DataAvailable with
            | false -> acc
            | true ->
              let getBuffer () = async { return! stream.AsyncRead count }
              let buffer = getBuffer() |> Async.RunSynchronously
              printfn "Buffer size = %i" buffer.Length
              let acc' = Array.concat [|acc; buffer|]
              
              fn acc' stream
            
          let result = fn (Array.zeroCreate 0) stream |> System.Text.Encoding.UTF8.GetString

          printfn "Received: %s" result
          let response = [|
            "HTTP/1.1 200 OK\r\n"B
            "Content-Type: text/plain\r\n"B
            "\r\n"B
            "Hello World!"B |] |> Array.concat
          try
            try
              let! bytesSent = stream.AsyncWrite(response)
              printfn "Sent response"
            with e -> printfn "An error occurred: %s" e.Message
          finally
            stream.Close()
            socket.Shutdown(SocketShutdown.Both)
            socket.Close()
          return! loop() }

        Async.Start(loop(), cancellationToken = cts.Token)
        { new IDisposable with member x.Dispose() = cts.Cancel(); listener.Close() }