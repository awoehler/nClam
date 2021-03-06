﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace nClam.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args == null || args.Length != 1)
            {
                Console.WriteLine("Invalid arguments.  Usage: nClam.ConsoleTest [FileName]");
                return;
            }

            var fileInfo = new FileInfo(args[0]);

            var client = new ClamClient("10.0.130.86", 3310);

            Console.WriteLine("GetVersion(): {0}", client.GetVersion());
            Console.WriteLine("GetPing(): {0}", client.Ping());

            ClamServerStats stats = new ClamServerStats(client.ServerStats());
            Console.WriteLine("ServerStats(): POOLS:           {0}", stats.pools); // clamStats["POOLS"]);
            Console.WriteLine("ServerStats(): STATE:           {0}", stats.state); // clamStats["STATE"]);
            Console.WriteLine("ServerStats(): THREADS:         {0}", stats.threads.live); // clamStats["THREADS"]);
            Console.WriteLine("ServerStats(): QUEUE:           {0}", stats.queueSize); // clamStats["QUEUE"]);
            Console.WriteLine("ServerStats(): MEMSTATS(used):  {0}", stats.memstats.used ); // clamStats["MEMSTATS"]);

            if (!fileInfo.Exists)
            {
                Console.WriteLine("{0} could not be found.  Exiting.", fileInfo.FullName);
                return;
            }
            
            Console.WriteLine("ScanFileOnServer(): {0}", client.ScanFileOnServer(fileInfo.FullName));
            Console.WriteLine("ScanFileOnServerMultithreaded(): {0}", client.ScanFileOnServerMultithreaded(fileInfo.FullName));

            if (!IsFolder(fileInfo.FullName))
            {
                Console.WriteLine("SendAndScanFile(string): {0}", client.SendAndScanFile(fileInfo.FullName));
                Console.WriteLine("SendAndScanFile(byte[]): {0}", client.SendAndScanFile(File.ReadAllBytes(fileInfo.FullName)));
            }
            else
            {
                Console.WriteLine("SendAndScanFile(): Not run because argument is a folder, not a file.");
            }
            Console.WriteLine("Finished, Press <enter> to quit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Returns true if the given file path is a folder.
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>True if a folder</returns>
        public static bool IsFolder(string path)
        {
            return ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory);
        }
    }
}
