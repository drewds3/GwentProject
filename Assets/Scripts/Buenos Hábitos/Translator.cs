using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase token para los objetos de tipo "Token"
public class Token
{
    public string _type { get; }
    public string _value { get; }
    public int _position { get; }

    public Token(string type, string value, int position)
    {
        _type = type;
        _value = value;
        _position = position;
    }
}

//Clase principal
public class Translator : MonoBehaviour
{
    public void Mostrar()
    {
        Lexer lexer = new Lexer(CodeEditor.inputField.text);

        var tokens = lexer.Tokenize();

        Parser parser = new Parser(tokens);

       /*  foreach(Token token in tokens)
        {
            Debug.Log(token._type);
            Debug.Log(token._value);
            Debug.Log(token._position);
        } */

        Debug.Log(parser.Parse());
    }
}

//Clase encargada de realizar el análisis léxico
public class Lexer
{
    //Constructor de la clase con sus respectivas variables
    private string _input;
    private int _position;

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
    }

    //Método que encuentra y almacena los tokens
    public List<Token> Tokenize()
    {
        //Lista de tokens
        var tokens = new List<Token>();

        //Busca dichos tokens
        while (_position < _input.Length)
        {
            //Verifica si hay espacios en blanco y salta a la siguiente posición
            if (char.IsWhiteSpace(_input[_position]))
            {
                _position++;
                continue;
            }
            //Verifica si es una cadena de números y almacena el token
            else if (char.IsDigit(_input[_position]))
            {
                string number = "";
                while (_position < _input.Length && char.IsDigit(_input[_position]))
                {
                    number += _input[_position];
                    _position++;
                }
                tokens.Add(new Token("Number", number, _position - 1));
            }
            //Verifica si es una palabra clave
            else if (char.IsLetter(_input[_position]))
            {
                string identifier = "";
                while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
                {
                    identifier += _input[_position];
                    _position++;
                }
                tokens.Add(new Token("Identifier", identifier, _position - 1));
            }
            //Verifica si es un símbolo
            else if   (_input[_position] == ':')
            {
                _position++;

                tokens.Add(new Token("Colon", ":", _position));
            }
            else if(_input[_position] == ';')
            {
                _position++;

                tokens.Add(new Token("Semicolon", ";", _position));
            }
            else if(_input[_position] == ',')
            {
                _position++;

                tokens.Add(new Token("Comma", ",", _position));
            }
            else if(_input[_position] == '(')
            {
                _position++;

                tokens.Add(new Token("LParen", "(", _position));
            }
            else if(_input[_position] == ')')
            {
                _position++;

                tokens.Add(new Token("RParen", ")", _position));
            }
            else if(_input[_position] == '{')
            {
                _position++;

                tokens.Add(new Token("LCBracket", "{", _position));
            }
            else if(_input[_position] == '}')
            {
                _position++;

                tokens.Add(new Token("RCBracket", "}", _position));
            }
            else if(_input[_position] == '[')
            {
                _position++;

                tokens.Add(new Token("LSBracket", "[", _position));
            }
            else if(_input[_position] == ']')
            {
                _position++;

                tokens.Add(new Token("RSBracket", "]", _position));
            }
            else if(_input[_position] == '+')
            {
                _position++;

                tokens.Add(new Token("Plus", "+", _position));
            }
            else if(_input[_position] == '-')
            {
                _position++;

                tokens.Add(new Token("Minus", "-", _position));
            }
            else if(_input[_position] == '*')
            {
                _position++;

                tokens.Add(new Token("Multiplication", "*", _position));
            }
            else if(_input[_position] == '/')
            {
                _position++;

                tokens.Add(new Token("Division", "/", _position));
            }
        }

        //Devuelve la lista de tokens
        return tokens;
    }
}

//Clase encargada de realizar el análisis sintáctico
public class Parser
{
    //Constructor de la clase con sus respectivas variables
    private readonly List<Token> _tokens;
    private int _currentTokenIndex;
    private Token _currentToken;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _currentTokenIndex = 0;
        _currentToken = _tokens[_currentTokenIndex];
    }

    //Método para cambiar al siguiente token
     private void Next(string type)
    {
        if (_currentToken._type == type)
        {
            _currentTokenIndex++;
            _currentToken = _currentTokenIndex < _tokens.Count ? _tokens[_currentTokenIndex] : new Token("Fin", "null", 0);
        }
        else
        {
            Debug.LogError($"Unexpected token: {_currentToken._type}");
        }
    }

    //Método para procesar números
    private int Factor()
    {
        Token token = _currentToken;

        /*Si el token es correcto pasa al siguiente
         y devuelve el valor del número o expresión entre paréntesis*/
        if(token._type ==  "Number")
        {
            Next("Number");

            return int.Parse(token._value);
        }
        else if(token._type == "LParen")
        {
            Next("LParen");
            int result = Expr();
            Next("RParen");

            return result;
        }
        Debug.LogError($"Unexpected token: {token._type}");
        return -1;
    }

    //Método para procesar multiplicaciones y divisiones
    private int Term()
    {
        int result = Factor();

        /*Mientras que el token actual sea una multiplicación o división
          se avanza al siguiente y se realiza la operación en cuestión*/
        while (_currentToken._type == "Multiplication" || _currentToken._type == "Division")
        {
            Token token = _currentToken;
            if (token._type == "Multiplication")
            {
                Next("Multiplication");
                result *= Factor();
            }
            else if (token._type == "Division")
            {
                Next("Division");
                result /= Factor();
            }
        }
        return result;
    }

    //Método para procesar sumas y restas
    private int Expr()
    {
        int result = Term();

        /*Mientras que el token actual sea una suma o resta
          se avanza al siguiente y se realiza la operación en cuestión*/
        while (_currentToken._type == "Plus" || _currentToken._type == "Minus")
        {
            Token token = _currentToken;
            if (token._type == "Plus")
            {
                Next("Plus");
                result += Term();
            }
            else if (token._type == "Minus")
            {
                Next("Minus");
                result -= Term();
            }
        }
        return result;
    }

    //Método principal
    public int Parse()
    {
        return Expr();
    }
}