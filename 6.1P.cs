using System;
using System.Collections.Generic;

namespace try_1
{
    public enum MenuOption
    {
        AddAccount, Withdraw, Deposit, Print, Transfer, TransactionHistory, Quit
    }
    class Account
    {
        public decimal accbalance;
        private string accname;
        //Class Properties
        public string Name
        {
            get { return accname; }
        }

        //Parameterized constructor
        public Account(string name, decimal balance)
        {
            this.accname = name;
            if (balance <= 0)
                return;
            this.accbalance = balance;
        }

        //Method to increase the balance in an account
        public void Deposit(decimal _amount)
        {
            this.accbalance += _amount;
            Console.WriteLine("Status Changing");
        }

        //Method to decrease the balance in an account  
        public void withdraw(decimal _amount)
        {


            if (this.accbalance >= _amount)
            {
                this.accbalance = this.accbalance - _amount;
            }

            else
            {
                Console.WriteLine("Insufficient Balance!");
            }

        }

        public void Print()
        {
            Console.WriteLine();
            Console.WriteLine("========Account Details========");
            Console.WriteLine();
            Console.WriteLine("Balance in your Account:  " + Convert.ToString(this.accbalance));
            Console.WriteLine("Name of the Account Holder:  " + this.accname);
        }
    }

    public abstract class Transaction
    {
        protected decimal _amount;
        protected bool _success;
        private bool _executed;
        private bool _reversed;
        private DateTime _dateStamp;

        public abstract bool Success
        {
            get;
        }

        public bool Executed
        {
            get { return _executed; }
        }

        public bool Reversed
        {
            get { return _reversed; }
        }

        public DateTime DateStamp
        {
            get { return _dateStamp; }
        }

        public Transaction(decimal _amount)
        {
            this._amount = _amount;
            _success = false;
            _executed = false;
            _reversed = false;
            _dateStamp = new DateTime();
        }

        public abstract void Print();

        public virtual void Execute()
        {
            _dateStamp = DateTime.Now;
            _executed = true;
            _success = true;
            _reversed = false;
        }

        public virtual void Rollback()
        {
            _dateStamp = DateTime.Now;
            _executed = false;
            _success = false;
            _reversed = true;
        }
    }
    class WithdrawTransaction : Transaction
    {
        //Class variables
        private Account account;

        //Class Properties
        public override bool Success
        { get { return _success; } }

        //Function which will be used to get confirmation before proceeding with the transaction
        public bool Input(string prompt)
        {
            bool answered = false;
            do
            {
                Console.WriteLine(prompt);
                string answer = Console.ReadLine();
                if (answer.ToUpper() == "CONFIRM")
                {
                    return true;
                }

                else if (answer.ToUpper() == "CANCEL")
                {
                    return false;
                }
            } while (!answered);
            return false;
        }

        //Class constructor
        public WithdrawTransaction(decimal amount, Account account) : base(amount)
        {
            this.account = account;
        }

        //Member to print the account details
        public override void Print()
        {
            account.Print();
            if (Success)
            {
                Console.WriteLine("Time stamp (Successful Transaction ) " + DateStamp);
            }

            else
            {
                Console.WriteLine("Time Stamp :: " + DateStamp);
            }
        }

        //Member to actually execute the transaction
        public override void Execute()
        {
            if (_success)
            {
                throw new InvalidOperationException("Transaction has already taken place");
            }
            else if (_amount > account.accbalance)
            {
                throw new InvalidOperationException("Oops! Insufficient Funds in your account.");
            }

            else if (_amount <= 0)
            {
                throw new InvalidOperationException("Enter a valid withdrawl amount Here :");
            }
            else
            {
                if (Input("Confirm to Withdraw the Amount"))
                {
                    account.withdraw(_amount);
                    base.Execute();

                    Console.WriteLine("Transaction was successful ! , Your Account Details: ");
                    Print();
                }
            }
        }

        //Member to rollback the transaction
        public override void Rollback()
        {
            if (Reversed)
            {
                throw new InvalidOperationException("Transaction has already been rolled !!");
            }
            if (_success)
            {
                if (Input("Undo the Transaction ! Please Confirm"))
                {
                    account.accbalance = account.accbalance + _amount;
                    base.Rollback();
                    Console.WriteLine(" Account Details: ");
                    Print();
                }
            }
            else
            {
                throw new InvalidOperationException("Sorry ! Rollback of the Transaction Failed");
            }
        }
    }
    class DepositTransaction : Transaction
    {
        //Class variables
        private Account account;

        //Class Properties
        public override bool Success
        { get { return _success; } }

