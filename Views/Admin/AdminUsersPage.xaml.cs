using PFSoftware.Extensions;
using PFSoftware.Extensions.ListViewHelp;
using PFSoftware.TimeClock.Models;
using PFSoftware.TimeClock.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PFSoftware.TimeClock.Views.Admin
{
    /// <summary>Interaction logic for AdminUsersPage.xaml</summary>
    public partial class AdminUsersPage
    {
        internal List<User> AllUsers = new List<User>();
        internal AdminPage PreviousPage { get; set; }
        private ListViewSort _sort = new ListViewSort();
        private List<User> _allUsers = new List<User>();

        /// <summary>Refreshes the LVUsers's ItemSource.</summary>
        internal void RefreshItemsSource()
        {
            _allUsers = AppState.AllUsers;
            LVUsers.ItemsSource = _allUsers;
            LVUsers.Items.Refresh();
        }

        /// <summary>Toggles the buttons.</summary>
        /// <param name="enabled">Should buttons be enabled?</param>
        private void ToggleButtons(bool enabled)
        {
            BtnModifyTimes.IsEnabled = enabled;
            BtnModifyUser.IsEnabled = enabled;
            BtnDeleteUser.IsEnabled = enabled;
        }

        #region Click

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        private async void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            string message = "Are you sure you want to delete this User?";
            if (AppState.CurrentUser.Shifts.Any())
                message += $" You will also be deleting their {AppState.CurrentUser.Shifts.Count()} shifts.";
            message += " This action cannot be undone.";
            if (AppState.YesNoNotification(message, "Time Clock"))
            {
                await AppState.DeleteUser(AppState.CurrentUser).ConfigureAwait(false);
                RefreshItemsSource();
            }
        }

        private void BtnModifyTimes_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new AdminManageUserTimesPage());

        private void BtnModifyUser_Click(object sender, RoutedEventArgs e)
        {
            AdminManageUserPage manageUserPage = new AdminManageUserPage { OriginalUser = new User(AppState.CurrentUser) };
            AppState.Navigate(manageUserPage);
            manageUserPage.Reset();
        }

        private void BtnNewUser_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new AdminManageUserPage { OriginalUser = new User() });

        private void LVUsersColumnHeader_Click(object sender, RoutedEventArgs e) => _sort = Functions.ListViewColumnHeaderClick(sender, _sort, LVUsers, "#CCCCCC");

        private void LVUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppState.CurrentUser = LVUsers.SelectedIndex >= 0 ? (User)LVUsers.SelectedItem : new User();
            ToggleButtons(LVUsers.SelectedIndex >= 0);
        }

        #endregion Click

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public AdminUsersPage() => InitializeComponent();

        private void AdminUsersPage_OnLoaded(object sender, RoutedEventArgs e) => RefreshItemsSource();

        #endregion Page-Manipulation Methods
    }
}