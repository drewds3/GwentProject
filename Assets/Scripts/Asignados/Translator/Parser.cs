using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//Clase encargada de realizar el análisis sintáctico
public class Parser
{
    //Constructor de la clase con sus respectivos campos
    private readonly List<Token> _tokens;
    private int _currentTokenIndex;
    private Token _currentToken;
    private List<Property> properties; //Esto es para añadirlo a la carta
    private Instruction CurrentInstruction =new();
    private List<Effect> effects = new();

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
        //Comprueba si el primer token es correcto
        if(_currentToken._type == "Word")
        {
            //De ser así y mientras sea "effect" se crearan los efectos correspondientes
            while(_currentToken._value == "effect")
            {
                Next("Word");
                Next("LCBracket");

                //Se recoge el nombre del efecto
                if(_currentToken._value == "Name") Next("Word");
                else throw new Exception("You have not declared the name of the effect correctly");    
                Next("Colon");
                Next("QMark");
                string nameEffect = _currentToken._value;
                Next("Word");
                Next("QMark");
                Next("Comma");

                //Se recogen los parámetros si es que tiene
                LinkedList<Param> parameters = new();
                if(_currentToken._value == "Params")
                {
                    Next("Word");
                    Next("Colon");
                    Next("LCBracket");

                    while(true)
                    {
                        string name = _currentToken._value;
                        Next("Word");
                        Next("Colon");
                        string type;
                        
                        if(_currentToken._value == "Number" || _currentToken._value == "Bool" || _currentToken._value == "String") type = _currentToken._value;
                        else throw new Exception("This type of parameter does not exist");
                        
                        Next("Word");
                        
                        Param param = new(name, type);
                        parameters.AddLast(param);

                        if(_currentToken._type == "Comma") Next("Comma");
                        else break;
                    }
                    Next("RCBracket");
                    Next("Comma");
                }

                if(parameters.Count != 0)
                {
                    Debug.Log("Parámetros del efecto:");
                    foreach(Param param in parameters)
                    {
                        Debug.Log($"Parámetro: {param.Name}, Tipo: {param.Type}");
                    }
                }
                else Debug.Log("El efecto no tiene parámetros");

                //Prosigue con la acción del efecto
                if(_currentToken._value == "Action") Next("Word");
                else throw new Exception("You have not declared the action of the effect correctly");
                
                //Se recogen los parámetros "targets" y "context"
                Next("Colon");
                Next("LParen");
                string targets = _currentToken._value;
                Next("Word");
                Next("Comma");
                string context = _currentToken._value;
                Next("Word");
                Next("RParen");
                Next("Equal");
                Next("Greater");
                Next("LCBracket");

                //Cuerpo del método (acción del efecto en sí)
                //Se procede a recolectar las instrucciones en las siguiente lista
                LinkedList<Instruction> instructions = new();

                //Se añaden como variables los parámetros "targets" y "context"
                List<Variable> variables = new() {{new Variable(targets, "List<Card>")}, {new Variable(context, "Context")}};
                
                int count = 0;

                InstructionsRecolector();

                //Se muestran en consola las instrucciones recogidas
                foreach(Instruction instruction in instructions)
                {
                    instruction.Debug();
                }
                
                //Se crea (por fin) el efecto
                effects.Add(new Effect(nameEffect, instructions));
                Debug.Log("Efecto creado con éxito");

                //--------------------------------------------Métodos-del-Método----------------------------------------------------------
                //Método que recoge las varibles y llamadas a métodos
                void InstructionsRecolector()
                {   
                    //Se añaden instrucciones hasta que el token actual sea una llave de cierre
                    if(_currentToken._type != "RCBracket")
                    {
                        //Si es una declaración de una variable se añade si está correctamente declarada
                        if(_currentToken._type == "Word" && Enum.IsDefined(typeof(KeyWords), _currentToken._value)) throw new Exception($"You shouldn't use \"{_currentToken._value}\" like that");
                        else if(!IsVariable() && _currentToken._type == "Word")
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.AddLast(new Instruction());
                            CurrentInstruction = instructions.Last();

                            string name = _currentToken._value;
                            NextAndSave("Word");
                            NextAndSave("Equal");
                            if(IsVariable()) variables.Add(new Variable(name, WichTypeIs("Semicolon")));
                            else if(_currentToken._type == "Number")
                            {
                                Variable number = new(name, "Number") {Value = Expr()};
                                variables.Add(number);
                            }

                            //Si se llega al final de la instrucción se recoge la siguiente de haberla declarado correctamente
                            if(_currentToken._type == "Semicolon")
                            {
                                Next("Semicolon");
                                Debug.Log($"Instrucción {++count} recogida correctamente: \"Variable\"");
                                InstructionsRecolector();
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else if(IsVariable()) //Si es una variable se espera que llame a un método
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.AddLast(new Instruction());
                            CurrentInstruction = instructions.Last();

                            //Se recoge la instrucción de ser correcta
                            if(!CheckMethodCall()) throw new Exception("Bad method call");
                            
                            //Si se llega al final de la instrucción se recoge la siguiente de haberla
                            if(_currentToken._type == "Semicolon")
                            {
                                Next("Semicolon");
                                Debug.Log($"Instrucción {++count} recogida correctamente: \"Método\"");
                                InstructionsRecolector();
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else throw new Exception("An instruction was expected");
                    }
                    else
                    {
                        Next("RCBracket");
                        Next("RCBracket");
                        Debug.Log("Instrucciones recogidas correctamente");
                    }
                }

                //Método para cambiar al siguiente token y guardarlo como parte de la instrucción
                void NextAndSave(string type)
                {
                    CurrentInstruction.Add(_currentToken._value);
                    Next(type);
                }

                //Método para comprobar si es una variable
                bool IsVariable()
                {
                    if(_currentToken._type == "Word" && Enum.IsDefined(typeof(KeyWords), _currentToken._value)) return false;
                    else
                    {
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == _currentToken._value) return true;
                        }
                        return false;
                    }
                }
                
                //Método que recoge la instrucción de llamada a un método mientras que verifica que sea correcta
                bool CheckMethodCall()
                {
                    //Comprueba si es una variable
                    if(IsVariable())
                    {
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == _currentToken._value)
                            {
                                if(variable.Type == "Context")
                                {
                                    NextAndSave("Word");
                                    if(_currentToken._type == "Point") Next("Point");
                                    else return false;
                                    if(_currentToken._type == "Word" && Enum.IsDefined(typeof(ContextMethods), _currentToken._value)) return CheckMethodCall();
                                    else return false;
                                }
                                else if(variable.Type == "List<Card>")
                                {
                                    NextAndSave("Word");
                                    if(_currentToken._type == "Point") Next("Point");
                                    else return false;
                                    if(_currentToken._type == "Word" && Enum.IsDefined(typeof(CardListMethods), _currentToken._value)) return CheckMethodCall();
                                    else return false;
                                }
                                else return false;
                            }
                        }
                        return false;
                    }
                    //De lo contrario comprueba que sea un método válido
                    else if(_currentToken._type == "Word" && Enum.IsDefined(typeof(ContextMethods), _currentToken._value))
                    {
                        if(_currentToken._type == "Word" && Enum.IsDefined(typeof(SyntacticSugarContext), _currentToken._value))
                        {
                            NextAndSave("Word");
                            Next("Point");
                            if(_currentToken._type == "Word" && Enum.IsDefined(typeof(CardListMethods), _currentToken._value)) return CheckMethodCall();
                            else return false;
                        }
                        else
                        {
                            NextAndSave("Word");
                            Next("LParen");

                            bool correct = false;
            
                            foreach(Variable variable in variables)
                            {
                                if(variable.Name == _currentToken._value && WichTypeIs("RParen") == "Player") correct = true;
                            }

                            if(correct) Next("RParen");
                            else return false;

                            Next("Point");
                            
                            if(_currentToken._type == "Word" && Enum.IsDefined(typeof(CardListMethods), _currentToken._value)) return CheckMethodCall();
                            else return false;
                        }
                    }
                    else if(_currentToken._type == "Word" && Enum.IsDefined(typeof(CardListMethods), _currentToken._value))
                    {
                        if(_currentToken._value == "Shuffle")
                        {
                            NextAndSave("Word");
                            Next("LParen");
                            Next("RParen");
                            if(_currentToken._type == "Semicolon") return true;
                            else return false;
                        }
                        else
                        {
                            NextAndSave("Word");
                            Next("LParen");

                            if(WichTypeIs("RParen") == "Card")
                            {
                                Next("RParen");
                                if(_currentToken._type == "Semicolon") return true;
                                else return false;
                            }
                            else return false;  
                        }
                    }
                    else return false;
                }

