using Micro.Models;
using Micro.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Micro.Infrastructure.Commands;

namespace Micro.ViewModels
{
    internal class CpuViewModel
    {

        #region Commands

        public ICommand RestartCpuCommand {get; set; }

        private bool CanRestartCpuCommandexecute(object p) => true;

        private void OnRestartCpuCommandExecuted(object p)
        {


        }

        #endregion

        private readonly CpuState _cpuState;

        public CpuViewModel(CpuState cpuState) {

            #region Commands

            RestartCpuCommand = new LambdaCommand(OnRestartCpuCommandExecuted, CanRestartCpuCommandexecute);

            #endregion


            _cpuState = cpuState;
            
        }





        
    }
}
