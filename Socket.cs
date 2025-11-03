using WebSocketSharp;

public class SocketManager : WebSocketSharp.Server.WebSocketBehavior {
    public static readonly IEnumerable<(Card.Suit, Card.Rank)> deck = Card.GetDeck(1);

    protected override void OnMessage(MessageEventArgs e) {
        Send(Card.StringFromCard(Card.RandomFromDeck(deck)));
    }
}
