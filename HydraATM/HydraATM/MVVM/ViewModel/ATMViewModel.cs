using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HydraATM.MVVM.Model;
using Microsoft.UI.Xaml;

namespace HydraATM.MVVM.ViewModel
{
    public class ATMViewModel : INotifyPropertyChanged
    {
        private readonly ATMService _atmService;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                UpdateCommands();
            }
        }

        private string _surname;
        public string Surname
        {
            get => _surname;
            set
            {
                _surname = value;
                OnPropertyChanged();
                UpdateCommands();
            }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get => _accountNumber;
            set
            {
                _accountNumber = value;
                OnPropertyChanged();
                UpdateCommands();
            }
        }

        private string _amount;
        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
                UpdateCommands();
            }
        }

        public ObservableCollection<ATMModel> TransactionHistory => _atmService.GetTransactionHistory();

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand WithdrawCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }

        public ATMViewModel()
        {
            _atmService = new ATMService();

            WithdrawCommand = new RelayCommand(ExecuteWithdraw, CanExecuteWithdraw);
            ResetCommand = new RelayCommand(ExecuteReset);

            ResetFields();
        }

        private bool CanExecuteWithdraw()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Surname) &&
                   !string.IsNullOrWhiteSpace(AccountNumber) &&
                   !string.IsNullOrWhiteSpace(Amount) &&
                   decimal.TryParse(Amount, out decimal amount) &&
                   amount > 0;
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
        }

        private void UpdateCommands()
        {
            WithdrawCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}