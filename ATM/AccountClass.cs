using System;
using System.Collections.Generic;

namespace ATM{
    class AccountClass{
        public int id { get; set; } 
        
        public int pin { get; set; }

        public string name { get; set; }

        public double wallet { get; set;} 

        public List<TransactionsClass> Transactions {get; set;}

        
        public void DepositMoney(double money){
            this.wallet +=  money;
        }

        public void WithdrawMoney(double money){
            this.wallet -= money;
        }

        public void CheckWallet(){
            Console.WriteLine($"Account Balance: {this.wallet}");
        }
    }
}