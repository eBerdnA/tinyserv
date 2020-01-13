using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Files;
using Swan;
using Swan.Logging;

namespace Tinyserv
{
    public class Server
    {
        private const bool UseFileCache = true;
        private RunCommand Cmd;
        public Server(RunCommand cmd)
        {
            Cmd = cmd;
        }
        internal void Run()
        {
            
            var url = $"http://{Cmd.Hostname}:{Cmd.Port}/";
            url.Info();
            using (var ctSource = new CancellationTokenSource())
            {
                Task.WaitAll(
                    RunWebServerAsync(url, ctSource.Token),
                    Cmd.OpenBrowser ? ShowBrowserAsync(url.Replace("*", "localhost"), ctSource.Token) : Task.CompletedTask,
                    WaitForUserBreakAsync(ctSource.Cancel));
            }

            // Clean up
            "Bye".Info(nameof(Program));
            Terminal.Flush();

            Console.WriteLine("Press any key to exit.");
            WaitForKeypress();
        }

        // Open the default browser on the web server's home page.
        private static async Task ShowBrowserAsync(string url, CancellationToken cancellationToken)
        {
            // Be sure to run in parallel.
            await Task.Yield();

            // Fire up the browser to show the content!
            using var browser = new Process();
            browser.StartInfo = new ProcessStartInfo(url) {
                UseShellExecute = true
            };
            browser.Start();
        }

        private static async Task WaitForUserBreakAsync(Action cancel)
        {
            // Be sure to run in parallel.
            await Task.Yield();

            "Press any key to stop the web server.".Info(nameof(Program));
            WaitForKeypress();
            "Stopping...".Info(nameof(Program));
            cancel();
        }

        // Clear the console input buffer and wait for a keypress
        private static void WaitForKeypress()
        {
            while (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.ReadKey(true);
        }

        // Create and run a web server.
        private async Task RunWebServerAsync(string url, CancellationToken cancellationToken)
        {
            using var server = CreateWebServer(url);
            await server.RunAsync(cancellationToken).ConfigureAwait(false);
        }

        // Gets the local path of shared files.
        // When debugging, take them directly from source so we can edit and reload.
        // Otherwise, take them from the deployment directory.
        public string HtmlRootPath
        {
            get
            {
                if (!string.IsNullOrEmpty(Cmd.Directory))
                {
                    return Cmd.Directory;
                }
                else
                {
                var assemblyPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);

#if DEBUG
                return Path.Combine(Directory.GetParent(assemblyPath).Parent.Parent.FullName, "html");
#else
                return Path.Combine(assemblyPath, "html");
#endif
                }
            }
        }

        private void CheckServingDirectory(string path)
        {
            if(!Directory.Exists(path))
            {
                string errMsg = $"Directory [{path}] to serve from does not exist";
                errMsg.Error();
                throw new Exception(errMsg);
            }
        }

        private WebServer CreateWebServer(string url)
        {
            var htmlRootPath = HtmlRootPath;
            CheckServingDirectory(htmlRootPath);
            $"Serving: {htmlRootPath}".Info();
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))                    
            .WithStaticFolder("/", htmlRootPath, true, m => m
            .WithContentCaching(Cmd.UseFileCache)); // Add static files after other modules to avoid conflicts

            server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();

            return server;
        }
    }
}
