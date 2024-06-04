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

//Clase encargada de realizar el análisis léxico
public class Lexer2
{
    private string _input;
    private int _position;

    public Lexer2(string input)
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
            else if   ((_input[_position] == ':') || (_input[_position] == ';') || (_input[_position] == ',')
                    || (_input[_position] == '{') || (_input[_position] == '}') || (_input[_position] == '[')
                    || (_input[_position] == ']') || (_input[_position] == '(') || (_input[_position] == ')'))
            {
                string symbol = "" + _input[_position];
                _position++;

                tokens.Add(new Token("Symbol", symbol, _position));
            }
        }

        //Devuelve la lista de tokens
        return tokens;
    }
}

public class Lexer : MonoBehaviour
{
    public void Mostrar()
    {
        Lexer2 lexer = new Lexer2(CodeEditor.inputField.text);

        var tokens = lexer.Tokenize();

        foreach(Token token in tokens)
        {
            Debug.Log(token._type);
            Debug.Log(token._value);
            Debug.Log(token._position);
        }
    }
}