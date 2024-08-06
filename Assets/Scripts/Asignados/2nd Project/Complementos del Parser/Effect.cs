using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    //Instrucción actual
    private Instruction CurrentInstruction;
    private int CurrentInstructionIndex;

    //KeyWord actual
    private string CurrentKeyWord;
    private int CurrentKeyWordIndex;

    //
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
            PostEffect = PostEffect,
            Predicate = (Instruction)Predicate.Clone()
        };

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
        Source();
        FinishInstruction();
    }

    private void FinishInstruction()
    {
        Debug.Log("Instruccion iniciada");

        CurrentInstruction.Debug();

        // Si el segundo string es un "igual" entonces es una declaración
        if(CurrentInstruction.KeyWords[1] == "=") 
        {
            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord && variable.Value is int)
                {
                    Next();
                    Next();
                    variable.Value = Expr();
                    break;
                }
                else if(variable.Name == CurrentKeyWord && variable.Value is bool)
                {
                    Next();
                    Next();
                    variable.Value = BooleanExpression();
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
        else if(CurrentKeyWord == "++" || IsNumericVariable())
        {
            Increase();
        }
        else //De lo contrario es una llamada a un método
        {
            MethodCall();
        }

        if(CurrentInstruction.Count != 0)
        {
            FinishInstruction();
        }
        else PostEffect?.Activate();
    }

    private void Source()
    {
        foreach(Variable variable in Variables)
        {
            if(variable.Name == TargetsName)
            {
                if(SourceTargets == "board") variable.Value = Board;
                else if(SourceTargets == "hand") variable.Value = HandOfPlayer(TriggerPlayer);
                else if(SourceTargets == "otherHand") variable.Value = HandOfPlayer(OtherPlayer);
                else if(SourceTargets == "deck") variable.Value = DeckOfPlayer(TriggerPlayer);
                else if(SourceTargets == "otherDeck") variable.Value = DeckOfPlayer(OtherPlayer);
                else if(SourceTargets == "field") variable.Value = FieldOfPlayer(TriggerPlayer);
                else if(SourceTargets == "otherField") variable.Value = FieldOfPlayer(OtherPlayer);
                else if(SourceTargets == "void") variable.Value = new List<GameObject>();

                if(Single) 
                {
                    List<GameObject> cards = (List<GameObject>)variable.Value;
                    List<GameObject> cards2 = new(){cards[0]};
                    variable.Value = cards2;
                }

                Predicate.Debug();
                CurrentInstruction = Predicate;
                CurrentKeyWord = CurrentInstruction.KeyWords[0]; 
                Variable variable2 = SetVariableValue(CurrentKeyWord);
                Next();

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
                }
                
                variable.Value = cards3;

                CurrentInstruction = Instructions[0];
                CurrentKeyWord = CurrentInstruction.KeyWords[0];
                CurrentInstructionIndex = 0;

                return;
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

    private object Declaration()
    {
        Next(); 
        Next();
        return Parameter();
    }

    private object Parameter()
    {
        if(VariableType() == "Context")
        {
            Next();
            
            if(CurrentKeyWordIndex == 0) return GameObject.Find("Tablero").GetComponent<Context>();
            else if(CurrentKeyWord == "TriggerPlayer")
            {
                Next();
                return TriggerPlayer;
            }
            else
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

                while(CurrentKeyWord == "Pop" || CurrentKeyWord == "Find" || CurrentKeyWordIndex == 0)
                {
                    if(CurrentKeyWordIndex == 0) return cards;
                    else if(CurrentKeyWord == "Pop")
                    {
                        Next();
                        GameObject card = cards.Last();
                        cards.Remove(card);
                        return card;
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

                        if(CurrentKeyWord != "Pop" && CurrentKeyWord != "Find") return cards;
                    }      
                    else throw new Exception();    
                }
                throw new Exception();
            }
        }
        else if(VariableType() == "List<Card>")
        {
            string nameVariable = CurrentKeyWord;

            List<GameObject> cards = (List<GameObject>)SetVariableValue(CurrentKeyWord).Value;

            Next();

            while(CurrentKeyWord == "Pop" || CurrentKeyWord == "Find" || CurrentKeyWordIndex == 0)
            {
                if(CurrentKeyWordIndex == 0) return SetVariableValue(nameVariable).Value;
                else if(CurrentKeyWord == "Pop")
                {
                    Next();
                    GameObject card = cards.Last();
                    cards.Remove(card);
                    return card;
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

                    if(CurrentKeyWord != "Pop" && CurrentKeyWord != "Find") return cards;
                } 
                else throw new Exception();  
            }
            throw new Exception(); 
        }
        else if(VariableType() == "Card")
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
            
            throw new NotImplementedException();
        }
        else if(VariableType() == "Player")
        {
            string nameVariable = CurrentKeyWord;

            Next();
            
            return SetVariableValue(nameVariable).Value;
        } 
        else throw new Exception();
    }

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

    private Variable SetVariableValue(string name)
    {
        foreach(Variable variable in Variables)
        if(variable.Name == name) return variable;

        throw new Exception();
    }

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

    public bool IsNumericVariable()
    {
        foreach(Variable variable in Variables)
        if(variable.Name == CurrentKeyWord && variable.Value is int)
        return true;
        
        return false;
    }

    private void Increase()
    {
        if(CurrentKeyWord == "++") Next();

        foreach(Variable variable in Variables)
        {
            if(variable.Name == CurrentKeyWord)
            {
                int temp = (int)variable.Value;
                temp++;
                variable.Value = temp;

                Next();
                if(CurrentKeyWord == "++") Next();
                return;
            } 
        } 
        throw new Exception();
    }

    private void ForIntruction()
    {
        Next();
        string nameCard = CurrentKeyWord;
        Next();
        List<GameObject> cards = (List<GameObject>)SetVariableValue(CurrentKeyWord).Value;
        Next();

        Instruction startInstruction = CurrentInstruction;
        int startInstructionIndex = CurrentInstructionIndex;

        int count = cards.Count - 1;

        foreach(GameObject card in cards)
        {
            Variable variable = SetVariableValue(nameCard);
            variable.Value = card;

            FinishFor(count--);
        }

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
                    if(variable.Name == CurrentKeyWord && variable.Value is int)
                    {
                        Next();
                        Next();  
                        variable.Value = Expr();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord && variable.Value is bool)
                    {
                        Next();
                        Next();
                        variable.Value = BooleanExpression();
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
             else if(CurrentKeyWord == "++" || IsNumericVariable())
            {
                Increase();
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
    
    private void WhileInstruction()
    {
        Next();

        Instruction startInstruction = CurrentInstruction;
        int startInstructionIndex = CurrentInstructionIndex;

        bool argument = BooleanExpressionWhile();

        if(!argument)
        {
            while(CurrentKeyWord != "WhileFinal")
            {
                Next();
            }
            Next();
        }  

        while(argument)
        {
            FinishWhile();

            if(!BooleanExpressionWhile())
            {
                argument = false;

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
                    if(variable.Name == CurrentKeyWord && variable.Value is int)
                    {
                        Next();
                        Next();
                        variable.Value = Expr();
                        break;
                    }
                    else if(variable.Name == CurrentKeyWord && variable.Value is bool)
                    {
                        Next();
                        Next();
                        variable.Value = BooleanExpression();
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
            else if(CurrentKeyWord == "++" || IsNumericVariable())
            {
                Increase();
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

        bool BooleanExpressionWhile()
        {
            CurrentInstruction = startInstruction;
            CurrentInstructionIndex = startInstructionIndex;
            CurrentKeyWord = startInstruction.KeyWords[1];
            CurrentKeyWordIndex = 1;

            return BooleanExpression();
        }
    }

    private void Context()
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
        else if(CurrentKeyWord == "Deck")
        {
            Next();
            listReturn = DeckOfPlayer;
            player = TriggerPlayer;
            cards = listReturn(player);
        }
        else throw new Exception();
        
        CardList(cards);
    }

    private void CardList(List<GameObject> cards)
    {
        if(CurrentKeyWord == "Push" || CurrentKeyWord == "Add")
        {
            Next();
            cards.Add((GameObject)Parameter());
        }
        else if(CurrentKeyWord == "SendBottom")
        {
            Next();
            cards.Insert(0, (GameObject)Parameter());
        }
        else if(CurrentKeyWord == "Remove")
        {
            Next();
            cards.Remove((GameObject)Parameter());
        }
        else if(CurrentKeyWord == "Shuffle")
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
            int leftValue = Convert.ToInt32(left);
            int rightValue;
           
            if (CurrentKeyWord == "<")
            {
                Next();
                rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue < rightValue;
            }
            else if (CurrentKeyWord == ">" )
            {
                Next();
                rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue > rightValue;
            }
            else if (CurrentKeyWord == "<=")
            {
                Next();
                rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue <= rightValue;
            }
            else if (CurrentKeyWord == ">=")
            {
                Next();
                rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue >= rightValue;
            }
            else if (CurrentKeyWord == "==")
            {
                Next();
                rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue == rightValue;
            }
            else if (CurrentKeyWord == "!=")
            {
                Next();
                rightValue = Convert.ToInt32(ParsePrimaryExpression());
                left = leftValue != rightValue;
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
            }

            throw new Exception("Is not a correct variable");
        }
        else
        {
            throw new Exception("Invalid boolean expression");
        }
    }

    //----------------------------------------------------Numéricos------------------------------------------------------------------------
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
                result *= Expr();
                Next();

                keyWord = CurrentKeyWord;
            }
            return result;
        }
        else if(keyWord == "(")
        {
            Next();
            int result = Expr();
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
                result *= Expr();
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
                        result *= Expr();
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
                        result *= Expr();
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
    private int Expr()
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
}