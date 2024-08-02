using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;
using System.Threading;

//Clase token para los objetos de tipo "Token"
public class Token
{
    public TokenType Type { get; }
    public string Value { get; }
    public int Position { get; }

    public Token(TokenType type, string value, int position)
    {
        Type = type;
        Value = value;
        Position = position;
    }
}

//Clase encargada de realizar el análisis léxico
public class Lexer
{
    //Constructor de la clase con sus respectivas variables
    private string Input;
    private int Position;

    public Lexer(string input)
    {
        Input = input;
        Position = 0;
    }

    //Método que encuentra y almacena los tokens
    public List<Token> Tokenize()
    {
        //Lista de tokens
        var tokens = new List<Token>();

        //Busca dichos tokens
        while (Position < Input.Length)
        {
            //Verifica si hay espacios en blanco y salta a la siguiente posición
            if(char.IsWhiteSpace(Input[Position]))
            {
                Position++;
                continue;
            }
            //Verifica si es una cadena de números y almacena el token
            else if(char.IsDigit(Input[Position]))
            {
                string number = "";
                while (Position < Input.Length && char.IsDigit(Input[Position]))
                {
                    number += Input[Position];
                    Position++;
                }
                if(Position < Input.Length && char.IsLetter(Input[Position])) throw new SystemException($"Syntax error in: {Position}");
                tokens.Add(new Token(TokenType.Number, number, Position - 1));
            }
            //Verifica si es una palabra
            else if(char.IsLetter(Input[Position]))
            {
                string word = "";
                while (Position < Input.Length && (char.IsLetterOrDigit(Input[Position]) || Input[Position] == '_'))
                {
                    word += Input[Position];
                    Position++;
                }

                //Si es una palabra reservada se guarda como tal, de lo contrario se guarda como palabra
                if(Enum.IsDefined(typeof(KeyWords), word)) tokens.Add(new Token(TokenType.Keyword, word, Position - 1));
                else tokens.Add(new Token(TokenType.Word, word, Position - 1));
            }
            //Verifica si es un símbolo
            else if(Input[Position] == '.')
            {
                Position++;

                tokens.Add(new Token(TokenType.Point, ".", Position));
            }
            else if(Input[Position] == ':')
            {
                Position++;

                tokens.Add(new Token(TokenType.Colon, ":", Position));
            }
            else if(Input[Position] == ';')
            {
                Position++;

                tokens.Add(new Token(TokenType.Semicolon, ";", Position));
            }
            else if(Input[Position] == ',')
            {
                Position++;

                tokens.Add(new Token(TokenType.Comma, ",", Position));
            }
            else if(Input[Position] == '(')
            {
                Position++;

                tokens.Add(new Token(TokenType.LParen, "(", Position));
            }
            else if(Input[Position] == ')')
            {
                Position++;

                tokens.Add(new Token(TokenType.RParen, ")", Position));
            }
            else if(Input[Position] == '{')
            {
                Position++;

                tokens.Add(new Token(TokenType.LCBracket, "{", Position));
            }
            else if(Input[Position] == '}')
            {
                Position++;

                tokens.Add(new Token(TokenType.RCBracket, "}", Position));
            }
            else if(Input[Position] == '[')
            {
                Position++;

                tokens.Add(new Token(TokenType.LSBracket, "[", Position));
            }
            else if(Input[Position] == ']')
            {
                Position++;

                tokens.Add(new Token(TokenType.RSBracket, "]", Position));
            }
            else if(Input[Position] == '+')
            {
                Position++;

                if(Input[Position] == '+')
                {
                    Position++;
                    
                    tokens.Add(new Token(TokenType.Increase, "++", Position));
                } 
                else tokens.Add(new Token(TokenType.Plus, "+", Position));
            }
            else if(Input[Position] == '-')
            {
                Position++;

                tokens.Add(new Token(TokenType.Minus, "-", Position));
            }
            else if(Input[Position] == '*')
            {
                Position++;

                tokens.Add(new Token(TokenType.Multi, "*", Position));
            }
            else if(Input[Position] == '/')
            {
                Position++;

                tokens.Add(new Token(TokenType.Division, "/", Position));
            }
            else if(Input[Position] == '"')
            {
                Position++;
                
                tokens.Add(new Token(TokenType.QMark, ""+'"', Position));
            }
            else if(Input[Position] == '=')
            {
                Position++;
                
                if(Input[Position] == '=')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.Equal, "==", Position));
                } 
                else tokens.Add(new Token(TokenType.Asign, "=", Position));
            }
            else if(Input[Position] == '!')
            {
                Position++;
                
                if(Input[Position] == '=')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.NotEqual, "!=", Position));
                }
                else throw new Exception($"Unvalid token in {Position}");
            }
            else if(Input[Position] == '<')
            {
                Position++;
                
                if(Input[Position] == '=') 
                {
                   Position++; 
                   tokens.Add(new Token(TokenType.SmallerOrEqual, "<=", Position));
                }
                
                else tokens.Add(new Token(TokenType.Smaller, "<", Position));
            }
            else if(Input[Position] == '>')
            {
                Position++;
                
                if(Input[Position] == '=')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.GreaterOrEqual, ">=", Position));
                }

                else tokens.Add(new Token(TokenType.Greater, ">", Position));
            }
            else if(Input[Position] == '|')
            {
                Position++;
            
                if(Input[Position] == '|')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.Or, "||", Position));
                }
                else throw new Exception($"Unvalid token in {Position}");
            }
            else if(Input[Position] == '&')
            {
                Position++;
                
                if(Input[Position] == '&')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.And, "&&", Position));
                }       
                else throw new Exception($"Unvalid token in {Position}");
            }
            else throw new Exception($"Unvalid token in {Position}");
        }

        //Devuelve la lista de tokens
        return tokens;
    }
}