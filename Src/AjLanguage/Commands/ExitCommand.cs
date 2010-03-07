namespace AjLanguage.Commands
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class ExitCommand : ICommand
    {
        public void Execute(IBindingEnvironment environment)
        {
            throw new ExitException();
        }
    }
}
