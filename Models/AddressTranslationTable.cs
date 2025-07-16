using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Models
{
    public class AddressTranslationTable
    {
        public class TableRow() : INotifyPropertyChanged
        {
            private byte _address;

            public byte Address
            {
                get => _address;
                set => SetField(ref _address, value);
            }

            private string _opcode;

            public string Opcode
            {
                get => _opcode;
                set => SetField(ref _opcode, value);
            }

            public TableRow(byte address, string opcode) : this()
            {
                _address = address;
                _opcode = opcode;
            }


            #region INotifyPropertyChanged
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
            }
            #endregion
        }
        public List<TableRow> Table { get; set; }

        public AddressTranslationTable()
        {
            Table = [];
            for (var i = 0; i < 20; i++)
            {
                Table.Add(new TableRow(0, "0000000000000000"));
            }
        }
    }
}
