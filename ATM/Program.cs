using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ATM
{
    class Program
    {
        static List<AccountClass> AccountsList = new List<AccountClass>();

        static string filePath = Path.Combine(Environment.CurrentDirectory, "Accounts.json");

        static int AccountNumber = -1;

        static bool LoggedIn = false;

        static void Main(string[] args)
        {
            //Get the accounts from the Json File
            RetrieveAccountsFileToList();

            Console.WriteLine("Choose an action (Sign In: 0 or Sign Up: 1)");
            ActionDecisionMethod(Convert.ToInt32(Console.ReadLine()));

            int Action = -1;
            if (AccountNumber > -1 && LoggedIn)
            {
                Console.WriteLine($"\nWelcome {AccountsList[AccountNumber].name}:");

                while (Action != 7)
                {
                    Console.WriteLine($"\nChoose an action (Balance: 2 / Deposit: 3 / Withdraw: 4 / Transfer: 5 / Transactions: 6 / Logout: 7)");
                    Action = Convert.ToInt32(Console.ReadLine());
                    if (Action != 0 && Action != 1)
                    {
                        ActionDecisionMethod(Action);
                    }else{
                        Console.WriteLine("\nChoose the right action...");
                    }
                }
            }
        }


        //Create list with dummy data as an example
        public static void SetupDummyData()
        {
            List<TransactionsClass> transactionsList = new List<TransactionsClass>();
            transactionsList.Add(new TransactionsClass() { FromID = 0, ToID = 1, TransactionType = "Transfer", TransactionAmount = 100 });
            transactionsList.Add(new TransactionsClass() { FromID = 1, ToID = 0, TransactionType = "Transfer", TransactionAmount = 50 });


            List<TransactionsClass> transactionsList2 = new List<TransactionsClass>();
            transactionsList2.Add(new TransactionsClass() { FromID = 0, ToID = 1, TransactionType = "Transfer", TransactionAmount = 100 });
            transactionsList2.Add(new TransactionsClass() { FromID = 1, ToID = 0, TransactionType = "Transfer", TransactionAmount = 50 });


            AccountsList.Add(new AccountClass() { id = 0, pin = 1111, name = "Mario", wallet = 50, Transactions = transactionsList });
            AccountsList.Add(new AccountClass() { id = 1, pin = 1111, name = "Maria", wallet = 50, Transactions = transactionsList2 });
        }

        //Create File of the accounts 
        public static void CreateAccountsFile()
        {
            try
            {
                //Allow the file to be written with identention
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                //Serialize List to json
                var JsonString = JsonSerializer.Serialize(AccountsList, options);

                //Write the Json File
                File.WriteAllText(filePath, JsonString);
            }
            catch (IOException ioexp)
            {
                Console.WriteLine("Error: {0}", ioexp.Message);
            }
        }

        //Read Data from File 
        public static void RetrieveAccountsFileToList()
        {
            //Retrieve the Json File in Bytes
            byte[] jsonUtf8Bytes = File.ReadAllBytes(filePath);

            if (jsonUtf8Bytes.Length > 0) //Only if the json file contains data
            {
                //Convert the Json Byte Array into a list
                var utf8Reader = new Utf8JsonReader(jsonUtf8Bytes);
                AccountsList = JsonSerializer.Deserialize<List<AccountClass>>(ref utf8Reader);
            }
            else
            {
                Console.WriteLine("No accounts found.");
            }



        }

        //Function with the job to take decisions in the algorithm
        public static void ActionDecisionMethod(int Action)
        {
            switch (Action)
            {
                case 0:
                    SignIn();
                    break;
                case 1:
                    SignUp();
                    CreateAccountsFile();
                    break;
                case 2:
                    CheckBalance();
                    break;
                case 3:
                    Deposit();
                    CreateAccountsFile();
                    break;
                case 4:
                    Withdraw();
                    CreateAccountsFile();
                    break;
                case 5:
                    CreateNewTransaction();
                    CreateAccountsFile();
                    break;
                case 6:
                    CheckTransactions();
                    break;
                case 7:
                    LoggedIn = false;
                    break;
                default:
                    Console.WriteLine("Something went wrong...");
                    break;
            };
        }

        //Function with the job to SignIn
        public static void SignIn()
        {
            Console.WriteLine("Insert Account Number:");
            AccountNumber = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Insert Account Pin:");
            int AccountPin = Convert.ToInt32(Console.ReadLine());

            if (AccountsList[AccountNumber].pin == AccountPin)
            {
                LoggedIn = true;
            }
            else
            {
                Console.WriteLine("The Account Sign In is Incorrect.");
                LoggedIn = false;
            }
        }

        //Function with the job to SignUp
        public static void SignUp()
        {
            Console.WriteLine("Insert your Name: ");
            string nameInput = Console.ReadLine();
            Console.WriteLine("Choose a pin of 4 digits for your Account: ");
            int pinInput = Convert.ToInt32(Console.ReadLine());
            int AccountNumber = AccountsList.Count;

            AccountsList.Add(new AccountClass { id = AccountNumber, name = nameInput, pin = pinInput });

            Console.WriteLine($"\nWelcome {AccountsList[AccountNumber].name}:"
             + $"\n Account Number: {AccountsList[AccountNumber].id}"
             + $"\n Account Pin: {AccountsList[AccountNumber].pin}");
        }

        //Function that checks account balance of the current user
        public static void CheckBalance()
        {
            if (AccountNumber > -1 && LoggedIn)
            {
                Console.WriteLine($"\nYour balance is: {AccountsList[AccountNumber].wallet} euros.");
            }
        }


        //Function that check the Transactions History
        public static void CheckTransactions()
        {
            if (AccountNumber > -1 && LoggedIn)
            {
                Console.WriteLine("\nYour Transactions:");
                foreach (var item in AccountsList[AccountNumber].Transactions)
                {
                    if (item.TransactionType.Equals("Transfer"))
                    {
                        if (item.FromID == AccountsList[AccountNumber].id)
                        {
                            Console.WriteLine($" {item.TransactionType} -{item.TransactionAmount} euros to: {AccountsList[item.ToID].name}.");
                        }
                        else
                        {
                            Console.WriteLine($" {item.TransactionType} +{item.TransactionAmount} euros from: {AccountsList[item.FromID].name}.");
                        }
                    }
                    else if (item.TransactionType.Equals("Deposit"))
                    {
                        Console.WriteLine($" {item.TransactionType} +{item.TransactionAmount} euros.");
                    }
                    else if (item.TransactionType.Equals("Withdraw"))
                    {
                        Console.WriteLine($" {item.TransactionType} -{item.TransactionAmount} euros.");
                    }
                }
            }
        }

        //Function that takes care of the deposit
        public static void Deposit()
        {
            Console.WriteLine("Insert the amount you would like to deposit:");
            double Amount = Convert.ToDouble(Console.ReadLine());
            AccountsList[AccountNumber].DepositMoney(Amount);
            //Add a new transaction to the list
            AccountsList[AccountNumber].Transactions.Add(new TransactionsClass { TransactionType = "Deposit", TransactionAmount = Amount });
            Console.WriteLine($"Deposit of: {Amount} Succesful!");
        }


        //Function that takes care of the withdraw 
        public static void Withdraw()
        {
            Console.WriteLine("Insert the amount you would like to withdraw:");
            double Amount = Convert.ToDouble(Console.ReadLine());
            AccountsList[AccountNumber].WithdrawMoney(Amount);
            AccountsList[AccountNumber].Transactions.Add(new TransactionsClass { TransactionType = "Withdraw", TransactionAmount = Amount });
            Console.WriteLine($"Withdraw of: {Amount}  Succesful!");
        }


        //Function that creates the new transaction 
        public static void CreateNewTransaction()
        {
            Console.WriteLine("\nInsert the account number you would like to transfer to:");
            int AccNumTo = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Insert the amount:");
            int amount = Convert.ToInt32(Console.ReadLine());


            AccountsList[AccountNumber].Transactions.Add(new TransactionsClass
            {
                FromID = AccountsList[AccountNumber].id,
                ToID = AccNumTo,
                TransactionType = "Transfer",
                TransactionAmount = amount
            });


            AccountsList[AccNumTo].Transactions.Add(new TransactionsClass
            {
                FromID = AccountNumber,
                ToID = AccNumTo,
                TransactionType = "Transfer",
                TransactionAmount = amount
            });


            Console.WriteLine("Transaction Succesful!");
        }






























    }
}

