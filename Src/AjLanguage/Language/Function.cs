namespace AjLanguage.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;

    [Serializable]
    public class Function : ICallable
    {
        private string[] parameterNames;
        private ICommand body;
        private int arity;
        private IBindingEnvironment environment;
        private bool isdefault;
        private bool hasvariableparameters;

        public Function(string[] parameterNames, ICommand body)
            : this(parameterNames, body, null, false, false)
        {
        }

        public Function(string[] parameterNames, ICommand body, IBindingEnvironment environment, bool isdefault, bool hasvariableparameters)
        {
            this.parameterNames = parameterNames;
            this.body = body;
            this.hasvariableparameters = hasvariableparameters;

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

        public bool HasVariableParameters { get { return this.hasvariableparameters; } }

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
                if (!this.hasvariableparameters || this.arity - 1 > argcount)
                    throw new InvalidOperationException("Invalid number of arguments");

            BindingEnvironment newenv = new BindingEnvironment(environment);

            if (this.hasvariableparameters)
            {
                for (int k = 0; k < this.arity-1; k++)
                    newenv.SetLocalValue(this.parameterNames[k], arguments[k]);
                if (argcount == 0)
                    newenv.SetLocalValue(this.parameterNames[0], new object[] { });
                else
                {
                    object[] pars = new object[argcount - this.arity + 1];
                    Array.Copy(arguments, argcount - pars.Length, pars, 0, pars.Length);
                    newenv.SetLocalValue(this.parameterNames[this.arity - 1], pars);
                }
            }
            else
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
