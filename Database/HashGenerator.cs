using System;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = "123456";
            int workFactor = 11;
            
            for (int i = 1; i <= 25; i++)
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor);
                Console.WriteLine($"    ('{i}','$2a$11${hash.Substring(7)}'),");
            }
            
        }
    }
}
