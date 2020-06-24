using PFSoftware.Extensions;
using PFSoftware.TimeClock.Models;
using PFSoftware.TimeClock.Models.Entities;
using PFSoftware.TimeClock.Views.SharedPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace PFSoftware.TimeClock.Views.Users
{
    /// <summary>Interaction logic for PFSoftware.TimeClockPage.xaml</summary>
    public partial class TimeClockPage
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        /// <summary>Checks information regarding the In/Out button.</summary>
        private void CheckButton()
        {
            BtnInOut.Content = AppState.CurrentUser.LoggedIn ? "_OUT" : "_IN";
            if (!AppState.CurrentUser.LoggedIn)
                BtnInOut.IsEnabled = CmbRoles.SelectedIndex >= 0;
        }

        #region Button-Click Methods

        private async void BtnInOut_Click(object sender, RoutedEventArgs e)
        {
            BtnInOut.IsEnabled = false;

            if (!AppState.CurrentUser.LoggedIn)
            {
                Shift newShift = new Shift(AppState.CurrentUser.ID, CmbRoles.SelectedItem.ToString(), DateTime.Now);
                if (await AppState.LogIn(newShift).ConfigureAwait(false))
                {
                    AppState.CurrentUser.AddShift(newShift);
                    AppState.CurrentUser.LoggedIn = true;
                }
                _timer.Start();
            }
            else
            {
                Shift currentShift = new Shift(AppState.CurrentUser.GetMostRecentShift()) { EndTimeUtc = DateTime.Now };
                if (currentShift.Length > new TimeSpan(0, 0, 1) && await AppState.LogOut(currentShift).ConfigureAwait(false))
                {
                    AppState.CurrentUser.ModifyShift(AppState.CurrentUser.GetMostRecentShift(), currentShift);
                    AppState.CurrentUser.LoggedIn = false;
                }
                _timer.Stop();
            }
            List<Shift> allShifts = new List<Shift>(AppState.CurrentUser.Shifts);
            TimeSpan total = new TimeSpan();
            foreach (Shift shift in allShifts)
            {
                DateTime startOfWeek = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
                if (shift.StartTimeUtc >= startOfWeek)
                {
                    total.Add(shift.Length);
                }
            }
            TimeSpan ts = new TimeSpan(allShifts.Where(shift => shift.StartTimeUtc >= DateTime.Now.StartOfWeek(DayOfWeek.Sunday)).ToList().Sum(shift => shift.Length.Ticks));
            Dispatcher.Invoke(() =>
            {
                BtnInOut.IsEnabled = true;
                CheckButton();
            });
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e) => AppState.Navigate(
            new ChangePasswordPage());

        private void BtnLog_Click(object sender, RoutedEventArgs e) => AppState.Navigate(new UserLogPage());

        private void BtnBack_Click(object sender, RoutedEventArgs e) => ClosePage();

        #endregion Button-Click Methods

        private void Timer_Tick(object sender, EventArgs e) => AppState.CurrentUser.UpdateBindings();

        #region Page-Manipulation Methods

        /// <summary>Closes the Page.</summary>
        private void ClosePage() => AppState.GoBack();

        public TimeClockPage()
        {
            InitializeComponent();
            DataContext = AppState.CurrentUser;
            CmbRoles.ItemsSource = AppState.CurrentUser.Roles;
            CmbRoles.SelectedIndex = 0;
            CheckButton();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            if (AppState.CurrentUser.LoggedIn)
                _timer.Start();
        }

        private void CmbRoles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => CheckButton();

        #endregion Page-Manipulation Methods
    }
}