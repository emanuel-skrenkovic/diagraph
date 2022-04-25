using System.Text.RegularExpressions;

namespace Diagraph.Infrastructure.Parsing.Language;

public class TemplateLanguageParser
{
    private readonly List<Token> _tokens = new();

    private int _position = -1;

    private Token _previous;
    private Token _current;

    private readonly IDictionary<string, object> _eventData;
    private readonly IDictionary<string, object> _data;

    private readonly Dictionary<string, Func<string, string, string>> _functions = new()
    {
        ["regex"] = ApplyRegex
    };

    public TemplateLanguageParser
    (
        IDictionary<string, object> eventData,
        IDictionary<string, object> data 
    )
    {
        _eventData  = eventData;
        _data       = data;
    }
    
    public void ApplyRule(string expression, string[] allowedFields = null)
    {
        Ensure.NotNullOrEmpty(expression);

        ScanTokens(expression);
        
        Advance();

        Consume(Symbol.Identifier, "Expression must start with a field identifier.");
        Token field = _previous;

        if (allowedFields?.Any() == true)
        {
            if (!allowedFields.Contains(field.Value))
            {
                throw new Exception($"'{field.Value}' is not an allowed Event field");
            }
        }
        
        Consume(Symbol.Equal, "Event identifier must be followed by '='.");
        
        Token current = _current;
        string result = "";
        while (current != null)
        {
            try
            {
                result = AddExpr(result);
                current = Advance();
            }
            catch (ParseEnd)
            {
                break;
            }
        }
        
        _eventData[field.Value] = result;

        _tokens.Clear();
        _current  = null;
        _position = -1;
    }
    
    private string AddExpr(string result)
    {
        result = SelectorExpr(result);
        
        while (Match(Symbol.Plus))
        {
            result = SelectorExpr(result);
        }

        return result;
    }

    private string SelectorExpr(string result)
    {
        result = LiteralExpr(result);
        
        if (Check(Symbol.Selector))
        {
            if (PeekNext().Type == Symbol.Dot)
            {
                Advance();
                result += CallExpr(result);
            }
            else
            {
                if (_data.ContainsKey(_current.Value))
                {
                    result += _data[_current.Value];
                }                
            }
        }
        
        return result;
    }
    
    private string CallExpr(string result)
    {
        _data.TryGetValue(_previous.Value, out object inputObj);
        string input = inputObj as string ?? "";
        
        if (Match(Symbol.Dot))
        {
            if (!Match(Symbol.Identifier))
            {
                throw new Exception
                (
                    "Function call dot (.) operator must be followed by an open bracket."
                );
            }
            string functionIdentifier = _previous.Value;
        
            Consume
            (
                Symbol.LeftParen, 
                "Function call dot (.) operator must be followed by an open bracket."
            );

            string arg = LiteralExpr("");
            var function = _functions[functionIdentifier];
            result += function(input, arg);
            
            /*
            Advance();
            Consume(Symbol.RightParen, "Function argument must be closed with a right bracket ')'.");
            */
        }
        else
        {
            result = LiteralExpr(result);
        }

        return result;
    }

    private string LiteralExpr(string result)
    {
        if (Check(Symbol.Literal))
        {
            result += _current.Value;
        }
        
        return result;
    }

    private static string ApplyRegex(string result, string regEx)
    {
        Ensure.NotNullOrEmpty(regEx);
        
        Regex regex = new(regEx);
        Match match = regex.Match(result);

        return !string.IsNullOrWhiteSpace(match.Value) 
            ? match.Value 
            : result;
    }

    private void ScanTokens(string expression)
    {
        int current = 0;
        while (current < expression.Length)
        {
            char c = expression[current];
            switch (c)
            {
                case ' ':
                case '\n':
                case '\0':
                case '\t':
                    break;

                case '@':
                {
                    c = expression[++current];

                    string value;
                    if (c == '"')
                    {
                        c = expression[++current];
                        int start = current;
                        while (c != '"')
                        {
                            current++;
                            c = expression[current];
                        }

                        value = expression.Substring(start, current - start);
                        
                        // c = expression[++current];
                    }
                    else
                    {
                        int start = current;
                        while (Char.IsDigit(c) || Char.IsLetter(c))
                        {
                            if (++current >= expression.Length) break;
                            c = expression[current];
                        } 
                        
                        value = expression.Substring(start, current - start);
                        
                        current--; // oh no
                    }
                    
                    _tokens.Add(new(Symbol.Selector, value));
                    break;
                }
                case '"':
                {
                    c = expression[++current];
                    int start = current;
                    while (c != '"')
                    {
                        current++;
                        c = expression[current];
                    }

                    _tokens.Add
                    (
                        new
                        (
                            Symbol.Literal, 
                            expression.Substring(start, current - start)
                        )
                    );
                    
                    break;
                }
                case '(': _tokens.Add(new(Symbol.LeftParen, c.ToString()));  break;
                case ')': _tokens.Add(new(Symbol.RightParen, c.ToString())); break;
                case '+': _tokens.Add(new(Symbol.Plus, c.ToString()));       break;
                case '.': _tokens.Add(new(Symbol.Dot, c.ToString()));        break;
                case '=': _tokens.Add(new(Symbol.Equal, c.ToString()));      break;
                default:
                {
                    int start = current;
                    while (Char.IsDigit(c) || Char.IsLetter(c))
                    {
                        current++;
                        c = expression[current];
                    }

                    _tokens.Add
                    (
                        new(Symbol.Identifier, expression.Substring(start, current - start))
                    );
                    
                    // TODO: stupid
                    if (start != current) current--;
                    break;
                }
            }
            current++;
        }
    }

    private bool Check(Symbol symbol) => _current?.Type == symbol;
    
    private bool Match(Symbol symbol)
    {
        if (!Check(symbol)) return false;
        
        Advance();
        return true;
    }

    private void Consume(Symbol symbol, string errorMessage)
    {
        if (!Check(symbol)) throw new Exception(errorMessage);

        Advance();
    }

    private Token Advance()
    {
        _previous = _current is null ? null : _current with { };
        
        if (++_position == _tokens.Count)
        {
            throw new ParseEnd();
        }
        
        _current = _tokens[_position];
        
        return _current;
    }

    private Token PeekNext()
        => _position + 1 >= _tokens.Count
            ? _tokens[^1] 
            :_tokens[_position + 1]; 
    
    // "occurredAtUtc = @Datum.regex() + "
    
    private enum Symbol
    {
        Selector, Literal,
        LeftParen, RightParen,
        Plus, Dot, Equal,
        Identifier
    }

    private record Token(Symbol Type, string Value);
    
    private class ParseEnd : Exception { }
}