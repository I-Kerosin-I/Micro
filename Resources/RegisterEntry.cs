using Micro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Resources
{
    class RegisterEntry : INotifyPropertyChanged
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
