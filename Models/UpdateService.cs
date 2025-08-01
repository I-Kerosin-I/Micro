using Micro.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Micro.ViewModels;

namespace Micro.Models
{
    public static class UpdateService
    {
        private const string VersionUrl = "https://raw.githubusercontent.com/I-Kerosin-I/Micro/master/App.xaml";
        private const string NotesUrl = "https://raw.githubusercontent.com/I-Kerosin-I/Micro/master/App.xaml";
        private const string BinaryBaseUrl = "https://raw.githubusercontent.com/I-Kerosin-I/Micro/master/App.xaml";

        private const string CurrentVersion = "1.0.0";

        public static async void CheckForUpdateAsync()
        {
            using var http = new HttpClient();

            var version = (await http.GetStringAsync(VersionUrl)).Trim();
            if (version == CurrentVersion) return;
            var notes = await http.GetStringAsync(NotesUrl);
            
            var updateWindow = new UpdateWindow
            {
                DataContext = new UpdateViewModel(version, notes, BinaryBaseUrl + $"myapp-v{version}.exe")
            };
            updateWindow.ShowDialog();
        }

        public static async Task<string> DownloadUpdateAsync()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(BinaryBaseUrl));
            MessageBox.Show(tempPath);
            using var http = new HttpClient();
            var data = await http.GetByteArrayAsync(BinaryBaseUrl);
            using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await fs.WriteAsync(data, 0, data.Length);

            return tempPath;
        }

        public static void ApplyUpdate(string newExePath)
        {
            string currentExe = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(new ProcessStartInfo
            {
                FileName = newExePath,
                Arguments = $"--update \"{currentExe}\"",
                UseShellExecute = false
            });
            Environment.Exit(0);
        }

        public static void TryReplaceOldExeIfNeeded()
        {
            var args = Environment.GetCommandLineArgs();
            
            if (args.Length < 3 || args[1] != "--update") return;
            
            var oldPath = args[2];
            Thread.Sleep(1000);
            File.Copy(Process.GetCurrentProcess().MainModule.FileName, oldPath, true);
            Process.Start(oldPath);
            Environment.Exit(0);
        }
    }
}
