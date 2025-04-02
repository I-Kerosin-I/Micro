using Micro.Models;
using System.ComponentModel;
using System.Globalization;

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
            _cpuState.Registers.PropertyChanged += CpuState_PropertyChanged;
        }
        private void CpuState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Name) OnPropertyChanged(nameof(Value));
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