        //Function which will be used to get confirmation before proceeding with the transaction
        public bool Input(string prompt)
        {
            bool answered = false;
            do
            {
                Console.WriteLine(prompt);
                string answer = Console.ReadLine();
                if (answer.ToUpper() == "CONFIRM")
                {
                    return true;
                }

                else if (answer.ToUpper() == "CANCEL")
                {
                    return false;
                }
            } while (!answered);
            return false;
        }

        //Class constructor
        public DepositTransaction(Account account, decimal amount) : base(amount)
        {
            this.account = account;
        }

        //Member to print the account details
        public override void Print()
        {
            account.Print();
            if (Success)
            {
                Console.WriteLine("Time stamp (Successful Transaction ) " + DateStamp);
            }

            else
            {
                Console.WriteLine("Time Stamp :: " + DateStamp);
            }
        }

        //Member that will actually carry out the transaction
        public override void Execute()
        {
            if (Success)
            {
                throw new InvalidOperationException("Transaction has already taken place");
            }

            else if (_amount <= 0)
            {
                throw new InvalidOperationException("Enter a valid Deposit amount Here");
            }
            else
            {
                if (Input("Are you sure you want to Deposit this amount the entered Amount ?"))
                {
                    account.Deposit(_amount);
                    base.Execute();

                    Console.WriteLine("Transaction was successful ! , Your Account Details: ");
                    Print();
                }
            }
        }

        //Member to rollback the transaction
        public override void Rollback()
        {
            if (Reversed)
            {
                throw new InvalidOperationException("Transaction has already been rolled back !!");
            }
            if (Success)
            {
                if (Input("Undo the Transaction ! Please Confirm"))
                {
                    account.accbalance = account.accbalance - _amount;
                    base.Rollback();
                    Console.WriteLine("Current Account details: ");
                    Print();
                }
            }
            else
            {
                throw new InvalidOperationException("Sorry ! Rollback of the Transaction Failed ");
            }
        }
    }

    class TransferTransaction : Transaction
    {
        //Class variables
        private Account fromAccount;
        private Account toAccount;
        private DepositTransaction _deposit;
        private WithdrawTransaction _withdraw;

        //Class Properties
        public override bool Success
        { get { return _deposit.Success && _withdraw.Success; } }

        //Function which will be used to get confirmation before proceeding with the transaction
        public bool Input(string prompt)
        {
            bool answered = false;
            do
            {
                Console.WriteLine(prompt);
                string answer = Console.ReadLine();
                if (answer.ToUpper() == "CONFIRM")
                {
                    return true;
                }

                else if (answer.ToUpper() == "CANCEL")
                {
                    return false;
                }
            } while (!answered);
            return false;
        }

        //Class constructor
        public TransferTransaction(Account fromAccount, Account toAccount, decimal amount) : base(amount)
        {
            this.fromAccount = fromAccount;
            this.toAccount = toAccount;
            _deposit = new DepositTransaction(toAccount, this._amount);
            _withdraw = new WithdrawTransaction(this._amount, fromAccount);
        }

        //Member to print the account details
        public override void Print()
        {
            Console.WriteLine("----Transfer of Funds---- ");
            Console.WriteLine(" From Account " + fromAccount.Name + "  To the account " + toAccount.Name);
            Console.WriteLine("Current Details of the Account :-");
            _withdraw.Print();
            _deposit.Print();
        }

        //Member to actually execute the transaction
        public override void Execute()
        {
            if (_deposit.Success && _withdraw.Success)
            {
                throw new InvalidOperationException("Transaction has already been placed");
            }
            else if (_amount > fromAccount.accbalance)
            {
                throw new InvalidOperationException("Transaction Failed ! , Insufficient funds in Your Account ..");
            }

            else if (_amount <= 0)
            {
                throw new InvalidOperationException("Enter a valid withdrawl amount to Continue:");
            }
            else
            {
                if (Input("Confirm the Transfer of the enterd amount Here "))
                {
                    _withdraw.Execute();
                    if (_withdraw.Success)
                    {
                        _deposit.Execute();
                        if (_deposit.Success)
                        {
                            base.Execute();

                            Console.WriteLine("Transaction SUCCESSFUL");
                            Print();
                        }
                        else
                        {
                            _withdraw.Rollback();
                            _deposit.Rollback();
                        }
                    }
                    else
                    {
                        _withdraw.Rollback();
                    }
                }
            }
        }

        //Member to rollback the transaction
        public void rollback()
        {
            if (Reversed)
            {
                throw new InvalidOperationException("Transaction has already been rolled back");
            }

            if (Input("Undo the Transaction ! Please Confirm"))
            {
                _deposit.Rollback();
                _withdraw.Rollback();
                base.Rollback();
                Console.WriteLine("Current Account details: ");
                Print();
            }

            else
            {
                throw new InvalidOperationException("Sorry ! Rollback of the Transaction Failed ");
            }
        }
    }

