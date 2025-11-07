var sv = new WebSocketSharp.Server.WebSocketServer("ws://127.0.0.1:6969");

sv.AddWebSocketService<BankMessageManager>("/bank");
"Registered Bank Service".Info();

sv.AddWebSocketService<BlackjackJoinManager>("/blackjack/join");
"Registered Join Manager".Info();
sv.AddWebSocketService<BlackjackCardsManager>("/blackjack/cards");
"Registered Cards Manager".Info();
sv.AddWebSocketService<BlackjackHitManager>("/blackjack/hit");
"Registered Hit Manager".Info();
sv.AddWebSocketService<BlackjackStandManager>("/blackjack/stand");
"Registered Stand Manager".Info();

sv.Start();

"Started Server".Info();
$"Listening On ws://{sv.Address.ToString()}:{sv.Port}".Debug();

Console.ReadKey(true);
sv.Stop();
