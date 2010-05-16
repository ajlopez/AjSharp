namespace AjLanguage.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public interface IHost
    {
        Guid Id { get; }

        void Execute(ICommand command);
        object Evaluate(IExpression expression);
        object Invoke(IObject receiver, string name, params object[] arguments);
        object Invoke(Guid receiver, string name, params object[] arguments);
        object ResultToObject(object result);
    }
}
