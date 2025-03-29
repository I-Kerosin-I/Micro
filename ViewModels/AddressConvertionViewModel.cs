using Micro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.ViewModels
{
    class AddressConvertionViewModel
    {
        private CpuState _cpuState;
        public AddressConvertionViewModel(CpuState cpuState) {
        
            _cpuState = cpuState;
        }
    }
}
