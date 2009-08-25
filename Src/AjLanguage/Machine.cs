namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;

    public class Machine
    {
        private static FunctionStatus currentFunctionStatus = new FunctionStatus();

        public static FunctionStatus CurrentFunctionStatus
        {
            get
            {
                return currentFunctionStatus;
            }

            set
            {
                currentFunctionStatus = value;
            }
        }
    }
}
