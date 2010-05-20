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
        // Host Id
        Guid Id { get; }

        bool IsLocal { get; }

        // Host Invocation
        void Execute(ICommand command);
        object Evaluate(IExpression expression);
        object Invoke(ICallable function, params object[] arguments);

        // Host Registration
        void RegisterHost(string address);

        // Host Invoke Object (To Review)
        object Invoke(IObject receiver, string name, params object[] arguments);
        object Invoke(IObject receiver, ICallable method, params object[] arguments);

        // Host Invoke Object by Guid (To Review)
        object Invoke(Guid receiver, ICallable method, params object[] arguments);
        object Invoke(Guid receiver, string name, params object[] arguments);

        // Marshalling (To Review)
        object ResultToObject(object result);
    }
}
