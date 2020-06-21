using PFSoftware.Extensions;
using PFSoftware.Extensions.Encryption;
using PFSoftware.TimeClock.Models;
using PFSoftware.TimeClock.Models.Entities;
using PFSoftware.TimeClock.Views.Users;
using System.Windows;

namespace PFSoftware.TimeClock.Views
{
    /// <summary>Interaction logic for LoginPage.xaml</summary>
    public partial class LoginPage
    {
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            User checkUser = AppState.AllUsers.Find(user => user.Username == TxtUserID.Text.Trim());
            if (checkUser != null && checkUser != new User() && PBKDF2.ValidatePassword(PswdPassword.Password.Trim(), checkUser.Password))
            {
                AppState.CurrentUser = checkUser;
                TxtUserID.Clear();
                PswdPassword.Clear();
                TxtUserID.Focus();
                AppState.Navigate(new TimeClockPage());
                AppState.MainWindow.MnuAdmin.IsEnabled = false;
            }
            else
                AppState.DisplayNotification("Invalid login.", "Time Clock");
        }

        public LoginPage()
        {
            InitializeComponent();
            TxtUserID.Focus();
            AppState.MainWindow.MnuAdmin.IsEnabled = true;
        }

        private void PswdPassword_GotFocus(object sender, RoutedEventArgs e) => Functions.PasswordBoxGotFocus(sender);

        private void TxtUserID_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);
    }
}