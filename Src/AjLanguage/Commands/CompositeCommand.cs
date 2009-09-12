namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CompositeCommand : ICommand
    {
        private ICollection<ICommand> commands;

        public CompositeCommand(ICollection<ICommand> commands)
        {
            this.commands = commands;
        }

        public int CommandCount { get { return this.commands.Count; } }

        public ICollection<ICommand> Commands { get { return this.commands; } }

        public void Execute(IBindingEnvironment environment)
        {
            foreach (ICommand command in this.commands)
            {
                if (Machine.CurrentFunctionStatus.Returned)
                    return;

                command.Execute(environment);
            }
        }
    }
}
