using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Files;
using ManyConsole;
using Swan;
using Swan.Logging;

namespace Tinyserv
{
    class Program
    {
        
        static int Main(string[] args)
        {
            var commands = GetCommands();
            try
            {
                return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return 1;
            }
        }

        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }        
    }
}
