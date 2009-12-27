namespace AjLanguage.Commands
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using AjLanguage.Expressions;

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
            GoCommandParameter parameter = new GoCommandParameter() { Machine = Machine.Current, Environment = new LocalBindingEnvironment(environment) };
            thread.Start(parameter);
        }

        private void ExecuteGo(object obj)
        {
            GoCommandParameter parameter = (GoCommandParameter)obj;

            parameter.Machine.SetCurrent();

            this.command.Execute(parameter.Environment);
        }
    }

    internal class GoCommandParameter
    {
        internal Machine Machine { get; set; }

        internal BindingEnvironment Environment { get; set; }
    }
}
