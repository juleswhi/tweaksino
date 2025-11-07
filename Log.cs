public static class Tea {
    public static void Debug(this string str) {
        var now = DateTime.Now;
        Console.Write($"{now.Hour}:{now.Minute} ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"DEBU ");
        Console.ResetColor();
        Console.WriteLine($"{str}");
    }

    public static void Warn(this string str) {
        var now = DateTime.Now;

        Console.Write($"{now.Hour}:{now.Minute} ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"WARN ");
        Console.ResetColor();
        Console.WriteLine($"{str}");
    }

    public static void Info(this string str) {
        var now = DateTime.Now;
        Console.Write($"{now.Hour}:{now.Minute} ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write($"INFO ");
        Console.ResetColor();
        Console.WriteLine($"{str}");
    }
}

