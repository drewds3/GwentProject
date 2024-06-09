using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
                if(_position < _input.Length && char.IsLetter(_input[_position])) throw new SystemException($"Syntax error in: {_position}");
                tokens.Add(new Token("Number", number, _position - 1));
            }
            //Verifica si es una palabra
            else if (char.IsLetter(_input[_position]))
            {
                string word = "";
                while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
                {
                    word += _input[_position];
                    _position++;
                }
                tokens.Add(new Token("Word", word, _position - 1));
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
            else if(_input[_position] == '"')
            {
                _position++;
                
                tokens.Add(new Token("QMark", ""+'"', _position));
            }
        }

        //Devuelve la lista de tokens
        return tokens;
    }
}