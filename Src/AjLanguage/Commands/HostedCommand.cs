namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Hosting;

    [Serializable]
    public class HostedCommand : ICommand
    {
        private ICommand command;
        private IExpression hostexpression;

        public HostedCommand(ICommand command, IExpression hostexpression)
        {
            this.command = command;
            this.hostexpression = hostexpression;
        }

        public ICommand Command { get { return this.command; } }

        public IExpression HostExpression { get { return this.hostexpression; } }

        public void Execute(IBindingEnvironment environment)
        {
            IHost host = (IHost) this.hostexpression.Evaluate(environment);
            host.Execute(this.command);
        }
    }
}
