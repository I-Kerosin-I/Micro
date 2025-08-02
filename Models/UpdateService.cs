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
        private const string VersionUrl = "https://raw.githubusercontent.com/I-Kerosin-I/Micro/binaries/latestversion.txt";
        private const string NotesUrl = "https://raw.githubusercontent.com/I-Kerosin-I/Micro/binaries/releasenotes.txt";
        private const string BinaryBaseUrl = "https://raw.githubusercontent.com/I-Kerosin-I/Micro/binaries/Micro.exe";

        private const string CurrentVersion = "1.0.0";

        public static async void CheckForUpdateAsync()
        {
            using var http = new HttpClient();

            var version = (await http.GetStringAsync(VersionUrl)).Trim();
            if (version == CurrentVersion) return;
            var notes = await http.GetStringAsync(NotesUrl);
            var updateWindow = new UpdateWindow
            {
                DataContext = new UpdateViewModel(version, notes, BinaryBaseUrl)
            };
            updateWindow.Owner = Application.Current.MainWindow;
            updateWindow.ShowDialog();
        }

        public static async Task<string> DownloadUpdateAsync()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(BinaryBaseUrl)); // Качаем обнову во временные файлы
            using var http = new HttpClient();
            var data = await http.GetByteArrayAsync(BinaryBaseUrl);
            var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await fs.WriteAsync(data, 0, data.Length);
            fs.Dispose(); // using не канает, Process.Start не может запустить exe-шник, пришлось вручную закрыть

            ApplyUpdate(tempPath); // Запускаем обновлённую версию, чтоб она заменяла старую
            return tempPath;
        }

        public static void ApplyUpdate(string newExePath)
        {
            var currentExe = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(new ProcessStartInfo
            {
                FileName = newExePath, // Путь к обновлённому exe-шнику
                Arguments = $"--update \"{currentExe}\"", // Передаём путь к текущему exe-шнику, чтобы он его заменил
                UseShellExecute = false
            });

            Application.Current.Shutdown(); // Закрываем старую версию
        }

        public static void TryReplaceOldExeIfNeeded()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length < 3 || args[1] != "--update") return; // Если не обновление, то выходим
            
            var oldPath = args[2]; // Получаем путь к старому exe-шнику
            //Thread.Sleep(1000);
            File.Copy(Process.GetCurrentProcess().MainModule.FileName, oldPath, true); // Заменяем старый exe-шник на новый
        }
    }
}