                //Método que devuelve el tipo de una variable y verifica que esté bien declarada
                string WichTypeIs(string finalToken)
                {
                    //Si es una palabra...
                    if(_currentToken._type == "Word")
                    {
                        //Comprueba si es una variable
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == _currentToken._value)
                            {
                                //De ser así verifica el tipo de la variable
                                if(variable.Type == "Context")
                                {
                                    NextAndSave("Word");
                                    if(_currentToken._type == finalToken) return "Context";
                                    else if(_currentToken._type == "Point")
                                    {
                                        Next("Point");
                                        if(_currentToken._type == "Word" && Enum.IsDefined(typeof(ContextPropertiesAndMethods), _currentToken._value)) return WichTypeIs("Semicolon");
                                        else throw new Exception("Uknown method or property of context");
                                    } 
                                }
                                else if(variable.Type == "List<Card>")
                                {
                                    NextAndSave("Word");
                                    if(_currentToken._type == finalToken) return "List<Card>";
                                    else if(_currentToken._type == "Point")
                                    {
                                        Next("Point");
                                        if(_currentToken._type == "Word" && Enum.IsDefined(typeof(CardListProperties), _currentToken._value)) return WichTypeIs("Semicolon");
                                        else throw new Exception("Uknown method or property of context");
                                    } 
                                }
                                else if(variable.Type == "Card")
                                {
                                    NextAndSave("Word");
                                    if(_currentToken._type == finalToken) return "Card";
                                    else if(_currentToken._type == "Point")
                                    {
                                        Next("Point");
                                        if(_currentToken._type == "Word" && Enum.IsDefined(typeof(CardProperties), _currentToken._value)) return WichTypeIs("Semicolon");
                                        else throw new Exception("Uknown method or property of context");
                                    } 
                                }
                                else if(variable.Type == "Player")
                                {
                                    NextAndSave("Word");
                                    if(_currentToken._type == finalToken) return "Player";
                                    else throw new Exception("Uknown method or property of context"); 
                                }
                            } 
                        }
                        
