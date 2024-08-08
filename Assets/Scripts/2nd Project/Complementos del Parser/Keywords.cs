public enum TokenType
{
    Number,
    Word,
    Keyword,
    Point,
    Colon,
    Semicolon,
    Comma,
    LParen,
    RParen,
    LCBracket,
    RCBracket,
    LSBracket,
    RSBracket,
    Plus,
    Minus,
    Multi,
    Division,
    Increase,
    QMark,
    String,
    Asign,
    Equal,
    NotEqual,
    Smaller,
    Greater,
    SmallerOrEqual,
    GreaterOrEqual,
    Or,
    And,
    XOR,
    At,
    DoubleAt,
    Fin
}

enum KeyWords
{
    effect,
    Name,
    Params,
    Action,
    TriggerPlayer,
    Board,
    HandOfPlayer,
    FieldOfPlayer,
    GraveyardOfPlayer,
    DeckOfPlayer,
    Hand,
    Field,
    Deck,
    Graveyard,
    Owner,
    Find,
    Push,
    SendBottom,
    Pop,
    Remove,
    Shuffle,
    Add,
    Number,
    String,
    Bool,
    card,
    Type,
    Faction,
    Power,
    Range,
    OnActivation,
    Effect,
    Selector,
    Source,
    Single,
    Predicate,
    PostAction
}

enum ContextPropertiesAndMethods
{
    TriggerPlayer,
    Board,
    HandOfPlayer,
    FieldOfPlayer,
    GraveyardOfPlayer,
    DeckOfPlayer,
    Hand,
    Field,
    Deck,
    Graveyard
}

enum CardProperties
{
    Owner,
    Power,
    Range,
    Faction,
    Type,
    Name
}

enum CardListProperties
{
    Find,
    Pop,
}

enum SyntacticSugarContext
{
    Board,
    Hand,
    Field,
    Deck,
    Graveyard
}

enum ContextMethods
{
    Board,
    HandOfPlayer,
    FieldOfPlayer,
    GraveyardOfPlayer,
    DeckOfPlayer,
    Hand,
    Field,
    Deck,
    Graveyard
}

enum CardListMethods
{
    Push,
    SendBottom,
    Remove,
    Shuffle,
    Add,
    Find
}

public enum Source
{
    hand,
    otherHand,
    deck,
    otherDeck,
    field,
    otherField,
    parent
}

public enum Booleans
{
    Equal,
    NotEqual,
    Smaller,
    Greater,
    SmallerOrEqual,
    GreaterOrEqual,
    Or,
    And
}