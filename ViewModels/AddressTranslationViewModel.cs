using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Infrastructure.Entrys;

namespace Micro.ViewModels
{
    
    class AddressTranslationViewModel
    {
        private CpuState _cpuState;
        public AddressTranslationTable AddressTranslationTable { get; set; }
        public AddressTranslationViewModel(CpuState cpuState) {
        
            _cpuState = cpuState;
            AddressTranslationTable = cpuState.AddressTranslationTable;
        }
    }
}
