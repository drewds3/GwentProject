using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Context;
using static SetPlayers;

//Efecto de las cartas
public class Effect : ICloneable
{
    //Nombre del efecto
    public string Name {get;}

    //Parámetros de la carta (si es que tiene)
    public LinkedList<Variable> Params;

    //Nombre de la variable "targets"
    private string TargetsName;

    //Targets que se buscaran
    public string SourceTargets {get;set;}

    //Single
    public bool Single {get;set;}

    //Predicate del Selector
    public Instruction Predicate = new();

    //Variables
    public LinkedList<Variable> Variables;

    //Instrucciones a ejecutar por el efecto
    private List<Instruction> Instructions;

    //Efecto anidado (si es que tiene)
    public Effect PostEffect {get;set;}

    //Efecto padre (si es que tiene)
    public Effect ParentEffect {get;set;}

    //Instrucción actual
    private Instruction CurrentInstruction;
    private int CurrentInstructionIndex;

    //KeyWord actual
    private string CurrentKeyWord;
    private int CurrentKeyWordIndex;

    // Delegado para mayor eficiencia en las listas devueltas
    delegate List<GameObject> ListReturn(Player player);

    //Constructores
    public Effect(string name, string targetsName, List<Instruction> instructions, LinkedList<Variable> variables)
    {
        Name = name;
        TargetsName = targetsName;
        Instructions = instructions;
        Variables = variables;
        CurrentInstruction = Instructions[0];
        CurrentKeyWord = CurrentInstruction.KeyWords[0];
    } 

    public Effect(string name, string targetsName, List<Instruction> instructions, LinkedList<Variable> variables ,LinkedList<Variable> Params)
    {
        Name = name;
        TargetsName = targetsName;
        Instructions = instructions;
        Variables = variables;
        this.Params = Params;
        CurrentInstruction = Instructions[0];
        CurrentKeyWord = CurrentInstruction.KeyWords[0];
    }

    //Método para clonar el efecto y trabajar con él sin que sea de referencia
    public object Clone()
    {
        Effect effect = new(Name, TargetsName, Instructions, Variables, Params)
        {
            SourceTargets = SourceTargets,
            Single = Single,
            Predicate = (Instruction)Predicate.Clone()
        };

        if(PostEffect != null) effect.PostEffect = (Effect)PostEffect.Clone();
        if(ParentEffect != null) effect.ParentEffect = (Effect)ParentEffect.Clone();

        List<Instruction> instructions = new();

        foreach(Instruction instruction in Instructions)
        {
            instructions.Add((Instruction)instruction.Clone());
        }

        effect.Instructions = instructions;

        LinkedList<Variable> variables = new();

        foreach(Variable variable in Variables)
        {
            variables.AddLast((Variable)variable.Clone());
        }

        effect.Variables = variables;

        LinkedList<Variable> parameters = new();

        foreach(Variable parameter in Params)
        {
            parameters.AddLast((Variable)parameter.Clone());
        }

        effect.Params = parameters;

        return effect;
    }

    //Método para activar el efecto
    public void Activate()
    {   
        Debug.Log($"Inicio de ejecución del efecto: \"{Name}\"");
        
        // Primero se le otorga un valor a los "targets"
        Source();

        // Luego, se comienzan a seguir las instrucciones
        FinishInstruction();
    }

