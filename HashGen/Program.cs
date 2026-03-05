class Program
{
    static void Main(string[] args)
    {
        string hash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        Console.WriteLine(hash);
    }
}