using Micro.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Micro.ViewModels
{
    class MicroViewModel : INotifyPropertyChanged
    {

        public CpuState CpuState { get; }

        public CpuViewModel CpuVM { get; }
        public MicroProgramMemoryViewModel MicroprogramMemoryVM { get; }
        public RamViewModel RamVM { get; }
        public RegistersViewModel RegistersVM { get; }
        public AddressConvertionViewModel AddressConvertionVM { get; }


        public event PropertyChangedEventHandler PropertyChanged;
        
        public ObservableCollection<byte> Ram { get; set; }

        public MicroViewModel()
        {
            CpuState = new CpuState();

            CpuVM = new CpuViewModel(CpuState);
            MicroprogramMemoryVM = new MicroProgramMemoryViewModel(CpuState);
            RamVM = new RamViewModel(CpuState);
            RegistersVM = new RegistersViewModel(CpuState);
            AddressConvertionVM = new AddressConvertionViewModel(CpuState);

            
        }
    
    }
    
}
