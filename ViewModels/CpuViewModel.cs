using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using Micro.Models;
using System.Windows.Input;
using Micro.Infrastructure.Commands;
using Micro.Resources;
using System.Collections.ObjectModel;
using Micro.Infrastructure.Entrys;

namespace Micro.ViewModels
{
    internal class CpuViewModel : INotifyPropertyChanged
    {

        #region Commands

        #region RestartCpuCommand
        public ICommand RestartCpuCommand {get; set; }
        private bool CanRestartCpuCommandExecute(object p) => true;
        private void OnRestartCpuCommandExecuted(object p) => _cpuState.RestartCpu();
        #endregion

        #region ExecuteMicrocommandCommand
        public ICommand ExecuteMicrocommandCommand {get; set; }
        private bool CanExecuteMicrocommandCommandExecute(object p) => true;
        private void OnExecuteMicrocommandCommandExecuted(object p) => _cpuState.ExecuteMicroCommand();
        #endregion

        #endregion

        private readonly CpuState _cpuState;
        private readonly ObservableCollection<RegisterEntry> _registers;
        public ObservableCollection<RegisterEntry> Registers => _registers;
        public string Alu => _cpuState.Alu.ToString("X4");
        public string Sda => _cpuState.Sda.ToString("X4");

        public CpuViewModel(CpuState cpuState, ObservableCollection<RegisterEntry> registers) {

            #region Commands

            RestartCpuCommand = new LambdaCommand(OnRestartCpuCommandExecuted, CanRestartCpuCommandExecute);
            ExecuteMicrocommandCommand = new LambdaCommand(OnExecuteMicrocommandCommandExecuted, CanExecuteMicrocommandCommandExecute);
            
            #endregion

            _cpuState = cpuState;
            _cpuState.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(_cpuState.Alu):
                        OnPropertyChanged(nameof(_cpuState.Alu));
                        break;
                    case nameof(_cpuState.Sda):
                        OnPropertyChanged(nameof(_cpuState.Sda));
                        break;
                }
            };
            _registers = registers;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
