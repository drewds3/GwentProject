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
                throw new SystemException("No has implementado nada aún");
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
            Debug.Log($"Propiedad: {property.Type}, Valor: {property.ValueS}");
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

    //Método para recolectar las propiedades declaradas
    private void Properties()
    {   
        int count = 0;
        bool isSpecialCard = false;
        
        //Se va guardando el tipo de la propiedad y el valor asignado
        while(_currentToken._value == "Type" || _currentToken._value == "Name" || _currentToken._value == "Faction"
                                             || _currentToken._value == "Power" || _currentToken._value == "Range")
        {
            Token token = _currentToken;

            Next("Word");
            Next("Colon");
            if(token._value == "Power") ToProperty(token._value);
            else if(token._value == "Range")
            {
                Next("LSBracket");
                Next("QMark");
                ToProperty(token._value, _currentToken._value);

                while(_currentToken._type == "Comma")
                {
                    Next("Comma");
                    Next("QMark");
                    ToProperty(token._value, _currentToken._value);
                }

                Next("RSBracket");
            }
            else
            {   
                Next("QMark");
                if(token._value == "Type" && _currentToken._value != "Gold" && _currentToken._value != "Silver")
                {
                    isSpecialCard = true;
                    Debug.Log("Es especiallll");
                } 
                ToProperty(token._value, _currentToken._value);
            } 

            count++;

            if((count < 3 && isSpecialCard) || (count < 5 && !isSpecialCard)) Next("Comma");
            //else if(count == 5) count = 0;
        }

        //Luego de recoger todas las propiedades declaradas se verifica si son correctas
        CheckProperties();
    }

    //Método para añadir la propiedad de la carta a una lista con todas las propiedades de la misma
    private void ToProperty(string type, string value)
    {
        Next("Word");
        Next("QMark");

        Property property = new(type, value);

        properties.Add(property);
    }

    //Sobrecarga del método anterior en el caso de que la propiedad sea el "Power"
    private void ToProperty(string type)
    {
        int value = Expr();

        Property property = new(type, value);

        properties.Add(property);
    }

    //Método para agregarle las propiedades a la carta nueva
    public void SetProperties(Card card)
    {
        int count = 0;

        for(int i = 0; i < properties.Count; i++)
        {
            //Se le otorga en dependencia del tipo de la propiedad
            if(properties[i].Type == "Type") card.Type = (string)properties[i].ValueS;
            else if(properties[i].Type == "Name") card.Name = (string)properties[i].ValueS;
            else if(properties[i].Type == "Faction") card.Faction = (string)properties[i].ValueS;
            else if(properties[i].Type == "Power") card.Power = (int)properties[i].ValueI;
            else if(properties[i].Type == "Range")
            {
                //En el caso del rango se comprueba que sea uno de los establecidos y lanza una excepción de no serlo
                if(properties[i].ValueS == "Melee" || properties[i].ValueS == "Ranged" || properties[i].ValueS == "Siege")
                if(count==0)
                {
                    card.Range1 = (string)properties[i].ValueS;
                    count++;
                } 
                else if(count==1)
                {
                    card.Range2 = (string)properties[i].ValueS;
                    count++;
                }
                else
                {
                    card.Range3 = (string)properties[i].ValueS;
                    count++;
                }
                else throw new Exception("Unvalid card range");
            } 
        }
    }

    //Método para comprobar que las propiedades de las carta sean correctas
    public void CheckProperties()
    {
        bool isSpecialCard = false;

        bool hasType = false;
        bool hasName = false;
        bool hasFaction = false;

        bool hasPower = false;
        bool hasRange = false;

        //Comprueba si tiene o no las propiedades necesarias
        foreach(Property property in properties)
        {
            if(property.Type == "Type")
            {   
                if(property.ValueS == "Clearance" || property.ValueS == "Climate" || property.ValueS == "Leader"
                                                  || property.ValueS == "Lure" || property.ValueS == "Increase") isSpecialCard = true;
                else if(property.ValueS != "Gold" && property.ValueS != "Silver") throw new Exception("This type of card does not exist");
            
                hasType = true;
            }
            else if(property.Type == "Name") hasName = true;
            else if(property.Type == "Faction") hasFaction = true;
            else if(property.Type == "Power") hasPower = true;
            else if(property.Type == "Range") hasRange = true;
        }

        //Si le faltan o tiene de más lanza una excepción
        if((isSpecialCard && (!hasType || !hasFaction || !hasName || hasPower || hasRange))
        || (!isSpecialCard && (!hasType || !hasFaction || !hasName || !hasPower || !hasRange)))
        throw new Exception("Misstatement of card properties");
    }

    public string WhichTypeCardIS()
    {
        foreach(Property property in properties)
        {
            if(property.Type == "Type" && (property.ValueS == "Gold" || property.ValueS == "Silver")) return "Unit";
            else if(property.Type == "Type" && property.ValueS == "Leader") return "Leader";
            else return "Other";
        }
        throw new Exception("This card has no type");
    }
}

//Clase propiedad
class Property
{
    public readonly string Type;
    public readonly string ValueS;
    public readonly int ValueI;
    
    //Dos constructores, la segunda es para el caso particular del "Power" y la primera para el resto
    public Property(string type, string value)
    {
        Type = type;
        ValueS = value;
    }

    public Property(string type, int value)
    {
        Type = type;
        ValueI = value;
        ValueS = $"{value}";
    }
}