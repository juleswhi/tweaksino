using WebSocketSharp;

public static class BlackjackManager {
    public static Blackjack blackjack = new();
}

// refactor to use json

public class BlackjackJoinManager : WebSocketSharp.Server.WebSocketBehavior {
    protected override void OnOpen() {
        var id = BlackjackManager.blackjack.AddPlayer();
        $"Added Player {id}".Debug();
        Send(id.ToString());
    }
}

public class BlackJackData {
    public int id;
    public string[] cards;
}

public class BlackjackHitManager : WebSocketSharp.Server.WebSocketBehavior {
    protected override void OnMessage(MessageEventArgs e) {
        if(!int.TryParse(e.Data, out int id)) {
            return;
        }

        BlackjackManager.blackjack.Hit(id);

        var values = BlackjackManager.blackjack.Value(id);

        string val_str = $"{values[0].ToString()}";

        if(values.Count > 1) {
            val_str += $",{values[1].ToString()}";
        }

        Send(val_str);

        if(values.Min() > 21) {
            Send("bust");
        }
    }
}

public class BlackjackCardsManager : WebSocketSharp.Server.WebSocketBehavior {
    protected override void OnMessage(MessageEventArgs e) {
        if(!int.TryParse(e.Data, out int id)) {
            return;
        }

        var cards = BlackjackManager.blackjack.Cards(id)
                        .Aggregate("", (a, c) => a += $",{c}");

        Send(cards);
    }
}

public class BlackjackStandManager : WebSocketSharp.Server.WebSocketBehavior {
    protected override void OnMessage(MessageEventArgs e) {
        if(!int.TryParse(e.Data, out int id)) {
            return;
        }
    }
}
