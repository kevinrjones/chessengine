namespace Game
{
    public static class Lookups
    {
        public const int BoardSize = 120;
        public const int OffBoard = 100;


        public static int[] FilesBoard = new int[BoardSize];
        public static int[] RanksBoard = new int[BoardSize];

        static Lookups()
        {
            ResetLookups();
        }

        public static void ResetLookups()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                FilesBoard[i] = OffBoard;
                RanksBoard[i] = OffBoard;
            }

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    int sq = Board.FileRankToSquare(file, rank);
                    FilesBoard[sq] = file;
                    RanksBoard[sq] = rank;
                }
            }
        }
    }
}