using System;
using System.Collections.Generic;
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

            long specificSize = 0;
            int s = 0, e = 0;
            Random rnd = new Random();
            if (sizeStr.Contains("~"))
            {
                s = int.Parse(Regex.Match(sizeStr.Trim().Split('~')[0], @"\d+").Value);
                e = int.Parse(Regex.Match(sizeStr.Trim().Split('~')[1], @"\d+").Value);
            }
            else
                specificSize = long.Parse(Regex.Match(sizeStr, @"\d+").Value);

            Console.WriteLine("Generate to?");
            string dir = Console.ReadLine();

            Parallel.For(0, howMany,
                   i =>
                   {
                       long actualSize = s != 0 ? MultipleByUnit(new Random(i).Next(s, e), sizeStr) : MultipleByUnit(specificSize, sizeStr);
                       long howManyPiece = 0, remainder = 0;
                       int arrayMaxSize = 1024 * 1024;
                       if (actualSize > arrayMaxSize)
                       {
                           howManyPiece = actualSize / arrayMaxSize;
                           remainder = actualSize % arrayMaxSize;
                       }
                       using (FileStream fileStream = new FileStream($@"{dir}\dfg{i}.dat", FileMode.Create, FileAccess.Write))
                       {
                           Random rng = new Random(Guid.NewGuid().GetHashCode());

                           for (int j = 0; j < howManyPiece; j++)
                           {
                               byte[] temp = new byte[arrayMaxSize];
                               rnd.NextBytes(temp);
                               fileStream.Write(temp, 0, arrayMaxSize);
                           }
                           byte[] temp2 = new byte[remainder];
                           rnd.NextBytes(temp2);
                           fileStream.Write(temp2, 0, temp2.Length);
                       }

                       string sha256AsFileName;
                       using (FileStream fsSource = new FileStream($@"{dir}\dfg{i}.dat", FileMode.Open, FileAccess.Read))
                       {
                           sha256AsFileName = ComputeSha256Hash(fsSource);
                       }
                       File.Move($@"{dir}\dfg{i}.dat", $@"{dir}\{sha256AsFileName.ToUpper()}.dat");
                   });

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        private static string ComputeSha256Hash(Stream rawData)
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
