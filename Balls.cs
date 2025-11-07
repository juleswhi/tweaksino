public class Balls : WebSocketSharp.Server.WebSocketBehavior {
    protected override void OnOpen() {
        Send("kys");
    }
}
