using System;
using System.Globalization;

namespace PFSoftware.TimeClock.Models.Entities
{
    /// <summary>Represents a shift that was started or worked.</summary>
    internal class Shift : BaseINPC
    {
        private int _id;
        private string _role;
        private readonly string fullDateFormat = @"yyyy-MM-dd hh\:mm\:ss tt";
        private readonly string shiftWeekFormat = "dd':'hh':'mm':'ss";
        private readonly string shiftDayFormat = "hh':'mm':'ss";
        private readonly CultureInfo culture = new CultureInfo("en-US");
        private DateTime _startTimeUtc, _endTimeUtc;
        private TimeSpan _startUtcOffset, _endUtcOffset;
        private bool _edited;

        #region Modifying Properties

        /// <summary>User ID</summary>
        public int ID
        {
            get => _id;
            private set { _id = value; NotifyPropertyChanged(nameof(ID)); }
        }

        /// <summary>The <see cref="User"/>'s role this <see cref="Shift"/>.</summary>
        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                NotifyPropertyChanged(nameof(Role));
            }
        }

        /// <summary>Time <see cref="Shift"/> started.</summary>
        public DateTime StartTimeUtc
        {
            get => _startTimeUtc;
            private set { _startTimeUtc = value; NotifyPropertyChanged(nameof(StartTimeUtc), nameof(StartTimeUtcToString), nameof(StartTimeLocal), nameof(StartTimeLocalToString), nameof(Length), nameof(LengthToString)); }
        }

        /// <summary>The UTC offset from the time the <see cref="Shift"/> started.</summary>
        public TimeSpan StartUtcOffset
        {
            get => _startUtcOffset;
            set { _startUtcOffset = value; NotifyPropertyChanged(nameof(StartUtcOffset), nameof(StartTimeLocal), nameof(StartTimeLocalToString)); }
        }

        /// <summary>Time <see cref="Shift"/> ended.</summary>
        public DateTime EndTimeUtc
        {
            get => _endTimeUtc;
            set
            {
                _endTimeUtc = value;
                NotifyPropertyChanged(nameof(EndTimeUtc), nameof(EndTimeUtcToString), nameof(EndTimeLocal), nameof(EndTimeLocalToString), nameof(Length), nameof(LengthToString));
            }
        }

        /// <summary>The UTC offset from the time the <see cref="Shift"/> ended.</summary>
        public TimeSpan EndUtcOffset
        {
            get => _endUtcOffset;
            set { _endUtcOffset = value; NotifyPropertyChanged(nameof(EndUtcOffset), nameof(EndTimeLocal), nameof(EndTimeLocalToString)); }
        }

        /// <summary>Has this <see cref="Shift"/> been edited?</summary>
        public bool Edited
        {
            get => _edited;
            set
            {
                _edited = value;
                NotifyPropertyChanged(nameof(Edited));
            }
        }

        #endregion Modifying Properties

        #region Helper Properties

        /// <summary>Time <see cref="Shift"/> started, formatted to string.</summary>
        public string StartTimeUtcToString => StartTimeUtc.ToString(fullDateFormat, culture);

        /// <summary>Time <see cref="Shift"/> ended, formatted to string.</summary>
        public string EndTimeUtcToString => EndTimeUtc != DateTime.MinValue ? EndTimeUtc.ToString(fullDateFormat, culture) : "";

        /// <summary>Time <see cref="Shift"/> started, formatted to local time.</summary>
        public DateTime StartTimeLocal => StartTimeUtc + StartUtcOffset;

        /// <summary>Time <see cref="Shift"/> ended in local time.</summary>
        public DateTime EndTimeLocal => EndTimeUtc + EndUtcOffset;

        /// <summary>Time <see cref="Shift"/> started, formatted to string.</summary>
        public string StartTimeLocalToString => StartTimeLocal.ToString(fullDateFormat, culture);

        /// <summary>Time <see cref="Shift"/> ended in local time, formatted to string.</summary>
        public string EndTimeLocalToString => EndTimeLocal != DateTime.MinValue ? EndTimeLocal.ToString(fullDateFormat, culture) : "";

        /// <summary>The UTC offset from the time the <see cref="Shift"/> started, formatted.</summary>
        public string StartUtcOffsetToString => StartUtcOffset.ToString();

        /// <summary>The UTC offset from the time the <see cref="Shift"/> ended, formatted.</summary>
        public string EndUtcOffsetToString => EndTimeUtc != DateTime.MinValue ? EndUtcOffset.ToString() : "";

        /// <summary>Length of <see cref="Shift"/>.</summary>
        public TimeSpan Length => EndTimeUtc != DateTime.MinValue ? EndTimeUtc - StartTimeUtc : DateTime.UtcNow - StartTimeUtc;

        /// <summary>Length of <see cref="Shift"/>, formatted to string.</summary>
        public string LengthToString => EndTimeUtc != DateTime.MinValue
            ? Length.Days > 0
            ? Length.ToString(shiftWeekFormat, culture)
            : Length.ToString(shiftDayFormat, culture)
            : "";

        #endregion Helper Properties

        #region Override Operators

        private static bool Equals(Shift left, Shift right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return left.ID == right.ID && left.Role == right.Role && left.StartTimeUtc == right.StartTimeUtc && left.EndTimeUtc == right.EndTimeUtc && left.Edited == right.Edited;
        }

        public override bool Equals(object obj) => Equals(this, obj as Shift);

        public bool Equals(Shift otherShift) => Equals(this, otherShift);

        public static bool operator ==(Shift left, Shift right) => Equals(left, right);

        public static bool operator !=(Shift left, Shift right) => !Equals(left, right);

        public override int GetHashCode() => base.GetHashCode() ^ 17;

        public override string ToString() => $"{ID}: {StartTimeUtc}, {EndTimeUtc}";

        #endregion Override Operators

        #region Constructors

        /// <summary>Initalizes a default instance of <see cref="Shift"/>.</summary>
        internal Shift()
        {
            StartTimeUtc = new DateTime();
            EndTimeUtc = new DateTime();
        }

        /// <summary>Initializes a new instance of <see cref="Shift"/> by assigning only the ShiftStart Property.</summary>
        /// <param name="id">ID</param>
        /// <param name="role"></param>
        /// <param name="start">Start time of <see cref="Shift"/></param>
        /// <param name="startOffset">The UTC offset from the time the <see cref="Shift"/> started</param>
        internal Shift(int id, string role, DateTime start, TimeSpan startOffset) : this(id, role, start, startOffset, new DateTime(), TimeSpan.Zero, false)
        {
        }

        /// <summary>Initializes a new instance of <see cref="Shift"/> by assigning Properties.</summary>
        /// <param name="id">ID</param>
        /// <param name="role">The <see cref="User"/>'s role this <see cref="Shift"/></param>
        /// <param name="start">Start of <see cref="Shift"/></param>
        /// <param name="startOffset">The UTC offset from the time the <see cref="Shift"/> started</param>
        /// <param name="end">End of <see cref="Shift"/></param>
        /// <param name="endOffset">The UTC offset from the time the <see cref="Shift"/> ended</param>
        /// <param name="edited">Has this <see cref="Shift"/> been edited?</param>
        internal Shift(int id, string role, DateTime start, TimeSpan startOffset, DateTime end, TimeSpan endOffset, bool edited)
        {
            ID = id;
            Role = role;
            StartTimeUtc = start;
            StartUtcOffset = startOffset;
            EndTimeUtc = end;
            EndUtcOffset = endOffset;
            Edited = edited;
        }

        /// <summary>Replaces this instance of <see cref="Shift"/> with another instance.</summary>
        /// <param name="other">Instance of <see cref="Shift"/> to replace this instance</param>
        internal Shift(Shift other) : this(other.ID, other.Role, other.StartTimeUtc, other.StartUtcOffset, other.EndTimeUtc, other.EndUtcOffset, other.Edited)
        {
        }

        #endregion Constructors
    }
}