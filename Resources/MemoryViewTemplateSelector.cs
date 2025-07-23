using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Micro.ViewModels;

namespace Micro.Resources
{
    public class MemoryViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ByteTemplate { get; set; }
        public DataTemplate WordTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is RamViewModel vm)
            {
                switch (vm.RamViewMode)
                {
                    case RamViewModel.RamViewModes.Bytes:
                        return ByteTemplate;
                    case RamViewModel.RamViewModes.Words:
                        return WordTemplate;
                    default:
                        return ByteTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }

}
