namespace AjSharp.Compiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    using AjSharp;

    public class Parser : IDisposable
    {
        private Lexer lexer;

        public Parser(string text)
            : this(new Lexer(text))
        {
        }

        public Parser(TextReader reader)
            : this(new Lexer(reader))
        {
        }

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        public ICommand ParseCommand()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                return null;

            if (token.TokenType == TokenType.Name)
            {
                if (token.Value == "if")
                    return this.ParseIfCommand();

                if (token.Value == "while")
                    return this.ParseWhileCommand();

                if (token.Value == "at")
                {
                    IExpression hostexpression = this.ParseExpression();
                    return this.ParseHostedCommand(hostexpression);
                }

                if (token.Value == "foreach")
                    return this.ParseForEachCommand();

                if (token.Value == "for")
                    return this.ParseForCommand();

                if (token.Value == "return")
                    return this.ParseReturnCommand();

                if (token.Value == "default")
                {
                    Token token2 = this.lexer.NextToken();

                    if (token2.Value == "function" || token2.Value == "sub")
                        return this.ParseFunctionCommand(true);

                    throw new UnexpectedTokenException(token2);
                }

                if (token.Value == "function" || token.Value == "sub")
                    return this.ParseFunctionCommand(false);

                if (token.Value == "class")
                    return this.ParseClassAgentCommand(false);

                if (token.Value == "agent")
                    return this.ParseClassAgentCommand(true);

                if (token.Value == "global")
                    return this.ParseGlobalCommand();

                if (token.Value == "object")
                    return this.ParseObjectCommand();

                if (token.Value == "go")
                    return new GoCommand(this.ParseCommand());
            }

            if (token.TokenType == TokenType.Separator && token.Value == "{")
                return this.ParseCompositeCommand();

            this.lexer.PushToken(token);

            ICommand command = this.ParseSimpleCommand();

            if (command == null)
                throw new UnexpectedTokenException(token);

            this.Parse(TokenType.Separator, ";");

            return command;
        }

        // TODO Refactor, is a partial copy of ParseCommand
        private ICommand ParseHostedCommand(IExpression hostexpression)
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                return null;

            if (token.TokenType == TokenType.Name)
            {
                if (token.Value == "if")
                    return this.ParseIfCommand();

                if (token.Value == "while")
                    return this.ParseWhileCommand();

                if (token.Value == "foreach")
                    return this.ParseForEachCommand();

                if (token.Value == "for")
                    return this.ParseForCommand();

                if (token.Value == "class")
                    return this.ParseClassAgentCommand(false);

                if (token.Value == "agent")
                    return this.ParseClassAgentCommand(true);

                if (token.Value == "global")
                    return this.ParseGlobalCommand();

                if (token.Value == "object")
                    return this.ParseObjectCommand();

                if (token.Value == "go")
                    return new GoCommand(this.ParseCommand());
            }

            ICommand command = null;

            if (token.TokenType == TokenType.Separator && token.Value == "{")
                command = this.ParseCompositeCommand();
            else
            {
                this.lexer.PushToken(token);

                command = this.ParseSimpleCommand();

                if (command == null)
                    throw new UnexpectedTokenException(token);
            }

            if (command is ExpressionCommand && this.TryParse(TokenType.Name, "with"))
            {
                this.lexer.NextToken();
                IList<IExpression> arguments = this.ParseArgumentList();
                this.Parse(TokenType.Separator, ";");
                return new ExpressionCommand(new HostedInvocationExpression(((ExpressionCommand)command).Expression, arguments, hostexpression));
            }

            if (!(command is CompositeCommand))
                this.Parse(TokenType.Separator, ";");

            return new HostedCommand(command, hostexpression);
        }

        public IExpression ParseExpression()
        {
            if (this.TryParse(TokenType.Name, "new"))
                return this.ParseNewExpression();

            if (this.TryParse(TokenType.Name, "expression"))
            {
                this.lexer.NextToken();
                return new ConstantExpression(this.ParseExpression());
            }

            if (this.TryParse(TokenType.Name, "at"))
            {
                this.lexer.NextToken();
                IExpression hostexpr = this.ParseExpression();
                IExpression expr = this.ParseExpression();

                if (this.TryParse(TokenType.Name, "with"))
                {
                    this.lexer.NextToken();
                    IList<IExpression> arguments = this.ParseArgumentList();
                    return new HostedInvocationExpression(expr, arguments, hostexpr);
                }

                return new HostedExpression(expr, hostexpr);
            }

            if (this.TryParse(TokenType.Name, "command"))
            {
                this.lexer.NextToken();
                return new ConstantExpression(this.ParseCommand());
            }

            return this.ParseBinaryLogicalExpressionLevelOne();
        }

        public void Dispose()
        {
            if (this.lexer != null)
                this.lexer.Dispose();
        }

        private ICommand ParseSimpleCommand()
        {
            if (this.TryParse(TokenType.Name, "var"))
            {
                this.lexer.NextToken();
                return this.ParseVarCommand();
            }

            if (this.TryParse(TokenType.Name, "exit"))
            {
                this.lexer.NextToken();
                return new ExitCommand();
            }

            IExpression expression = this.ParseExpression();

            if (expression == null)
                return null;

            if (this.TryParse(TokenType.Operator, "="))
            {
                this.lexer.NextToken();

                ICommand command = null;

                if (expression is ArrayExpression)
                {
                    ArrayExpression aexpr = (ArrayExpression)expression;
                    command = new SetArrayCommand(aexpr.Expression, aexpr.Arguments, this.ParseExpression());
                }
                else
                    command = new SetCommand(expression, this.ParseExpression());

                return command;
            }

            if (this.TryParse(TokenType.Operator, "<-"))
            {
                this.lexer.NextToken();
                return new SetValueCommand(expression, this.ParseExpression());
            }

            return new ExpressionCommand(expression);
        }

        private static bool IsName(Token token, string value)
        {
            return IsToken(token, value, TokenType.Name);
        }

        private static bool IsToken(Token token, string value, TokenType type)
        {
            if (token == null)
                return false;

            if (token.TokenType != type)
                return false;

            if (type == TokenType.Name)
                return token.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase);

            return token.Value.Equals(value);
        }

        private IExpression ParseFunctionExpression(bool isdefault)
        {
            bool hasvariableparameters = false;
            this.lexer.NextToken();
            string[] parameterNames = this.ParseParameters(ref hasvariableparameters);
            ICommand body = this.ParseCommand();

            return new FunctionExpression(parameterNames, body, isdefault, hasvariableparameters);
        }

        private ICommand ParseGlobalCommand()
        {
            string name = this.ParseName();
            this.Parse(TokenType.Separator, ";");

            return new GlobalCommand(name);
        }

        // TODO Refactor
        private IExpression ParseNewExpression()
        {
            this.lexer.NextToken();

            IExpression expression;

            bool isDynamic = false;

            if (this.TryPeekName())
            {
                string typename = this.ParseQualifiedName();

                if (this.TryParse(TokenType.Separator, "["))
                    return this.ParseNewArrayExpression(typename);

                ICollection<IExpression> arguments = this.ParseArgumentList();

                expression = new NewExpression(typename, arguments);

                if (!this.TryParse(TokenType.Separator, "{"))
                    return expression;
            }
            else
            {
                isDynamic = true;
                expression = new NewExpression("AjLanguage.Language.DynamicObject", null);
            }

            this.Parse(TokenType.Separator, "{");

            List<string> names = new List<string>();
            List<IExpression> expressions = new List<IExpression>();

            bool wasMember = false;

            while (!this.TryParse(TokenType.Separator, "}"))
            {
                if (names.Count > 0 && !wasMember)
                    this.Parse(TokenType.Separator, ",");

                if (isDynamic && this.TryParse(TokenType.Name, "var", "function", "sub"))
                {
                    wasMember = true;

                    Token token = this.lexer.NextToken();

                    if (token.Value == "var")
                        this.ParseMemberVariable(names, expressions);
                    else if (token.Value == "function" || token.Value == "sub")
                        this.ParseMemberMethod(names, expressions, false);
                    else
                        throw new UnexpectedTokenException(token);
                }
                else
                {
                    wasMember = false;

                    names.Add(this.ParseName());
                    this.Parse(TokenType.Operator, "=");
                    expressions.Add(this.ParseExpression());
                }
            }

            this.Parse(TokenType.Separator, "}");

            return new MultipleSetExpression(expression, names.ToArray(), expressions);
        }

        private IExpression ParseNewArrayExpression(string typename)
        {
            ICollection<IExpression> arguments = this.ParseArrayArgumentList();

            if (arguments.Count == 0)
            {
                ICollection<IExpression> values = this.ParseArrayValues();

                return new InitializeArrayExpression(typename, values);
            }

            return new NewArrayExpression(typename, arguments);
        }

        private ICollection<IExpression> ParseArrayValues()
        {
            this.Parse(TokenType.Separator, "{");

            List<IExpression> expressions = new List<IExpression>();

            while (!this.TryParse(TokenType.Separator, "}"))
            {
                if (expressions.Count > 0)
                    this.Parse(TokenType.Separator, ",");

                expressions.Add(this.ParseExpression());
            }

            this.Parse(TokenType.Separator, "}");

            return expressions;
        }

        private IExpression ParseBinaryLogicalExpressionLevelOne()
        {
            IExpression expression = this.ParseBinaryLogicalExpressionLevelTwo();

            if (expression == null)
                return null;

            while (this.TryParse(TokenType.Operator, "||"))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseBinaryLogicalExpressionLevelTwo();

                expression = new OrExpression(expression, right);
            }

            return expression;
        }

        private IExpression ParseBinaryLogicalExpressionLevelTwo()
        {
            IExpression expression = this.ParseBinaryExpressionZerothLevel();

            if (expression == null)
                return null;

            while (this.TryParse(TokenType.Operator, "&&"))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseBinaryExpressionZerothLevel();

                expression = new AndExpression(expression, right);
            }

            return expression;
        }

        private IExpression ParseBinaryExpressionZerothLevel()
        {
            IExpression expression = this.ParseBinaryExpressionFirstLevel();

            if (expression == null)
                return null;

            while (this.TryParse(TokenType.Operator, "<", ">", "==", ">=", "<=", "!="))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseBinaryExpressionFirstLevel();

                ComparisonOperator op = 0;

                if (oper.Value == "<")
                    op = ComparisonOperator.Less;
                if (oper.Value == ">")
                    op = ComparisonOperator.Greater;
                if (oper.Value == "<=")
                    op = ComparisonOperator.LessEqual;
                if (oper.Value == ">=")
                    op = ComparisonOperator.GreaterEqual;
                if (oper.Value == "==")
                    op = ComparisonOperator.Equal;
                if (oper.Value == "!=")
                    op = ComparisonOperator.NotEqual;

                expression = new CompareExpression(op, expression, right);
            }

            return expression;
        }

        private IExpression ParseBinaryExpressionFirstLevel()
        {
            IExpression expression = this.ParseBinaryExpressionSecondLevel();

            if (expression == null)
                return null;

            while (this.TryParse(TokenType.Operator, "+", "-"))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseBinaryExpressionSecondLevel();
                ArithmeticOperator op = oper.Value == "+" ? ArithmeticOperator.Add : ArithmeticOperator.Subtract;

                expression = new ArithmeticBinaryExpression(op, expression, right);
            }

            return expression;
        }

        private IExpression ParseBinaryExpressionSecondLevel()
        {
            IExpression expression = this.ParseUnaryExpression();

            if (expression == null)
                return null;

            while (this.TryParse(TokenType.Operator, "*", "/", @"\", "%"))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseUnaryExpression();
                ArithmeticOperator op;

                if (oper.Value == "*")
                    op = ArithmeticOperator.Multiply;
                else if (oper.Value == "/")
                    op = ArithmeticOperator.Divide;
                else if (oper.Value == "\\")
                    op = ArithmeticOperator.IntegerDivide;
                else if (oper.Value == "%")
                    op = ArithmeticOperator.Modulo;
                else
                    throw new ParserException(string.Format("Invalid operator '{0}'", oper.Value));

                expression = new ArithmeticBinaryExpression(op, expression, right);
            }

            return expression;
        }

        private IExpression ParseUnaryExpression()
        {
            if (this.TryParse(TokenType.Operator, "+", "-", "!"))
            {
                Token oper = this.lexer.NextToken();

                IExpression unaryExpression = this.ParseUnaryExpression();

                if (oper.Value == "!")
                    return new NotExpression(unaryExpression);

                ArithmeticOperator op = oper.Value == "+" ? ArithmeticOperator.Plus : ArithmeticOperator.Minus;

                return new ArithmeticUnaryExpression(op, unaryExpression);
            }

            if (this.TryParse(TokenType.Operator, "++", "--"))
            {
                Token oper = this.lexer.NextToken();

                IExpression expression = this.ParseTermExpression();

                IncrementOperator op = oper.Value == "++" ? IncrementOperator.PreIncrement : IncrementOperator.PreDecrement;

                return new IncrementExpression(expression, op);
            }

            if (this.TryParse(TokenType.Operator, "<-"))
            {
                this.lexer.NextToken();

                IExpression expression = this.ParseTermExpression();

                return new GetValueExpression(expression);
            }

            IExpression termexpr = this.ParseTermExpression();

            if (this.TryParse(TokenType.Operator, "++", "--"))
            {
                Token oper = this.lexer.NextToken();

                IncrementOperator op = oper.Value == "++" ? IncrementOperator.PostIncrement : IncrementOperator.PostDecrement;

                return new IncrementExpression(termexpr, op);
            }

            return termexpr;
        }

        private IExpression ParseTermExpression()
        {
            IExpression expression = this.ParseSimpleTermExpression();

            while (this.TryParse(TokenType.Operator, ".") || this.TryParse(TokenType.Separator, "[", "("))
            {
                if (this.TryParse(TokenType.Separator, "("))
                    expression = new InvokeExpressionExpression(expression, this.ParseArgumentList());
                else if (this.TryParse(TokenType.Operator, "."))
                {
                    this.lexer.NextToken();
                    string name = this.ParseName();
                    List<IExpression> arguments = null;

                    if (this.TryParse(TokenType.Separator, "("))
                        arguments = this.ParseArgumentList();

                    expression = new DotExpression(expression, name, arguments);
                }
                else
                    expression = new ArrayExpression(expression, this.ParseArrayArgumentList());
            }

            return expression;
        }

        private List<IExpression> ParseArgumentList()
        {
            List<IExpression> expressions = new List<IExpression>();

            this.Parse(TokenType.Separator, "(");

            while (!this.TryParse(TokenType.Separator, ")"))
            {
                if (expressions.Count > 0)
                    this.Parse(TokenType.Separator, ",");

                expressions.Add(this.ParseExpression());
            }

            this.Parse(TokenType.Separator, ")");

            return expressions;
        }

        private List<IExpression> ParseArrayArgumentList()
        {
            List<IExpression> expressions = new List<IExpression>();

            this.Parse(TokenType.Separator, "[");

            while (!this.TryParse(TokenType.Separator, "]"))
            {
                if (expressions.Count > 0)
                    this.Parse(TokenType.Separator, ",");

                expressions.Add(this.ParseExpression());
            }

            this.Parse(TokenType.Separator, "]");

            return expressions;
        }

        private IExpression ParseSimpleTermExpression()
        {
            if (this.TryParse(TokenType.Name, "default"))
            {
                if (this.TryParse(TokenType.Name, "function") || this.TryParse(TokenType.Name, "sub"))
                    return this.ParseFunctionExpression(true);

                throw new UnexpectedTokenException(this.lexer.NextToken());
            }

            if (this.TryParse(TokenType.Name, "function") || this.TryParse(TokenType.Name, "sub"))
                return this.ParseFunctionExpression(false);

            Token token = this.lexer.NextToken();

            if (token == null)
                return null;

            switch (token.TokenType)
            {
                case TokenType.Separator:
                    if (token.Value == "(")
                    {
                        IExpression expr = this.ParseExpression();
                        this.Parse(TokenType.Separator, ")");
                        return expr;
                    }

                    break;
                case TokenType.Boolean:
                    bool booleanValue = Convert.ToBoolean(token.Value);
                    return new ConstantExpression(booleanValue);
                case TokenType.Integer:
                    int intValue = Int32.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture);
                    return new ConstantExpression(intValue);
                case TokenType.Real:
                    double realValue = Double.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture);
                    return new ConstantExpression(realValue);
                case TokenType.String:
                    IList<string> parts = StringUtilities.SplitText(token.Value);

                    if (parts.Count == 1)
                        return new ConstantExpression(token.Value);

                    IExpression strexpr = new ConstantExpression(parts[0]);

                    for (int k = 1; k < parts.Count; k++)
                        if ((k % 2) == 0)
                            strexpr = new ConcatenateExpression(strexpr, new ConstantExpression(parts[k]));
                        else
                        {
                            Parser parser = new Parser(parts[k]);
                            strexpr = new ConcatenateExpression(strexpr, parser.ParseExpression());
                        }

                    return strexpr;
                case TokenType.Name:
                    if (this.TryParse(TokenType.Separator, "("))
                    {
                        List<IExpression> arguments = this.ParseArgumentList();
                        return new InvokeExpression(token.Value, arguments);
                    }

                    if (this.TryParse(TokenType.Operator, "..."))
                    {
                        this.lexer.NextToken();

                        return new VariableVariableExpression(token.Value);
                    }

                    return new VariableExpression(token.Value);
            }

            throw new UnexpectedTokenException(token);
        }

        private ICommand ParseCompositeCommand()
        {
            IList<ICommand> commands = new List<ICommand>();

            while (!this.TryParse(TokenType.Separator, "}"))
                commands.Add(this.ParseCommand());

            this.lexer.NextToken();

            return new CompositeCommand(commands);
        }

        private ICommand ParseReturnCommand()
        {
            if (this.TryParse(TokenType.Separator, ";"))
            {
                this.lexer.NextToken();
                return new ReturnCommand();
            }

            IExpression expression = this.ParseExpression();

            this.Parse(TokenType.Separator, ";");

            return new ReturnCommand(expression);
        }

        private ICommand ParseIfCommand()
        {
            this.Parse(TokenType.Separator, "(");
            IExpression condition = this.ParseExpression();
            this.Parse(TokenType.Separator, ")");
            ICommand thencmd = this.ParseCommand();

            if (!this.TryParse(TokenType.Name, "else"))
                return new IfCommand(condition, thencmd);

            this.lexer.NextToken();

            ICommand elsecmd = this.ParseCommand();

            return new IfCommand(condition, thencmd, elsecmd);
        }

        private ICommand ParseWhileCommand()
        {
            this.Parse(TokenType.Separator, "(");
            IExpression condition = this.ParseExpression();
            this.Parse(TokenType.Separator, ")");
            ICommand command = this.ParseCommand();

            return new WhileCommand(condition, command);
        }

        private ICommand ParseForEachCommand()
        {
            this.Parse(TokenType.Separator, "(");
            string name = this.ParseName();
            bool localvar = false;

            if (name == "var")
            {
                localvar = true;
                name = this.ParseName();
            }

            this.Parse(TokenType.Name, "in");
            IExpression values = this.ParseExpression();
            this.Parse(TokenType.Separator, ")");
            ICommand command = this.ParseCommand();

            return new ForEachCommand(name, values, command, localvar);
        }

        private ICommand ParseForCommand()
        {
            this.Parse(TokenType.Separator, "(");
            ICommand initial = this.ParseSimpleCommand();
            this.Parse(TokenType.Separator, ";");
            IExpression condition = this.ParseExpression();
            this.Parse(TokenType.Separator, ";");
            ICommand endcmd = this.ParseSimpleCommand();
            this.Parse(TokenType.Separator, ")");
            ICommand command = this.ParseCommand();

            return new ForCommand(initial, condition, endcmd, command);
        }

        private ICommand ParseFunctionCommand(bool isdefault)
        {
            string name = this.ParseName();
            bool hasvariableparameters = false;
            string[] parameterNames = this.ParseParameters(ref hasvariableparameters);
            ICommand body = this.ParseCommand();

            return new DefineFunctionCommand(name, parameterNames, body, isdefault, hasvariableparameters);
        }

        private ICommand ParseVarCommand()
        {
            string name = this.ParseName();
            IExpression expression = null;

            if (this.TryParse(TokenType.Operator, "="))
            {
                this.lexer.NextToken();
                expression = this.ParseExpression();
            }

            return new VarCommand(name, expression);
        }

        private ICommand ParseClassAgentCommand(bool isagent)
        {
            string name = this.ParseName();
            List<string> memberNames = new List<string>();
            List<IExpression> memberExpressions = new List<IExpression>();

            this.Parse(TokenType.Separator, "{");

            while (this.TryParse(TokenType.Name, "var", "function", "sub", "default"))
            {
                Token token = this.lexer.NextToken();

                if (token.Value == "var")
                    this.ParseMemberVariable(memberNames, memberExpressions);
                else if (token.Value == "default")
                {
                    Token token2 = this.lexer.NextToken();

                    if (token2.Value == "function" || token2.Value == "sub")
                        this.ParseMemberMethod(memberNames, memberExpressions, true);
                    else
                        throw new UnexpectedTokenException(token2);
                }
                else if (token.Value == "function" || token.Value == "sub")
                    this.ParseMemberMethod(memberNames, memberExpressions, false);
                else
                    throw new UnexpectedTokenException(token);
            }

            this.Parse(TokenType.Separator, "}");

            if (isagent)
                return new DefineAgentCommand(name, memberNames.ToArray(), memberExpressions);

            DefineClassCommand cmd = new DefineClassCommand(name, memberNames.ToArray(), memberExpressions);

            return cmd;
        }

        private ICommand ParseObjectCommand()
        {
            string name = this.ParseName();
            List<string> memberNames = new List<string>();
            List<IExpression> memberExpressions = new List<IExpression>();

            this.Parse(TokenType.Separator, "{");

            while (this.TryParse(TokenType.Name, "var", "function", "sub", "default"))
            {
                Token token = this.lexer.NextToken();

                if (token.Value == "var")
                    this.ParseMemberVariable(memberNames, memberExpressions);
                else if (token.Value == "default")
                {
                    Token token2 = this.lexer.NextToken();

                    if (token2.Value == "function" || token2.Value == "sub")
                        this.ParseMemberMethod(memberNames, memberExpressions, true);
                    else
                        throw new UnexpectedTokenException(token2);
                }
                else if (token.Value == "function" || token.Value == "sub")
                    this.ParseMemberMethod(memberNames, memberExpressions, false);
                else
                    throw new UnexpectedTokenException(token);
            }

            this.Parse(TokenType.Separator, "}");

            DefineObjectCommand cmd = new DefineObjectCommand(name, memberNames.ToArray(), memberExpressions);

            return cmd;
        }

        private void ParseMemberVariable(IList<string> memberNames, IList<IExpression> memberExpressions)
        {
            string name = this.ParseName();
            IExpression expression = null;

            if (this.TryParse(TokenType.Operator, "="))
            {
                this.lexer.NextToken();

                expression = this.ParseExpression();
            }

            this.Parse(TokenType.Separator, ";");

            memberNames.Add(name);
            memberExpressions.Add(expression);
        }

        private void ParseMemberMethod(IList<string> memberNames, IList<IExpression> memberExpressions, bool isdefault)
        {
            string name = this.ParseName();
            bool hasvariableparameters = false;
            string[] parameterNames = this.ParseParameters(ref hasvariableparameters);
            ICommand body = this.ParseCommand();

            memberNames.Add(name);
            memberExpressions.Add(new FunctionExpression(parameterNames, body, isdefault, hasvariableparameters));
        }

        private string[] ParseParameters(ref bool hasvariableparameters)
        {
            hasvariableparameters = false;
            List<string> names = new List<string>();

            this.Parse(TokenType.Separator, "(");

            while (!this.TryParse(TokenType.Separator, ")"))
            {
                if (names.Count > 0)
                    this.Parse(TokenType.Separator, ",");

                names.Add(this.ParseName());

                if (this.TryParse(TokenType.Operator, "..."))
                {
                    this.lexer.NextToken();
                    this.Parse(TokenType.Separator, ")");
                    hasvariableparameters = true;
                    return names.ToArray();
                }
            }

            this.lexer.NextToken();

            return names.ToArray();
        }

        private bool TryPeekName()
        {
            Token token = this.lexer.PeekToken();

            if (token == null)
                return false;

            return token.TokenType == TokenType.Name;
        }

        private object ParseValue()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                throw new UnexpectedEndOfInputException();

            if (token.TokenType == TokenType.String)
                return token.Value;

            if (token.TokenType == TokenType.Integer)
                return int.Parse(token.Value);

            throw new UnexpectedTokenException(token);
        }

        private bool TryParse(TokenType type, params string[] values)
        {
            Token token = this.lexer.PeekToken();

            if (token == null)
                return false;

            if (type == TokenType.Name)
            {
                foreach (string value in values)
                    if (IsName(token, value))
                        return true;

                return false;
            }

            if (token.TokenType == type)
                foreach (string value in values)
                    if (token.Value == value)
                        return true;

            return false;
        }

        private void Parse(TokenType type, string value)
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                throw new UnexpectedEndOfInputException();

            if (type == TokenType.Name)
                if (IsName(token, value))
                    return;
                else
                    throw new UnexpectedTokenException(token);

            if (token.TokenType != type || token.Value != value)
                throw new UnexpectedTokenException(token);
        }

        private string ParseName()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                throw new ParserException(string.Format("Unexpected end of input"));

            if (token.TokenType == TokenType.Name)
                return token.Value;

            throw new UnexpectedTokenException(token);
        }

        private string ParseQualifiedName()
        {
            string name = this.ParseName();

            while (this.TryParse(TokenType.Operator, "."))
            {
                this.lexer.NextToken();
                name += "." + this.ParseName();
            }

            return name;
        }
    }
}
