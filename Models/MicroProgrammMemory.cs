using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Models
{
    public class MicroProgrammMemory 
    {
        public ObservableCollection<MicroCommand> MicroCommands { get; set; }


        public MicroProgrammMemory()
        {
            MicroCommands = new ObservableCollection<MicroCommand>();
            for (ushort i = 0; i < 64; i++) MicroCommands.Add(new MicroCommand(i));
        }

        public MicroCommand this[int index]
        {
            get => MicroCommands[index];
            set => MicroCommands[index] = value;
        }
/*
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }*/

    }

    public class MicroCommand
    {
        public ushort Address { get; set; }
        public ushort A { get; set; }
        public ushort B { get; set; }
        public ushort MA { get; set; }
        public ushort MB { get; set; }
        public ushort MEM { get; set; }
        public ushort SRC { get; set; }
        public ushort SH { get; set; }
        public ushort N { get; set; }
        public ushort ALU { get; set; }
        public ushort CCX { get; set; }
        public ushort F { get; set; }
        public ushort DST { get; set; }
        public ushort WM { get; set; }
        public ushort JFI { get; set; }
        public ushort CC { get; set; }
        public ushort CHA { get; set; }
        public ushort CONST { get; set; }

        public MicroCommand(ushort Addr)
        {
            Address = Addr;
            A = 0;
            B = 0;
            MA = 0;
            MB = 0;
            MEM = 0;
            SRC = 1;
            SH = 0;
            N = 0;
            ALU = 6;
            CCX = 0;
            F = 0;
            DST = 0;
            WM = 0;
            JFI = 0;
            CC = 0;
            CHA = 7;
            CONST = 0;
        }
    }
}
