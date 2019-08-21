using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Static;

namespace TestZXing.ViewModels
{
    class UserSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string UserStatus
        {
            get
            {
                return Static.RuntimeSettings.CurrentUser.Icon;
            }
        }

        public string UserSurname {
            get
            {
                return RuntimeSettings.CurrentUser.Surname;
            }
        }
    }
}
