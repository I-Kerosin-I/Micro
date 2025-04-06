using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.ViewModels
{
    internal class MicroProgramMemoryViewModel : INotifyPropertyChanged
    {
        #region Commands



        #endregion

        private readonly CpuState _cpuState;
        private readonly MicroProgrammMemory _microProgramMemory;

        public ObservableCollection<MicroCommand> MicroProgramMemory => _microProgramMemory.MicroCommands;

        //public ObservableCollection<MicroCommand> MicroCommands => _microProgramMemory.MicroCommands;
        public event PropertyChangedEventHandler PropertyChanged;

        
        public MicroProgramMemoryViewModel(CpuState cpuState)
        {
            _cpuState = cpuState;
            _microProgramMemory = _cpuState.MicroProgramMemory;
        }
    }

}
