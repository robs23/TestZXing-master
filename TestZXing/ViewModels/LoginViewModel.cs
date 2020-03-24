using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;
using Xamarin.Forms;

namespace TestZXing.ViewModels
{
    public class LoginViewModel: INotifyPropertyChanged
    {
        public List<User> Users { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel(List<User> nUsers)
        {
            Users = new List<User>();
            Users = nUsers;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool _RememberMe { get; set; } = false;
        public bool RememberMe
        {
            get
            {
                return _RememberMe;
            }
            set
            {
                if (value != _RememberMe)
                {
                    _RememberMe = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task SaveUserCredentials()
        {
            Application.Current.Properties["RememberedUser"] = JsonConvert.SerializeObject(SelectedUser);
            Application.Current.Properties["UserRememberedAt"] = DateTime.Now.ToLongDateString();
        }

        private User _selectedUser { get; set; }
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if(_selectedUser != value)
                {
                    _selectedUser = value;
                }
            }
        }

        private int _selectedUserId { get; set; }
    }
}
