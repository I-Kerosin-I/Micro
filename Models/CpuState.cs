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
using System.Windows.Media.Media3D;


namespace Micro.Models 
{
    public class CpuState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private Stack<ushort> _stack = new Stack<ushort>(); 
        public MicroProgramMemory MicroProgramMemory;
        public RegisterMemory Registers;
        public RamMemory Memory;

        public CpuState()
        {
            MicroProgramMemory = new MicroProgramMemory();
            Registers = new RegisterMemory();
            Memory = new RamMemory();
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
            
            ushort r, s;
            switch (mk.SRC)
            {
                case 0:
                    r = 0;
                    s = 0;
                    break;
                case 1:
                    r = Registers["RGA"];
                    s = Registers["RGB"];
                    break;
                case 2:
                    r = Registers["RGA"];
                    s = Registers["RGQ"];
                    break;
                case 3:
                    r = Registers["RGA"];
                    s = Registers["RGR"];
                    break;
                case 4:
                    r = (ushort)(Registers["RGA"] << 1);
                    s = Registers["RGB"];
                    break;
                case 5:
                    r = mk.CONST;
                    s = Registers["RGB"];
                    break;
                case 6:
                    r = mk.CONST;
                    s = Registers["RGR"];
                    break;
                case 7:
                    r = mk.CONST;
                    s = Registers["RGQ"];
                    break;
                default:
                    throw new Exception("Invalid SRC value");
            }
            
            byte c0 = mk.CCX;
            if ((mk.CCX & 0b10) == 2) c0 = (byte)(Registers["RFI"] & 1);

            UInt32 _alu32;
            switch (mk.ALU) 
            {
                case 0:
                    _alu32 = 0;
                    break;
                case 1:
                    _alu32 = (uint)(s - r - 1 + c0);
                    break;
                case 2:
                    _alu32 = (uint)(r - s - 1 + c0);
                    break;
                case 3:
                    _alu32 = (uint)(r + s + c0);
                    break;
                case 4:
                    _alu32 = (uint)(s + c0);
                    break;
                case 5:
                    _alu32 = (uint)(~s + c0);
                    break;
                case 6:
                    _alu32 = (uint)(r + c0);
                    break;
                case 7:
                    _alu32 = (uint)(~r + c0);
                    break;
                case 8:
                    _alu32 = (uint)(r + c0); //TODO: Умножение на 2 бита
                    break;
                case 9:
                    _alu32 = (uint)(r & s);
                    break;
                case 10:
                    _alu32 = (uint)(r & ~s); ;
                    break;
                case 11:
                    _alu32 = (uint)~(r & s); 
                    break;
                case 12:
                    _alu32 = (uint)(r | s);
                    break;
                case 13:
                    _alu32 = (uint)~(r | s);
                    break;
                case 14:
                    _alu32 = (uint)(r ^ s);
                    break;
                case 15:
                    _alu32 = (uint)~(r ^ s);
                    break;
                default:
                    throw new ArgumentException("Invalid ALU value");
            } //Готово, не протестировано

            Alu = (ushort)(_alu32 & 0x0000FFFF);
            Registers["RFI"] = (ushort)((Alu & 0x8000) >> 8) ; // Формирование флага N (S)  - 7 бит RFI
            Registers["RFI"] |= (ushort)(Alu == 0 ? 0x40 : 0); // Формирование флага Z  - 6 бит RFI
            Registers["RFI"] |= (ushort)((_alu32 & 0x10000) >> 16); // Формирование флага C  - 0 бит RFI
            Registers["RFI"] |= (ushort)(((Registers["RFI"] << 4) ^ (Alu >> 4)) & 0x800); // Формирование флага V  - 11 бит RFI
            byte p = 0;
            for (byte i = 0; i < 8; i++) p ^= (byte)((Alu >> i) & 1); // Формирование флага P  - 2 бит RFI
            Registers["RFI"] |= (ushort)(p << 2);
            
            //TODO: Узнать про формирование Pf и Vf

            switch (mk.SH) // TODO: Написать логику сдвигателей
            {
                case 0:
                    Sda = Alu;
                    break;
                case 1:
                    Sda = Alu;
                    break;
                case 2:
                    Sda = (ushort)(Alu >> mk.N);
                    break;
                case 3:
                    Sda = Alu;
                    break;
                case 4:
                    Sda = Alu;
                    break;
                case 5:
                    Sda = Alu;
                    break;
                case 6:
                    Sda = Alu;
                    Registers["RGQ"] = Alu;
                    break;
                case 8:
                    Sda = (ushort)(Alu << mk.N);
                    break;
                case 10:
                    Sda = Alu;
                    break;
                case 14:
                    Sda = Alu;
                    break;
                default:
                    throw new ArgumentException("Invalid SH value");
            }

            switch (mk.DST) 
            {
                case 0:
                    break;
                case 1:
                    Registers[mk.B] = Registers["RGR"];
                    break;
                case 2:
                    Registers[mk.B] |= (ushort)(Registers["RGR"] << 8);
                    break;
                case 3:
                    Registers[mk.B] |= (ushort)(Registers["RGR"] >> 8);
                    break;
                case 4:
                    Registers[mk.B] = Sda;
                    break;
            }

            switch (mk.WM) // Готово, протестировано
            {
                case 0:
                    break;
                case 1:
                    Registers["RGW"] = Sda;
                    break;
                case 2:
                    Registers["ARAM"] = Sda;
                    break;
                case 3:
                    Registers["ARAM"] = Registers["RGB"];
                    break;
                default:
                    throw new ArgumentException("Invalid WM value");
            }

            switch (mk.MEM) 
            {
                case 4:
                    Registers["RGR"] = Memory[Registers["ARAM"]];
                    break;
                case 5:
                    Registers["RGR"] = Memory.Word[Registers["ARAM"]];
                    break;
                case 6:
                    Memory[Registers["ARAM"]] = (byte)(Registers["RGW"] & 0xff);
                    break;
                case 7:
                    Memory.Word[Registers["ARAM"]] = Registers["RGW"];
                    break;
            }
            bool jmpCond = true;

            if ((mk.JFI & 4) == 0)
            {
                String condFlagRegister = (mk.JFI & 2) == 0 ? "RFI" : "RFD";

                switch (mk.CC) //TODO: Реализовать остальные условия
                {
                    case 0:
                        jmpCond = (Registers[condFlagRegister] & 16) != 0; //P = 1   JP
                        break;
                    case 1:
                        jmpCond = (Registers[condFlagRegister] & 2) != 0; //Z = 1   JZ
                        break;
                    case 2:
                        jmpCond = (Registers[condFlagRegister] & 1) != 0; //N = 1   JS
                        break;
                }

                if ((mk.JFI & 1) != 0) jmpCond = !jmpCond;
            } // Формирование условия (преобразовано с учётом JFI)
            
            switch (mk.CHA) // TODO: Обработка некорректного ввода адреса перехода
            {
                case 0:
                    Registers["CMK"] = 0;
                    break;
                case 1:
                    if (jmpCond)
                    {
                        _stack.Push((ushort)(mk.Address + 1));
                        Registers["CMK"] = mk.CONST;
                    }
                    else Registers["CMK"]++;
                    
                    break;
                case 2: //TODO: Дешифрация RGK
                    break;
                case 3:
                    if (jmpCond) Registers["CMK"] = mk.CONST;
                    else Registers["CMK"]++;
                    break;
                case 4: // Если RACT != 0, декремент, переход по CONST иначе CONT
                    if (Registers["RACT"] != 0)
                    {
                        Registers["RACT"]--;
                        Registers["CMK"] = mk.CONST;
                    }
                    else Registers["CMK"]++;
                    break;
                case 5:
                    if (jmpCond)
                    {
                        if (_stack.Count != 0)
                            Registers["CMK"] =
                                _stack.Pop();
                        else
                        {
                            // TODO: выдавать ошибку "Пустой стэк"
                            Registers["CMK"]++;
                        }    
                    }
                    else Registers["CMK"]++;
                        break;
                case 6:
                    Registers["RACT"] = mk.CONST;
                    Registers["CMK"]++;
                    break;
                case 7:
                    Registers["CMK"]++;
                    break;
            }

            
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


    

}
