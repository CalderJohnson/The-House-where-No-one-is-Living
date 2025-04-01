using System;
using System.Collections.Generic;
using System.Text;

public class LogicParser
{
    private string input;
    private int pos;
    private Token currentToken;

    public ExpressionNode Parse(string str)
    {
        input = str;
        pos = 0;
        NextToken();
        ExpressionNode expr = ParseImplication();
        if (currentToken.type != TokenType.End)
            throw new Exception("Unexpected token at end of expression.");
        return expr;
    }

    #region Parser Functions

    // Highest level: Implication and Biconditional have lowest precedence.
    private ExpressionNode ParseImplication()
    {
        ExpressionNode left = ParseOr();
        while (currentToken.type == TokenType.Implies || currentToken.type == TokenType.Biconditional)
        {
            TokenType op = currentToken.type;
            NextToken();
            ExpressionNode right = ParseOr();
            if (op == TokenType.Implies)
                left = new OperatorNode(OperatorType.Implies, left, right);
            else
                left = new OperatorNode(OperatorType.Biconditional, left, right);
        }
        return left;
    }

    // Or has lower precedence than And.
    private ExpressionNode ParseOr()
    {
        ExpressionNode left = ParseAnd();
        while (currentToken.type == TokenType.Or)
        {
            NextToken();
            ExpressionNode right = ParseAnd();
            left = new OperatorNode(OperatorType.Or, left, right);
        }
        return left;
    }

    private ExpressionNode ParseAnd()
    {
        ExpressionNode left = ParseNot();
        while (currentToken.type == TokenType.And)
        {
            NextToken();
            ExpressionNode right = ParseNot();
            left = new OperatorNode(OperatorType.And, left, right);
        }
        return left;
    }

    private ExpressionNode ParseNot()
    {
        if (currentToken.type == TokenType.Not)
        {
            NextToken();
            ExpressionNode operand = ParseNot();
            return new OperatorNode(OperatorType.Not, operand);
        }
        else
        {
            return ParsePrimary();
        }
    }

    private ExpressionNode ParsePrimary()
    {
        if (currentToken.type == TokenType.Variable)
        {
            string var = currentToken.text;
            NextToken();
            return new VariableNode(var);
        }
        else if (currentToken.type == TokenType.Constant)
        {
            bool value = currentToken.text.ToLower() == "true";
            NextToken();
            return new ConstantNode(value);
        }
        else if (currentToken.type == TokenType.LeftParen)
        {
            NextToken();
            ExpressionNode expr = ParseImplication();
            if (currentToken.type != TokenType.RightParen)
                throw new Exception("Missing closing parenthesis");
            NextToken();
            return expr;
        }
        else
        {
            throw new Exception("Unexpected token: " + currentToken.text);
        }
    }

    #endregion

    #region Tokenizer

    private enum TokenType { Variable, Constant, And, Or, Not, Implies, Biconditional, LeftParen, RightParen, End }

    private class Token
    {
        public TokenType type;
        public string text;
        public Token(TokenType type, string text)
        {
            this.type = type;
            this.text = text;
        }
    }

    private void NextToken()
    {
        SkipWhiteSpace();
        if (pos >= input.Length)
        {
            currentToken = new Token(TokenType.End, "");
            return;
        }

        char ch = input[pos];
        if (char.IsLetter(ch))
        {
            // Read an identifier or constant (like true/false)
            StringBuilder sb = new StringBuilder();
            while (pos < input.Length && char.IsLetter(input[pos]))
            {
                sb.Append(input[pos]);
                pos++;
            }
            string ident = sb.ToString();
            if (ident.ToLower() == "true" || ident.ToLower() == "false")
                currentToken = new Token(TokenType.Constant, ident);
            else
                currentToken = new Token(TokenType.Variable, ident);
        }
        else if (ch == '~')
        {
            pos++;
            currentToken = new Token(TokenType.Not, "~");
        }
        else if (ch == '^')
        {
            pos++;
            currentToken = new Token(TokenType.And, "^");
        }
        else if (ch == 'v')
        {
            pos++;
            currentToken = new Token(TokenType.Or, "v");
        }
        else if (ch == '=')
        {
            // Could be "=>" for implies.
            pos++;
            if (pos < input.Length && input[pos] == '>')
            {
                pos++;
                currentToken = new Token(TokenType.Implies, "=>");
            }
            else
            {
                throw new Exception("Expected '>' after '='");
            }
        }
        else if (ch == '<')
        {
            // Expecting "<=>"
            pos++;
            if (pos < input.Length && input[pos] == '=')
            {
                pos++;
                if (pos < input.Length && input[pos] == '>')
                {
                    pos++;
                    currentToken = new Token(TokenType.Biconditional, "<=>");
                }
                else
                    throw new Exception("Expected '>' after '<='");
            }
            else
            {
                throw new Exception("Expected '=>' after '<'");
            }
        }
        else if (ch == '(')
        {
            pos++;
            currentToken = new Token(TokenType.LeftParen, "(");
        }
        else if (ch == ')')
        {
            pos++;
            currentToken = new Token(TokenType.RightParen, ")");
        }
        else
        {
            throw new Exception("Unrecognized character: " + ch);
        }
    }

    private void SkipWhiteSpace()
    {
        while (pos < input.Length && char.IsWhiteSpace(input[pos]))
            pos++;
    }

    #endregion
}

