namespace AjLanguage.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;

    public interface IHost
    {
        Guid Id { get; }

        void Execute(ICommand command);
        Guid Evaluate(IExpression expression);
        Guid Invoke(Guid receiver, string name, params object[] arguments);
    }
}
