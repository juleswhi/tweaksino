using WebSocketSharp;

public static class Bank {
    public static Bitch Join(string pass) {
        var bbb = new Bitch{
            Id = RandomString(6),
            Money = 0,
            Password = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(pass))
        };

        DatabaseLayer.Create(bbb);

        return bbb;
    }

    public static Bitch Get(string id) {
        var bitches = DatabaseLayer.Query<Bitch>();
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
        DatabaseLayer.Update(b);
    }

    public static bool Take(string id, double amount) {
        var b = Get(id);
        if(b.Money - amount < 0) {
            return false;
        }
        b.Money -= amount;

        DatabaseLayer.Update(b);
        return true;
    }

    public static bool CorrectPassword(string id, string inp) {
        var b = Get(id);
        var base64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(inp));
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
        var inp = e.Data.Split(" ");
        var cmd = inp[0];

        string id = "";
        string[] rest = [];

        // Server Key
        if(inp.Length <= 2) return;
       if(inp[1] != key) {
           return;
       }

        // must have id
        if(inp.Length >= 4) {
            id = inp[2];

            // must have client key
            if(!Bank.CorrectPassword(id, inp[3])) {
                return;
            }

            rest = inp[4..];
        } else {
            rest = inp[2..];
        }

        Action run = e.Data.ToLower().Split(" ")[0] switch {
            "new" => () => {
                var b = Bank.Join(rest[0]);
                var id = new string(b.Id.Where(x => x != '\'').ToArray());
                Send(id);
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
            "orders" => () => {
                var orders = DatabaseLayer
                    .Query<ShopOrder>()
                    .Where(x => x.UserId.Id == id)
                    .Select(x => $"{x.Id} {x.McItemId} {x.Quantity}")
                    .Aggregate((x, y) => $"{x},{y}");
                Send(orders);
            },
            "add" => () => {
                if(rest.Length != 2) return;
                var item_name = rest[0];
                if(!int.TryParse(rest[1], out int quantity)) return;

                var shop = DatabaseLayer.Query<Shop>().Where(x => x.Id == 0).First();

                var item = DatabaseLayer
                    .Query<McItem>()
                    .Where(x => x.Id == item_name)
                    .FirstOrDefault();

                // TODO: Smart pricing here
                if(item is null) {
                    item = new McItem() {
                        Id = $"{item_name}",
                        Price = 0.5,
                    };

                    DatabaseLayer.Create(item);
                    Console.WriteLine($"Creating item: {item}");
                } else {
                    Console.WriteLine($"{item}");
                }

                var shop_item = DatabaseLayer
                    .Query<ShopItem>()
                    .Where(x => x.McItemId == item.Id)
                    .FirstOrDefault();

                if(shop_item is null) {
                    var order = new ShopItem() {
                        ShopId = shop.Id,
                        McItemId = item.Id,
                        Quantity = quantity,
                    };

                    DatabaseLayer.Create(order);
                    return;
                }

                shop_item.Quantity += quantity;
                DatabaseLayer.Update(shop_item);

            },
            "collect" => () => {
                if(!int.TryParse(rest[0], out int order_id)) return;
                var order = DatabaseLayer
                    .Query<ShopOrder>()
                    .FirstOrDefault(x => x.Id == order_id);

                if(order is null) return;

                order.Collected = true;

                DatabaseLayer.Update(order);
            },
            _ => () => {},
        };

        run();
    }
}

public class Bitch : IDatabaseModel {
    [PrimaryKey]
    public string Id { get; set; } = "";
    public double Money { get; set; } = 0;
    public string Password { get; set; } = "";
}

public class Shop : IDatabaseModel {
    [PrimaryKey]
    public int Id { get; set; } = 0;
}

public class McItem : IDatabaseModel {
    [PrimaryKey]
    public string Id { get; set; } = "";
    public double Price { get; set; } = 1;
}

public class ShopItem : IDatabaseModel {
    [PrimaryKey]
    [ForeignKey(typeof(Shop))]
    public int ShopId { get; set; }

    [PrimaryKey]
    [ForeignKey(typeof(McItem))]
    public string McItemId { get; set; } = "";

    public int Quantity { get; set; }
}

public class ShopOrder : IDatabaseModel {
    [PrimaryKey]
    public int Id { get; set; }

    [ForeignKey(typeof(Shop))]
    public int ShopId { get; set; } = new();

    [ForeignKey(typeof(McItem))]
    public string McItemId { get; set; } = "";

    [ForeignKey(typeof(Bitch))]
    public int UserId { get; set; } = new();

    public int Quantity { get; set; }
    public bool Collected { get; set; } = false;
}

