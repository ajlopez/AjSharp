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

                if (token.Value == "foreach")
                    return this.ParseForEachCommand();

                if (token.Value == "return")
                    return this.ParseReturnCommand();

                if (token.Value == "function" || token.Value == "sub")
                    return this.ParseFunctionCommand();

                //if (this.TryParse(TokenType.Separator, "("))
                //    return this.ParseInvokeCommand(token.Value);

                //return this.ParseSetCommand(token.Value);
            }

            if (token.TokenType == TokenType.Separator && token.Value == "{")
                return this.ParseCompositeCommand();

            this.lexer.PushToken(token);

            IExpression expr = this.ParseExpression();

            if (this.TryParse(TokenType.Separator, ";")) 
            {
                this.lexer.NextToken();

                return new ExpressionCommand(expr);
            }

            if (this.TryParse(TokenType.Operator, "=")) 
            {
                this.lexer.NextToken();

                ICommand command = new SetCommand(expr, this.ParseExpression());

                this.Parse(TokenType.Separator, ";");

                return command;
            }

            throw new UnexpectedTokenException(token);
        }

        public IExpression ParseExpression()
        {
            if (this.TryParse(TokenType.Name, "new"))
                return this.ParseNewExpression();

            if (this.TryParse(TokenType.Name, "function") || this.TryParse(TokenType.Name, "sub"))
                return this.ParseFunctionExpression();

            return this.ParseBinaryExpressionZerothLevel();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.lexer != null)
                this.lexer.Dispose();
        }

        #endregion

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

        private IExpression ParseFunctionExpression()
        {
            this.lexer.NextToken();
            string[] parameterNames = this.ParseParameters();
            ICommand body = this.ParseCommand();

            return new FunctionExpression(parameterNames, body);
        }

        private IExpression ParseNewExpression()
        {
            this.lexer.NextToken();
            string typename = this.ParseQualifiedName();
            ICollection<IExpression> arguments = this.ParseArgumentList();

            return new NewExpression(typename, arguments);
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

            while (this.TryParse(TokenType.Operator, "*", "/", @"\"))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseUnaryExpression();
                ArithmeticOperator op = oper.Value == "*" ? ArithmeticOperator.Multiply : (oper.Value == "/" ? ArithmeticOperator.Divide : ArithmeticOperator.IntegerDivide);

                expression = new ArithmeticBinaryExpression(op, expression, right);
            }

            return expression;
        }

        private IExpression ParseUnaryExpression()
        {
            if (this.TryParse(TokenType.Operator, "+", "-"))
            {
                Token oper = this.lexer.NextToken();

                IExpression unaryExpression = this.ParseUnaryExpression();

                ArithmeticOperator op = oper.Value == "+" ? ArithmeticOperator.Plus : ArithmeticOperator.Minus;

                return new ArithmeticUnaryExpression(op, unaryExpression);
            }

            return this.ParseTermExpression();
        }

        private IExpression ParseTermExpression()
        {
            IExpression expression = this.ParseSimpleTermExpression();

            while (this.TryParse(TokenType.Operator, "."))
            {
                this.lexer.NextToken();
                string name = this.ParseName();
                List<IExpression> arguments = null;

                if (this.TryParse(TokenType.Separator, "("))
                    arguments = this.ParseArgumentList();

                expression = new DotExpression(expression, name, arguments);
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

        private IExpression ParseSimpleTermExpression()
        {
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
                    return new ConstantExpression(token.Value);
                case TokenType.Name:
                    if (this.TryParse(TokenType.Separator, "("))
                    {
                        List<IExpression> arguments = this.ParseArgumentList();
                        return new InvokeExpression(token.Value, arguments);
                    }

                    return new VariableExpression(token.Value);

                    break;
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
            this.Parse(TokenType.Name, "in");
            IExpression values = this.ParseExpression();
            this.Parse(TokenType.Separator, ")");
            ICommand command = this.ParseCommand();

            return new ForEachCommand(name, values, command);
        }

        private ICommand ParseFunctionCommand()
        {
            string name = this.ParseName();
            string[] parameterNames = this.ParseParameters();
            ICommand body = this.ParseCommand();

            return new DefineFunctionCommand(name, parameterNames, body);
        }

        private string[] ParseParameters()
        {
            List<string> names = new List<string>();

            this.Parse(TokenType.Separator, "(");

            while (!this.TryParse(TokenType.Separator, ")"))
            {
                if (names.Count > 0)
                    this.Parse(TokenType.Separator, ",");

                names.Add(this.ParseName());
            }

            this.lexer.NextToken();

            return names.ToArray();
        }

        private bool TryPeekName()
        {
            Token token = this.lexer.PeekToken();

            if (token == null)
                return false;

            this.lexer.PushToken(token);

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
