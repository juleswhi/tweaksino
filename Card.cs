public static class Card {
    public static (Suit, Rank) GetCard(int s, int r) {
        return ((Suit)s, (Rank)r);
    }

    public enum Suit {
        Diamonds,
        Clubs,
        Spades,
        Hearts
    }

    public enum Rank {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public static IEnumerable<(Suit, Rank)> GetDeck(int shoe) {
        var cards = Enumerable.Range(0, 4 * shoe).Select(suit =>
                Enumerable.Range(0, 13).Select(rank =>
                    Card.GetCard(suit % shoe, rank)));

        return cards.Aggregate([], (IEnumerable<(Card.Suit, Card.Rank)> a, IEnumerable<(Card.Suit, Card.Rank)> x) => a.Concat(x));
    }

    public static (Suit, Rank) RandomFromDeck(IEnumerable<(Suit, Rank)> deck) =>
        deck.ElementAt(new Random().Next() % deck.Count());

    public static string StringFromCard((Suit, Rank) card) =>
        $"{card.Item2.ToString()} of {card.Item1.ToString()}";

    public static List<int> Value(Rank r) =>
        r switch {
            Rank.Two => [2],
            Rank.Three => [3],
            Rank.Four => [4],
            Rank.Five => [5],
            Rank.Six => [6],
            Rank.Seven => [7],
            Rank.Eight => [8],
            Rank.Nine => [9],
            Rank.Ten => [10],
            Rank.Jack => [10],
            Rank.Queen => [10],
            Rank.King => [10],
            Rank.Ace => [1, 11],
            _ => []
        };
}

public static class CardHelper {
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list){
        var r = new Random((int)DateTime.Now.Ticks);
        var shuffledList = list.Select(x => new { Number = r.Next(), Item = x }).OrderBy(x => x.Number).Select(x => x.Item);
        return shuffledList.ToList();
    }
}
