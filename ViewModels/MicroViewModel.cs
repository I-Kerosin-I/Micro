using Micro.Models;
using Micro.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace Micro.ViewModels
{
    internal class MicroViewModel : INotifyPropertyChanged
    {

        public CpuState CpuState { get; }

        public CpuViewModel CpuVm { get; }
        public MicroProgramMemoryViewModel MicroprogramMemoryVm { get; }
        public RamViewModel RamVm { get; }
        public RegistersViewModel RegistersVm { get; }
        public AddressConvertionViewModel AddressConvertionVm { get; }


        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<RegisterEntry> Registers { get; }


        public MicroViewModel()
        {
            CpuState = new CpuState();

            Registers = new ObservableCollection<RegisterEntry>(CpuState.Registers.Select(r => new RegisterEntry(r.Key, r.Value, CpuState)));
            
            CpuVm = new CpuViewModel(CpuState, Registers);
            MicroprogramMemoryVm = new MicroProgramMemoryViewModel(CpuState);
            RamVm = new RamViewModel(CpuState);
            RegistersVm = new RegistersViewModel(CpuState, Registers);
            AddressConvertionVm = new AddressConvertionViewModel(CpuState);

            
        }
    }
}
