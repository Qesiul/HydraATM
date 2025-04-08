using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraATM.MVVM.Model
{
    public class ATMModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }

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

        // Business logic for processing a withdrawal
        public bool ProcessWithdrawal(ATMModel transaction)
        {

            if (!transaction.ValidateTransaction())
                return false;

            _transactionHistory.Add(transaction);

            return true;
        }
    }
}