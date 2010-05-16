namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    [Serializable]
    public class GlobalCommand : ICommand
    {
        private string name;

        public GlobalCommand(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }

        public void Execute(IBindingEnvironment environment)
        {
            environment.DefineGlobal(this.name);
        }
    }
}
