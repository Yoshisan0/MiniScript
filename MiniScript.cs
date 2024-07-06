using System;
using System.Collections.Generic;

public class MiniScript
{
    public static string Calc(string expression)
    {
        try
        {
            var tokens = Tokenize(expression);
            var rpn = ConvertToRPN(tokens);
            var result = EvaluateRPN(rpn);
            return result.ToString();
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }

    private static List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var number = "";
        bool lastCharWasOperator = true;

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if (char.IsDigit(c) || c == '.')
            {
                number += c;
                lastCharWasOperator = false;
            }
            else
            {
                if (number != "")
                {
                    tokens.Add(number);
                    number = "";
                }

                if (c == '-' && (lastCharWasOperator || i == 0))
                {
                    // マイナス記号が先頭または演算子の直後にある場合
                    if (i + 1 < expression.Length && expression[i + 1] == '(')
                    {
                        // 次の文字が開き括弧の場合、-1* を挿入
                        tokens.Add("-1");
                        tokens.Add("*");
                    }
                    else
                    {
                        number += c; // マイナス記号を数値として扱う
                    }
                }
                else
                {
                    tokens.Add(c.ToString());
                    lastCharWasOperator = (c != ')');
                }
            }
        }

        if (number != "")
        {
            tokens.Add(number);
        }

        return tokens;
    }

    private static List<string> ConvertToRPN(List<string> tokens)
    {
        var output = new List<string>();
        var operators = new Stack<string>();
        var precedence = new Dictionary<string, int>
        {
            { "+", 1 },
            { "-", 1 },
            { "*", 2 },
            { "/", 2 },
            { "(", 0 }
        };

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                output.Add(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                operators.Pop();
            }
            else
            {
                while (operators.Count > 0 && precedence[operators.Peek()] >= precedence[token])
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
        }

        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }

        return output;
    }

    private static double EvaluateRPN(List<string> rpn)
    {
        var stack = new Stack<double>();

        foreach (var token in rpn)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else
            {
                var right = stack.Pop();
                var left = stack.Pop();

                switch (token)
                {
                    case "+":
                        stack.Push(left + right);
                        break;
                    case "-":
                        stack.Push(left - right);
                        break;
                    case "*":
                        stack.Push(left * right);
                        break;
                    case "/":
                        stack.Push(left / right);
                        break;
                }
            }
        }

        return stack.Pop();
    }
}
