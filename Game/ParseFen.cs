using System;
using System.Globalization;

namespace Game
{
    // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
    public class ParseFen
    {
        readonly string[] _sections;
        const int SideToMoveSection = 8;
        const int CastleSection = 9;
        const int EnPassantSection = 10;

        public ParseFen(string fen)
        {
            _sections = fen.Split('/', ' ');

            if (_sections.Length != 13)
            {
                throw new ArgumentException("fen");
            }
        }

        public Piece[] ParseRankAndFile()
        {
            var squares = CreateBoard();
            
            const int ranks = 8;
            for (int rank = 0; rank < ranks; rank++)
            {
                int file = 0;
                var section = _sections[rank];
                foreach (char pieceIdentifier in section)
                {
                    var countOfPieces = 1;

                    Piece piece;
                    if (pieceIdentifier >= '0' && pieceIdentifier <= '8')
                    {
                        int.TryParse(pieceIdentifier.ToString(CultureInfo.CurrentCulture), out countOfPieces);
                        piece = new EmptyPiece();
                    }
                    else
                    {
                        piece = Lookups.FenPieceLookup[pieceIdentifier];
                    }


                    for (var count = 0; count < countOfPieces; count++)
                    {
                        int boardNdx = Board.FileRankToSquare(file, rank);
                        squares[boardNdx] = piece;
                        file++;
                    }
                }
            }
            return squares;
        }

        public int ParseEnPassantSection()
        {
            string section = _sections[EnPassantSection];
            if (section == "-") return 0;

            if (section.Length != 2) throw new ArgumentException(section);

            int file = section[0] - 'a';
            int rank = section[1] - '1';

            return Board.FileRankToSquare(file, rank);
        }

        public CastlePermissions ParseCastleSection()
        {
            string section = _sections[CastleSection];
            CastlePermissions castlePermission = 0;
            foreach (var flag in section)
            {
                switch (flag)
                {
                    case 'K': castlePermission |= CastlePermissions.WhiteKing; break;
                    case 'k': castlePermission |= CastlePermissions.BlackKing; break;
                    case 'Q': castlePermission |= CastlePermissions.WhiteQueen; break;
                    case 'q': castlePermission |= CastlePermissions.BlackQueen; break;
                }
            }

            return castlePermission;
        }

        public Color SideToMove()
        {
            return _sections[SideToMoveSection] == "w" ? Color.White : Color.Black;
        }

        private Piece[] CreateBoard()
        {
            var squares = new Piece[120];
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i] = new OffBoardPiece();
            }

            for (int i = 0; i < 64; i++)
            {
                squares[Board.GameIndexToFullIndex(i)] = new EmptyPiece();
            }

            return squares;
        }
    }
}