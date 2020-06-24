using PFSoftware.TimeClock.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TimeClock.Models.Entities;

namespace PFSoftware.TimeClock.Views.Admin
{
    /// <summary>Interaction logic for AdminManageUserRolesPage.xaml</summary>
    public partial class AdminManageUserRolesPage : INotifyPropertyChanged
    {
        internal AdminManageUserPage PreviousPage { get; set; }

        private List<Role> _availableRoles = new List<Role>();
        private List<Role> _assignedRoles = new List<Role>();

        #region Data-Binding

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        /// <summary>Updates the ListBoxes.</summary>
        private void UpdateBindings()
        {
            _availableRoles.Clear();
            _availableRoles = new List<Role>(AppState.AllRoles);
            _assignedRoles.Clear();
            _assignedRoles = new List<Role>(AppState.CurrentUser.Roles);

            foreach (Role role in _assignedRoles)
                _availableRoles.Remove(role);

            LstAvailable.ItemsSource = _availableRoles;
            LstAssigned.ItemsSource = _assignedRoles;
            LstAvailable.Items.Refresh();
            LstAssigned.Items.Refresh();
        }

        #endregion Data-Binding

        #region Click

        private void BtnAssign_Click(object sender, RoutedEventArgs e)
        {
            AppState.CurrentUser.AddRole((Role)LstAvailable.SelectedItem);
            UpdateBindings();
        }

        private void BtnUnassign_Click(object sender, RoutedEventArgs e)
        {
            AppState.CurrentUser.RemoveRole((Role)LstAssigned.SelectedItem);
            UpdateBindings();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        #endregion Click

        #region Page Manipulation

        public AdminManageUserRolesPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e) => UpdateBindings();

        private void LstAvailable_SelectionChanged(object sender, SelectionChangedEventArgs e) => BtnAssign.IsEnabled = LstAvailable.SelectedIndex >= 0;

        private void LstAssigned_SelectionChanged(object sender, SelectionChangedEventArgs e) => BtnUnassign.IsEnabled = LstAssigned.SelectedIndex >= 0;

        #endregion Page Manipulation
    }
}