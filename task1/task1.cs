using System;
using System.Text;

public static class Program
{
    public static bool IsPowerOfTwo(long value)
    {
        return value > 0 && (value & (value - 1)) == 0;
    }

    public static string Reverse(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var characters = text.ToCharArray();
        Array.Reverse(characters);
        return new string(characters);
    }

    public static string Replicate(string text, int count)
    {
        if (string.IsNullOrEmpty(text) || count <= 0)
        {
            return string.Empty;
        }

        var buffer = new StringBuilder(text.Length * count);
        for (var i = 0; i < count; i++)
        {
            buffer.Append(text);
        }

        return buffer.ToString();
    }

    public static void PrintOddNumbers()
    {
        for (var i = 1; i <= 100; i += 2)
        {
            Console.WriteLine(i);
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine(IsPowerOfTwo(8));
        Console.WriteLine(IsPowerOfTwo(9));
        Console.WriteLine(Reverse("Hello"));
        Console.WriteLine(Replicate("Hi", 3));
        PrintOddNumbers();
    }
}
