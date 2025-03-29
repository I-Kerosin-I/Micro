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
    class MicroProgramMemoryViewModel : INotifyPropertyChanged
    {
        #region Commands



        #endregion

        private readonly CpuState _cpuState;
        public ObservableCollection<MicroCommand> MicroProgramMemory { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        
        public MicroProgramMemoryViewModel(CpuState cpuState)
        {
            _cpuState = cpuState;
            MicroProgramMemory = new ObservableCollection<MicroCommand>(_cpuState.MicroProgramMemory);
        }
    }

}
