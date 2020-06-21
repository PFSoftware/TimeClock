using PFSoftware.Extensions;
using PFSoftware.Extensions.ListViewHelp;
using PFSoftware.TimeClock.Models;
using System.Windows;

namespace PFSoftware.TimeClock.Views.Users
{
    /// <summary>Interaction logic for UserLogPage.xaml</summary>
    public partial class UserLogPage
    {
        private ListViewSort _sort = new ListViewSort();

        #region Click Methods

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        private void LVShiftsColumnHeader_Click(object sender, RoutedEventArgs e) => _sort =
            Functions.ListViewColumnHeaderClick(sender, _sort, LVShifts, "#CCCCCC");

        #endregion Click Methods

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public UserLogPage()
        {
            InitializeComponent();
            LVShifts.ItemsSource = AppState.CurrentUser.Shifts;
        }

        #endregion Page-Manipulation Methods
    }
}