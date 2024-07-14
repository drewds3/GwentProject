using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Context;

//Efecto de las cartas
public class Effect : ICloneable
{
    //Nombre del efecto
    public string Name {get;}

    //Parámetros de la carta (si es que tiene)
    public LinkedList<Variable> Params;

    //
    private string TargetsName;

    //Targets que se buscaran
    public string SourceTargets {get;set;}

    //Single
    public bool Single {get;set;}

    //Variables
    private LinkedList<Variable> Variables;

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
            PostEffect = PostEffect
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

        //Si el segundo string es un "igual" entonces es una declaración
        if(CurrentInstruction.KeyWords[1] == "=")
        {
            foreach(Variable variable in Variables)
            {
                if(variable.Name == CurrentKeyWord)
                {
                    variable.Value = Declaration();
                    break;
                }
            }
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
                    player = PlayerParam();
                    cards = listReturn(player);
                } 
                else if(CurrentKeyWord == "FieldOfPlayer")
                {
                    Next();
                    listReturn = FieldOfPlayer;
                    player = PlayerParam();
                    cards = listReturn(player);
                } 
                else if(CurrentKeyWord == "GraveyardOfPlayer")
                {
                    Next();
                    listReturn = GraveyardOfPlayer;
                    player = PlayerParam();
                    cards = listReturn(player);
                }
                else if(CurrentKeyWord == "DeckOfPlayer")
                {
                    Next();
                    listReturn = DeckOfPlayer;
                    player = PlayerParam();
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

                if(CurrentKeyWordIndex == 0) return cards;
                else if(CurrentKeyWord == "Pop")
                {
                    Next();
                    GameObject card = cards.Last();
                    cards.Remove(card);
                    return card;
                }
                else //if(CurrentKeyWord == "Find")
                {
                    throw new NotImplementedException();
                }          
            }
        }
        else if(VariableType() == "List<Card>")
        {
            string nameVariable = CurrentKeyWord;

            Next();

            if(CurrentKeyWordIndex == 0) return SetVariableValue(nameVariable).Value;
            else if(CurrentKeyWord == "Pop")
            {
                Next();
                List<GameObject> cards = (List<GameObject>)SetVariableValue(nameVariable).Value;
                GameObject card = cards.Last();
                cards.Remove(card);
                return card;
            }
            else //if(CurrentKeyWord == "Find")
            {
                throw new NotImplementedException();
            }   
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

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
        else // if(VariableType() == "Player")
        {
            string nameVariable = CurrentKeyWord;

            Next();
            
            return SetVariableValue(nameVariable).Value;
        } 
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

    private Player PlayerParam()
    {
        foreach(Variable variable in Variables)
        {
            if(variable.Name == CurrentKeyWord && variable.Type == "Context")
            {
                Next();
                Next();
                return TriggerPlayer;
            }
            else if(variable.Name == CurrentKeyWord && variable.Type == "Player")
            {
                Next();
                return (Player)variable.Value;                    
            }
        }

        throw new Exception();
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
            player = PlayerParam();
            cards = listReturn(player);
        } 
        else if(CurrentKeyWord == "FieldOfPlayer")
        {
            Next();
            listReturn = FieldOfPlayer;
            player = PlayerParam();
            cards = listReturn(player);
        } 
        else if(CurrentKeyWord == "GraveyardOfPlayer")
        {
            Next();
            listReturn = GraveyardOfPlayer;
            player = PlayerParam();
            cards = listReturn(player);
        }
        else if(CurrentKeyWord == "DeckOfPlayer")
        {
            Next();
            listReturn = DeckOfPlayer;
            player = PlayerParam();
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
            cards.Insert(0, (GameObject)Declaration());
        }
        else if(CurrentKeyWord == "Remove")
        {
            Next();
            cards.Remove((GameObject)Declaration());
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
    }
}