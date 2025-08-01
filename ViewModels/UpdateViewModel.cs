using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Micro.Infrastructure.Commands;
using Micro.Models;

namespace Micro.ViewModels
{
    internal class UpdateViewModel
    {
        #region Commands
        #region Update
        public ICommand UpdateCommand { get; set; }
        private bool CanUpdateCommandExecute(object p) => true;
        private void OnUpdateCommandExecuted(object p) => Update();
        #endregion

    
        #endregion
        private void Update()
        {
            //MessageBox.Show("Just kidding, there is no update yet");
            UpdateService.DownloadUpdateAsync();
        }

        public string LatestVersion { get; set; }
        public string ReleaseNotes { get; set; }
        public string DownloadUrl { get; set; }

        public UpdateViewModel(string latestVersion,string releaseNotes,string downloadUrl)
        {                      
            #region Commands
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
            #endregion            
            LatestVersion = latestVersion;
            ReleaseNotes = releaseNotes;
            DownloadUrl = downloadUrl;
        }
    }
}
