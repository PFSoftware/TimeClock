﻿using PFSoftware.Extensions;
using PFSoftware.Extensions.Encryption;
using PFSoftware.Extensions.Enums;
using PFSoftware.TimeClock.Models;
using PFSoftware.TimeClock.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PFSoftware.TimeClock.Views.Admin
{
    /// <summary>Interaction logic for AdminModifyUserPage.xaml</summary>
    public partial class AdminManageUserPage
    {
        /// <summary>The original <see cref="User"/>, if applicable.</summary>
        internal User OriginalUser { get; set; }

        /// <summary>Determines if buttons should be enabled.</summary>
        internal void CheckInput()
        {
            bool enabled = TxtUsername.Text.Length > 0
                && TxtFirstName.Text.Length > 0
                && TxtLastName.Text.Length > 0
                || TxtUsername.Text != AppState.CurrentUser.Username
                || TxtFirstName.Text != AppState.CurrentUser.FirstName
                || TxtLastName.Text != AppState.CurrentUser.LastName
                || (PswdPassword.Password.Length > 0 && PswdConfirm.Password.Length > 0)
                && !OriginalUser.Roles.Except(AppState.CurrentUser.Roles).Any();
            BtnSubmit.IsEnabled = enabled;
            BtnReset.IsEnabled = enabled;
        }

        /// <summary>Resets all inputs to their default values.</summary>
        internal void Reset()
        {
            TxtUsername.Text = OriginalUser.Username;
            TxtFirstName.Text = OriginalUser.FirstName;
            TxtLastName.Text = OriginalUser.LastName;
            AppState.CurrentUser = new User(OriginalUser);
            PswdPassword.Password = "";
            PswdConfirm.Password = "";
            TxtUsername.Focus();
        }

        #region User Manipulation

        /// <summary>Adds a new <see cref="User"/> to the database.</summary>
        private async Task NewUser()
        {
            User checkUser = await AppState.LoadUser(TxtUsername.Text).ConfigureAwait(false);
            if (checkUser == new User() || checkUser == null)
            {
                if (PswdPassword.Password.Length >= 4 && PswdPassword.Password == PswdConfirm.Password)
                {
                    Dispatcher.Invoke(() =>
                    {
                        AppState.CurrentUser.Username = TxtUsername.Text.Trim();
                        AppState.CurrentUser.FirstName = TxtFirstName.Text.Trim();
                        AppState.CurrentUser.LastName = TxtLastName.Text.Trim();
                        AppState.CurrentUser.Password = PBKDF2.HashPassword(PswdPassword.Password.Trim());
                    });
                    if (AppState.CurrentUser.Roles.ToList().Count > 0)
                    {
                        if (await AppState.NewUser(AppState.CurrentUser).ConfigureAwait(false))
                            Dispatcher.Invoke(() => AppState.GoBack());
                    }
                    else
                        AppState.DisplayNotification("There are no roles assigned to this User.", "Time Clock");
                }
                else
                    AppState.DisplayNotification("Please ensure the passwords match and are at least 4 characters in length.", "Time Clock");
            }
            else
                AppState.DisplayNotification("This username has already been taken.", "Time Clock");
        }

        /// <summary>Modifies a <see cref="User"/> in the database.</summary>
        private async Task ModifyUser()
        {
            AppState.CurrentUser.Username = TxtUsername.Text.Trim();
            AppState.CurrentUser.FirstName = TxtFirstName.Text.Trim();
            AppState.CurrentUser.LastName = TxtLastName.Text.Trim();
            AppState.CurrentUser.Password = PswdPassword.Password.Length >= 4 ? PBKDF2.HashPassword(PswdPassword.Password.Trim()) : AppState.CurrentUser.Password;

            if (AppState.CurrentUser != OriginalUser)
            {
                if (AppState.CurrentUser.Roles.ToList().Count > 0)
                {
                    if (await AppState.ChangeUserDetails(OriginalUser, AppState.CurrentUser).ConfigureAwait(false))
                        Dispatcher.Invoke(() => AppState.GoBack());
                }
                else
                    AppState.DisplayNotification("There are no roles assigned to this User.", "Time Clock");
            }
            else
                AppState.DisplayNotification("There are no changes to save.", "Time Clock");
        }

        #endregion User Manipulation

        #region Button-Click Methods

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        private void BtnManageRoles_Click(object sender, RoutedEventArgs e)
        {
            AdminManageUserRolesPage RolesPage = new AdminManageUserRolesPage { PreviousPage = this };
            AppState.Navigate(RolesPage);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e) => Reset();

        private async void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (TxtUsername.Text.Length >= 4 && TxtFirstName.Text.Length >= 2 && TxtLastName.Text.Length >= 2 && ((PswdPassword.Password.Length == 0 && PswdConfirm.Password.Length == 0) || (PswdPassword.Password.Length >= 4 && PswdConfirm.Password.Length >= 4)) && PswdPassword.Password == PswdConfirm.Password)
            {
                if (OriginalUser == null || OriginalUser == new User())
                    await NewUser().ConfigureAwait(false);
                else
                    await ModifyUser().ConfigureAwait(false);
            }
            else if (PswdPassword.Password.Length != 0 && PswdConfirm.Password.Length != 0 && PswdPassword.Password.Length < 4 && PswdConfirm.Password.Length < 4)
                AppState.DisplayNotification("Please ensure the new password is 4 or more characters in length.", "Time Clock");
            else if (PswdPassword.Password != PswdConfirm.Password)
                AppState.DisplayNotification("Please ensure the passwords match.", "Time Clock");
            else if (TxtUsername.Text.Length < 4 || TxtFirstName.Text.Length < 2 || TxtLastName.Text.Length < 2)
                AppState.DisplayNotification("Please ensure the username and password are at least 4 characters long, and first and last names are at least 2 characters long.", "Time Clock");
        }

        #endregion Button-Click Methods

        #region Page-Manipulation Methods

        public AdminManageUserPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e) => CheckInput();

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => Functions.TextBoxGotFocus(sender);

        private void Pswd_GotFocus(object sender, RoutedEventArgs e) => Functions.PasswordBoxGotFocus(sender);

        private void TxtName_PreviewKeyDown(object sender, KeyEventArgs e) => Functions.PreviewKeyDown(e, KeyType.Letters);

        private void Txt_TextChanged(object sender, TextChangedEventArgs e) => CheckInput();

        private void Pswd_TextChanged(object sender, RoutedEventArgs e) => CheckInput();

        #endregion Page-Manipulation Methods
    }
}