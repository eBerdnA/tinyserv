using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManyConsole;
using Mono.Options;

namespace Tinyserv
{
    public class RunCommand : ConsoleCommand
    {
        public Dictionary<string, string> OptionalArgumentList = new Dictionary<string, string>();
        private const string PortValueKey = "port";
        private const string HostnameValueKey = "hostname";
        private const string DirectoryValueKey = "directory";
        private bool Browser { get; set; }
        private bool _UseFileCache { get; set; }
        public RunCommand()
        {
            this.IsCommand("RunCommand", "");
            Options = new OptionSet()
            {
                {"p|port=", "Port of webserver", v => OptionalArgumentList.Add(PortValueKey,v)},
                {"h|hostname=", "Hostname", v => OptionalArgumentList.Add(HostnameValueKey,v)},
                {"d|directory=", "Directory to server from", v => OptionalArgumentList.Add(DirectoryValueKey,v)}
            };

            this.HasOption("b|browser", "Boolean flag option", b => Browser = true);
            this.HasOption("c|fileCache", "Use file caching", b => _UseFileCache = true);
        }
        public override int Run(string[] remainingArguments)
        {
            var server = new Server(this);
            server.Run();
            return 0;
        }

        internal bool OpenBrowser
        {
            get
            {
                return this.Browser;
            }
        }

        internal bool UseFileCache
        {
            get
            {
                return this._UseFileCache;
            }
        }

        internal string Hostname
        {
            get
            {
                string hostname = string.Empty;
                OptionalArgumentList.TryGetValue(HostnameValueKey, out hostname);
                if (string.IsNullOrEmpty(hostname))
                {
                    return "localhost";
                }
                else
                {
                    return hostname;
                }
            }
        }

        internal string Directory
        {
            get
            {
                string directory = string.Empty;
                OptionalArgumentList.TryGetValue(DirectoryValueKey, out directory);
                return directory;
            }
        }

        internal int Port        
        {
            get
            {
                string portValue = string.Empty;
                OptionalArgumentList.TryGetValue(PortValueKey, out portValue);
                if (string.IsNullOrEmpty(portValue))
                {
                    return 8080;
                }
                else
                {
                    return Convert.ToInt32(portValue);
                }
            }

        }
    }
}