using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//Clase encargada de realizar el análisis sintáctico
public class Parser
{
    private readonly List<Token> Tokens; //Lista de tokens
    private int CurrentTokenIndex; //Índice del token actual
    private Token CurrentToken; //Token actual
    private List<Property> properties; //Propiedades que se añadiran a la carta actual
    private Instruction CurrentInstruction =new();
    private List<Effect> Effects = new(); //Lista de todos los efectos declarados
    private List<Effect> CardEffects = new(); //Efectos que se añadirán a la carta actual

    //Constructor de la clase
    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
        CurrentTokenIndex = 0;
        CurrentToken = Tokens[CurrentTokenIndex];
        properties = new();
    }

    //Método principal
    public void Parse()
    {
        //Comprueba si el primer token es correcto
        if(CurrentToken.Type == TokenType.Keyword)
        {
            //De ser así y mientras sea "effect" se crearan los efectos correspondientes
            while(CurrentToken.Value == "effect")
            {
                Next(TokenType.Keyword);
                Next(TokenType.LCBracket);

                //Se recoge el nombre del efecto
                if(CurrentToken.Value == "Name") Next(TokenType.Keyword);
                else throw new Exception("You have not declared the name of the effect correctly");    
                Next(TokenType.Colon);
                Next(TokenType.QMark);
                string nameEffect = CurrentToken.Value;
                Next(TokenType.Word);
                Next(TokenType.QMark);
                Next(TokenType.Comma);

                /*Se recogen los parámetros si es que tiene 
                (la lista de variables es solo para comprobar que esté declarado todo correctamente)*/
                LinkedList<Variable> parameters = new();
                LinkedList<Variable> variables = new();

                if(CurrentToken.Value == "Params")
                {
                    Next(TokenType.Keyword);
                    Next(TokenType.Colon);
                    Next(TokenType.LCBracket);

                    while(true)
                    {
                        string name = CurrentToken.Value;
                        Next(TokenType.Word);
                        Next(TokenType.Colon);
                        string type;
                        
                        if(CurrentToken.Value == "Number" || CurrentToken.Value == "Bool" || CurrentToken.Value == "String") type = CurrentToken.Value;
                        else throw new Exception("This type of parameter does not exist");
                        
                        Next(TokenType.Keyword);
                        
                        Variable param = new(name, type);
                        parameters.AddLast(param);
                        variables.AddLast(param);

                        if(CurrentToken.Type == TokenType.Comma) Next(TokenType.Comma);
                        else break;
                    }
                    Next(TokenType.RCBracket);
                    Next(TokenType.Comma);
                }

                if(parameters.Count != 0)
                {
                    Debug.Log("Parámetros del efecto:");
                    foreach(Variable param in parameters)
                    {
                        Debug.Log($"Parámetro: {param.Name}, Tipo: {param.Type}");
                    }
                }
                else Debug.Log("El efecto no tiene parámetros");

                //Prosigue con la acción del efecto
                if(CurrentToken.Value == "Action") Next(TokenType.Keyword);
                else throw new Exception("You have not declared the action of the effect correctly");
                
                //Se recogen los parámetros "targets" y "context"
                Next(TokenType.Colon);
                Next(TokenType.LParen);
                string targets = CurrentToken.Value;
                Next(TokenType.Word);
                Next(TokenType.Comma);
                string context = CurrentToken.Value;
                Next(TokenType.Word);
                Next(TokenType.RParen);
                Next(TokenType.Equal);
                Next(TokenType.Greater);
                Next(TokenType.LCBracket);

                //Cuerpo del método (acción del efecto en sí)
                //Se procede a recolectar las instrucciones en las siguiente lista
                List<Instruction> instructions = new();

                //Se añaden como variables los parámetros "targets" y "context"

                variables.AddLast(new Variable(targets, "List<Card>"));
                variables.AddLast(new Variable(context, "Context")); 
                
                int count = 0;

                InstructionsRecolector();

                //Se muestran en consola las instrucciones recogidas
                foreach(Instruction instruction in instructions)
                {
                    instruction.Debug();
                }
                
                //Se crea (por fin) el efecto
                if(parameters != null) Effects.Add(new Effect(nameEffect, targets, instructions, variables, parameters));
                else Effects.Add(new Effect(nameEffect, targets, instructions, variables));
                
                Debug.Log("Efecto creado con éxito");

                //--------------------------------------------Métodos-del-Método----------------------------------------------------------
                //Método que recoge las varibles y llamadas a métodos
                void InstructionsRecolector()
                {   
                    //Se añaden instrucciones hasta que el token actual sea una llave de cierre
                    if(CurrentToken.Type != TokenType.RCBracket)
                    {
                        //Si es una declaración de una variable se añade si está correctamente declarada
                        if(Enum.IsDefined(typeof(KeyWords), CurrentToken.Value)) throw new Exception($"You shouldn't use \"{CurrentToken.Value}\" like that");
                        else if(!IsVariable() && CurrentToken.Type == TokenType.Word)
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();

                            string name = CurrentToken.Value;
                            NextAndSave(TokenType.Word);
                            NextAndSave(TokenType.Equal);
                            if(IsVariable()) variables.AddLast(new Variable(name, WichTypeIs(TokenType.Semicolon)));
                            else if(CurrentToken.Type == TokenType.Number)
                            {
                                Variable number = new(name, "Number") {Value = Expr()};
                                variables.AddLast(number);
                            }

                            //Si se llega al final de la instrucción se recoge la siguiente de haberla declarado correctamente
                            if(CurrentToken.Type == TokenType.Semicolon)
                            {
                                Next(TokenType.Semicolon);
                                Debug.Log($"Instrucción {++count} recogida correctamente: \"Variable\"");
                                InstructionsRecolector();
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else if(IsVariable()) //Si es una variable se espera que llame a un método
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();

                            //Se recoge la instrucción de ser correcta
                            if(!CheckMethodCall()) throw new Exception("Bad method call");
                            
                            //Si se llega al final de la instrucción se recoge la siguiente de haberla
                            if(CurrentToken.Type == TokenType.Semicolon)
                            {
                                Next(TokenType.Semicolon);
                                Debug.Log($"Instrucción {++count} recogida correctamente: \"Método\"");
                                InstructionsRecolector();
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else throw new Exception("An instruction was expected");
                    }
                    else
                    {
                        Next(TokenType.RCBracket);
                        Next(TokenType.RCBracket);
                        Debug.Log("Instrucciones recogidas correctamente");
                    }
                }

                //Método para cambiar al siguiente token y guardarlo como parte de la instrucción
                void NextAndSave(TokenType type)
                {
                    CurrentInstruction.Add(CurrentToken.Value);
                    Next(type);
                }

                //Método para comprobar si es una variable
                bool IsVariable()
                {
                    if(Enum.IsDefined(typeof(KeyWords), CurrentToken.Value)) return false;
                    else
                    {
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == CurrentToken.Value) return true;
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
                            if(variable.Name == CurrentToken.Value)
                            {
                                if(variable.Type == "Context")
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == TokenType.Point) Next(TokenType.Point);
                                    else return false;
                                    if(Enum.IsDefined(typeof(ContextMethods), CurrentToken.Value)) return CheckMethodCall();
                                    else return false;
                                }
                                else if(variable.Type == "List<Card>")
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == TokenType.Point) Next(TokenType.Point);
                                    else return false;
                                    if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value)) return CheckMethodCall();
                                    else return false;
                                }
                                else return false;
                            }
                        }
                        return false;
                    }
                    //De lo contrario comprueba que sea un método válido
                    else if(Enum.IsDefined(typeof(ContextMethods), CurrentToken.Value))
                    {
                        if(Enum.IsDefined(typeof(SyntacticSugarContext), CurrentToken.Value))
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.Point);
                            if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value)) return CheckMethodCall();
                            else return false;
                        }
                        else
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);

                            bool correct = false;
            
                            foreach(Variable variable in variables)
                            {
                                if(variable.Name == CurrentToken.Value && WichTypeIs(TokenType.RParen) == "Player") correct = true;
                            }

                            if(correct) Next(TokenType.RParen);
                            else return false;

                            Next(TokenType.Point);
                            
                            if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value)) return CheckMethodCall();
                            else return false;
                        }
                    }
                    else if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value))
                    {
                        if(CurrentToken.Value == "Shuffle")
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);
                            Next(TokenType.RParen);
                            if(CurrentToken.Type == TokenType.Semicolon) return true;
                            else return false;
                        }
                        else
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);

                            if(WichTypeIs(TokenType.RParen) == "Card")
                            {
                                Next(TokenType.RParen);
                                if(CurrentToken.Type == TokenType.Semicolon) return true;
                                else return false;
                            }
                            else return false;  
                        }
                    }
                    else return false;
                }

                //Método que devuelve el tipo de una variable y verifica que esté bien declarada
                string WichTypeIs(TokenType finalToken)
                {
                    //Si es una palabra comprueba si es una variable
                    if(CurrentToken.Type == TokenType.Word)
                    {
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == CurrentToken.Value)
                            {
                                //De ser así verifica el tipo de la variable
                                if(variable.Type == "Context")
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == finalToken) return "Context";
                                    else if(CurrentToken.Type == TokenType.Point)
                                    {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(ContextPropertiesAndMethods), CurrentToken.Value)) return WichTypeIs(TokenType.Semicolon);
                                        else throw new Exception("Uknown method or property of context");
                                    } 
                                }
                                else if(variable.Type == "List<Card>")
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == finalToken) return "List<Card>";
                                    else if(CurrentToken.Type == TokenType.Point)
                                    {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(CardListProperties), CurrentToken.Value)) return WichTypeIs(TokenType.Semicolon);
                                        else throw new Exception("Uknown method or property of context");
                                    } 
                                }
                                else if(variable.Type == "Card")
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == finalToken) return "Card";
                                    else if(CurrentToken.Type == TokenType.Point)
                                    {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(TokenType.Semicolon);
                                        else throw new Exception("Uknown method or property of context");
                                    } 
                                }
                                else if(variable.Type == "Player")
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == finalToken) return "Player";
                                    else throw new Exception("Uknown method or property of context"); 
                                }
                            } 
                        }
                        throw new Exception("Variable expected");
                    }
                    else if(CurrentToken.Type == TokenType.Keyword)
                    {
                        //Comprueba si es un método y devuelve su tipo de serlo
                        if(Enum.IsDefined(typeof(ContextPropertiesAndMethods), CurrentToken.Value))
                        {
                            if(CurrentToken.Value == "TriggerPlayer")
                            {
                                NextAndSave(TokenType.Keyword);
                                return "Player";
                            } 
                            else if(Enum.IsDefined(typeof(SyntacticSugarContext), CurrentToken.Value))
                            {
                                NextAndSave(TokenType.Keyword);
                                if(CurrentToken.Type == finalToken) return "List<Card>";
                                else if(CurrentToken.Type == TokenType.Point)
                                {
                                    Next(TokenType.Point);
                                    return WichTypeIs(TokenType.Semicolon);
                                }
                                else throw new Exception("Expected semicolon or method call");
                            } 
                            else
                            {
                                NextAndSave(TokenType.Keyword);
                                Next(TokenType.LParen);

                                if(WichTypeIs(TokenType.RParen) == "Player") Next(TokenType.RParen);
                                else throw new Exception("A parameter of type player was expected");

                                if(CurrentToken.Type == finalToken) return "List<Card>";
                                else if(CurrentToken.Type == TokenType.Point)
                                {
                                    Next(TokenType.Point);
                                    if(Enum.IsDefined(typeof(CardListProperties), CurrentToken.Value)) return WichTypeIs(TokenType.Semicolon);
                                    else throw new Exception("Uknown method or property of context");
                                }
                                else throw new Exception("Expected semicolon or method call");
                            }
                        }
                        else if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value))
                        {
                            NextAndSave(TokenType.Keyword);
                            return "Player";
                        } 
                        else if(Enum.IsDefined(typeof(CardListProperties), CurrentToken.Value))
                        {
                            if(CurrentToken.Value == "Pop")
                            {
                                NextAndSave(TokenType.Keyword);
                                Next(TokenType.LParen);
                                Next(TokenType.RParen);
                                if(CurrentToken.Type == finalToken) return "Card";
                                else if(CurrentToken.Type == TokenType.Point)
                                {
                                    Next(TokenType.Point);

                                    if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(TokenType.Semicolon);
                                    else throw new Exception("Expected a method that returned some value");
                                }
                                else throw new Exception("Expected semicolon or method call");
                            } 
                            else if(CurrentToken.Value == "Find") return "List<Card>";
                            else throw new Exception("Expected a method that returned some value");
                        }
                        else throw new Exception("A variable or method was expected");
                    }
                   else throw new Exception("A variable or method was expected");
                }
            }
            //Al concluir, si le sigue "card" creará la carta, de lo contrario lanzará una excepción
            if(CurrentToken.Value != "card" && CurrentToken.Type != TokenType.Fin) throw new Exception($"To the declaration of effects should be followed by the declaration of at least one card");
            while(CurrentToken.Value == "card")
            {
                //Se recogen las propiedades
                Next(TokenType.Keyword);
                Next(TokenType.LCBracket);
                Properties(); 
                
                //Si tiene efecto se procede en consecuencia
                if(CurrentToken.Type == TokenType.Comma)
                {
                    Next(TokenType.Comma);
                    if(CurrentToken.Value == "OnActivation") Next(TokenType.Keyword);
                    else throw new Exception("OnActivation keyword expected");
                    Next(TokenType.Colon);
                    Next(TokenType.LSBracket);
                    CardEffect();
                    Next(TokenType.RSBracket);  
                } 
                
                Next(TokenType.RCBracket);

                //Luego, se crea (por fin) la carta
                CardCreator cardCreator = GameObject.Find("Botón Confirmar").GetComponent<CardCreator>();
                if(!IsCardWithEffectCorrect() && CardEffects.Count == 0) cardCreator.Create(properties);
                else if(IsCardWithEffectCorrect()) cardCreator.Create(properties, CardEffects);
                else throw new Exception("This card cannot have effects");

                Debug.Log("Carta creada con las siguientes propiedades:");
                foreach(Property property in properties)
                {
                    Debug.Log($"Propiedad: {property.Type}, Valor: {property.ValueS}");
                }
                
                properties.Clear();
                CardEffects.Clear();
            }
            if(CurrentToken.Type != TokenType.Fin) throw new Exception($"Syntax error in: {CurrentToken.Position}");  
        }
        else throw new Exception($"Syntax error in: {CurrentToken.Position}");
    }

    //Método para cambiar al siguiente token
    private void Next(TokenType tokenType)
    {
        if (CurrentToken.Type == tokenType)
        {
            CurrentTokenIndex++;
            CurrentToken = CurrentTokenIndex < Tokens.Count ? Tokens[CurrentTokenIndex] : new Token(TokenType.Fin, "null", 0);
        }
        else
        {
            throw new SystemException($"Unexpected token: {CurrentToken.Type}, was expected {tokenType}");
        }
    }

    //Método para procesar números
    private int Factor()
    {
        Token token = CurrentToken;

        /*Si el token es correcto pasa al siguiente
         y devuelve el valor del número o expresión entre paréntesis*/
        if(token.Type ==  TokenType.Number)
        {
            int result = int.Parse(token.Value);

            Next(TokenType.Number);

            token = CurrentToken;

            while(token.Type == TokenType.LParen)
            {
                Next(TokenType.LParen);
                result *= Expr();
                Next(TokenType.RParen);

                token = CurrentToken;
            }
            return result;
        }
        else if(token.Type == TokenType.LParen)
        {
            Next(TokenType.LParen);
            int result = Expr();
            Next(TokenType.RParen);

            token = CurrentToken;

            while(token.Type == TokenType.LParen)
            {
                Next(TokenType.LParen);
                result *= Expr();
                Next(TokenType.RParen);

                token = CurrentToken;
            }
            return result;
        }
        throw new ArithmeticException($"Unexpected token: {token.Type}");
    }

    //Método para procesar multiplicaciones y divisiones
    private int Term()
    {
        int result = Factor();

        /*Mientras que el token actual sea una multiplicación o división
          se avanza al siguiente y se realiza la operación en cuestión*/
        while (CurrentToken.Type == TokenType.Multi || CurrentToken.Type == TokenType.Division)
        {
            Token token = CurrentToken;
            if (token.Type == TokenType.Multi)
            {
                Next(TokenType.Multi);
                result *= Factor();
            }
            else if (token.Type == TokenType.Division)
            {
                Next(TokenType.Division);
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
        while (CurrentToken.Type == TokenType.Plus || CurrentToken.Type == TokenType.Minus)
        {
            Token token = CurrentToken;
            if (token.Type == TokenType.Plus)
            {
                Next(TokenType.Plus);
                result += Term();
            }
            else if (token.Type == TokenType.Minus)
            {
                Next(TokenType.Minus);
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
        while(CurrentToken.Value == "Type" || CurrentToken.Value == "Name" || CurrentToken.Value == "Faction"
                                             || CurrentToken.Value == "Power" || CurrentToken.Value == "Range")
        {
            Token token = CurrentToken;

            Next(TokenType.Keyword);
            Next(TokenType.Colon);
            if(token.Value == "Power") ToProperty(token.Value);
            else if(token.Value == "Range")
            {
                Next(TokenType.LSBracket);
                Next(TokenType.QMark);
                ToProperty(token.Value, CurrentToken.Value);

                while(CurrentToken.Type == TokenType.Comma)
                {
                    Next(TokenType.Comma);
                    Next(TokenType.QMark);
                    ToProperty(token.Value, CurrentToken.Value);
                }

                Next(TokenType.RSBracket);
            }
            else
            {   
                Next(TokenType.QMark);
                if(token.Value == "Type" && CurrentToken.Value != "Gold" && CurrentToken.Value != "Silver")
                {
                    isSpecialCard = true;
                } 
                ToProperty(token.Value, CurrentToken.Value);
            } 

            count++;

            if((count < 3 && isSpecialCard) || (count < 5 && !isSpecialCard)) Next(TokenType.Comma);
        }

        //Luego de recoger todas las propiedades declaradas se verifica si son correctas
        CheckProperties();
    }

    //Método para añadir la propiedad de la carta a una lista con todas las propiedades de la misma
    private void ToProperty(string type, string value)
    {
        Next(TokenType.Word);
        Next(TokenType.QMark);

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
            else if(property.Type == "Power")
            {
                if(property.ValueI < 0) throw new Exception("Power cannot be negative");
                else hasPower = true;
            } 
            else if(property.Type == "Range") hasRange = true;
        }

        //Si le faltan o tiene de más lanza una excepción
        if((isSpecialCard && (!hasType || !hasFaction || !hasName || hasPower || hasRange))
        || (!isSpecialCard && (!hasType || !hasFaction || !hasName || !hasPower || !hasRange)))
        throw new Exception("Misstatement of card properties");
    }

    //Método que recoge los efectos que tiene la carta
    private void CardEffect()
    {
        Next(TokenType.LCBracket);
        if(CurrentToken.Value == "Effect") Next(TokenType.Keyword);
        else throw new Exception("Effect keyword expected");
        Next(TokenType.Colon);

        Effect effect;

        //Se verifica si se usó la azúcar sintáctica y si el efecto existe
        bool isSyntacticSugarOn = false;

        if(CurrentToken.Type == TokenType.QMark)
        {
            Next(TokenType.QMark);
            effect = FindEffect();
            Next(TokenType.Word);
            Next(TokenType.QMark);
            isSyntacticSugarOn = true;
        }
        else if(CurrentToken.Type == TokenType.LCBracket)
        {
            Next(TokenType.LCBracket);
            if(CurrentToken.Value == "Name") Next(TokenType.Keyword);
            else throw new Exception("Effect name expected");
            Next(TokenType.Colon);

            Next(TokenType.QMark);
            effect = FindEffect();
            Next(TokenType.Word);
            Next(TokenType.QMark);
        }
        else throw new Exception("Effect name expected");

        //De ser así se comprueba si tiene parámetros y de tenerlos se le otorgan los valores
        if(effect.Params.Count != 0)
        {
            if(isSyntacticSugarOn) Next(TokenType.LCBracket);
            else Next(TokenType.Comma);

            int count = 0;

            SetParams(effect, ref count);

            if(count != effect.Params.Count) throw new Exception("There are missing parameters to assign");

            Next(TokenType.RCBracket);
        }
        else if(!isSyntacticSugarOn) Next(TokenType.RCBracket);

        //Luego comprueba si sigue un Selector
        if(CurrentToken.Type == TokenType.Comma)
        {
            Next(TokenType.Comma);

            if(CurrentToken.Value == "Selector")
            {
                Next(TokenType.Keyword);
                Selector(null, effect);
            } 
            //De no tener un Selector se le otorga void al Source para que en ejecución los targets sean una lista vacía
            else effect.SourceTargets = "void"; 
        }
        
        //Comprueba si hay PostAction y procede en consecuencia
        if(CurrentToken.Type == TokenType.Comma)
        {
            Next(TokenType.Comma);
            PostAction(effect);
        }
        else if(CurrentToken.Value == KeyWords.PostAction.ToString() && effect.SourceTargets == "void") PostAction(effect);
        else effect.SourceTargets ??= "void";

        Next(TokenType.RCBracket);

        Debug.Log($"Parámetros otorgados correctamente al efecto: \"{effect.Name}\"");

        //Se guarda el efecto
        CardEffects.Add(effect);

        //De haber otro efecto, se recoge también
        if(CurrentToken.Type == TokenType.Comma)
        {
            Next(TokenType.Comma);
            CardEffect();
        } 
    }

    //Método para buscar el efecto y devolverlo
    private Effect FindEffect()
    {
        foreach(Effect effect in Effects)
        {
            if(effect.Name == CurrentToken.Value)
            {
                return (Effect)effect.Clone();
            }
        }

        throw new Exception("That effect does not exist");
    }

    private void SetParams(Effect effect, ref int count)
    {
        bool isCorrectParam = false;

        foreach(Variable param in effect.Params)
        {
            if(param.Name == CurrentToken.Value)
            {
                Next(TokenType.Word);
                Next(TokenType.Colon);
                if(param.Type == "Number") param.Value = Expr();
                else if(param.Type == "Bool") throw new NotImplementedException();
                else if(param.Type == "String")
                {
                    Next(TokenType.QMark);
                    param.Value = CurrentToken;
                    Next(TokenType.Word);
                    Next(TokenType.QMark);
                }
                else throw new Exception("Invalid parameter type");

                count++;

                if(CurrentToken.Type == TokenType.Comma)
                {
                    Next(TokenType.Comma);
                    SetParams(effect, ref count);
                }

                isCorrectParam = true;
                break;
            }
        }

        if(!isCorrectParam) throw new Exception("This effect does not contain this parameter");
    }

    //Método para el Selector
    public void Selector(Effect effectParent, Effect effect)
    {
        Next(TokenType.Colon);
        Next(TokenType.LCBracket);

        //Source
        if(CurrentToken.Value == "Source") Next(TokenType.Keyword);
        else throw new Exception("Source expected");

        Next(TokenType.Colon);
        Next(TokenType.QMark);

        if(Enum.IsDefined(typeof(Source), CurrentToken.Value) || CurrentToken.Value != "parent") effect.SourceTargets = CurrentToken.Value;
        else if(CurrentToken.Value == "parent" && effectParent != null) effect.SourceTargets= effectParent.SourceTargets;
        else throw new Exception("Invalid card filter");

        Next(TokenType.Word);
        Next(TokenType.QMark);

        Next(TokenType.Comma);
        
        //Single
        if(CurrentToken.Value == "Single") Next(TokenType.Keyword);
        else throw new Exception("Single expected");

        Next(TokenType.Colon);

        if(CurrentToken.Value == "false")
        {
            effect.Single = false;
            Next(TokenType.Word);
        }
        else if(CurrentToken.Value == "true")
        {
            effect.Single = true;
            Next(TokenType.Word);
        }
        else throw new Exception("Expected true or false");

        Next(TokenType.Comma);

        //Predicate
        if(CurrentToken.Value == "Predicate") Next(TokenType.Keyword);
        else throw new Exception("Predicate expected");

        Next(TokenType.Colon);
        Next(TokenType.Word); //No está implementado de momento
        Next(TokenType.RCBracket);
    }

    //Método para el PostAction
    private void PostAction(Effect effectParent)
    {
        if(CurrentToken.Value == KeyWords.PostAction.ToString()) Next(TokenType.Keyword);
        else throw new Exception("PostAction expected");

        Next(TokenType.Colon);
        Next(TokenType.LCBracket);

        if(CurrentToken.Value == KeyWords.Type.ToString()) Next(TokenType.Keyword);
        else throw new Exception("Type expected");

        Next(TokenType.Colon);

        //Se busca si el efecto existe
        Next(TokenType.QMark);
        Effect effect = FindEffect();
        Next(TokenType.Word);
        Next(TokenType.QMark);

        //De ser así se comprueba si tiene parámetros y de tenerlos se le otorgan los valores
        if(effect.Params.Count != 0)
        {
            Next(TokenType.Comma);

            int count = 0;

            SetParams(effect, ref count);

            if(count != effect.Params.Count) throw new Exception("There are missing parameters to assign");
        }

        //Comprueba si sigue un Selector u otro PostAction
        if(CurrentToken.Type == TokenType.Comma)
        {
            Next(TokenType.Comma);
            if(CurrentToken.Value == KeyWords.Selector.ToString())
            {
                Next(TokenType.Keyword);
                Selector(effectParent, effect);

                if(CurrentToken.Type == TokenType.Comma)
                {
                    Next(TokenType.Comma);
                    PostAction(effect);
                }
            }
            else if(CurrentToken.Value == KeyWords.PostAction.ToString())
            {
                PostAction(effect);
            }
            else throw new Exception("Selector or PostAction was expected");
        }

        Next(TokenType.RCBracket);

        //Se le otorga como efecto posterior al efecto padre
        effectParent.PostEffect = effect;

        Debug.Log("Efecto posterior correcto");
    }

    private bool IsCardWithEffectCorrect()
    {
        foreach(Property property in properties)
        if(property.ValueS == "Silver" || property.ValueS == "Gold" || property.ValueS == "Leader") return true;
                    
        return false;
    }               
}