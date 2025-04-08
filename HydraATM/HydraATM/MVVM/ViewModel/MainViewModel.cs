using HydraATM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HydraATM.MVVM.View;
using HydraATM.MVVM.Model;

namespace HydraATM.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private readonly IConfiguration _config;
        public RelayCommand ATMViewCommand { get; set; }
        public RelayCommand DepositViewCommand { get; set; }
        public RelayCommand BTCViewCommand { get; set; }

        public ATMViewModel ATMVM { get; set; }
        public DepositViewModel DepositVM { get; set; }
        public BTCViewModel BTCVM { get; set; }

        private object currentViewModel;

        public object CurrentViewModel
        {
            get { return currentViewModel; }
            set {currentViewModel = value; OnPropertyChanged();}
        }

        public MainViewModel()
        {
            ATMVM = new ATMViewModel();
            DepositVM = new DepositViewModel();
            BTCVM = new BTCViewModel();
            CurrentViewModel = ATMVM;

            ATMViewCommand = new RelayCommand(() =>
            {
                CurrentViewModel = ATMVM;
            });

            DepositViewCommand = new RelayCommand(() =>
            {
                CurrentViewModel = DepositVM;
            });

            BTCViewCommand = new RelayCommand(() =>
            {
                CurrentViewModel = BTCVM;
            });
        }
    }
}
