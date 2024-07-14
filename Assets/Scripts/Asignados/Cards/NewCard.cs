using System.Collections.Generic;

public class NewCard : Card
{
    public List<Effect> Effects = new();

    public int count;

    void Update()
    {
        count = Effects.Count;
    }
}
