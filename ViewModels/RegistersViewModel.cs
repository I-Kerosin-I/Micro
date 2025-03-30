using Micro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.ViewModels
{
    internal class RegistersViewModel
    {
        #region Commands



        #endregion


        private readonly CpuState _cpuState;
        public RegistersViewModel(CpuState cpuState) {
            
            _cpuState = cpuState;
        }
    }
}
