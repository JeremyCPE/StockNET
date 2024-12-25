namespace StockNET.Models
{

    public class Evaluation
    {
        public double Score { get; private set; }
        public List<IMove> Moves { get; } = new List<IMove>();

        public void AddMove(IMove move)
        {
            Moves.Add(move);
            Score = BestMove.Centipawn; // Get the highest centipawn value
        }

        public IMove BestMove => Moves.OrderByDescending(x => x.Centipawn).First();
    }
}