namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class DefineAgentCommand : ICommand
    {
        private string name;
        private string[] memberNames;
        private ICollection<IExpression> memberExpressions;

        public DefineAgentCommand(string name, string[] memberNames, ICollection<IExpression> memberExpressions)
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
            AgentClass dynclass = new AgentClass(this.name);

            int k = 0;

            if (this.memberExpressions != null)
                foreach (IExpression expression in this.memberExpressions)
                {
                    string name = this.memberNames[k++];
                    object value = null;

                    if (expression != null)
                        value = expression.Evaluate(environment);

                    if (value is ICallable && !(value is AgentFunction))
                        value = new AgentFunction((ICallable) value);

                    dynclass.SetMember(name, value);
                }

            Machine.Current.Environment.SetValue(this.name, dynclass);
        }
    }
}
