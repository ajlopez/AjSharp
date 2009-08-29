namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class DefineFunctionCommand : ICommand
    {
        private string name;
        private string[] parameterNames;
        private ICommand body;

        public DefineFunctionCommand(string name, string[] parameterNames, ICommand body)
        {
            this.name = name;
            this.parameterNames = parameterNames;
            this.body = body;
        }

        public string FunctionName { get { return this.name; } }

        public string[] ParameterNames { get { return this.parameterNames; } }

        public ICommand Body { get { return this.body; } }

        public void Execute(BindingEnvironment environment)
        {
            environment.SetValue(this.name, new Function(this.parameterNames, this.body));
        }
    }
}