    // Método para controlar el cumplimiento de las instrucciones
    private void FinishInstruction()
    {
        Debug.Log("Instruccion iniciada");

        CurrentInstruction.Debug();

        // Si el segundo string es un "igual" entonces es una declaración
        if(CurrentInstruction.KeyWords[1] == "=") 
        {
            // Se comprueba de qué tipo es la variable declarada y se procede en consecuencia
            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord && variable.Value is int)
                {
                    Next();
                    Next();
                    variable.Value = NumericExpression();
                    break;
                }
                else if(variable.Name == CurrentKeyWord && variable.Value is bool)
                {
                    Next();
                    Next();
                    variable.Value = BooleanExpression();
                    break;
                }
                else if(variable.Name == CurrentKeyWord && variable.Value is string)
                {
                    Next();
                    Next();
                    variable.Value = StringExpression();
                    break;
                }
                else if(variable.Name == CurrentKeyWord)
                {
                    variable.Value = Declaration();
                    break;
                }
            }
        }
        else if(CurrentKeyWord == "for")
        {
            ForIntruction();
        }
        else if(CurrentKeyWord == "while")
        {
            WhileInstruction();
        }
        else if(CurrentKeyWord == "++" || IsNumericVariable() || IsNumericProperty())
        {
            IncreaseAndComposedOperations();
        }
        else //De lo contrario es una llamada a un método
        {
            MethodCall();
        }

        // Si aún no se han terminado las instrucciones se continua con la siguiente
        if(CurrentInstruction.Count != 0)
        {
            FinishInstruction();
        }
        else if(PostEffect != null) // De haberse concluido con el efecto, se activa el PostAction de tenerlo
        {
            PostEffect.ParentEffect = this;
            PostEffect.Activate();
        } 
    }

    // Método para otorgar valor a los "targets"
    private void Source()
    {
        foreach(Variable variable in Variables)
        {
            // Se le otorga el valor del "source"
            if(variable.Name == TargetsName)
            {
                if(SourceTargets == "board") variable.Value = Board;
                else if(SourceTargets == "hand") variable.Value = HandOfPlayer(TriggerPlayer);
                else if(SourceTargets == "otherHand") variable.Value = HandOfPlayer(OtherPlayer);
                else if(SourceTargets == "deck") variable.Value = DeckOfPlayer(TriggerPlayer);
                else if(SourceTargets == "otherDeck") variable.Value = DeckOfPlayer(OtherPlayer);
                else if(SourceTargets == "field") variable.Value = FieldOfPlayer(TriggerPlayer);
                else if(SourceTargets == "otherField") variable.Value = FieldOfPlayer(OtherPlayer);
                else if(SourceTargets == "parent") 
                {
                    // En este caso se le otorga el valor del efecto padre
                    foreach(Variable variableP in ParentEffect.Variables)
                    if(variableP.Name == ParentEffect.TargetsName) variable.Value = variableP.Value;
                } 
                else if(SourceTargets == "void") variable.Value = new List<GameObject>();

                // Si es diferente de "void" se continua
                if(SourceTargets != "void")
                {   
                    if(Single) // Se comprueba si se debe tomar una única carta o todas
                    {
                        List<GameObject> cards = (List<GameObject>)variable.Value;
                        List<GameObject> cards2 = new(){cards[0]};
                        variable.Value = cards2;
                    }

                    // Por último se descartan las cartas que no cumplan con el predicado
                    Predicate.Debug();
                    CurrentInstruction = Predicate;
                    CurrentKeyWord = CurrentInstruction.KeyWords[0]; 
                    Variable variable2 = SetVariableValue(CurrentKeyWord);
                    
                    Next();

                    // Como no se puede modificar una lista mientras está en un foreach se crea una nueva igual a la actual
                    List<GameObject> cards3 = new();
                    foreach(GameObject card in (List<GameObject>)variable.Value)
                    cards3.Add(card);

                    foreach(GameObject card in (List<GameObject>)variable.Value)
                    {
                        variable2.Value = card;
                        
                        bool casa = BooleanExpression();

                        if(!casa) cards3.Remove(card);

                        CurrentInstruction = Predicate;
                        CurrentKeyWord = CurrentInstruction.KeyWords[1];
                        CurrentKeyWordIndex = 1;
                    } 
                    
                    variable.Value = cards3;

                    // Finalmente se establece la instrucción actual como la primera de la lista del efecto
                    CurrentInstruction = Instructions[0];
                    CurrentKeyWord = CurrentInstruction.KeyWords[0];
                    CurrentInstructionIndex = 0;
                    CurrentKeyWordIndex = 0;

                    return;
                } 
                else return;
            }
        }

        throw new Exception();
    }

    //Método para avanzar a la siguiente indicación o instrucción
    private void Next()
    {
        //Avanza dentro de la instrucción y de llegar al final la "finaliza"
        if(CurrentKeyWord != "Fin")
        {
            CurrentKeyWordIndex++;
            CurrentKeyWord = CurrentKeyWordIndex < CurrentInstruction.Count ?
            CurrentInstruction.KeyWords[CurrentKeyWordIndex] : CurrentKeyWord = "Fin";
        }
       
       //Si se llega al final de la instrucción se pasa a la siguiente
        if(CurrentKeyWord == "Fin")
        {
            CurrentKeyWordIndex = 0;
            CurrentInstructionIndex++;
            CurrentInstruction = CurrentInstructionIndex < Instructions.Count ?
            Instructions[CurrentInstructionIndex] : new Instruction();

            //Si no se ha llegado a la última instrucción se pasa a la siguiente
            if(CurrentInstruction.Count > 0)
            {
                CurrentKeyWord = CurrentInstruction.KeyWords[CurrentKeyWordIndex];
            }
        }
    }

    // Método para controllar las declaraciones de variables
    private object Declaration()
    {
        Next(); 
        Next();
        return Parameter();
    }

    // Método para controlar los párametros y complementario del anterior
    private object Parameter()
    {
        // Comprueba de qué tipo es la variable
        if(VariableType() == "Context") // En caso de ser de tipo "context"
        {
            Next();
            
            if(CurrentKeyWordIndex == 0) return GameObject.Find("Tablero").GetComponent<Context>();
            else if(CurrentKeyWord == "TriggerPlayer")
            {
                Next();
                return TriggerPlayer;
            }
            else // Si es una propiedad de tipo "List<Card>"
            {
                ListReturn listReturn;
                Player player;
                List<GameObject> cards;

                if(CurrentKeyWord == "Board")
                {
                    Next();
                    cards = Board;
                }
                else if(CurrentKeyWord == "HandOfPlayer")
                {
                    Next();
                    listReturn = HandOfPlayer;
                    player = (Player)Parameter();
                    cards = listReturn(player);
                } 
                else if(CurrentKeyWord == "FieldOfPlayer")
                {
                    Next();
                    listReturn = FieldOfPlayer;
                    player = (Player)Parameter();
                    cards = listReturn(player);
                } 
                else if(CurrentKeyWord == "GraveyardOfPlayer")
                {
                    Next();
                    listReturn = GraveyardOfPlayer;
                    player = (Player)Parameter();
                    cards = listReturn(player);
                }
                else if(CurrentKeyWord == "DeckOfPlayer")
                {
                    Next();
                    listReturn = DeckOfPlayer;
                    player = (Player)Parameter();
                    cards = listReturn(player);
                }
                else if(CurrentKeyWord == "Hand")
                {
                    Next();
                    listReturn = HandOfPlayer;
                    player = TriggerPlayer;
                    cards = listReturn(player);
                }
                else if(CurrentKeyWord == "Field")
                {
                    Next();
                    listReturn = FieldOfPlayer;
                    player = TriggerPlayer;
                    cards = listReturn(player);
                }
                else if(CurrentKeyWord == "Graveyard")
                {
                    Next();
                    listReturn = GraveyardOfPlayer;
                    player = TriggerPlayer;
                    cards = listReturn(player);
                }
                else //if Current == "Deck"
                {
                    Next();
                    listReturn = DeckOfPlayer;
                    player = TriggerPlayer;
                    cards = listReturn(player);
                }

                // Mientras que sea alguna de esos métodos o se indexe
                while(CurrentKeyWord == "Pop" || CurrentKeyWord == "Find" || CurrentKeyWordIndex == 0 || CurrentKeyWord == "[")
                {
                    if(CurrentKeyWordIndex == 0) return cards;
                    else if(CurrentKeyWord == "Pop")
                    {
                        Next();
                        GameObject card = cards.Last();
                        cards.Remove(card);
                        return card;
                    }
                    else if(CurrentKeyWord == "[")
                    {
                        Next();
                        GameObject card = cards[NumericExpression()];
                        Next();
                        
                        if(CurrentKeyWordIndex == 0) return card;
                        else if(CurrentKeyWord == "Owner")
                        {
                            Next();

                            if(card.GetComponent<Card>().Owner == "Player1") return player1;
                            else if(card.GetComponent<Card>().Owner == "Player2") return player2;
                        }
                    }
                    else if(CurrentKeyWord == "Find")
                    {
                        Next();
                        Variable variable = SetVariableValue(CurrentKeyWord);
                        Next();
            
                        Instruction startInstruction = CurrentInstruction;
                        int startInstructionIndex = CurrentInstructionIndex;
                        string startKeyWord = CurrentKeyWord;
                        int startKeyWordIndex = CurrentKeyWordIndex; 
                    

                        List<GameObject> cards2 = new();
                        foreach(GameObject card in cards)
                        cards2.Add(card);

                        foreach(GameObject card in cards)
                        {
                            CurrentInstruction = startInstruction;
                            CurrentInstructionIndex = startInstructionIndex;
                            CurrentKeyWord = startKeyWord;
                            CurrentKeyWordIndex = startKeyWordIndex;

                            variable.Value = card;
                                
                            bool casa = BooleanExpression();

                            if(!casa) cards2.Remove(card);
                        }
                            
                        cards = cards2;

                        if(CurrentKeyWord != "Pop" && CurrentKeyWord != "Find" && CurrentKeyWord != "[") return cards;
                    }      
                    else throw new Exception();    
                }
                throw new Exception();
            }
        }
        else if(VariableType() == "List<Card>") // En caso de ser de tipo "List<Card>"
        {
            string nameVariable = CurrentKeyWord;

            List<GameObject> cards = (List<GameObject>)SetVariableValue(CurrentKeyWord).Value;

            Next();

            while(CurrentKeyWord == "Pop" || CurrentKeyWord == "Find" || CurrentKeyWordIndex == 0 || CurrentKeyWord == "[")
            {
                if(CurrentKeyWordIndex == 0) return SetVariableValue(nameVariable).Value;
                else if(CurrentKeyWord == "Pop")
                {
                    Next();
                    GameObject card = cards.Last();
                    cards.Remove(card);
                    return card;
                }
                else if(CurrentKeyWord == "[")
                {
                    Next();
                    GameObject card = cards[NumericExpression()];
                    Next();

                    if(CurrentKeyWordIndex == 0) return card;
                    else if(CurrentKeyWord == "Owner")
                    {
                        Next();

                        if(card.GetComponent<Card>().Owner == "Player1") return player1;
                        else if(card.GetComponent<Card>().Owner == "Player2") return player2;
                    }
                }
                else if(CurrentKeyWord == "Find")
                {
                    Next();
                    Variable variable = SetVariableValue(CurrentKeyWord);
                    Next();
        
                    Instruction startInstruction = CurrentInstruction;
                    int startInstructionIndex = CurrentInstructionIndex;
                    string startKeyWord = CurrentKeyWord;
                    int startKeyWordIndex = CurrentKeyWordIndex; 
                

                    List<GameObject> cards2 = new();
                    foreach(GameObject card in cards)
                    cards2.Add(card);

                    foreach(GameObject card in cards)
                    {
                        CurrentInstruction = startInstruction;
                        CurrentInstructionIndex = startInstructionIndex;
                        CurrentKeyWord = startKeyWord;
                        CurrentKeyWordIndex = startKeyWordIndex;

                        variable.Value = card;
                            
                        bool casa = BooleanExpression();

                        if(!casa) cards2.Remove(card);
                    }
                        
                    cards = cards2;

                    if(CurrentKeyWord != "Pop" && CurrentKeyWord != "Find" && CurrentKeyWord != "[") return cards;
                } 
                else throw new Exception();  
            }
            throw new Exception(); 
        }
        else if(VariableType() == "Card") // En caso de ser de tipo "Card"
        {
            string nameVariable = CurrentKeyWord;

            Next();

            if(CurrentKeyWordIndex == 0) return SetVariableValue(nameVariable).Value;
            else if(CurrentKeyWord == "Owner")
            {
                Next();

                GameObject card = (GameObject)SetVariableValue(nameVariable).Value;

                if(card.GetComponent<Card>().Owner == "Player1") return player1;
                else if(card.GetComponent<Card>().Owner == "Player2") return player2;
            }

            throw new Exception();
        }
        else if(VariableType() == "Player") // En caso de ser de tipo "Player"
        {
            string nameVariable = CurrentKeyWord;

            Next();
            
            return SetVariableValue(nameVariable).Value;
        } 
        else throw new Exception();
    }

    // Devuelve el tipo de la variable actual
    private string VariableType()
    {
        foreach(Variable variable in Variables)
        {
            if(variable.Name == CurrentKeyWord)
            {
                return variable.Type;
            }
        }
        return null;
    }

    // Devuelve el valor de la variable actual
    private Variable SetVariableValue(string name)
    {
        foreach(Variable variable in Variables)
        if(variable.Name == name) return variable;

        throw new Exception();
    }

    // Para controlar las llamadas a métodos
    private void MethodCall()
    {
        foreach(Variable variable in Variables)
        {
            if(variable.Name == CurrentKeyWord)
            {
                Next();
                if(variable.Type == "Context") Context();
                else if(variable.Type == "List<Card>") CardList((List<GameObject>)variable.Value);
                break;
            } 
        } 
    }

    // Comprueba si es una variable numérica
    public bool IsNumericVariable()
    {
        foreach(Variable variable in Variables)
        if(variable.Name == CurrentKeyWord && variable.Value is int)
        return true;
        
        return false;
    }

    // Comprueba si es una propiedad con valor numérico
    public bool IsNumericProperty()
    {
        foreach(Variable variable in Variables)
        if(variable.Name == CurrentKeyWord && variable.Type == "Card"
        && CurrentInstruction.KeyWords[CurrentKeyWordIndex+1] == "Power")
        return true;
        
        return false;
    }

    // Para controlar las operaciones compuestas y los incrementos
    private void IncreaseAndComposedOperations()
    {
        if(CurrentKeyWord != "++") // Si es una operación compuesta o el incremento está a la derecha
        {
            string variableName = CurrentKeyWord;
            int newValue = 0;

            // Se comprueba si es una variable o propiedad de valor numérico
            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord && variable.Value is int value)
                {
                    newValue = value;

                    Next();
                    break;
                } 
                else if(variable.Name == CurrentKeyWord && variable.Type == "Card" 
                && CurrentInstruction.KeyWords[CurrentKeyWordIndex+1] == "Power")
                {   
                    GameObject card = (GameObject)variable.Value;
                    newValue = card.GetComponent<Card>().Power;

                    Next();
                    Next();
                    break;
                }
            } 

            // Se realiza la operación en cuestión
            if(CurrentKeyWord == "+=")
            {
                Next();
                newValue += NumericExpression();
            }
            else if(CurrentKeyWord == "-=")
            {
                Next();
                newValue -= NumericExpression();
            }
            else if(CurrentKeyWord == "/=")
            {
                Next();
                newValue /= NumericExpression();
            }
            else if(CurrentKeyWord == "*=")
            {
                Next();
                newValue *= NumericExpression();
            }
            else if(CurrentKeyWord == "^=")
            {
                Next();
                newValue ^= NumericExpression();
            }
            else if(CurrentKeyWord == "++")
            {
                Next();
                newValue++;
            }

            // Se le otorga el nuevo valor a la variable o propiedad
            foreach(Variable variable in Variables)
            {
                if(variable.Name == variableName && variable.Value is int value1)
                {
                    variable.Value = newValue;
                    break;
                } 
                else if(variable.Name == variableName && variable.Type == "Card")
                {   
                    GameObject card = (GameObject)variable.Value;

                    if(card.GetComponent<Card>().Type == "Silver" || card.GetComponent<Card>().Type == "Gold") 
                    card.GetComponent<Card>().Power = newValue;
                    else Debug.Log("No se puede modificar el valor del poder de cartas especiales");

                    break;
                }
            }
        }
        else // Si es un incremento a la izquierda
        {
            Next();

            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord && variable.Value is int value)
                {
                    int temp = value;
                    temp++;
                    variable.Value = temp;

                    Next();
                    return;
                } 
                else if(variable.Name == CurrentKeyWord && variable.Type == "Card" 
                && CurrentInstruction.KeyWords[CurrentKeyWordIndex+1] == "Power")
                {
                    GameObject card = (GameObject)variable.Value;
                    if(card.GetComponent<Card>().Type == "Silver" || card.GetComponent<Card>().Type == "Gold") 
                    card.GetComponent<Card>().Power++;
                    else Debug.Log("No se puede modificar el valor del poder de cartas especiales");
                    
                    Next();
                    Next();
                    return;
                }
            } 
            throw new Exception();
        }
    }

    // Controlador de los bucles "for"
    private void ForIntruction()
    {
        Next();
        string nameCard = CurrentKeyWord;
        Next();
        List<GameObject> cards = (List<GameObject>)SetVariableValue(CurrentKeyWord).Value;
        Next();

        // Se guarda la instrucción actual
        Instruction startInstruction = CurrentInstruction;
        int startInstructionIndex = CurrentInstructionIndex;

        int count = cards.Count - 1;

        // Se inicia el bucle
        foreach(GameObject card in cards)
        {   
            // Se le otorga el valor actual a la variable "card"
            Variable variable = SetVariableValue(nameCard);
            variable.Value = card;

            // Se realizan las operaciones del bucle
            FinishFor(count--);
        }

        // Al terminar el bucle se sigue por su final
        while(CurrentKeyWord != "ForFinal") Next();

        Next();

        //Método para que se cumpla el ciclo
        void FinishFor(int count)
        {
            Debug.Log("Instruccion del for iniciada");

            CurrentInstruction.Debug();

            //Si el segundo string es un "igual" entonces es una declaración
            if(CurrentInstruction.KeyWords[1] == "=")
            {
                foreach(Variable variable in Variables)
                {
                    // Se comprueba el tipo de la variable
                    if(variable.Name == CurrentKeyWord && variable.Value is int)
                    {
                        Next();
                        Next();  
                        variable.Value = NumericExpression();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord && variable.Value is bool)
                    {
                        Next();
                        Next();
                        variable.Value = BooleanExpression();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord && variable.Value is string)
                    {
                        Next();
                        Next();
                        variable.Value = StringExpression();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord)
                    {
                        variable.Value = Declaration();
                        break;
                    }
                }
            }
            else if(CurrentKeyWord == "for")
            {
                ForIntruction();
            }
            else if(CurrentKeyWord == "while")
            {
                WhileInstruction();
            }
             else if(CurrentKeyWord == "++" || IsNumericVariable() || IsNumericProperty())
            {
                IncreaseAndComposedOperations();
            }
            else //De lo contrario es una llamada a un método
            {
                MethodCall();
            }

            //Si no ha concluido el ciclo, entonces lee la siguiente instrucción del mismo
            if(CurrentKeyWord != "ForFinal")
            {
                FinishFor(count);
            }
            else if(CurrentKeyWord == "ForFinal" && count > 0) // De lo contrario vuelve a empezar el ciclo si aún quedan cartas por iterar
            {
                CurrentInstruction = startInstruction;
                CurrentInstructionIndex = startInstructionIndex;
                CurrentKeyWord = startInstruction.KeyWords[0];
            }
        }
    }
    
    // Para controlar los bucles "while"
    private void WhileInstruction()
    {
        Next();

        // Se guarda la instrucción actual
        Instruction startInstruction = CurrentInstruction;
        int startInstructionIndex = CurrentInstructionIndex;

        bool argument = BooleanExpressionWhile(); // Se recoge el argumento del bucle

        if(!argument)
        {
            while(CurrentKeyWord != "WhileFinal")
            {
                Next();
            }
            Next();
        }  

        // Se inicia el bucle
        while(argument)
        {
            // Se realizan las instrucciones del bucle
            FinishWhile();

            // Se vuelve a comprobar el valor del argumento
            if(!BooleanExpressionWhile())
            {
                argument = false;

                // Si es falso entonces se termina el bucle y se sigue por su final
                while(CurrentKeyWord != "WhileFinal")
                {
                    Next();
                }
                Next();
            }  
        }

        //Método para que se cumpla el ciclo
        void FinishWhile()
        {
            Debug.Log("Instruccion del while iniciada");

            CurrentInstruction.Debug();

            //Si el segundo string es un "igual" entonces es una declaración
            if(CurrentInstruction.KeyWords[1] == "=")
            {
                foreach(Variable variable in Variables)
                {   
                    // Se comprueba el tipo de variable
                    if(variable.Name == CurrentKeyWord && variable.Value is int)
                    {
                        Next();
                        Next();
                        variable.Value = NumericExpression();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord && variable.Value is bool)
                    {
                        Next();
                        Next();
                        variable.Value = BooleanExpression();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord && variable.Value is string)
                    {
                        Next();
                        Next();
                        variable.Value = StringExpression();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord)
                    {
                        variable.Value = Declaration();
                        break;
                    }
                }
            }
            else if(CurrentKeyWord == "for")
            {
                ForIntruction();
            }
            else if(CurrentKeyWord == "while")
            {
                WhileInstruction();
            }
            else if(CurrentKeyWord == "++" || IsNumericVariable() || IsNumericProperty())
            {
                IncreaseAndComposedOperations();
            }
            else //De lo contrario es una llamada a un método
            {
                MethodCall();
            }

            //Lee la siguiente instrucción del mismo hasta que concluya
            if(CurrentKeyWord != "WhileFinal")
            {
                FinishWhile();
            }
        }

        // Método para volver a comprobar el valor del argumento del bucle
        bool BooleanExpressionWhile()
        {
            CurrentInstruction = startInstruction;
            CurrentInstructionIndex = startInstructionIndex;
            CurrentKeyWord = startInstruction.KeyWords[1];
            CurrentKeyWordIndex = 1;

            return BooleanExpression();
        }
    }

    // Método para controlar los métodos del "Contexto"
    private void Context()
    {
        ListReturn listReturn;
        Player player;
        List<GameObject> cards;

        // Se otorgan los valores
        if(CurrentKeyWord == "Board")
        {
            Next();
            cards = Board;
        }
            else if(CurrentKeyWord == "HandOfPlayer")
        {
            Next();
            listReturn = HandOfPlayer;
            player = (Player)Parameter();
            cards = listReturn(player);
        } 
        else if(CurrentKeyWord == "FieldOfPlayer")
        {
            Next();
            listReturn = FieldOfPlayer;
            player = (Player)Parameter();
            cards = listReturn(player);
        } 
        else if(CurrentKeyWord == "GraveyardOfPlayer")
        {
            Next();
            listReturn = GraveyardOfPlayer;
            player = (Player)Parameter();
            cards = listReturn(player);
        }
        else if(CurrentKeyWord == "DeckOfPlayer")
        {
            Next();
            listReturn = DeckOfPlayer;
            player = (Player)Parameter();
            cards = listReturn(player);
        }
        else if(CurrentKeyWord == "Hand")
        {
            Next();
            listReturn = HandOfPlayer;
            player = TriggerPlayer;
            cards = listReturn(player);
        }
        else if(CurrentKeyWord == "Field")
        {
            Next();
            listReturn = FieldOfPlayer;
            player = TriggerPlayer;
            cards = listReturn(player);
        }
        else if(CurrentKeyWord == "Graveyard")
        {
            Next();
            listReturn = GraveyardOfPlayer;
            player = TriggerPlayer;
            cards = listReturn(player);
        }
        else if(CurrentKeyWord == "Deck")
        {
            Next();
            listReturn = DeckOfPlayer;
            player = TriggerPlayer;
            cards = listReturn(player);
        }
        else throw new Exception();
        
        // Y se pasan al controlador de las listas de cartas
        CardList(cards);
    }

    // Para controlar las listas de cartas
    private void CardList(List<GameObject> cards)
    {
        // Se comprueba a cuál método se está llamando
        if(CurrentKeyWord == "Push" || CurrentKeyWord == "Add" || CurrentKeyWord == "SendBottom")
        {
            string function = CurrentKeyWord;

            Next();

            //Se recoge la carta a la que se hace referencia
            GameObject card = (GameObject)Parameter();
            
            // En dependencia de dónde se añadan se procede de manera diferente
            if(cards.SequenceEqual(HandOfPlayer(player1)))
            {
                //Se instancia en la mano
                GameObject cardCopy = MonoBehaviour.Instantiate(card, player1.Hand.transform);
                NewCard properties = cardCopy.GetComponent<NewCard>();

                //Se añaden los efectos porque se suelen perder al ser por referencia
                foreach(Effect effect in card.GetComponent<NewCard>().Effects)
                properties.Effects.Add((Effect)effect.Clone());

                //Se cambia el dueño y el tag, y la facción de ser necesario
                if(properties.Faction != "Neutral") properties.Faction = player1.Faction;

                properties.Owner = "Player1";
                cardCopy.tag = "Carta";

                //Se activa el componente de arrastre
                cardCopy.GetComponent<DragHandler>().enabled = true;
            }
            if(cards.SequenceEqual(HandOfPlayer(player2)))
            {
                //Se instancia en la mano
                GameObject cardCopy = MonoBehaviour.Instantiate(card, player2.Hand.transform);
                NewCard properties = cardCopy.GetComponent<NewCard>();

                //Se añaden los efectos porque se suelen perder al ser por referencia
                foreach(Effect effect in card.GetComponent<NewCard>().Effects)
                properties.Effects.Add((Effect)effect.Clone());

                //Se cambia el dueño y el tag, y la facción de ser necesario
                if(properties.Faction != "Neutral") properties.Faction = player2.Faction;

                properties.Owner = "Player2";
                cardCopy.tag = "Carta";

                //Se activa el componente de arrastre
                cardCopy.GetComponent<DragHandler>().enabled = true;
            }
            else if(cards.SequenceEqual(GraveyardOfPlayer(player1)))
            {
                //Se instancia en el cementerio
                GameObject cardCopy = MonoBehaviour.Instantiate(card, player1.Graveyard.transform);
                NewCard properties = cardCopy.GetComponent<NewCard>();

                //Se añaden los efectos porque se suelen perder al ser por referencia
                foreach(Effect effect in card.GetComponent<NewCard>().Effects)
                properties.Effects.Add((Effect)effect.Clone());

                //Se cambia el dueño y el tag, y la facción de ser necesario
                if(properties.Faction != "Neutral") properties.Faction = player1.Faction;

                properties.Owner = "Player1";
                cardCopy.tag = "CartaDescartada1";

                //Se activa el componente de arrastre
                cardCopy.GetComponent<DragHandler>().enabled = false;
            }
            else if(cards.SequenceEqual(GraveyardOfPlayer(player2)))
            {
                //Se instancia en el cementerio
                GameObject cardCopy = MonoBehaviour.Instantiate(card, player2.Graveyard.transform);
                Card properties = cardCopy.GetComponent<Card>();

                //Se añaden los efectos porque se suelen perder al ser por referencia
                if(properties is NewCard properties2)
                foreach(Effect effect in card.GetComponent<NewCard>().Effects)
                properties2.Effects.Add((Effect)effect.Clone());

                //Se cambia el dueño y el tag, y la facción de ser necesario
                if(properties.Faction != "Neutral") properties.Faction = player2.Faction;

                properties.Owner = "Player2";
                cardCopy.tag = "CartaDescartada2";

                //Se activa el componente de arrastre
                cardCopy.GetComponent<DragHandler>().enabled = false;
            }
            else if(cards.SequenceEqual(DeckOfPlayer(player1)))
            {
                card = MonoBehaviour.Instantiate(card, GameObject.Find("NewDeck").transform);
                Card properties = card.GetComponent<Card>();

                //Se cambia el dueño y el tag, y la facción de ser necesario
                if(properties.Faction != "Neutral") properties.Faction = player1.Faction;

                properties.Owner = "Player1";
                card.tag = "Carta";
            }
            else if(cards.SequenceEqual(DeckOfPlayer(player2)))
            {
                GameObject cardCopy = MonoBehaviour.Instantiate(card, GameObject.Find("NewDeck2").transform);
                NewCard properties = cardCopy.GetComponent<NewCard>();

                //Se añaden los efectos porque se suelen perder al ser por referencia
                foreach(Effect effect in card.GetComponent<NewCard>().Effects)
                properties.Effects.Add((Effect)effect.Clone());

                //Se cambia el dueño y el tag, y la facción de ser necesario
                if(properties.Faction != "Neutral") properties.Faction = player2.Faction;

                properties.Owner = "Player2";
                cardCopy.tag = "Carta";

                card = cardCopy;
            }
            else if(cards.SequenceEqual(Board) || cards.SequenceEqual(FieldOfPlayer(player1)) || cards.SequenceEqual(FieldOfPlayer(player2)))
            Debug.Log("Al tablero y campos solo se le pueden colocar cartas manualmente");

            //Finalmente se le añade la carta a la lista
            if(function != "SendBottom") cards.Add(card);
            else cards.Insert(0, card);
        }
        else if(CurrentKeyWord == "Remove") // En caso del "Remove"
        {
            Next();

            GameObject card = (GameObject)Parameter();
            
            if(cards.SequenceEqual(Board) || cards.SequenceEqual(FieldOfPlayer(player1)) || cards.SequenceEqual(FieldOfPlayer(player2)) 
            || cards.SequenceEqual(GraveyardOfPlayer(player1)) || cards.SequenceEqual(GraveyardOfPlayer(player2))
            || cards.SequenceEqual(HandOfPlayer(player1)) || cards.SequenceEqual(HandOfPlayer(player2)))
            {
                card.transform.position = GameObject.Find("DeleteCards").transform.position;
                card.transform.SetParent(GameObject.Find("DeleteCards").transform);
                card.tag = "Carta";
            }

            cards.Remove(card);
        }
        else if(CurrentKeyWord == "Shuffle") // En caso del "Shuffle
        {
            Next();

            //Utilizamos el algoritmo de Fisher-Yates para barajear la lista
            System.Random random = new();

            int n = cards.Count;

            while(n > 1)
            {
                n--;
                int k = random.Next(n+1);
                (cards[n], cards[k]) = (cards[k], cards[n]);
            }
        }
        else if(CurrentKeyWord == "Find") // En caso del "Find"
        {
            Next();
            Variable variable = SetVariableValue(CurrentKeyWord);
            Next();

            // Se guarda la instrucción actual
            Instruction startInstruction = CurrentInstruction;
            int startInstructionIndex = CurrentInstructionIndex;
            string startKeyWord = CurrentKeyWord;
            int startKeyWordIndex = CurrentKeyWordIndex; 
            

            List<GameObject> cards2 = new();
            foreach(GameObject card in cards)
            cards2.Add(card);

            // Se descartan las cartas que no cumplan con el predicado
            foreach(GameObject card in cards)
            {
                CurrentInstruction = startInstruction;
                CurrentInstructionIndex = startInstructionIndex;
                CurrentKeyWord = startKeyWord;
                CurrentKeyWordIndex = startKeyWordIndex;

                variable.Value = card;
                    
                bool casa = BooleanExpression();

                if(!casa) cards2.Remove(card);
            }
                
            cards = cards2;
            CardList(cards);
        }
    }

    //--------------------------------------Booleanos-----------------------------------------------------------------------------------
    bool BooleanExpression()
        {
            //Resultado que se mostrará
            var result = ParseOrExpression();
            
            return result;
        }

        //Para parsear el "or"
        bool ParseOrExpression()
        {
            var result = ParseAndExpression();

            while (CurrentKeyWord == "||")
            {
                Next();
                var result2 = ParseAndExpression();
                result = result || result2;
            }
            return result;
        }

    //Para parsear el "and"
    bool ParseAndExpression()
    {
        var result = ParseRelationalExpression();

        while (CurrentKeyWord == "&&")
        {
            Next();
            var result2 = ParseRelationalExpression();
            result = result && result2;
        }
        return result;
    }

    //Para parsear operadores de relaciones
    bool ParseRelationalExpression()
    {
        var left = ParsePrimaryExpression();
       
        while (CurrentKeyWord == "<" || CurrentKeyWord == ">" || CurrentKeyWord == "<=" 
            || CurrentKeyWord == ">=" || CurrentKeyWord == "==" || CurrentKeyWord == "!=")
        {
            if (CurrentKeyWord == "<")
            {
                Next();
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue < rightValue;
                }
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentKeyWord == ">")
            {
                Next();

                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue > rightValue;
                } 
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentKeyWord == "<=")
            {
                Next();
               
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue <= rightValue;
                } 
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentKeyWord == ">=")
            {
                Next();
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue >= rightValue;
                } 
                else throw new Exception("This operation only can be use on int and bool");
            }
            else if (CurrentKeyWord == "==")
            {
                Next();
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue == rightValue;
                } 
                else if(left is string value)
                {
                    string rightValue = StringExpression();
                    left = value == rightValue;
                }
            }
            else if (CurrentKeyWord == "!=")
            {
                Next();
                
                if(left is int or bool)
                {
                    int leftValue = Convert.ToInt32(left);
                    int rightValue = Convert.ToInt32(ParsePrimaryExpression());
                    left = leftValue != rightValue;
                } 
                else if(left is string value)
                {
                    string rightValue = StringExpression();
                    left = value != rightValue;
                }
            }
        }
        return (bool)left;
    }

    //Para parsear booleanos, números y paréntesis
    object ParsePrimaryExpression()
    {
        if (CurrentKeyWord == "true")
        {
            Next();
            return true;
        }
        else if (CurrentKeyWord == "false")
        {
            Next();
            return false;
        }
        else if (int.TryParse(CurrentKeyWord, out int value))
        {
            Next();

            return value;
        }
        else if(CurrentKeyWord == "++")
        {
            Next();

            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord)
                {
                    Next();
                    int numberTemp = (int)variable.Value;
                    numberTemp++;
                    variable.Value = numberTemp;
                    return numberTemp;
                }
            }
            
            throw new Exception();
        }
        else if (CurrentKeyWord == "(")
        {
            Next();
            bool result = ParseOrExpression();
            Next();
            return result;
        }
        else if(char.IsLetter(CurrentKeyWord[0]))
        {
            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord && variable.Value is int value2)
                {
                    Next();

                    if(CurrentKeyWord == "++")
                    {       
                        Next();
                        int numberTemp = (int)variable.Value;
                        numberTemp++;
                        value2 = (int)variable.Value;
                        variable.Value = numberTemp;
                     }
                    
                    return value2;
                }
                else if(variable.Name == CurrentKeyWord && variable.Value is bool value3)
                {
                    Next();
                    return value3;
                }
                else if(variable.Name == CurrentKeyWord && variable.Value is string value4) return StringExpression();
                else if(variable.Name == CurrentKeyWord && variable.Type == "Card" 
                && (CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Power"))
                {
                    Next();
                    Next();

                    GameObject card = (GameObject)variable.Value;

                    return card.GetComponent<Card>().Power;
                }
                else if(variable.Name == CurrentKeyWord && variable.Type == "Card" 
                && (CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Name" 
                || CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Type"
                || CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Range" 
                || CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Faction"))
                return StringExpression();    
            }

            throw new Exception("Is not a correct variable");
        }
        else if(CurrentKeyWord == "\"") return StringExpression();
        else throw new Exception("Invalid boolean expression");
    }

    //----------------------------------------------------Números------------------------------------------------------------------------
    //Método para procesar números
    private int Factor()
    {
        string keyWord = CurrentKeyWord;

        /*Si el keyWord es correcto pasa al siguiente
         y devuelve el valor del número o expresión entre paréntesis*/
        if(int.TryParse(CurrentKeyWord, out int value))
        {
            int result = value;

            Next();

            if(CurrentKeyWord == "^")
            {
                Next();

                result ^= Factor(); 
            }

            keyWord = CurrentKeyWord;

            while(keyWord == "(")
            {
                Next();
                result *= NumericExpression();
                Next();

                keyWord = CurrentKeyWord;
            }
            return result;
        }
        else if(keyWord == "(")
        {
            Next();
            int result = NumericExpression();
            Next();

            if(CurrentKeyWord == "^")
            {
                Next();

                result ^= Factor(); 
            }

            keyWord = CurrentKeyWord;

            while(keyWord == "(")
            {
                Next();
                result *= NumericExpression();
                Next();

                keyWord = CurrentKeyWord;
            }
            return result;
        }
        else if(char.IsLetter(CurrentKeyWord[0]))
        {
            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord && variable.Value is int value2)
                {
                    int result = value2;
                        
                    Next();

                    if(CurrentKeyWord == "++")
                    {
                        int temp = (int)variable.Value;
                        temp++;
                        variable.Value = temp;
                        Next();
                    }

                    if(CurrentKeyWord == "^")
                    {
                        Next();

                        result ^= Factor(); 
                    }

                    keyWord = CurrentKeyWord;

                    while(keyWord == "(")
                    {
                        Next();
                        result *= NumericExpression();
                        Next();

                        keyWord = CurrentKeyWord;
                    }
                    return result;
                }  
                else if(variable.Name == CurrentKeyWord && variable.Type == "Card" && CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Power")
                {
                    GameObject card = (GameObject)variable.Value;
                    
                    int result = card.GetComponent<Card>().Power;

                    Next();
                    Next();

                    if(CurrentKeyWord == "++")
                    {
                        if(card.GetComponent<Card>().Type == "Silver" || card.GetComponent<Card>().Type == "Gold") 
                        card.GetComponent<Card>().Power++;
                        else Debug.Log("No se puede modificar el valor del poder de cartas especiales");
                        
                        Next();
                    }


                    if(CurrentKeyWord == "^")
                    {
                        Next();

                        result ^= Factor(); 
                    }

                    keyWord = CurrentKeyWord;

                    while(keyWord == "(")
                    {
                        Next();
                        result *= NumericExpression();
                        Next();

                        keyWord = CurrentKeyWord;
                    }
                    return result;
                } 
            }
            throw new Exception("This word is not a variable with a \"int value\"");
        }
        else if(CurrentKeyWord == "++")
        {
            Next();

            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord && variable.Value is int value2)
                {
                    value2++;
                    variable.Value = value2;

                    int result = value2;   

                    Next();

                    if(CurrentKeyWord == "^")
                    {
                        Next();

                        result ^= Factor(); 
                    }

                    keyWord = CurrentKeyWord;

                    while(keyWord == "(")
                    {
                        Next();
                        result *= NumericExpression();
                        Next();

                        keyWord = CurrentKeyWord;
                    }
                    return result;
                }   
            }
            throw new Exception("This word is not a variable with a \"int value\"");
        }
        else throw new Exception($"Unexpected keyWord: {keyWord}");
    }

    //Método para procesar multiplicaciones y divisiones
    private int Term()
    {
        int result = Factor();

        /*Mientras que el keyWord actual sea una multiplicación o división
          se avanza al siguiente y se realiza la operación en cuestión*/
        while(CurrentKeyWord == "*" || CurrentKeyWord == "/")
        {
            string keyWord = CurrentKeyWord;

            if(keyWord == "*")
            {
                Next();
                result *= Factor();
            }
            else if(keyWord == "/")
            {
                Next();
                result /= Factor();
            }
        }
        return result;
    }

    //Método para procesar sumas y restas
    private int NumericExpression()
    {
        int result = Term();

        /*Mientras que el keyWord actual sea una suma o resta
          se avanza al siguiente y se realiza la operación en cuestión*/
        while(CurrentKeyWord == "+" || CurrentKeyWord == "-")
        {
            string keyWord = CurrentKeyWord;

            if(keyWord == "+")
            {
                Next();
                result += Term();
            }
            else if(keyWord == "-")
            {
                Next();
                result -= Term();
            }
        }
        return result;
    }

    //-------------------------------------------String--------------------------------------------------------------------------------
      // Para procesar strings
      private string StringExpression()
      { 
        string result = null;
        
        if(CurrentKeyWord == "\"") // Si hay una comilla se recoge el token siguiente
        {
            Next();
            result = "";
            result += CurrentKeyWord;
            Next();
            Next();
        }
        else if(char.IsLetter(CurrentKeyWord[0])) // En caso de ser una palabra
        {
            bool IsCorrect = false;

            foreach(Variable variable in Variables)
            {   
                // Se comprueba si es una variable de tipo string y se recoge su valor
                if(variable.Name == CurrentKeyWord && variable.Value is string value)
                {
                    result = "";
                    result += value;
                    Next();
                    IsCorrect = true;
                }
                // Si no lo es se verifica si es de tipo "Card" y si le sigue una propiedad de tipo string y se recoge también su valor
                else if(variable.Name == CurrentKeyWord && variable.Type == "Card" 
                        && (CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Name"
                        || CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Type" 
                        || CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Range" 
                        || CurrentInstruction.KeyWords[CurrentKeyWordIndex + 1] == "Faction"))
                {
                    GameObject card = (GameObject)variable.Value;
                    
                    Next();
                    
                    if(CurrentKeyWord == "Name") result = card.GetComponent<Card>().Name;
                    else if(CurrentKeyWord == "Type") result = card.GetComponent<Card>().Type;
                    else if(CurrentKeyWord == "Faction") result = card.GetComponent<Card>().Faction;
                    else if(CurrentKeyWord == "Range")
                    {
                        result = "";

                        if(card.GetComponent<Card>().Range1 != "null") result += card.GetComponent<Card>().Range1;
                        if(card.GetComponent<Card>().Range2 != "null") result += ", " + card.GetComponent<Card>().Range2;
                        if(card.GetComponent<Card>().Range3 != "null") result += ", " + card.GetComponent<Card>().Range3;
                   } 
                    
                    Next();
                    IsCorrect = true;
                }
            }
            
            if(!IsCorrect) throw new Exception("This word is not a correct variable");
        }

        // Finalmente se verifica si está concatenada con otros string
        if(CurrentKeyWord == "@")
        {
            Next();
            result += StringExpression(); 
        }
        else if(CurrentKeyWord == "@@")
        {
            Next();
            result += " " + StringExpression(); 
        }

        return result;
      }
}