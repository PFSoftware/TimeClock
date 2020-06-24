using PFSoftware.Extensions;
using PFSoftware.TimeClock.Models.Entities;
using System;

namespace TimeClock.Models.Entities
{
    public class Role : BaseINPC
    {
        private string _name;
        private decimal _payRate;
        private PayType _payType;
        private PayPeriod _payPeriod;

        /// <summary>Name of the <see cref="Role"/>.</summary>
        public string Name
        {
            get => _name;
            set { _name = value; NotifyPropertyChanged(nameof(Name)); }
        }

        /// <summary>Rate of pay for the <see cref="Role"/>.</summary>
        public decimal PayRate
        {
            get => _payRate;
            set { _payRate = value; NotifyPropertyChanged(nameof(PayRate), nameof(PayRateToString)); }
        }

        /// <summary>The type of pay the <see cref="Role"/> has, hourly or salary.</summary>
        internal PayType PayType
        {
            get => _payType; set
            {
                _payType = value;
                NotifyPropertyChanged(nameof(PayType));
            }
        }

        /// <summary>The rate at which the <see cref="Role"/> is paid.</summary>
        internal PayPeriod PayPeriod
        {
            get => _payPeriod; set
            {
                _payPeriod = value;
                NotifyPropertyChanged(nameof(PayPeriod));
            }
        }

        /// <summary>Rate of pay for the <see cref="Role"/>, formatted.</summary>
        public string PayRateToString => PayRate.ToString("C2");

        #region Override Operators

        private static bool Equals(Role left, Role right)
        {
            if (left is null && right is null) return true;
            if (left is null ^ right is null) return false;
            return string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase) && left.PayRate == right.PayRate && left.PayPeriod == right.PayPeriod && left.PayType == right.PayType;
        }

        public override bool Equals(object obj) => Equals(this, obj as Role);

        public bool Equals(Role other) => Equals(this, other);

        public static bool operator ==(Role left, Role right) => Equals(left, right);

        public static bool operator !=(Role left, Role right) => !Equals(left, right);

        public override int GetHashCode() => base.GetHashCode() ^ 17;

        public override string ToString() => Name;

        #endregion Override Operators

        public Role(string name, decimal payRate, PayType payType, PayPeriod payPeriod)
        {
            Name = name;
            PayRate = payRate;
            PayType = payType;
            PayPeriod = payPeriod;
        }
    }
}