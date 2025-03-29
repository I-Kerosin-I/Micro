using Micro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.ViewModels
{
    class RegistersViewModel
    {
        private readonly CpuState _cpuState;
        public RegistersViewModel(CpuState cpuState) {
            
            _cpuState = cpuState;
        }
    }
}
