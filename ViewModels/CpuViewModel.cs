using Micro.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.ViewModels
{
    internal class CpuViewModel
    {
        private readonly CpuState _cpuState;

        public ObservableCollection<RegisterEntry> Registers { get; }
        public string test;
        public CpuViewModel(CpuState cpuState) { 
        
            test = "test";

            _cpuState = cpuState;
            Registers = new ObservableCollection<RegisterEntry>(
            cpuState.Registers.Select(r => new RegisterEntry(r.Key, r.Value, cpuState))
        );
        }





        public class RegisterEntry : INotifyPropertyChanged
        {
            private readonly CpuState _cpuState;
            public string Name { get; }

            public RegisterEntry(string name, ushort value, CpuState cpuState)
            {
                Name = name;
                _cpuState = cpuState;
            }

            public string Value
            {
                get => _cpuState.Registers[Name].ToString("X4");
                set
                {
                    if (ushort.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort result))
                    {
                        _cpuState.Registers[Name] = result;
                        OnPropertyChanged(nameof(Value));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
