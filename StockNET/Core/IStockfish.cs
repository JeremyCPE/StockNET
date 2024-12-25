using StockNET.Models;

namespace StockNET.Core
{
    public interface IStockfish
    {
        void SetPosition(params string[] move);
        string GetBoardVisual();
        string GetFenPosition();
        void SetFenPosition(string fenPosition);
        string GetNextBestMove();
        Evaluation GetEvaluation(int depth);
    }
}