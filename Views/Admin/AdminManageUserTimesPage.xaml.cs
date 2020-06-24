using PFSoftware.Extensions;
using PFSoftware.Extensions.DataTypeHelpers;
using PFSoftware.Extensions.ListViewHelp;
using PFSoftware.TimeClock.Models;
using PFSoftware.TimeClock.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PFSoftware.TimeClock.Views.Admin
{
    /// <summary>Interaction logic for AdminManageUserTimesPage.xaml</summary>
    public partial class AdminManageUserTimesPage : Page
    {
        private bool _newTime;
        private List<Shift> _allShifts = new List<Shift>();
        private Shift _selectedShift = new Shift();
        private ListViewSort _sort = new ListViewSort();

        /// <summary>Refreshes the LVUsers's ItemSource.</summary>
        internal void RefreshItemsSource()
        {
            _allShifts = AppState.CurrentUser.Shifts.ToList();
            Dispatcher.Invoke(() =>
            {
                LVShifts.ItemsSource = _allShifts;
                LVShifts.Items.Refresh();
            });
        }

        #region Display

        private void DisplayTimeIn()
        {
            if (_selectedShift.StartTimeUtc != DateTime.MinValue)
            {
                DateIn.SelectedDate = _selectedShift.StartTimeUtc;
                CmbHourIn.SelectedIndex = _selectedShift.StartTimeUtc.Hour % 12;
                CmbMinuteIn.SelectedItem = _selectedShift.StartTimeUtc.Minute;
                CmbSecondIn.SelectedItem = _selectedShift.StartTimeUtc.Second;
                CmbAMPMIn.SelectedIndex = _selectedShift.StartTimeUtc.Hour >= 12 ? 1 : 0;
            }
            else
            {
                CmbHourIn.SelectedIndex = -1;
                CmbMinuteIn.SelectedIndex = -1;
                CmbSecondIn.SelectedIndex = -1;
            }
        }

        private void DisplayTimeOut()
        {
            if (_selectedShift.EndTimeUtc != DateTime.MinValue)
            {
                DateOut.SelectedDate = _selectedShift.EndTimeUtc != DateTime.MinValue ? _selectedShift.EndTimeUtc : _selectedShift.StartTimeUtc;
                CmbHourOut.SelectedIndex = _selectedShift.EndTimeUtc.Hour % 12;
                CmbMinuteOut.SelectedItem = _selectedShift.EndTimeUtc.Minute;
                CmbSecondOut.SelectedItem = _selectedShift.EndTimeUtc.Second;
                CmbAMPMOut.SelectedIndex = _selectedShift.EndTimeUtc.Hour >= 12 ? 1 : 0;
            }
            else
            {
                CmbHourOut.SelectedIndex = -1;
                CmbMinuteOut.SelectedIndex = -1;
                CmbSecondOut.SelectedIndex = -1;
            }
        }

        private void DisplayRole()
        {
            if (string.IsNullOrWhiteSpace(_selectedShift.Role))
                CmbRole.SelectedIndex = -1;
            else
                CmbRole.SelectedItem = _selectedShift.Role;
        }

        #endregion Display

        private void Clear()
        {
            LVShifts.SelectedIndex = -1;
            CmbHourIn.SelectedIndex = -1;
            CmbMinuteIn.SelectedIndex = -1;
            CmbSecondIn.SelectedIndex = -1;
            CmbAMPMIn.SelectedIndex = -1;
            CmbHourOut.SelectedIndex = -1;
            CmbMinuteOut.SelectedIndex = -1;
            CmbSecondOut.SelectedIndex = -1;
            CmbAMPMOut.SelectedIndex = -1;
            CmbRole.SelectedIndex = -1;
            DateIn.SelectedDate = null;
            DateIn.DisplayDate = DateTime.Today;
            DateOut.SelectedDate = null;
            DateOut.DisplayDate = DateTime.Today;
            BtnSave.IsEnabled = false;
            BtnNew.IsEnabled = true;
            BtnClear.IsEnabled = false;
        }

        //TODO Turn this application into a fully-featured HR nightmare, with payroll calculations and everything else.
        //TODO Maybe combine managing all user stuff into a tabbed interface.

        #region Page Manipulation

        public AdminManageUserTimesPage()
        {
            InitializeComponent();
            LVShifts.ItemsSource = AppState.CurrentUser.Shifts;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CmbRole.ItemsSource = AppState.CurrentUser.Roles;
            CmbMinuteIn.Items.Add(0);
            CmbMinuteOut.Items.Add(0);
            CmbSecondIn.Items.Add(0);
            CmbSecondOut.Items.Add(0);
            CmbHourIn.Items.Add(12);
            CmbHourOut.Items.Add(12);
            for (int i = 1; i < 12; i++)
            {
                CmbHourIn.Items.Add(i);
                CmbMinuteIn.Items.Add(i);
                CmbSecondIn.Items.Add(i);
                CmbHourOut.Items.Add(i);
                CmbMinuteOut.Items.Add(i);
                CmbSecondOut.Items.Add(i);
            }
            for (int i = 12; i < 60; i++)
            {
                CmbMinuteIn.Items.Add(i);
                CmbSecondIn.Items.Add(i);
                CmbMinuteOut.Items.Add(i);
                CmbSecondOut.Items.Add(i);
            }
        }

        #endregion Page Manipulation

        #region Click

        private void BtnBack_Click(object sender, RoutedEventArgs e) => AppState.GoBack();

        private void BtnClear_Click(object sender, RoutedEventArgs e) => Clear();

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            _newTime = true;
            BtnSave.IsEnabled = true;
            BtnNew.IsEnabled = false;
            BtnClear.IsEnabled = true;
            DateIn.SelectedDate = DateTime.Today;
            DateOut.SelectedDate = DateTime.Today;
            CmbHourIn.SelectedItem = DateTime.Now.Hour % 12;
            CmbMinuteIn.SelectedItem = DateTime.Now.Minute;
            CmbSecondIn.SelectedItem = DateTime.Now.Second;
            CmbAMPMIn.SelectedIndex = DateTime.Now.Hour >= 12 ? 1 : 0;
            CmbRole.SelectedIndex = 0;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DateTime dayIn = DateTimeHelper.Parse(DateIn.SelectedDate);
            DateTime timeIn = new DateTime(dayIn.Year, dayIn.Month, dayIn.Day, CmbAMPMIn.Text == "AM" ? CmbHourIn.SelectedIndex : CmbHourIn.SelectedIndex + 12, Int32Helper.Parse(CmbMinuteIn.SelectedItem), Int32Helper.Parse(CmbSecondIn.SelectedItem));
            DateTime dayOut = DateTimeHelper.Parse(DateOut.SelectedDate);
            DateTime timeOut = CmbHourOut.SelectedIndex >= 0 ? new DateTime(dayOut.Year, dayOut.Month, dayOut.Day, CmbAMPMOut.Text == "AM" ? CmbHourOut.SelectedIndex : CmbHourOut.SelectedIndex + 12, Int32Helper.Parse(CmbMinuteOut.SelectedItem), Int32Helper.Parse(CmbSecondOut.SelectedItem)) : DateTime.MinValue;
            Shift newShift = new Shift(AppState.CurrentUser.ID, CmbRole.SelectedItem.ToString(), timeIn, timeOut, true);

            if (!_newTime)
                await AppState.ModifyShift(_selectedShift, newShift).ConfigureAwait(false);
            else
                await AppState.AddShift(newShift).ConfigureAwait(false);

            AppState.CurrentUser.LoggedIn = AppState.CurrentUser.GetMostRecentShift().EndTimeUtc == DateTime.MinValue;
            Dispatcher.Invoke(() =>
            {
                Clear();
                RefreshItemsSource();
            });
        }

        private void LVShiftsColumnHeader_Click(object sender, RoutedEventArgs e) => _sort =
            Functions.ListViewColumnHeaderClick(sender, _sort, LVShifts, "#CCCCCC");

        private void LVShifts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedShift = LVShifts.SelectedIndex >= 0 ? (Shift)LVShifts.SelectedItem : new Shift();
            BtnSave.IsEnabled = LVShifts.SelectedIndex >= 0;
            BtnNew.IsEnabled = LVShifts.SelectedIndex < 0;
            BtnClear.IsEnabled = LVShifts.SelectedIndex >= 0;
            _newTime = false;
            DisplayTimeIn();
            DisplayTimeOut();
            DisplayRole();
        }

        #endregion Click
    }
}