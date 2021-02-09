namespace Chess.Core
{
    /// <summary>
    /// The enum to understand which stalemate the players got.
    /// </summary>
    public enum StalemateBy
    {
        FiftyMoveRule,
        InsuficientMaterial,
        NoValidMoves,
        Repetition,
    }
}
