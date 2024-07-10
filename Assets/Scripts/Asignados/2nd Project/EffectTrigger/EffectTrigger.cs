using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    private Effect CurrentEffect;
    private LinkedList<Instruction> Instructions;


    public void Active(List<Effect> effects)
    {
        foreach(Effect effect in effects)
        {
            CurrentEffect = effect;
            Instructions = CurrentEffect.Instructions;
            CompleteInstructions();
        }
    }

    private void CompleteInstructions()
    {
        throw new NotImplementedException();
    }
}
