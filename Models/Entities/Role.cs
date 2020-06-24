using System;
using System.ComponentModel;

namespace TimeClock.Models.Entities
{
    public class Role : INotifyPropertyChanged, IEquatable<Role>
    {
        private string _name;
        private decimal _payRate;

        public string Name { get => _name; set => _name = value; }
        public decimal PayRate { get => _payRate; set => _payRate = value; }

        #region Data-Binding

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        #endregion Data-Binding

        #region Override Operators

        private static bool Equals(Role left, Role right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase) && left.PayRate == right.PayRate;
        }

        public override bool Equals(object obj) => Equals(this, obj as Role);

        public bool Equals(Role other) => Equals(this, other);

        public static bool operator ==(Role left, Role right) => Equals(left, right);

        public static bool operator !=(Role left, Role right) => !Equals(left, right);

        public override int GetHashCode() => base.GetHashCode() ^ 17;

        public override string ToString() => Name;

        #endregion Override Operators
    }
}