using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DummyFileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("How many?");
            int howMany = int.Parse(Console.ReadLine());

            Console.WriteLine("Size?");
            string sizeStr = Console.ReadLine();

            long actualSize = 0;
            int s = 0, e = 0;
            Random rnd = new Random();
            if (sizeStr.Contains("~"))
            {
                s = int.Parse(Regex.Match(sizeStr.Trim().Split('~')[0], @"\d+").Value);
                e = int.Parse(Regex.Match(sizeStr.Trim().Split('~')[1], @"\d+").Value);
            }
            else
                actualSize = long.Parse(Regex.Match(sizeStr, @"\d+").Value);

            Console.WriteLine("Generate to?");
            string dir = Console.ReadLine();

            Parallel.For(0, howMany,
                   i =>
                   {
                       byte[] data = new byte[s != 0 ? MultipleByUnit(new Random(i).Next(s, e), sizeStr) : MultipleByUnit(actualSize, sizeStr)];
                       Random rng = new Random(Guid.NewGuid().GetHashCode());
                       rng.NextBytes(data);
                       var sha256AsFileName = ComputeSha256Hash(data);
                       File.WriteAllBytes($@"{dir}\{sha256AsFileName.ToUpper()}.dat", data);
                   });

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        private static string ComputeSha256Hash(byte[] rawData)
        {
            // Create a SHA256   
            using (SHA256CryptoServiceProvider sha256Hash = new SHA256CryptoServiceProvider())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(rawData);

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static long MultipleByUnit(long actualSize, string sizeStr)
        {
            if (sizeStr.Contains("K") || sizeStr.Contains("k"))
            {
                actualSize *= 1024;
            }
            else if (sizeStr.Contains("M") || sizeStr.Contains("m"))
            {
                actualSize *= 1024 * 1024;
            }
            else if (sizeStr.Contains("G") || sizeStr.Contains("g"))
            {
                actualSize *= 1024 * 1024 * 1024;
            }

            return actualSize;
        }
    }
}
