using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HydraATM.MVVM.Model
{
    public class ATMModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _surname;
        public string Surname
        {
            get => _surname;
            set { _surname = value; OnPropertyChanged(); }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        public bool ValidateTransaction()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname))
                return false;

            if (string.IsNullOrWhiteSpace(AccountNumber) || AccountNumber.Length < 5)
                return false;

            if (Amount <= 0)
                return false;

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ATMService
    {
        private ObservableCollection<ATMModel> _transactionHistory;

        public ATMService()
        {
            _transactionHistory = new ObservableCollection<ATMModel>();
        }

        public ObservableCollection<ATMModel> GetTransactionHistory()
        {
            return _transactionHistory;
        }

        public bool ProcessWithdrawal(ATMModel transaction)
        {
            if (!transaction.ValidateTransaction())
                return false;

            _transactionHistory.Add(transaction);
            return true;
        }
    }
}