                        //Comprueba si es un método y devuelve su tipo de serlo
                        if(Enum.IsDefined(typeof(ContextPropertiesAndMethods), _currentToken._value))
                        {
                            if(_currentToken._value == "TriggerPlayer")
                            {
                                NextAndSave("Word");
                                return "Player";
                            } 
                            else if(Enum.IsDefined(typeof(SyntacticSugarContext), _currentToken._value))
                            {
                                NextAndSave("Word");
                                if(_currentToken._type == finalToken) return "List<Card>";
                                else if(_currentToken._type == "Point")
                                {
                                    Next("Point");
                                    return WichTypeIs("Semicolon");
                                }
                                else throw new Exception("Expected semicolon or method call");
                            } 
                            else
                            {
                                NextAndSave("Word");
                                Next("LParen");

                                if(WichTypeIs("RParen") == "Player") Next("RParen");
                                else throw new Exception("A parameter of type player was expected");

                                if(_currentToken._type == finalToken) return "List<Card>";
                                else if(_currentToken._type == "Point")
                                {
                                    Next("Point");
                                    if(_currentToken._type == "Word" && Enum.IsDefined(typeof(CardListProperties), _currentToken._value)) return WichTypeIs("Semicolon");
                                    else throw new Exception("Uknown method or property of context");
                                }
                                else throw new Exception("Expected semicolon or method call");
                            }
                        }
                        else if(Enum.IsDefined(typeof(CardProperties), _currentToken._value))
                        {
                            NextAndSave("Word");
                            return "Player";
                        } 
                        else if(Enum.IsDefined(typeof(CardListProperties), _currentToken._value))
                        {
                            if(_currentToken._value == "Pop")
                            {
                                NextAndSave("Word");
                                Next("LParen");
                                Next("RParen");
                                if(_currentToken._type == finalToken) return "Card";
                                else if(_currentToken._type == "Point")
                                {
                                    Next("Point");

                                    if(Enum.IsDefined(typeof(CardProperties), _currentToken._value)) return WichTypeIs("Semicolon");
                                    else throw new Exception("Expected a method that returned some value");
                                }
                                else throw new Exception("Expected semicolon or method call");
                            } 
                            else if(_currentToken._value == "Find") return "List<Card>";
                            else throw new Exception("Expected a method that returned some value");
                        }
                        else throw new Exception("A variable or method was expected");
                   }
                   else throw new Exception("A variable or method was expected");
                }
            }
            //Al concluir, si le sigue "card" creará la carta, de lo contrario lanzará una excepción
            if(_currentToken._value != "card" && _currentToken._type != "Fin") throw new Exception($"To the declaration of effects should be followed by the declaration of at least one card");
            while(_currentToken._value == "card")
            {
                Next("Word");
                Next("LCBracket");
                Properties();
                Next("RCBracket");
                CardCreator cardCreator = GameObject.Find("Botón Confirmar").GetComponent<CardCreator>();
                cardCreator.Create(properties);

                Debug.Log("Carta creada con las siguientes propiedades:");
                foreach(Property property in properties)
                {
                    Debug.Log($"Propiedad: {property.Type}, Valor: {property.ValueS}");
                }
                
                properties.Clear();
            }
            if(_currentToken._type != "Fin") throw new Exception($"Syntax error in: {_currentToken._position}");  
        }
        else throw new Exception($"Syntax error in: {_currentToken._position}");
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
}