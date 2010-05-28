namespace AjLanguage.Commands
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Transactions;

    [Serializable]
    public class TransactionCommand : ICommand
    {
        private ICommand command;

        public TransactionCommand(ICommand command)
        {
            this.command = command;
        }

        public ICommand Command { get { return this.command; } }

        public void Execute(IBindingEnvironment environment)
        {
            if (Machine.CurrentTransaction != null)
            {
                this.command.Execute(environment);
                return;
            }

            using (Transaction trans = new Transaction(Machine.Current))
            {
                Machine.CurrentTransaction = trans;
                this.command.Execute(environment);
                Machine.CurrentTransaction = null;
                trans.Complete();
            }
        }
    }
}
