using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Micro.Resources;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;


namespace Micro.Models 
{
    public class CpuState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RegisterMemory Registers;

        private ushort _alu;
        public ushort Alu {
            get => _alu;
            set => SetField(ref _alu, value);
        }
        private ushort _sda;
        public ushort Sda {
            get => _sda;
            set
            {
                _sda = value;
                OnPropertyChanged(nameof(Sda));
            }
        }
        public ObservableCollection<MemoryRow> Memory;
        public MicroProgrammMemory MicroProgramMemory;

        public CpuState()
        {
            Registers = new RegisterMemory();

            Memory = new ObservableCollection<MemoryRow>();
            for (int i = 0; i < 64; i++) Memory.Add(new MemoryRow(i));

            MicroProgramMemory = new MicroProgrammMemory();
            

        }

        public ushort this[string register]
        {
            get => Registers[register];
            set
            {
                if (Registers[register] != value)
                {
                    Registers[register] = value;
                    OnPropertyChanged(register);
                }
            }
        }

        public void RestartCpu()
        {
            Registers["CMK"] = 0;
        }

        public void ExecuteMicroCommand()
        {
            var mk = MicroProgramMemory[Registers["CMK"]];
            Registers["RGA"] = Registers[mk.A];
            Registers["RGB"] = Registers[mk.B];
            Registers["CMK"]++;
            Alu++;
            MicroProgramMemory[0].CCX = 1;
            
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public struct MemoryRow
    {
        public string Address { get; set; }  // HEX-адрес
        public ObservableCollection<byte> Bytes { get; set; }    // 16 байтов строки

        public MemoryRow(int address)
        {
            Address = address.ToString("X2");
            Bytes = new ObservableCollection<byte>(); // Заполняем нулями по умолчанию
            for (int i = 0; i < 16; i++) Bytes.Add(0);
        }
    }

    

}
