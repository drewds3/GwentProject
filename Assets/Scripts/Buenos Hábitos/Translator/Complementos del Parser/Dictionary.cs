using System.Collections.Generic;

public static class Dictionary
{
    static Dictionary<KeyWords, string> Methods = new()
    {
        { KeyWords.TriggerPlayer, "Player"},
        { KeyWords.Owner, "Player"},
        { KeyWords.Board, "List<Card>"},
        { KeyWords.HandOfPlayer, "List<Card>"},
        { KeyWords.FieldOfPlayer, "List<Card>"},
        { KeyWords.GraveyardOfPlayer, "List<Card>"},
        { KeyWords.DeckOfPlayer, "List<Card>"},
        { KeyWords.Hand, "List<Card>"},
        { KeyWords.Field, "List<Card>"},
        { KeyWords.Graveyard, "List<Card>"},
        { KeyWords.Deck, "List<Card>"},
        { KeyWords.Pop, "Card"},
        { KeyWords.Find, "List<Card>"},
    };
}
