using WebSocketSharp;

public static class Bank {
    private static List<Bitch> bitches = [];

    public static Bitch Join(string pass) {
        var bbb = new Bitch{
            Id = RandomString(1000),
            Money = 0,
            Password = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(pass.GetHashCode().ToString()))
        };

        bitches.Add(bbb);

        return bbb;
    }

    public static Bitch Get(string id) {
        var b = bitches.FirstOrDefault(x => x.Id == id);
        if(b is null) { return new Bitch{ Id = "" }; }

        return b;
    }

    public static double Money(string id) {
        var b = Get(id);
        return b.Money;
    }

    public static void Add(string id, double amount) {
        var b = Get(id);
        b.Money += amount;
    }

    public static bool Take(string id, double amount) {
        var b = Get(id);
        if(b.Money - amount < 0) {
            return false;
        }
        b.Money -= amount;

        return true;
    }

    public static bool CorrectPassword(string id, string inp) {
        var b = Get(id);
        var base64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(inp.GetHashCode().ToString()));
        return base64 == b.Password;
    }

    private static Random random = new Random();

    public static string RandomString(int length) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

public class BankMessageManager : WebSocketSharp.Server.WebSocketBehavior {
    private static readonly string key = "fx30";

    // 0 -> cmd
    // 1 -> server key
    // 3 -> optional ( id )
    // 2 -> optional ( client key )
    protected override void OnMessage(MessageEventArgs e) {
        Console.WriteLine($"{e.Data}");

        var inp = e.Data.ToLower().Split(" ");
        var cmd = inp[0];

        string id = "";
        string[] rest = [];

        // Server Key
        if(inp.Length <= 2) return;
        if(inp[1] != key) return;

        // must have id
        if(inp.Length >= 4) {
            id = Bank.RandomString(1_000);

            // must have client key
            if(!Bank.CorrectPassword(id, inp[3])) return;

            rest = inp[4..];
        } else {
            rest = inp[2..];
        }

        Action run = e.Data.ToLower().Split(" ")[0] switch {
            "new" => () => {
                var b = Bank.Join(rest[0]);
                Send(b.Id.ToString());
            },
            "money" => () => {
                var m = Bank.Money(id);
                Send(m.ToString());
            },
            "send" => () => {
                string account = rest[0];
                if(!int.TryParse(rest[1], out int amount)) {
                    return;
                }

                if(Bank.Take(id, amount)) {
                    Bank.Add(account, amount);
                }

                Send(Bank.Money(id).ToString());
            },
            "deposit" => () => {
                if(!int.TryParse(rest[0], out int amount)) return;

                Bank.Add(id, amount);

                Send(Bank.Money(id).ToString());
            },
            "withdraw" => () => {
                if(!int.TryParse(rest[0], out int amount)) return;

                if(Bank.Take(id, amount)) {
                    Send(Bank.Money(id).ToString());
                }
            },
            _ => () => {},
        };

        run();
    }
}

public class Bitch {
    public string Id = "";
    public double Money;
    public string Password = "";
}
