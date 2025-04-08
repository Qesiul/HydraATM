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
    public class ATMViewModel : INotifyPropertyChanged
    {
        private readonly ATMService _atmService;

        // Pola do formularza
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

        private string _amount;
        public string Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); UpdateCommands(); }
        }

        // Kolekcja transakcji + DataGrid
        public ObservableCollection<ATMModel> TransactionHistory => _atmService.GetTransactionHistory();

        // Aktualnie wybrana transakcja w DataGrid
        private ATMModel _selectedTransaction;
        public ATMModel SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged();

                // Gdy użytkownik wybierze wiersz w DataGrid, wypełniamy pola formularza
                if (_selectedTransaction != null)
                {
                    Name = _selectedTransaction.Name;
                    Surname = _selectedTransaction.Surname;
                    AccountNumber = _selectedTransaction.AccountNumber;
                    Amount = _selectedTransaction.Amount.ToString();
                }
                UpdateCommands();
            }
        }

        // Komunikat statusu (np. “Withdrawal successful”)
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // Komendy
        public RelayCommand WithdrawCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }
        public RelayCommand UpdateCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        // Konstruktor
        public ATMViewModel()
        {
            _atmService = new ATMService();

            WithdrawCommand = new RelayCommand(ExecuteWithdraw, CanExecuteWithdraw);
            ResetCommand = new RelayCommand(ExecuteReset);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteUpdate);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);

            ResetFields();
        }

        // --- Dodawanie nowej transakcji (Withdraw) ---
        private bool CanExecuteWithdraw()
        {
            return !string.IsNullOrWhiteSpace(Name)
                   && !string.IsNullOrWhiteSpace(Surname)
                   && !string.IsNullOrWhiteSpace(AccountNumber)
                   && !string.IsNullOrWhiteSpace(Amount)
                   && decimal.TryParse(Amount, out decimal amountVal)
                   && amountVal > 0;
        }

        private void ExecuteWithdraw()
        {
            if (!decimal.TryParse(Amount, out decimal amountValue))
            {
                StatusMessage = "Invalid amount";
                return;
            }

            var transaction = new ATMModel
            {
                Name = Name,
                Surname = Surname,
                AccountNumber = AccountNumber,
                Amount = amountValue
            };

            bool success = _atmService.ProcessWithdrawal(transaction);

            if (success)
            {
                StatusMessage = "Withdrawal successful";
                ResetFields();
            }
            else
            {
                StatusMessage = "Transaction failed. Please check your input.";
            }

            OnPropertyChanged(nameof(TransactionHistory));
        }

        // --- Reset pól formularza ---
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
            Amount = string.Empty;
            SelectedTransaction = null;
        }

        private bool CanExecuteUpdate()
        {
            return SelectedTransaction != null
                   && !string.IsNullOrWhiteSpace(Name)
                   && !string.IsNullOrWhiteSpace(Surname)
                   && !string.IsNullOrWhiteSpace(AccountNumber)
                   && !string.IsNullOrWhiteSpace(Amount)
                   && decimal.TryParse(Amount, out decimal amnt)
                   && amnt > 0;
        }

        private void ExecuteUpdate()
        {
            if (!decimal.TryParse(Amount, out decimal amountValue))
            {
                StatusMessage = "Invalid amount";
                return;
            }

            SelectedTransaction.Name = Name;
            SelectedTransaction.Surname = Surname;
            SelectedTransaction.AccountNumber = AccountNumber;
            SelectedTransaction.Amount = amountValue;

            StatusMessage = "Transaction updated!";
            OnPropertyChanged(nameof(TransactionHistory));
        }

        private bool CanExecuteDelete()
        {
            return SelectedTransaction != null;
        }

        private void ExecuteDelete()
        {
            if (SelectedTransaction != null)
            {
                _atmService.GetTransactionHistory().Remove(SelectedTransaction);
                ResetFields();
                StatusMessage = "Transaction removed!";
                OnPropertyChanged(nameof(TransactionHistory));
            }
        }

        private void UpdateCommands()
        {
            WithdrawCommand.RaiseCanExecuteChanged();
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
