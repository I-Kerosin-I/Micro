using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;

namespace Micro.Resources
{


    public class RegisterMemory : IEnumerable<KeyValuePair<string, ushort>>, INotifyPropertyChanged

    {

    private readonly String[] _registerNames =
    {
        "AX",
        "CX",
        "DX",
        "BX",
        "SP",
        "BP",
        "SI",
        "DI",
        "CS",
        "SS",
        "DS",
        "ES",
        "IP",
        "PSW",
        "RGK",
        "RW"
    };

    private readonly Dictionary<String, ushort> _registers = new Dictionary<string, ushort>
    {
        { "AX",    0x0000 },
        { "CX",    0x0000 },
        { "DX",    0x0000 },
        { "BX",    0x0000 },
        { "SP",    0x0000 },
        { "BP",    0x0000 },
        { "SI",    0x0000 },
        { "DI",    0x0000 },
        { "CS",    0x0000 },
        { "SS",    0x0000 },
        { "DS",    0x0000 },
        { "ES",    0x0000 },
        { "IP",    0x0000 },
        { "PSW",   0x0000 },
        { "RGK",   0x0000 },
        { "RW",    0x0000 },
        { "RFI",   0x0000 },
        { "RFD",   0x0000 },
        { "RGQ",   0x0000 },
        { "ARAM",  0x0000 },
        { "RGR",   0x0000 },
        { "RGW",   0x0000 },
        { "RACT",  0x0000 },
        { "CMK",   0x0000 },
        { "MUAD",  0x0000 },
        { "STP",   0x0000 },
        { "ERROR", 0x0000 },
        { "RGA",   0x0000 },
        { "RGB",   0x0000 },
    };


    public ushort this[int index]
    {
        get => _registers[_registerNames[index]];
        set
        {
            _registers[_registerNames[index]] = value;
            OnPropertyChanged(_registerNames[index]);
        }
    }   

    public ushort this[String name]
    {
        get => _registers[name];
        set
        {
            _registers[name] = value;
            OnPropertyChanged(name);
        }
    }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public IEnumerator<KeyValuePair<string, ushort>> GetEnumerator() => _registers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}