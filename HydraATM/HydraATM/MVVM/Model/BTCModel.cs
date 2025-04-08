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
    public enum TransactionType
    {
        Buy,
        Sell
    }

    public class BTCModel : INotifyPropertyChanged
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

        private string _walletAddress;
        public string WalletAddress
        {
            get => _walletAddress;
            set { _walletAddress = value; OnPropertyChanged(); }
        }

        private decimal _fiatAmount;
        public decimal FiatAmount
        {
            get => _fiatAmount;
            set
            {
                _fiatAmount = value;
                CalculateBitcoinAmount();
                OnPropertyChanged();
            }
        }

        private decimal _bitcoinAmount;
        public decimal BitcoinAmount
        {
            get => _bitcoinAmount;
            set { _bitcoinAmount = value; OnPropertyChanged(); }
        }

        private decimal _exchangeRate;
        public decimal ExchangeRate
        {
            get => _exchangeRate;
            set
            {
                _exchangeRate = value;
                CalculateBitcoinAmount();
                OnPropertyChanged();
            }
        }

        private decimal _transactionFee;
        public decimal TransactionFee
        {
            get => _transactionFee;
            set
            {
                _transactionFee = value;
                CalculateBitcoinAmount();
                OnPropertyChanged();
            }
        }

        private TransactionType _type;
        public TransactionType Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get => _timestamp;
            set { _timestamp = value; OnPropertyChanged(); }
        }

        private void CalculateBitcoinAmount()
        {
            if (ExchangeRate <= 0)
                return;

            decimal effectiveAmount = FiatAmount;
            if (Type == TransactionType.Buy)
            {
                effectiveAmount -= TransactionFee;
            }

            if (effectiveAmount > 0)
            {
                BitcoinAmount = effectiveAmount / ExchangeRate;
            }
            else
            {
                BitcoinAmount = 0;
            }
        }

        public bool ValidateTransaction()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Surname))
                return false;

            if (string.IsNullOrWhiteSpace(AccountNumber) || AccountNumber.Length < 5)
                return false;

            if (string.IsNullOrWhiteSpace(WalletAddress) || WalletAddress.Length < 26)
                return false;

            if (FiatAmount <= 0)
                return false;

            if (ExchangeRate <= 0)
                return false;

            if (BitcoinAmount <= 0)
                return false;

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BTCService
    {
        private ObservableCollection<BTCModel> _transactionHistory;
        private readonly decimal _defaultExchangeRate = 55000.00m; // Example rate: $55,000 per BTC
        private readonly decimal _defaultFee = 2.50m; // $2.50 transaction fee

        public BTCService()
        {
            _transactionHistory = new ObservableCollection<BTCModel>();
        }

        public ObservableCollection<BTCModel> GetTransactionHistory()
        {
            return _transactionHistory;
        }

        public decimal GetCurrentExchangeRate()
        {
            // In a real app, this would fetch the current rate from an API
            return _defaultExchangeRate;
        }

        public decimal GetTransactionFee()
        {
            return _defaultFee;
        }

        public bool ProcessTransaction(BTCModel transaction)
        {
            if (!transaction.ValidateTransaction())
                return false;

            // Set timestamp
            transaction.Timestamp = DateTime.Now;

            _transactionHistory.Add(transaction);
            return true;
        }
    }
}