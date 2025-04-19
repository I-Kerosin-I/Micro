using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Resources;

namespace Micro.ViewModels
{
    internal class RegistersViewModel
    {
        #region Commands



        #endregion


        private readonly CpuState _cpuState;
        private readonly ObservableCollection<RegisterEntry> _registers;
        public ObservableCollection<RegisterEntry> Registers => _registers;
        public RegistersViewModel(CpuState cpuState, ObservableCollection<RegisterEntry> Registers) {
            
            _cpuState = cpuState;
            _registers = Registers;
        }
    }
}
