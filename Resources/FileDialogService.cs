using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Resources
{
    public class FileDialogService
    { 
        public string OpenFile(string filter)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public string SaveFile(string filter)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }
}
