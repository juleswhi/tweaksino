var socket = new SocketManager();

var sv = new WebSocketSharp.Server.WebSocketServer("ws://127.0.0.1:6969");
sv.AddWebSocketService<SocketManager>("/");
sv.Start();
Console.WriteLine("started server");
Console.ReadKey(true);
sv.Stop();
