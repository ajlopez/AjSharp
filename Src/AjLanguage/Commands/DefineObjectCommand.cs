namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class DefineObjectCommand : ICommand
    {
        private string name;
        private string[] memberNames;
        private ICollection<IExpression> memberExpressions;

        public DefineObjectCommand(string name, string[] memberNames, ICollection<IExpression> memberExpressions)
        {
            this.name = name;
            this.memberNames = memberNames;
            this.memberExpressions = memberExpressions;
        }

        public string ObjectName { get { return this.name; } }

        public string[] MemberNames { get { return this.MemberNames; } }

        public ICollection<IExpression> MemberExpressions { get { return this.memberExpressions; } }

        public void Execute(IBindingEnvironment environment)
        {
            DynamicObject dynobj = new DynamicObject();

            int k = 0;

            if (this.memberExpressions != null)
                foreach (IExpression expression in this.memberExpressions)
                {
                    string name = this.memberNames[k++];
                    object value = null;

                    if (expression != null)
                        value = expression.Evaluate(environment);

                    dynobj.SetValue(name, value);
                }

            environment.SetValue(this.name, dynobj);
        }
    }
}
