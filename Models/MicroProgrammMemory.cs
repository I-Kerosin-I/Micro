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
            for (var i = 0; i < 64; i++) MicroCommands.Add(new MicroCommand(i));
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
        public int Address { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int MA { get; set; }
        public int MB { get; set; }
        public int MEM { get; set; }
        public int SRC { get; set; }
        public int SH { get; set; }
        public int N { get; set; }
        public int ALU { get; set; }
        public int CCX { get; set; }
        public int F { get; set; }
        public int DST { get; set; }
        public int WM { get; set; }
        public int JFI { get; set; }
        public int CC { get; set; }
        public int CHA { get; set; }
        public int CONST { get; set; }

        public MicroCommand(int Addr)
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
