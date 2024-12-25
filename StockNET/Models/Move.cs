namespace StockNET.Models
{
    public interface IMove
    {
        string? TopNextMoves { get; }

        double Centipawn { get; }
    }
    public class CentipawnMove : IMove
    {
        public string? TopNextMoves { get; }
        public double Centipawn { get; }
        public CentipawnMove(string moves, double centipawn)
        {
            TopNextMoves = moves;
            Centipawn = centipawn;
        }
    }

    public class MateMove : IMove
    {
        public string? TopNextMoves { get; }

        public double Centipawn { get; } = 100;
        public int MateInMoves { get; }
        public MateMove(string moves, int mateInMoves)
        {
            TopNextMoves = moves;
            MateInMoves = mateInMoves;
        }
    }

    public class DrawMove : IMove
    {
        public double Centipawn { get; } = 0;

        public DrawType DrawType { get; }
        public string? TopNextMoves { get; }
        public DrawMove(DrawType drawType = DrawType.Stalemate)
        {
            DrawType = drawType;
        }

        public DrawMove(string moves, DrawType drawType = DrawType.ThreefoldRepetition)
        {
            TopNextMoves = moves;
            DrawType = drawType;
        }
    }

    public enum DrawType
    {
        Stalemate,
        InsufficientMaterial,
        FiftyMoveRule,
        ThreefoldRepetition
    }
}
