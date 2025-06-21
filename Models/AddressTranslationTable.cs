using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Models
{
    public class AddressTranslationTable
    {
        public class TableRow(byte address, string opcode)
        {
            public byte Address { get; set; } = address;
            public string Opcode { get; set; } = opcode;
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
