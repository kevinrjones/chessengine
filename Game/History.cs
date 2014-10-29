namespace Game
{
    public class History
    {
        public Move Move { get; set; }
        public CastlePermissions CastlePermissions { get; set; }
        public int EnPassantSquare { get; set; }
        public int FiftyMoveCount { get; set; }
        public int PositionKey { get; set; }
    }
}