using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HydraATM.MVVM.Model;
using HydraATM.Core;

namespace HydraATM.MVVM.ViewModel
{
    public class BTCViewModel : INotifyPropertyChanged
    {
        private readonly BTCService _btcService;

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); UpdateCommands(); }
        }

        private string _surname;
        public string Surname
        {
            get => _surname;
            set { _surname = value; OnPropertyChanged(); UpdateCommands(); }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); UpdateCommands(); }
        }

        private string _walletAddress;
        public string WalletAddress
        {
            get => _walletAddress;
            set { _walletAddress = value; OnPropertyChanged(); UpdateCommands(); }
        }

        // Transaction type
        private bool _isBuyTransaction = true;
        public bool IsBuyTransaction
        {
            get => _isBuyTransaction;
            set
            {
                _isBuyTransaction = value;
                OnPropertyChanged();
                if (value)
                {
                    CurrentTransactionType = TransactionType.Buy;
                }
                UpdateCommands();
            }
        }

        private bool _isSellTransaction;
        public bool IsSellTransaction
        {
            get => _isSellTransaction;
            set
            {
                _isSellTransaction = value;
                OnPropertyChanged();
                if (value)
                {
                    CurrentTransactionType = TransactionType.Sell;
                }
                UpdateCommands();
            }
        }

        private TransactionType _currentTransactionType = TransactionType.Buy;
        public TransactionType CurrentTransactionType
        {
            get => _currentTransactionType;
            set
            {
                _currentTransactionType = value;
                CalculateBitcoinAmount();
                OnPropertyChanged();
            }
        }

        // Financial fields
        private string _fiatAmount;
        public string FiatAmount
        {
            get => _fiatAmount;
            set
            {
                _fiatAmount = value;
                CalculateBitcoinAmount();
                OnPropertyChanged();
                UpdateCommands();
            }
        }

        private decimal _bitcoinAmount;
        public decimal BitcoinAmount
        {
            get => _bitcoinAmount;
            set { _bitcoinAmount = value; OnPropertyChanged(); UpdateCommands(); }
        }

        private decimal _exchangeRate;
        public decimal ExchangeRate
        {
            get => _exchangeRate;
            set { _exchangeRate = value; OnPropertyChanged(); }
        }

        private decimal _transactionFee;
        public decimal TransactionFee
        {
            get => _transactionFee;
            set { _transactionFee = value; OnPropertyChanged(); }
        }

        // Transaction collection and DataGrid
        public ObservableCollection<BTCModel> TransactionHistory => _btcService.GetTransactionHistory();

        // Currently selected transaction in DataGrid
        private BTCModel _selectedTransaction;
        public BTCModel SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged();

                // When user selects a row in DataGrid, fill form fields
                if (_selectedTransaction != null)
                {
                    Name = _selectedTransaction.Name;
                    Surname = _selectedTransaction.Surname;
                    AccountNumber = _selectedTransaction.AccountNumber;
                    WalletAddress = _selectedTransaction.WalletAddress;
                    FiatAmount = _selectedTransaction.FiatAmount.ToString();
                    BitcoinAmount = _selectedTransaction.BitcoinAmount;

                    // Set transaction type radio buttons
                    if (_selectedTransaction.Type == TransactionType.Buy)
                    {
                        IsBuyTransaction = true;
                        IsSellTransaction = false;
                    }
                    else
                    {
                        IsBuyTransaction = false;
                        IsSellTransaction = true;
                    }
                }
                UpdateCommands();
            }
        }

        // Status message
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // Commands
        public RelayCommand ProcessTransactionCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }
        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        // Constructor
        public BTCViewModel()
        {
            _btcService = new BTCService();

            // Initialize commands
            ProcessTransactionCommand = new RelayCommand(ExecuteProcessTransaction, CanExecuteProcessTransaction);
            ResetCommand = new RelayCommand(ExecuteReset);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteUpdate);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);

            // Initialize exchange rate and fee
            ExchangeRate = _btcService.GetCurrentExchangeRate();
            TransactionFee = _btcService.GetTransactionFee();

            // Initialize form
            ResetFields();
        }

        private void CalculateBitcoinAmount()
        {
            if (!decimal.TryParse(FiatAmount, out decimal fiatValue) || ExchangeRate <= 0)
            {
                BitcoinAmount = 0;
                return;
            }

            decimal effectiveAmount = fiatValue;

            // Apply transaction fee for buying Bitcoin
            if (CurrentTransactionType == TransactionType.Buy)
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

        // --- Process Transaction (Buy/Sell Bitcoin) ---
        private bool CanExecuteProcessTransaction()
        {
            return !string.IsNullOrWhiteSpace(Name)
                   && !string.IsNullOrWhiteSpace(Surname)
                   && !string.IsNullOrWhiteSpace(AccountNumber)
                   && !string.IsNullOrWhiteSpace(WalletAddress)
                   && WalletAddress.Length >= 26 // Simplified validation for BTC addresses
                   && !string.IsNullOrWhiteSpace(FiatAmount)
                   && decimal.TryParse(FiatAmount, out decimal fiatValue)
                   && fiatValue > 0
                   && BitcoinAmount > 0;
        }

        private void ExecuteProcessTransaction()
        {
            if (!decimal.TryParse(FiatAmount, out decimal fiatValue))
            {
                StatusMessage = "Invalid amount";
                return;
            }

            var transaction = new BTCModel
            {
                Name = Name,
                Surname = Surname,
                AccountNumber = AccountNumber,
                WalletAddress = WalletAddress,
                FiatAmount = fiatValue,
                BitcoinAmount = BitcoinAmount,
                ExchangeRate = ExchangeRate,
                TransactionFee = TransactionFee,
                Type = CurrentTransactionType
            };

            bool success = _btcService.ProcessTransaction(transaction);

            if (success)
            {
                StatusMessage = $"Bitcoin {(CurrentTransactionType == TransactionType.Buy ? "purchase" : "sale")} successful!";
                ResetFields();
            }
            else
            {
                StatusMessage = "Transaction failed. Please check your input.";
            }

            OnPropertyChanged(nameof(TransactionHistory));
        }

        // --- Reset form fields ---
        private void ExecuteReset()
        {
            ResetFields();
            StatusMessage = string.Empty;
        }

        private void ResetFields()
        {
            Name = string.Empty;
            Surname = string.Empty;
            AccountNumber = string.Empty;
            WalletAddress = string.Empty;
            FiatAmount = string.Empty;
            BitcoinAmount = 0;
            IsBuyTransaction = true;
            IsSellTransaction = false;
            SelectedTransaction = null;
        }

        // --- Update transaction ---
        private bool CanExecuteUpdate()
        {
            return SelectedTransaction != null
                   && !string.IsNullOrWhiteSpace(Name)
                   && !string.IsNullOrWhiteSpace(Surname)
                   && !string.IsNullOrWhiteSpace(AccountNumber)
                   && !string.IsNullOrWhiteSpace(WalletAddress)
                   && !string.IsNullOrWhiteSpace(FiatAmount)
                   && decimal.TryParse(FiatAmount, out decimal fiatValue)
                   && fiatValue > 0
                   && BitcoinAmount > 0;
        }

        private void ExecuteUpdate()
        {
            if (!decimal.TryParse(FiatAmount, out decimal fiatValue))
            {
                StatusMessage = "Invalid amount";
                return;
            }

            SelectedTransaction.Name = Name;
            SelectedTransaction.Surname = Surname;
            SelectedTransaction.AccountNumber = AccountNumber;
            SelectedTransaction.WalletAddress = WalletAddress;
            SelectedTransaction.FiatAmount = fiatValue;
            SelectedTransaction.BitcoinAmount = BitcoinAmount;
            SelectedTransaction.Type = CurrentTransactionType;

            StatusMessage = "Transaction updated!";
            OnPropertyChanged(nameof(TransactionHistory));
        }

        // --- Delete transaction ---
        private bool CanExecuteDelete()
        {
            return SelectedTransaction != null;
        }

        private void ExecuteDelete()
        {
            if (SelectedTransaction != null)
            {
                _btcService.GetTransactionHistory().Remove(SelectedTransaction);
                ResetFields();
                StatusMessage = "Transaction removed!";
                OnPropertyChanged(nameof(TransactionHistory));
            }
        }

        private void UpdateCommands()
        {
            ProcessTransactionCommand.RaiseCanExecuteChanged();
            UpdateCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}