using System;
using System.IO;
using System.Reflection;

namespace PExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("<<< Usage >>>");
                Console.WriteLine("PExtract <root folder to extract from>");
                return;
            }

            var now = DateTime.Now;
            var dest = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, $"{now.Year}.{now.Month}.{now.Day} {now.Hour}.{now.Minute}.{now.Second} Extracted" );
            var extractor = new PhotoExtractor(new DirectoryInfo(args[0]), dest);

            foreach(var info in extractor.Extraction)
            {
                if (info.Error == null)
                    ; //Console.WriteLine($"{info.Source.FullName} -> {info.Destination.Name}");
                else
                    Console.WriteLine($"Error while extracting {info.Source.FullName}. Message : {info.Error.Message}");
            }
        }
    }
}
