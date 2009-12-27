namespace AjLanguage.Commands
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class ForEachCommand : ICommand
    {
        private string name;
        private IExpression expression;
        private ICommand command;
        private bool localvar;

        public ForEachCommand(string name, IExpression expression, ICommand command)
            : this(name, expression, command, false)
        {
        }

        public ForEachCommand(string name, IExpression expression, ICommand command, bool localvar)
        {
            this.name = name;
            this.expression = expression;
            this.command = command;
            this.localvar = localvar;
        }

        public string Name { get { return this.name; } }

        public IExpression Expression { get { return this.expression; } }

        public ICommand Command { get { return this.command; } }

        public bool LocalVariable { get { return this.localvar; } }

        public void Execute(IBindingEnvironment environment)
        {
            IBindingEnvironment newenv = environment;

            if (this.localvar)
            {
                newenv = new LocalBindingEnvironment(environment);
                newenv.SetLocalValue(this.name, null);
            }

            foreach (object result in (IEnumerable) this.expression.Evaluate(newenv))
            {
                newenv.SetValue(this.name, result);
                this.command.Execute(newenv);
            }
        }
    }
}
