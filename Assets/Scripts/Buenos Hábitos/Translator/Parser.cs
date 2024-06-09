using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Clase encargada de realizar el análisis sintáctico
public class Parser
{
    //Constructor de la clase con sus respectivas variables
    private readonly List<Token> _tokens;
    private int _currentTokenIndex;
    private Token _currentToken;
    private List<Property> properties; //Esto es para añadirlo a la carta

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _currentTokenIndex = 0;
        _currentToken = _tokens[_currentTokenIndex];
        properties = new();
    }

    //Método principal
    public void Parse()
    {
        if(_currentToken._type == "Word")
        {
            if(_currentToken._value == "effect")
            {
                throw new SystemException("No has implementado una mierda pibe");
            }
            else if(_currentToken._value == "card")
            {
                Next("Word");
                Next("LCBracket");
                Properties();
                Next("RCBracket");
            }
            else throw new SystemException($"Syntax error in: {_currentToken._position}");
        }
        else throw new SystemException($"Syntax error in: {_currentToken._position}");

        foreach(Property property in properties)
        {
            Debug.Log($"Propiedad: {property.Type}, Valor: {property.Value}");
        }
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
            throw new SystemException($"Unexpected token: {_currentToken._type}, was expected {type}");
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
            int result = int.Parse(token._value);

            Next("Number");

            token = _currentToken;

            while(token._type == "LParen")
            {
                Next("LParen");
                result *= Expr();
                Next("RParen");

                token = _currentToken;
            }
            return result;
        }
        else if(token._type == "LParen")
        {
            Next("LParen");
            int result = Expr();
            Next("RParen");

            token = _currentToken;

            while(token._type == "LParen")
            {
                Next("LParen");
                result *= Expr();
                Next("RParen");

                token = _currentToken;
            }
            return result;
        }
        throw new ArithmeticException($"Unexpected token: {token._type}");
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

    private void Properties()
    {   
        while(_currentToken._value == "Type" || _currentToken._value == "Name" || _currentToken._value == "Faction"
                                             || _currentToken._value == "Power" || _currentToken._value == "Range")
        {
            Token token = _currentToken;

            Next("Word");
            Next("Colon");
            Next("QMark");
            String(token._value, _currentToken._value);
            Next("Comma");
        }
    }

    private void String(string type, string name)
    {
        Next("Word");
        Next("QMark");

        Property property = new(name, type);

        properties.Add(property);
    }
}

class Property
{
    public readonly object Value;
    public readonly string Type;

    public Property(object value, string type)
    {
        Value = value;
        Type = type;
    }
}