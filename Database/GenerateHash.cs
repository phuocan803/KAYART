using System;
using BCrypt.Net;

namespace Database
{
    class GenerateHash
    {
        static void Main(string[] args)
        {
            string password = "123456";
            int workFactor = 11;
            string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor);
            Console.WriteLine($"BCrypt hash for '{password}' (work factor {workFactor}):");
            Console.WriteLine(hash);
        }
    }
}
