namespace Chess.Core
{
    /// <summary>
    /// The enum to understand which draw the players got.
    /// </summary>
    public enum DrawBy
    {
        FiftyMoveRule,
        InsuficientMaterial,
        MutualAgreement,
        Repetition,
        Stalemate,
    }
}
