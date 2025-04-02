using System.ComponentModel;
using System.Windows;
using Micro.Models;
using System.Windows.Input;
using Micro.Infrastructure.Commands;

namespace Micro.ViewModels
{
    internal class CpuViewModel
    {

        #region Commands

        public ICommand RestartCpuCommand {get; set; }
        private bool CanRestartCpuCommandexecute(object p) => true;
        private void OnRestartCpuCommandExecuted(object p) => _cpuState.RestartCpu();

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
