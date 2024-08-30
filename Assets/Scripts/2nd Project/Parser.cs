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
    private Instruction CurrentInstruction = new(); //Instrucción actual
    private List<Effect> Effects = new(); //Lista de todos los efectos declarados
    private List<Effect> CardEffects = new(); //Efectos que se añadirán a la carta actual
    private LinkedList<Variable> CurrentVariables = new(); //Lista de variables disponibles para usarse en los métodos de afuera del Parser()

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
        if(CurrentToken.Type == TokenType.Keyword || CurrentToken.Value == "card")
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
                dNext = Next;

                string nameEffect = ParseString();

                Next(TokenType.Comma);

                /*Se recogen los parámetros si es que tiene 
                (la lista de variables es solo para comprobar que esté declarado todo correctamente)*/
                LinkedList<Variable> parameters = new();
                LinkedList<Variable> variables = new();
                LinkedList<Variable> variables2 = new(); //Para comprobar semánticamente los ciclos

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
                        object value;
                        
                        //Se recoge el valor del parámetro junto con su tipo
                        if(CurrentToken.Value == "Number")
                        {
                            type = CurrentToken.Value;
                            value = 0;
                        } 
                        else if(CurrentToken.Value == "Bool")
                        {
                            type = CurrentToken.Value;
                            value = false;
                        } 
                        else if(CurrentToken.Value == "String")
                        {
                            type = CurrentToken.Value;
                            value = "";
                        } 
                        else throw new Exception("This type of parameter does not exist");
                        
                        Next(TokenType.Keyword);
                        
                        //Se guarda el parámetro
                        Variable param = new(name, type){Value = value};
                        parameters.AddLast(param);
                        variables.AddLast(param);
                        variables2.AddLast(param);

                        if(CurrentToken.Type == TokenType.Comma) Next(TokenType.Comma);
                        else break;
                    }
                    Next(TokenType.RCBracket);
                    Next(TokenType.Comma);
                }

                //Necesario al debuguear los parámetros
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
                Next(TokenType.Asign);
                Next(TokenType.Greater);
                Next(TokenType.LCBracket);

                //Cuerpo del método (acción del efecto en sí)
                //Se procede a recolectar las instrucciones en las siguiente lista
                List<Instruction> instructions = new();

                //Se añaden como variables los parámetros "targets" y "context"

                variables.AddLast(new Variable(targets, "List<Card>"));
                variables.AddLast(new Variable(context, "Context")); 

                variables2.AddLast(new Variable(targets, "List<Card>"));
                variables2.AddLast(new Variable(context, "Context")); 
                
                int count = 0; //Para saber el número de la instrucción al debuguear

                ActionRecolector();

                //Se muestran en consola las instrucciones recogidas
                foreach(Instruction instruction in instructions)
                {
                    instruction.Debug();
                }
                
                //Se crea (por fin) el efecto
                if(parameters != null) Effects.Add(new Effect(nameEffect, targets, instructions, variables2, parameters));
                else Effects.Add(new Effect(nameEffect, targets, instructions, variables2));

                CurrentVariables.Clear(); // Se vacía para ser utilizada en futuras ocasiones
                
                Debug.Log("Efecto creado con éxito");

                //--------------------------------------------Métodos-del-Método----------------------------------------------------------
                
                //Método que recoge las varibles y llamadas a métodos
                void ActionRecolector()
                {
                    InstructionsCollector(false);

                    Next(TokenType.RCBracket);
                    Next(TokenType.RCBracket);

                    Debug.Log("Action recolectado correctamente");
                }
                
                //Método complementario del anterior
                void InstructionsCollector(bool OneInstruction)
                {   
                    //Se añaden instrucciones hasta que el token actual sea una llave de cierre
                    if(CurrentToken.Type != TokenType.RCBracket)
                    {
                        if(Enum.IsDefined(typeof(KeyWords), CurrentToken.Value)) throw new Exception($"You shouldn't use \"{CurrentToken.Value}\" like that");
                        else if(CurrentToken.Value == "for") // En caso de ser un bucle for
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();

                            //Se guardan las variables declaradas fuera del "for"
                            List<Variable> variablesFor = new();

                            foreach(Variable variable in variables)
                            {
                                variablesFor.Add(variable);
                            }

                            NextAndSave(TokenType.Word);

                            //Se guarda la variable nueva
                            variables.AddLast(new Variable(CurrentToken.Value, "Card"));
                            variables2.AddLast(new Variable(CurrentToken.Value, "Card"));
                            NextAndSave(TokenType.Word);

                            if(CurrentToken.Value != "in") throw new Exception("\"in\" keyword expected");
                            Next(TokenType.Word);

                            //Se guarda la lista de cartas a iterar
                            if(!IsCardList()) throw new Exception("There can only be lists of cards");
                            NextAndSave(TokenType.Word);

                            bool isOneInstruction; // Necesario para saber no hay llaves y el bucle solo tendrá una única instrucción

                            if(CurrentToken.Type == TokenType.LCBracket)
                            {
                                Next(TokenType.LCBracket);
                                isOneInstruction = false;
                            } 
                            else isOneInstruction = true;
                            
                            //Se recogen las instrucciones dentro del for
                            InstructionsCollector(isOneInstruction);

                            //Se eliminan las variables declaradas dentro del "for" para que no se puedan usar desde fuera
                            variables.Clear();

                            foreach(Variable variable in variablesFor)
                            {
                                variables.AddLast(variable);
                            }

                            if(!isOneInstruction) 
                            {
                                Next(TokenType.RCBracket);
                                Next(TokenType.Semicolon);
                            }

                            //Se indica el final del "for"
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();
                            CurrentInstruction.Add("ForFinal");

                            Debug.Log($"Instrucción {++count} recogida correctamente: \"for\"");
                            if(!OneInstruction) InstructionsCollector(false);
                        }
                        else if(CurrentToken.Value == "while") // En caso de ser un bucle while
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();

                            //Se guardan las variables declaradas fuera del "while"
                            List<Variable> variablesWhile = new();

                            foreach(Variable variable in variables)
                            {
                                variablesWhile.Add(variable);
                            }

                            NextAndSave(TokenType.Word);

                            // Se verifica la expresión booleana
                            Next(TokenType.LParen);

                            dNext = NextAndSave;
                            ParseBooleanExpression();

                            Next(TokenType.RParen);

                            bool isOneInstruction; // Necesario para saber no hay llaves y el bucle solo tendrá una única instrucción

                            if(CurrentToken.Type == TokenType.LCBracket)
                            {
                                Next(TokenType.LCBracket);
                                isOneInstruction = false;
                            } 
                            else isOneInstruction = true;

                            //Se recogen las instrucciones dentro del while
                            InstructionsCollector(isOneInstruction);

                            if(!isOneInstruction)
                            {
                                Next(TokenType.RCBracket);
                                Next(TokenType.Semicolon);
                            }

                            //Se eliminan las variables declaradas dentro del "while" para que no se puedan usar desde fuera
                            variables.Clear();

                            foreach(Variable variable in variablesWhile)
                            {
                                variables.AddLast(variable);
                            }

                            //Se indica el final del "while"
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();
                            CurrentInstruction.Add("WhileFinal");

                            Debug.Log($"Instrucción {++count} recogida correctamente: \"while\"");
                            if(!OneInstruction) InstructionsCollector(false);
                        }
                        else if(IsNumericVariable() || IsNumericProperty()) //Incremento u operación compuesta
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();

                            //Comprueba si es una propiedad
                            if(IsNumericProperty())
                            {
                                NextAndSave(TokenType.Word);
                                Next(TokenType.Point);
                                NextAndSave(TokenType.Keyword);
                            }
                            else if(CurrentToken.Type == TokenType.Word) NextAndSave(TokenType.Word);

                            //Procede en dependencia de la operación compuesta
                            if(CurrentToken.Type == TokenType.PlusCom)
                            {
                                NextAndSave(TokenType.PlusCom);
                                dNext = NextAndSave;
                                Expr();
                            } 
                            else if(CurrentToken.Type == TokenType.MinusCom)
                            {
                                NextAndSave(TokenType.MinusCom);
                                dNext = NextAndSave;
                                Expr();
                            } 
                            else if(CurrentToken.Type == TokenType.MultiCom)
                            {
                                NextAndSave(TokenType.MultiCom);
                                dNext = NextAndSave;
                                Expr();
                            } 
                            else if(CurrentToken.Type == TokenType.DivisionCom)
                            {
                                NextAndSave(TokenType.DivisionCom);
                                dNext = NextAndSave;
                                Expr();
                            } 
                            else if(CurrentToken.Type == TokenType.XORCom)
                            {
                                NextAndSave(TokenType.XORCom);
                                dNext = NextAndSave;
                                Expr();
                            } 
                            else if(CurrentToken.Type == TokenType.Increase) NextAndSave(TokenType.Increase);

                            //Si se llega al final de la instrucción se recoge la siguiente de haberla
                            if(CurrentToken.Type == TokenType.Semicolon)
                            {
                                Next(TokenType.Semicolon);
                                Debug.Log($"Instrucción {++count} recogida correctamente: \"Incremento u Operación compuesta\"");
                                if(!OneInstruction) InstructionsCollector(false);
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else if(CurrentToken.Type == TokenType.Increase) // En caso de ser un incremento
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();

                            NextAndSave(TokenType.Increase);

                            //Comprueba si es una propiedad
                            if(IsNumericProperty())
                            {
                                NextAndSave(TokenType.Word);
                                Next(TokenType.Point);
                                NextAndSave(TokenType.Keyword);
                            }
                            else if(CurrentToken.Type == TokenType.Word) NextAndSave(TokenType.Word);

                            //Si se llega al final de la instrucción se recoge la siguiente de haberla
                            if(CurrentToken.Type == TokenType.Semicolon)
                            {
                                Next(TokenType.Semicolon);
                                Debug.Log($"Instrucción {++count} recogida correctamente: \"Incremento\"");
                                if(!OneInstruction) InstructionsCollector(false);
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else if(!IsVariable() && CurrentToken.Type == TokenType.Word) //Declaración de variable
                        {
                            //Se añade una nueva instrucción y se cambia la referencia de la instrucción actual
                            instructions.Add(new Instruction());
                            CurrentInstruction = instructions.Last();

                            //Se guarda el nombre de la nueva variable
                            string name = CurrentToken.Value;
                            NextAndSave(TokenType.Word);
                            NextAndSave(TokenType.Asign);

                            // Procede en dependencia del tipo de variable
                            if(IsVariable()) // Si es contexto, carta, lista...
                            {
                                CurrentVariables = variables;
                                
                                Variable variable = new(name, WichTypeIs(TokenType.Semicolon));
                                
                                variables.AddLast(variable);
                                variables2.AddLast(variable);
                            } 
                            else if(IsBoolean()) // Si es booleana
                            {
                                CurrentVariables = variables;

                                dNext = NextAndSave;
                                Variable boolean = new(name, "Bool") {Value = ParseBooleanExpression()};

                                variables.AddLast(boolean);
                                variables2.AddLast(boolean);
                            }
                            else if(CurrentToken.Type == TokenType.QMark || IsString()) // Si es string
                            {
                                CurrentVariables = variables;

                                dNext = NextAndSave;
                                Variable variable = new(name, "String") {Value = ParseString()};

                                variables.AddLast(variable);
                                variables2.AddLast(variable);
                            }
                            else // Si es un numérica
                            {
                                CurrentVariables = variables;

                                dNext = NextAndSave;
                                Variable number = new(name, "Number") {Value = Expr()};
                                
                                variables.AddLast(number);
                                variables2.AddLast(number);
                            }

                            //Si se llega al final de la instrucción se recoge la siguiente de haberla declarado correctamente
                            if(CurrentToken.Type == TokenType.Semicolon)
                            {
                                Next(TokenType.Semicolon);
                                Debug.Log($"Instrucción {++count} recogida correctamente: \"Variable\"");
                                if(!OneInstruction) InstructionsCollector(false);
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else if(IsVariable()) //Si es una variable que ya ha sido declarada entonces se espera que se llame a un método
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
                                if(!OneInstruction) InstructionsCollector(false);
                            }
                            else throw new Exception("Semicolon was expected");
                        }
                        else throw new Exception("An instruction was expected");
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
                            if(variable.Name == CurrentToken.Value && ((variable.Type != "Number" 
                            && variable.Type != "Bool" && variable.Type != "String" && variable.Type != "Card")
                            || (variable.Type == "Card" && Tokens[CurrentTokenIndex + 2].Value == "Owner"))) return true;
                        }
                        return false;
                    }
                }

                // Para comprobar si es variable numérica
                bool IsNumericVariable()
                {
                    if(Enum.IsDefined(typeof(KeyWords), CurrentToken.Value)) return false;
                    else
                    {
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == CurrentToken.Value && variable.Type == "Number") return true;
                        }
                        return false;
                    }
                }

                // Si es una propiedad numérica de una carta
                bool IsNumericProperty()
                {
                    if(Enum.IsDefined(typeof(KeyWords), CurrentToken.Value)) return false;
                    else
                    {
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == CurrentToken.Value && variable.Type == "Card" 
                               && Tokens[CurrentTokenIndex + 2].Value == "Power") return true;
                        }
                        return false;
                    }
                }

                // Si es un string
                bool IsString()
                {
                    if(Enum.IsDefined(typeof(KeyWords), CurrentToken.Value)) return false;
                    else
                    {
                        foreach(Variable variable in variables)
                        {
                            if(variable.Name == CurrentToken.Value && variable.Type == "String") return true;
                            else if(variable.Name == CurrentToken.Value && variable.Type == "Card" 
                                    && (Tokens[CurrentTokenIndex + 2].Value == "Name" || Tokens[CurrentTokenIndex + 2].Value == "Type"
                                    || Tokens[CurrentTokenIndex + 2].Value == "Range" || Tokens[CurrentTokenIndex + 2].Value == "Faction"))
                                    return true;
                        }
                        return false;
                    }
                }

                //Método para comprobar si es una variable de tipo "List<Card>"
                bool IsCardList()
                {
                    foreach(Variable variable in variables)
                    {
                        if(variable.Name == CurrentToken.Value && variable.Type == "List<Card>") return true;
                    }
                    return false;
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
                                if(variable.Type == "Context") // En caso de ser de tipo "contexto"
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == TokenType.Point) Next(TokenType.Point);
                                    else return false;
                                    if(Enum.IsDefined(typeof(ContextMethods), CurrentToken.Value)) return CheckMethodCall();
                                    else return false;
                                }
                                else if(variable.Type == "List<Card>") // En caso de ser de tipo "List<Card>"
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
                        // Si es la forma compacta de las propiedades del "contexto"
                        if(Enum.IsDefined(typeof(SyntacticSugarContext), CurrentToken.Value)) 
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.Point);
                            if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value)) return CheckMethodCall();
                            else return false;
                        }
                        else // Si es la forma completa
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);

                            bool correct = false;
         
                            //Se comprueba que el parámetro pasado sea correcto
                            foreach(Variable variable in variables)
                            {
                                if(variable.Name == CurrentToken.Value && WichTypeIs(TokenType.RParen) == "Player") correct = true;
                            }

                            if(correct) Next(TokenType.RParen);
                            else return false;

                            Next(TokenType.Point);
                            
                            //Se espera un método de la clase "List<Card>"
                            if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value)) return CheckMethodCall();
                            else return false;
                        }
                    }
                    else if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value)) // Si es de la clase "List<Card>"
                    {
                        // Se procede en dependencia del método
                        if(CurrentToken.Value == "Shuffle")
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);
                            Next(TokenType.RParen);
                            if(CurrentToken.Type == TokenType.Semicolon) return true;
                            else return false;
                        }
                        else if(CurrentToken.Value == "Add" || CurrentToken.Value == "Push" || CurrentToken.Value == "SendBottom" || CurrentToken.Value == "Remove")
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
                        else if(CurrentToken.Value == "Find")
                        {
                            NextAndSave(TokenType.Keyword);
                            Next(TokenType.LParen);
                            dNext = NextAndSave;
                            Predicate(variables2);
                            Next(TokenType.RParen);
                            
                            Next(TokenType.Point);

                            if(Enum.IsDefined(typeof(CardListMethods), CurrentToken.Value)) return CheckMethodCall();
                            else return false;
                        }
                        throw new Exception("This method is not valid");
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
                                        
                                        if(Enum.IsDefined(typeof(ContextPropertiesAndMethods), CurrentToken.Value)) return WichTypeIs(finalToken);
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
                                        if(Enum.IsDefined(typeof(CardListProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                        else throw new Exception("Uknown method or property of context");
                                    }
                                    else if(CurrentToken.Type == TokenType.LSBracket)
                                    {
                                        NextAndSave(TokenType.LSBracket);
                                        dNext = NextAndSave;
                                        Expr();
                                        NextAndSave(TokenType.RSBracket);
                                        if(CurrentToken.Type == finalToken) return "Card";
                                        else if(CurrentToken.Type == TokenType.Point)
                                        {
                                            Next(TokenType.Point);
                                            if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                            else throw new Exception("Uknown method or property of context");
                                        }
                                        else throw new Exception("Error to declarate a variable");
                                    }
                                }
                                else if(variable.Type == "Card")
                                {
                                    NextAndSave(TokenType.Word);
                                    if(CurrentToken.Type == finalToken) return "Card";
                                    else if(CurrentToken.Type == TokenType.Point)
                                    {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
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
                                    return WichTypeIs(finalToken);
                                }
                                else if(CurrentToken.Type == TokenType.LSBracket)
                                {
                                    NextAndSave(TokenType.LSBracket);
                                    dNext = NextAndSave;
                                    Expr();
                                    NextAndSave(TokenType.RSBracket);
                                    if(CurrentToken.Type == finalToken) return "Card";
                                    else if(CurrentToken.Type == TokenType.Point)
                                    {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                        else throw new Exception("Uknown method or property of context");
                                    }
                                    else throw new Exception("Error to declarate a variable");
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
                                    if(Enum.IsDefined(typeof(CardListProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                    else throw new Exception("Uknown method or property of context");
                                }
                                else if(CurrentToken.Type == TokenType.LSBracket)
                                {
                                    NextAndSave(TokenType.LSBracket);
                                    dNext = NextAndSave;
                                    Expr();
                                    NextAndSave(TokenType.RSBracket);
                                    if(CurrentToken.Type == finalToken) return "Card";
                                    else if(CurrentToken.Type == TokenType.Point)
                                    {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                        else throw new Exception("Uknown method or property of context");
                                    }
                                    else throw new Exception("Error to declarate a variable");
                                    }
                                else throw new Exception("Expected semicolon or method call");
                            }
                        }
                        else if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value))
                        {
                            Token token = CurrentToken;
                            NextAndSave(TokenType.Keyword);
                            if(token.Value == "Owner") return "Player";
                            else throw new Exception("Incorrect property of card");
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

                                    if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                    else throw new Exception("Expected a method that returned some value");
                                }
                                else throw new Exception("Expected semicolon or method call");
                            } 
                            else if(CurrentToken.Value == "Find")
                            {
                                NextAndSave(TokenType.Keyword);
                                Next(TokenType.LParen);
                                dNext = NextAndSave;
                                Predicate(variables2);
                                Next(TokenType.RParen);
                                if(CurrentToken.Type == finalToken) return "List<Card>";
                                else if(CurrentToken.Type == TokenType.Point)
                                {
                                    Next(TokenType.Point);

                                    if(Enum.IsDefined(typeof(CardListProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                    else throw new Exception("Expected a method that returned some value");
                                }
                                else if(CurrentToken.Type == TokenType.LSBracket)
                                {
                                    NextAndSave(TokenType.LSBracket);
                                    dNext = NextAndSave;
                                    Expr();
                                    NextAndSave(TokenType.RSBracket);
                                    if(CurrentToken.Type == finalToken) return "Card";
                                    else if(CurrentToken.Type == TokenType.Point)
                                    {
                                        Next(TokenType.Point);
                                        if(Enum.IsDefined(typeof(CardProperties), CurrentToken.Value)) return WichTypeIs(finalToken);
                                        else throw new Exception("Uknown method or property of context");
                                    }
                                    else throw new Exception("Error to declarate a variable");
                                }
                                else throw new Exception("Expected semicolon or method call");
                            } 
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
                Next(TokenType.Word);
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

                // Necesario para debuguear la carta y sus propiedades
                Debug.Log("Carta creada con las siguientes propiedades:");
                foreach(Property property in properties)
                {
                    Debug.Log($"Propiedad: {property.Type}, Valor: {property.Value}");
                }
                
                // Se limpian las variables para usarse nuevamente
                properties.Clear();
                CardEffects.Clear();
            }
            if(CurrentToken.Type != TokenType.Fin) throw new Exception($"Syntax error in: {CurrentToken.Position}");  
        }
        else throw new Exception($"Effect or card declaration was expected");
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

    //---------------------------------------------Números------------------------------------------------------------------------------
    //Método para procesar números
    private int Factor()
    {
        Token token = CurrentToken;

        /*Si el token es correcto pasa al siguiente
         y devuelve el valor del número o expresión entre paréntesis*/
        if(token.Type ==  TokenType.Number)
        {
            int result = int.Parse(token.Value);

            dNext(TokenType.Number);

            if(CurrentToken.Type == TokenType.XOR)
            {
                dNext(TokenType.XOR);

                result ^= Factor(); 
            }

            token = CurrentToken;

            while(token.Type == TokenType.LParen)
            {
                dNext(TokenType.LParen);
                result *= Expr();
                dNext(TokenType.RParen);

                token = CurrentToken;
            }
            return result;
        }
        else if(token.Type == TokenType.LParen)
        {
            dNext(TokenType.LParen);
            int result = Expr();
            dNext(TokenType.RParen);
            
            if(CurrentToken.Type == TokenType.XOR)
            {
                dNext(TokenType.XOR);

                result ^= Factor(); 
            }

            token = CurrentToken;

            while(token.Type == TokenType.LParen)
            {
                dNext(TokenType.LParen);
                result *= Expr();
                dNext(TokenType.RParen);

                token = CurrentToken;
            }
            return result;
        }
        else if(CurrentToken.Type == TokenType.Word) // En caso de ser una variable que alamcena un valor numérico
        {
            foreach(Variable variable in CurrentVariables)
            {
                if(variable.Name == CurrentToken.Value && variable.Value is int value)
                {
                    int result = value;

                    dNext(TokenType.Word);

                    if(CurrentToken.Type == TokenType.Increase) dNext(TokenType.Increase);

                    if(CurrentToken.Type == TokenType.XOR)
                    {
                        dNext(TokenType.XOR);

                        result ^= Factor(); 
                    }

                    token = CurrentToken;

                    while(token.Type == TokenType.LParen)
                    {
                        dNext(TokenType.LParen);
                        result *= Expr();
                        dNext(TokenType.RParen);

                        token = CurrentToken;
                    }
                    return result;
                }
                // En caso de ser una propiedad numérica
                else if(variable.Name == CurrentToken.Value && variable.Type == "Card" && Tokens[CurrentTokenIndex + 2].Value == "Power")
                {
                    dNext(TokenType.Word);
                    Next(TokenType.Point);
                    dNext(TokenType.Keyword);

                    int result = 0;

                    if(CurrentToken.Type == TokenType.Increase) throw new Exception("Properties cannot be increased");

                    if(CurrentToken.Type == TokenType.XOR)
                    {
                        dNext(TokenType.XOR);

                        result ^= Factor(); 
                    }

                    token = CurrentToken;

                    while(token.Type == TokenType.LParen)
                    {
                        dNext(TokenType.LParen);
                        result *= Expr();
                        dNext(TokenType.RParen);

                        token = CurrentToken;
                    }

                    return result;
                }  
            }
            throw new Exception("This word is not a variable with a \"int value\"");
        }
        else if(CurrentToken.Type == TokenType.Increase) // En caso de ser un incremento
        {
            dNext(TokenType.Increase);

            foreach(Variable variable in CurrentVariables)
            {
                if(variable.Name == CurrentToken.Value && variable.Value is int value)
                {
                    int result = value;

                    dNext(TokenType.Word);
                    
                    if(CurrentToken.Type == TokenType.XOR)
                    {
                        dNext(TokenType.XOR);

                        result ^= Factor(); 
                    }

                    token = CurrentToken;

                    while(token.Type == TokenType.LParen)
                    {
                        dNext(TokenType.LParen);
                        result *= Expr();
                        dNext(TokenType.RParen);

                        token = CurrentToken;
                    }
                    return result;
                }
                else if(variable.Name == CurrentToken.Value && variable.Type == "Card" && Tokens[CurrentTokenIndex + 2].Value == "Power")
                throw new Exception("Properties cannot be increased");
            }
            throw new Exception("This word is not a variable with a \"int value\"");
        }
        else throw new Exception($"Unexpected token: {token.Type}");
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
                dNext(TokenType.Multi);
                result *= Factor();
            }
            else if (token.Type == TokenType.Division)
            {
                dNext(TokenType.Division);
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
                dNext(TokenType.Plus);
                result += Term();
            }
            else if (token.Type == TokenType.Minus)
            {
                dNext(TokenType.Minus);
                result -= Term();
            }
        }
        return result;
    }

    //-----------------------------------------------------Propiedades-de-las-cartas------------------------------------------------------
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
            dNext = Next;

            // Se van añadiendo las propiedades
            if(token.Value == "Power") ToProperty(token.Value);
            else if(token.Value == "Range")
            {
                Next(TokenType.LSBracket);
                
                ToProperty(token.Value, ParseString());

                while(CurrentToken.Type == TokenType.Comma)
                {
                    Next(TokenType.Comma);
                   
                    ToProperty(token.Value, ParseString());
                }

                Next(TokenType.RSBracket);
            }
            else
            {   
                string type = ParseString();

                if(token.Value == "Type" && type != "Gold" && type != "Silver")
                {
                    isSpecialCard = true;
                } 
                ToProperty(token.Value, type);
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
        Property property = new(type, value);

        properties.Add(property);
    }

    //Sobrecarga del método anterior en el caso de que la propiedad sea el "Power"
    private void ToProperty(string type)
    {
        dNext = Next;
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
                if((string)property.Value == "Clearance" || (string)property.Value == "Climate" || (string)property.Value == "Leader"
                                                  || (string)property.Value == "Lure" || (string)property.Value == "Increase") isSpecialCard = true;
                else if((string)property.Value != "Gold" && (string)property.Value != "Silver") throw new Exception("This type of card does not exist");
            
                hasType = true;
            }
            else if(property.Type == "Name") hasName = true;
            else if(property.Type == "Faction") hasFaction = true;
            else if(property.Type == "Power")
            {
                if((int)property.Value < 0) throw new Exception("Power cannot be negative");
                else hasPower = true;
            } 
            else if(property.Type == "Range") hasRange = true;
        }

        //Si le faltan o tiene de más lanza una excepción
        if((isSpecialCard && (!hasType || !hasFaction || !hasName || hasPower || hasRange))
        || (!isSpecialCard && (!hasType || !hasFaction || !hasName || !hasPower || !hasRange)))
        throw new Exception("Misstatement of card properties");
    }

    //------------------------------------------------OnActivation---------------------------------------------------------------------
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
            effect = FindEffect(ParseString());

            isSyntacticSugarOn = true;
        }
        else if(CurrentToken.Type == TokenType.LCBracket)
        {
            Next(TokenType.LCBracket);
            if(CurrentToken.Value == "Name") Next(TokenType.Keyword);
            else throw new Exception("Effect name expected");
            Next(TokenType.Colon);

            effect = FindEffect(ParseString());
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
    private Effect FindEffect(string name)
    {
        foreach(Effect effect in Effects)
        {
            if(effect.Name == name)
            {
                return (Effect)effect.Clone();
            }
        }

        throw new Exception("That effect does not exist");
    }

    // Método para otorgarle el valor a los parámetros
    private void SetParams(Effect effect, ref int count)
    {
        bool isCorrectParam = false;

        foreach(Variable param in effect.Params)
        {
            if(param.Name == CurrentToken.Value)
            { 
                foreach(Variable variable in effect.Variables)
                {
                    if(variable.Name == param.Name)
                    {
                        Next(TokenType.Word);
                        Next(TokenType.Colon);
                        if(param.Type == "Number") variable.Value = Expr();
                        else if(param.Type == "Bool")
                        {
                            dNext = Next;
                            variable.Value = ParseBooleanExpression();
                        } 
                        else if(param.Type == "String")
                        {                     
                            variable.Value = ParseString();
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
        
        string source = ParseString();

        if(Enum.IsDefined(typeof(Source), source) || source != "parent") effect.SourceTargets = source;
        else if(source == "parent" && effectParent != null) effect.SourceTargets = effectParent.SourceTargets;
        else throw new Exception("Invalid card filter");

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
        if(CurrentToken.Value == "Predicate")
        {   
            Next(TokenType.Keyword);
            Next(TokenType.Colon);

            dNext = NextAndSavePredicate;
            Predicate(effect.Variables);

            //Se borra la variable declarada dentro del predicado para evitar posibles errores
            CurrentVariables.Clear();
        } 
        else throw new Exception("Predicate expected");

        Next(TokenType.RCBracket);

        //Método para cambiar al siguiente token y guardarlo como parte de la instrucción del "predicate"
        void NextAndSavePredicate(TokenType type)
        {
            effect.Predicate.Add(CurrentToken.Value);
            Next(type);
        }
    }

    // Método para el funcionamiento del predicate
    private void Predicate(LinkedList<Variable> variables)
    {
        Next(TokenType.LParen);

        foreach(Variable variable in CurrentVariables)
        if(variable.Name == CurrentToken.Value) 
        throw new Exception("You cannot use this variable in this context");
        
        // Se crea la variable nueva 
        if(CurrentToken.Type == TokenType.Word) 
        {
            CurrentVariables.AddLast(new Variable(CurrentToken.Value, "Card"));
            variables?.AddLast(new Variable(CurrentToken.Value, "Card"));

            dNext(TokenType.Word);
        }
        else throw new Exception("A variable was expected here");

        Next(TokenType.RParen);

        Next(TokenType.Asign);
        Next(TokenType.Greater);

        // Y finalmente se parsea la expresión lambda
        ParseBooleanExpression();
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
        Effect effect = FindEffect(ParseString());

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

    //-----------------------------------------------Extras---------------------------------------------------------------------------
    // Para comprobar si la carta puede tener efectos
    private bool IsCardWithEffectCorrect()
    {
        foreach(Property property in properties)
        if((string)property.Value == "Silver" || (string)property.Value == "Gold" || (string)property.Value == "Leader") return true;
                    
        return false;
    }   

    // Comprueba si la expresión es booleana
    private bool IsBoolean()
    {
        int startTokenIndex = CurrentTokenIndex;
        Token startToken = CurrentToken;

        while(CurrentToken.Type != TokenType.Semicolon && CurrentToken.Type != TokenType.Fin)
        {
            if(Enum.IsDefined(typeof(Booleans), CurrentToken.Type.ToString()) || CurrentToken.Value == "true" || CurrentToken.Value == "false")
            {
                CurrentTokenIndex = startTokenIndex;
                CurrentToken = startToken;
                return true;
            }
            

            foreach(Variable variable in CurrentVariables)
            {
                if(variable.Name == CurrentToken.Value && variable.Value is bool)
                {
                    CurrentTokenIndex = startTokenIndex;
                    CurrentToken = startToken;
                    return true;
                }
               
            }

            TokenType forceNext = CurrentToken.Type;
            Next(forceNext);
        }

        CurrentTokenIndex = startTokenIndex;
        CurrentToken = startToken;
        return false;
    }

    //-------------------------------------------Booleanos--------------------------------------------------------------------------------
    private delegate void DNext(TokenType tokenType);
    DNext dNext;
    
    //Método principal
    public bool ParseBooleanExpression()
    {
        //Resultado que se mostrará
        var result = ParseOrExpression();
        
        //Se comprueba si se llega al final
        if(CurrentToken.Type != TokenType.RCBracket && CurrentToken.Type != TokenType.Comma 
           && CurrentToken.Type != TokenType.RParen && CurrentToken.Type != TokenType.Semicolon)
        throw new Exception("Invalid boolean expression");
        
        return result;
    }

    //Para parsear el "or"
    private bool ParseOrExpression()
    {
        var result = ParseAndExpression();

        while (CurrentToken.Type == TokenType.Or)
        {
            dNext(TokenType.Or);
            var result2 = ParseAndExpression();
            result = result || result2;
        }
        return result;
    }

    //Para parsear el "and"
    private bool ParseAndExpression()
    {
        var result = ParseRelationalExpression();

        while (CurrentToken.Type == TokenType.And)
        {
            dNext(TokenType.And);
            var result2 = ParseRelationalExpression();
            result = result && result2;
        }
        return result;
    }

    //Para parsear operadores de relaciones
    private bool ParseRelationalExpression()
    {
        var left = ParsePrimaryExpression();
       
        while (CurrentToken.Type == TokenType.Smaller || CurrentToken.Type == TokenType.Greater ||
               CurrentToken.Type == TokenType.SmallerOrEqual || CurrentToken.Type == TokenType.GreaterOrEqual
               || CurrentToken.Type == TokenType.Equal || CurrentToken.Type == TokenType.NotEqual)
        {
            if (CurrentToken.Type == TokenType.Smaller)
            {
                dNext(TokenType.Smaller);
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue < rightValue;
                }
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentToken.Type == TokenType.Greater)
            {
                dNext(TokenType.Greater);

                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue > rightValue;
                } 
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentToken.Type == TokenType.SmallerOrEqual)
            {
                dNext(TokenType.SmallerOrEqual);
               
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue <= rightValue;
                } 
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentToken.Type == TokenType.GreaterOrEqual)
            {
                dNext(TokenType.GreaterOrEqual);
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue >= rightValue;
                } 
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentToken.Type == TokenType.Equal)
            {
                dNext(TokenType.Equal);
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue == rightValue;
                } 
                else if(left is string value)
                {
                    string rightValue = ParseString();
                    left = value == rightValue;
                }
            }
            else if (CurrentToken.Type == TokenType.NotEqual)
            {
                dNext(TokenType.NotEqual);
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue != rightValue;
                } 
                else if(left is string value)
                {
                    string rightValue = ParseString();
                    left = value != rightValue;
                }
            }
        }
        return (bool)left;
    }

    //Para parsear booleanos, números y paréntesis
    private object ParsePrimaryExpression()
    {
        if (CurrentToken.Value == "true")
        {
            dNext(TokenType.Word);
            return true;
        }
        else if (CurrentToken.Value == "false")
        {
            dNext(TokenType.Word);
            return false;
        }
        else if (CurrentToken.Type == TokenType.Number)
        {
            int value = int.Parse(CurrentToken.Value);
            dNext(TokenType.Number);
            return value;
        }
        else if (CurrentToken.Type == TokenType.LParen)
        {
            dNext(TokenType.LParen);
            bool result = ParseOrExpression();
            dNext(TokenType.RParen);
            return result;
        }
        else if(CurrentToken.Type == TokenType.Word)
        {
            foreach(Variable variable in CurrentVariables)
            {
                if(variable.Name == CurrentToken.Value && variable.Value is int value)
                {
                    dNext(TokenType.Word);
                    
                    if(CurrentToken.Type == TokenType.Increase) dNext(TokenType.Increase);

                    return value;
                }
                else if(variable.Name == CurrentToken.Value && variable.Value is bool value2)
                {
                    dNext(TokenType.Word);
                    return value2;
                }
                else if(variable.Name == CurrentToken.Value && variable.Value is string value3) return ParseString();
                else if(variable.Name == CurrentToken.Value && variable.Type == "Card" && (Tokens[CurrentTokenIndex + 2].Value == "Power"))
                {
                    dNext(TokenType.Word);
                    Next(TokenType.Point);
                    dNext(TokenType.Keyword);

                    return 0;
                }
                else if(variable.Name == CurrentToken.Value && variable.Type == "Card" 
                && (Tokens[CurrentTokenIndex + 2].Value == "Name" || Tokens[CurrentTokenIndex + 2].Value == "Type"
                || Tokens[CurrentTokenIndex + 2].Value == "Range" || Tokens[CurrentTokenIndex + 2].Value == "Faction"))
                {
                    return ParseString();
                }
            }

            throw new Exception("Is not a correct variable");
        }
        else if(CurrentToken.Type == TokenType.Increase)
        {
            dNext(TokenType.Increase);

            foreach(Variable variable in CurrentVariables)
            {
                if(variable.Name == CurrentToken.Value && variable.Value is int value)
                {
                    dNext(TokenType.Word);
                    return value;
                }
            }

            throw new Exception("Is not a correct variable");
        }
        else if(CurrentToken.Value == "\"") return ParseString();
        else throw new Exception("Invalid boolean expression");
    }

    //-------------------------------------------String--------------------------------------------------------------------------------
    // Para parsear strings
    private string ParseString()
    { 
        string result = null;

        if(CurrentToken.Type == TokenType.QMark)
        {
            dNext(TokenType.QMark);
            result = "";
            result += CurrentToken.Value;
            dNext(TokenType.String);
            dNext(TokenType.QMark);
        }
        else if(CurrentToken.Type == TokenType.Word)
        {
            bool IsCorrect = false;

            foreach(Variable variable in CurrentVariables)
            {
                if(variable.Name == CurrentToken.Value && variable.Value is string value)
                {
                    result = "";
                    result += value;
                    dNext(TokenType.Word);
                    IsCorrect = true;
                }
                else if(variable.Name == CurrentToken.Value && variable.Type == "Card" && (Tokens[CurrentTokenIndex + 2].Value == "Name"
                        || Tokens[CurrentTokenIndex + 2].Value == "Type" || Tokens[CurrentTokenIndex + 2].Value == "Range" 
                        || Tokens[CurrentTokenIndex + 2].Value == "Faction"))
                {
                    result = "";
                    dNext(TokenType.Word);
                    Next(TokenType.Point);
                    dNext(TokenType.Keyword);
                    IsCorrect = true;
                }
            }
            
            if(!IsCorrect) throw new Exception("This word is not a correct variable");
        }

        if(CurrentToken.Type == TokenType.At)
        {
            dNext(TokenType.At);
            result += ParseString(); 
        }
        else if(CurrentToken.Type == TokenType.DoubleAt)
        {
            dNext(TokenType.DoubleAt);
            result += " " + ParseString(); 
        }

        return result;
    }
}            