    class Bank
    {
        private List<Account> _accounts;
        private List<Transaction> _transactions;


        public int TransactionCount
        {
            get { return _transactions.Count; }
        }


        public Bank()
        {
            _accounts = new List<Account>();
            _transactions = new List<Transaction>();
        }

        public void AddAccount(Account account)
        {
            _accounts.Add(account);
        }

        //Member that returns the number of accounts in the bank
        public int GetNumberOfAccounts()
        {
            return _accounts.Count;
        }

        public Account GetAccount(string name)
        {

            string LowerString = name.ToLower();
            for (int i = 0; i < _accounts.Count; i++)
            {
                if (_accounts[i].Name.ToLower() == LowerString)
                {
                    return _accounts[i];
                }
            }


            return null;
        }


        public void ExecuteTransaction(Transaction transaction)
        {
            transaction.Execute();
            _transactions.Add(transaction);
        }


        public void RollbackTransaction(int index)
        {
            _transactions[index].Rollback();
        }


        public void PrintTransactionHistory()
        {
            for (int i = 0; i < TransactionCount; i++)
            {
                Console.Write((i + 1) + ") ");
                _transactions[i].Print();
                Console.WriteLine("\n");
            }
        }
    }

    class BankSystem
    {
        static void Main(string[] args)
        {
            try
            {
                bool loop = false;
                //Bank class to be used in the banking system
                Bank CurrentBank = new Bank();

                //Get the number of elements in the list
                Console.WriteLine("Number of Accounts you want to ADD :");
                int listMax = Convert.ToInt32(Console.ReadLine());
                while (listMax < 1)
                {
                    Console.WriteLine("Enter a positive number");
                    listMax = Convert.ToInt32(Console.ReadLine());
                }

                //Add elements into the list
                for (int i = 0; i < listMax; i++)
                {
                    Console.WriteLine("Account holders's Name :");
                    string InName = Console.ReadLine();
                    int num;
                    while (int.TryParse(InName, out num))
                    {
                        Console.WriteLine("Account Holder's Name :");
                        InName = Console.ReadLine();
                    }
                    Console.WriteLine("Current Balance :");
                    decimal Balance = Convert.ToDecimal(Console.ReadLine());
                    while (Balance <= 0)
                    {
                        Console.WriteLine("Enter a valid account balance");
                        Balance = Convert.ToDecimal(Console.ReadLine());
                    }
                    Account o = new Account(InName, Balance);
                    CurrentBank.AddAccount(o);
                }

                //Enter the menu and let the customer perform any transaction of choice
                do
                {
                    MenuOption choice = GetUserChoice();
                    switch (choice)
                    {
                        case MenuOption.AddAccount:
                            Console.WriteLine("Account Holder's Name :");
                            string InName = Console.ReadLine();
                            int num;
                            while (int.TryParse(InName, out num))
                            {
                                Console.WriteLine("Account Holder's Name :");
                                InName = Console.ReadLine();
                            }
                            Console.WriteLine("Current Balance :");
                            decimal Balance = Convert.ToDecimal(Console.ReadLine());
                            while (Balance <= 0)
                            {
                                Console.WriteLine("Enter a valid account balance");
                                Balance = Convert.ToDecimal(Console.ReadLine());
                            }
                            Account NewAccount = new Account(InName, Balance);
                            CurrentBank.AddAccount(NewAccount);
                            loop = true;
                            break;

                        //Allow the customer to withdraw money
                        case MenuOption.Withdraw:
                            DoWithdaw(CurrentBank);
                            loop = true;
                            break;

                        //Allow the customer to deposit money
                        case MenuOption.Deposit:
                            DoDeposit(CurrentBank);
                            loop = true;
                            break;

                        //Print the customer's account details
                        case MenuOption.Print:
                            DoPrint(CurrentBank);
                            loop = true;
                            break;

                        //Allow the customer to transfer money to another account
                        case MenuOption.Transfer:
                            //Stop transaction if there is only one account
                            if (CurrentBank.GetNumberOfAccounts() <= 1)
                            {
                                Console.WriteLine("Sorry No account to transfer ...");
                                break;
                            }

                            //Transfer
                            DoTransfer(CurrentBank);
                            loop = true;
                            break;

                        //Rollback a particular transaction
                        case MenuOption.TransactionHistory:
                            TransHistory(CurrentBank);
                            break;

                        //Allow the customer to quit
                        case MenuOption.Quit:
                            Console.WriteLine("Quiting ..... the BANKING SYSTEM ......");
                            loop = false;
                            break;
                    }
                } while (loop);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static MenuOption GetUserChoice()
        {
            bool testLoop = false;
            int MenuInt = 0;
            Console.WriteLine();
            Console.WriteLine("========== BANK SYSTEM MENU ==========");
            Console.WriteLine("Press 1: To Add an Account to the Bank System");
            Console.WriteLine("Press 2: To Withdraw a certain amount");
            Console.WriteLine("Press 3: To Deposit in your Account ");
            Console.WriteLine("Press 4: To Print User's Account Details ");
            Console.WriteLine("Press 5: Transfer of Funds between Accounts ");
            Console.WriteLine("Press 6: To check Transaction History");
            Console.WriteLine("Press 7: Quit the Program");
            Console.WriteLine("Enter any option : ");
            do
            {
                MenuInt = Convert.ToInt32(Console.ReadLine());
                if (MenuInt <= 7 && MenuInt >= 1)
                {
                    testLoop = false;
                }
                else
                {
                    testLoop = true;
                    Console.WriteLine("Choose a valid option");
                }
            } while (testLoop);
            return (MenuOption)(MenuInt - 1);
        }

        public static Account UserAccount(Bank CurrentBank)
        {
            Console.WriteLine("Enter your name");
            string UserName = Console.ReadLine();
            int numTest;
            while (int.TryParse(UserName, out numTest))
            {
                Console.WriteLine("Enter the your name");
                UserName = Console.ReadLine();
            }
            Account Self = CurrentBank.GetAccount(UserName);
            return Self;
        }

        public static void DoDeposit(Bank UserBank)
        {
            decimal amount = 0;
            bool validNumber = false, loop = false;
            Account account = UserAccount(UserBank);
            if (account != null)
            {
                do
                {
                    Console.WriteLine("Enter the amount you want to deposit ");
                    validNumber = decimal.TryParse(Console.ReadLine(), out amount);
                    if (validNumber)
                    {
                        DepositTransaction newDeposit = new DepositTransaction(account, amount);
                        UserBank.ExecuteTransaction(newDeposit);
                        loop = false;
                    }
                    else
                    {
                        Console.WriteLine("Enter a valid amount");
                        loop = true;
                    }
                } while (loop);
            }
            else
            {
                Console.WriteLine("Account not found");
            }
        }

        public static void DoWithdaw(Bank UserBank)
        {
            decimal amount = 0;
            bool validNumber = false;
            bool loop = false;
            Account account = UserAccount(UserBank);
            if (account != null)
            {
                do
                {
                    Console.WriteLine("Enter the amount you want to withdraw ");
                    validNumber = decimal.TryParse(Console.ReadLine(), out amount);
                    if (validNumber)
                    {
                        WithdrawTransaction newWithdrawl = new WithdrawTransaction(amount, account);
                        UserBank.ExecuteTransaction(newWithdrawl);
                        loop = false;
                    }
                    else
                    {
                        Console.WriteLine("Enter a valid amount");
                        loop = true;
                    }
                } while (loop);
            }
            else
            {
                Console.WriteLine("Account not found");
            }
        }

        //Static method to print account details
        public static void DoPrint(Bank UserBank)
        {
            Account account = UserAccount(UserBank);
            if (account != null)
            {
                account.Print();
            }
            else
            {
                Console.WriteLine("Account not found");
            }
        }

        //Static method to transfer funds from one account to another
        public static void DoTransfer(Bank CurrentBank)
        {
            decimal amount = 0;
            bool validNumber = false;
            bool loop = false;
            Account Sender = UserAccount(CurrentBank);
            if (Sender != null)
            {
                //Pick the reciever
                Console.WriteLine("Account Holder Name (Transfer Account ) : ");
                Account Reciever = CurrentBank.GetAccount(Console.ReadLine());
                if (Reciever != null)
                {
                    do
                    {
                        Console.WriteLine("Amount for Transfer : ");
                        validNumber = decimal.TryParse(Console.ReadLine(), out amount);
                        if (validNumber)
                        {
                            TransferTransaction newTransfer = new TransferTransaction(Sender, Reciever, amount);
                            CurrentBank.ExecuteTransaction(newTransfer);
                            loop = false;
                        }
                        else
                        {
                            Console.WriteLine("Enter a valid amount");
                            loop = true;
                        }
                    } while (loop);
                }
                else
                {
                    Console.WriteLine("Account not found");
                }
            }
            else
            {
                Console.WriteLine("Account not found");
            }
        }

        public static void TransHistory(Bank CurrentBank)
        {
            int index = 0;
            Console.WriteLine("======= TRANSACTION HISTORY =======  ");
            CurrentBank.PrintTransactionHistory();

            index = Convert.ToInt32(Console.ReadLine()) - 1;
            CurrentBank.RollbackTransaction(index);
        }
    }
}
