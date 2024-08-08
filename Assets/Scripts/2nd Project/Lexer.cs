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

                tokens.Add(new Token(TokenType.Point, ".", Position - 1));
            }
            else if(Input[Position] == ':')
            {
                Position++;

                tokens.Add(new Token(TokenType.Colon, ":", Position - 1));
            }
            else if(Input[Position] == ';')
            {
                Position++;

                tokens.Add(new Token(TokenType.Semicolon, ";", Position - 1));
            }
            else if(Input[Position] == ',')
            {
                Position++;

                tokens.Add(new Token(TokenType.Comma, ",", Position - 1));
            }
            else if(Input[Position] == '(')
            {
                Position++;

                tokens.Add(new Token(TokenType.LParen, "(", Position - 1));
            }
            else if(Input[Position] == ')')
            {
                Position++;

                tokens.Add(new Token(TokenType.RParen, ")", Position - 1));
            }
            else if(Input[Position] == '{')
            {
                Position++;

                tokens.Add(new Token(TokenType.LCBracket, "{", Position - 1));
            }
            else if(Input[Position] == '}')
            {
                Position++;

                tokens.Add(new Token(TokenType.RCBracket, "}", Position - 1));
            }
            else if(Input[Position] == '[')
            {
                Position++;

                tokens.Add(new Token(TokenType.LSBracket, "[", Position - 1));
            }
            else if(Input[Position] == ']')
            {
                Position++;

                tokens.Add(new Token(TokenType.RSBracket, "]", Position - 1));
            }
            else if(Input[Position] == '+')
            {
                Position++;

                if(Input[Position] == '+')
                {
                    Position++;
                    
                    tokens.Add(new Token(TokenType.Increase, "++", Position - 1));
                } 
                else tokens.Add(new Token(TokenType.Plus, "+", Position - 1));
            }
            else if(Input[Position] == '-')
            {
                Position++;

                tokens.Add(new Token(TokenType.Minus, "-", Position - 1));
            }
            else if(Input[Position] == '*')
            {
                Position++;

                tokens.Add(new Token(TokenType.Multi, "*", Position - 1));
            }
            else if(Input[Position] == '/')
            {
                Position++;

                tokens.Add(new Token(TokenType.Division, "/", Position - 1));
            }
            else if(Input[Position] == '"')
            {
                Position++;
                
                tokens.Add(new Token(TokenType.QMark, "\"", Position - 1));

                string word = "";

                //Se recoge todo lo que esté dentro del string
                while(Position < Input.Length && Input[Position] != '"')
                {
                    word += Input[Position];
                    Position++;
                }

                tokens.Add(new Token(TokenType.String, word, Position - 1));

                if(Position < Input.Length && Input[Position] == '"')
                {
                    tokens.Add(new Token(TokenType.QMark, "\"", Position - 1));
                    Position++;
                } 
            }
            else if(Input[Position] == '=')
            {
                Position++;
                
                if(Input[Position] == '=')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.Equal, "==", Position - 1));
                } 
                else tokens.Add(new Token(TokenType.Asign, "=", Position - 1));
            }
            else if(Input[Position] == '!')
            {
                Position++;
                
                if(Input[Position] == '=')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.NotEqual, "!=", Position - 1));
                }
                else throw new Exception($"Unvalid token in {Position - 1}");
            }
            else if(Input[Position] == '<')
            {
                Position++;
                
                if(Input[Position] == '=') 
                {
                   Position++; 
                   tokens.Add(new Token(TokenType.SmallerOrEqual, "<=", Position - 1));
                }
                
                else tokens.Add(new Token(TokenType.Smaller, "<", Position - 1));
            }
            else if(Input[Position] == '>')
            {
                Position++;
                
                if(Input[Position] == '=')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.GreaterOrEqual, ">=", Position - 1));
                }

                else tokens.Add(new Token(TokenType.Greater, ">", Position - 1));
            }
            else if(Input[Position] == '|')
            {
                Position++;
            
                if(Input[Position] == '|')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.Or, "||", Position - 1));
                }
                else throw new Exception($"Unvalid token in {Position - 1}");
            }
            else if(Input[Position] == '&')
            {
                Position++;
                
                if(Input[Position] == '&')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.And, "&&", Position - 1));
                }       
                else throw new Exception($"Unvalid token in {Position - 1}");
            }
            else if(Input[Position] == '^')
            {
                Position++;
        
                tokens.Add(new Token(TokenType.XOR, "^", Position - 1));
            }
            else if(Input[Position] == '@')
            {
                Position++;
                
                if(Input[Position] == '@')
                {
                    Position++;
                    tokens.Add(new Token(TokenType.DoubleAt, "@@", Position - 1));
                }
                else tokens.Add(new Token(TokenType.At, "@", Position - 1));
            }
            else throw new Exception($"Unvalid token in {Position - 1}");
        }

        //Devuelve la lista de tokens
        return tokens;
    }
}