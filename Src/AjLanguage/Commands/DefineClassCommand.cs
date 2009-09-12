namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class DefineClassCommand : ICommand
    {
        private string name;
        private string[] memberNames;
        private ICollection<IExpression> memberExpressions;

        public DefineClassCommand(string name, string[] memberNames, ICollection<IExpression> memberExpressions)
        {
            this.name = name;
            this.memberNames = memberNames;
            this.memberExpressions = memberExpressions;
        }

        public string ClassName { get { return this.name; } }

        public string[] MemberNames { get { return this.MemberNames; } }

        public ICollection<IExpression> MemberExpressions { get { return this.memberExpressions; } }

        public void Execute(IBindingEnvironment environment)
        {
            DynamicClass dynclass = new DynamicClass();

            int k = 0;

            if (this.memberExpressions != null)
                foreach (IExpression expression in this.memberExpressions)
                {
                    string name = this.memberNames[k++];

                    dynclass.SetMember(name, expression.Evaluate(environment));
                }

            Machine.Current.Environment.SetValue(this.name, dynclass);
        }
    }
}
