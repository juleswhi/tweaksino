public class Blackjack {

    public class Player {
        public required int id;
        public List<(Card.Suit, Card.Rank)> cards = [];
    }

    public List<(Card.Suit, Card.Rank)> Deck = Card.GetDeck(4).Shuffle().ToList();

    public List<Player> players = [];

    public int AddPlayer() {
        var num = players.Count == 0 ? 0 : players.Select(x => x.id).Max() + 1;
        players.Add(new Player { id = num });
        return num;
    }

    public void StartGame() {
        players.ForEach(x => {
                Console.WriteLine("Taking Two Cards");
                TakeCard(x);
                TakeCard(x);
        });
    }

    public void Hit(int id) {
        Player? player = players.FirstOrDefault(x => x.id == id);

        if(player is null)
            return;

        TakeCard(player);
    }

    public List<string> Cards(int id) {
        Player? player = players.FirstOrDefault(x => x.id == id);
        if(player is null) return [];

        return player.cards.Select(x => Card.StringFromCard(x)).ToList();
    }

    public List<int> Value(int id) {
        Player? player = players.FirstOrDefault(x => x.id == id);
        if(player is null) return [-1];

        var card_values = player.cards.Select(x => Card.Value(x.Item2));

        List<int> values = [card_values
                                .Select(x => x[0])
                                .Aggregate(0, (acc, rank) => acc + rank)];

        if(player.cards.Select(x => x.Item2).Contains(Card.Rank.Ace)) {
            values.Add(
                    card_values.Select(x => x.Count > 1 ? x[1] : x[0])
                        .Aggregate(0, (acc, rank) => acc + rank));
        }

        return values;
    }

    private void TakeCard(Player player) {
        var card = Deck.First();

        Deck = Deck.TakeLast(Deck.Count - 1).ToList();

        player.cards.Add(card);
    }
}
