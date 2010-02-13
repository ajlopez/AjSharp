﻿namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;

    public class Function : ICallable
    {
        private string[] parameterNames;
        private ICommand body;
        private int arity;
        private IBindingEnvironment environment;
        private bool isdefault;

        public Function(string[] parameterNames, ICommand body)
            : this(parameterNames, body, null, false)
        {
        }

        public Function(string[] parameterNames, ICommand body, IBindingEnvironment environment, bool isdefault)
        {
            this.parameterNames = parameterNames;
            this.body = body;

            if (parameterNames == null)
                this.arity = 0;
            else
                this.arity = parameterNames.Length;

            this.environment = environment;
            this.isdefault = isdefault;
        }

        public int Arity { get { return this.parameterNames == null ? 0 : this.parameterNames.Length; } }

        public string[] ParameterNames { get { return this.parameterNames; } }

        public ICommand Body { get { return this.body; } }

        public bool IsDefault { get { return this.isdefault; } }

        public IBindingEnvironment Environment { get { return this.environment; } }

        public object Call(IObject obj, params object[] parameters)
        {
            return obj.Invoke(this, parameters);
        }

        public object Invoke(object[] arguments)
        {
            return this.Invoke(this.environment, arguments);
        }

        public object Invoke(IBindingEnvironment environment, object[] arguments)
        {
            int argcount = 0;

            if (arguments != null)
                argcount = arguments.Length;

            if (this.arity != argcount)
                throw new InvalidOperationException("Invalid number of arguments");

            BindingEnvironment newenv = new BindingEnvironment(environment);

            if (argcount > 0)
                for (int k = 0; k < argcount; k++)
                    newenv.SetLocalValue(this.parameterNames[k], arguments[k]);

            FunctionStatus fstatus = Machine.CurrentFunctionStatus;

            Machine.CurrentFunctionStatus = new FunctionStatus();

            try
            {
                this.body.Execute(newenv);

                return Machine.CurrentFunctionStatus.ReturnValue;
            }
            finally
            {
                Machine.CurrentFunctionStatus = fstatus;
            }
        }
    }
}