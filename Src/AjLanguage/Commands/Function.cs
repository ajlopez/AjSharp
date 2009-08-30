namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Function : ICallable
    {
        private string[] parameterNames;
        private ICommand body;
        private int arity;

        public Function(string[] parameterNames, ICommand body)
        {
            this.parameterNames = parameterNames;
            this.body = body;

            if (parameterNames == null)
                this.arity = 0;
            else
                this.arity = parameterNames.Length;
        }

        public object Invoke(BindingEnvironment environment, object[] arguments)
        {
            int argcount = 0;

            if (arguments != null)
                argcount = arguments.Length;

            if (this.arity != argcount)
                throw new InvalidOperationException("Invalid number of arguments");

            BindingEnvironment newenv = new BindingEnvironment(environment);

            if (argcount > 0)
                for (int k = 0; k < argcount; k++)
                    newenv.SetValue(this.parameterNames[k], arguments[k]);

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
