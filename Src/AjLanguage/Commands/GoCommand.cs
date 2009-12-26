namespace AjLanguage.Commands
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using System.Threading;

    public class GoCommand : ICommand
    {
        private ICommand command;

        public GoCommand(ICommand command)
        {
            this.command = command;
        }

        public void Execute(IBindingEnvironment environment)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(this.ExecuteGo));
            thread.Start(new LocalBindingEnvironment(environment));
        }

        private void ExecuteGo(object environment)
        {
            this.command.Execute((IBindingEnvironment) environment);
        }
    }
}
