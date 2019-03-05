using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestZXing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessesPage : TabbedPage
    {
        public ProcessesPage()
        {
            InitializeComponent();
            Children.Add(new ActiveProcesses(new ActiveProcessesViewModel(true)));
            Children.Add(new ActiveProcesses(new ActiveProcessesViewModel()));
        }
    }
}