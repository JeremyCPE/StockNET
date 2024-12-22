using StockfishNET.Tests;
using StockNET.Core;
using Xunit.Abstractions;

namespace StockNET.Tests
{
    public class TestStockfishMethods
    {
        private Stockfish Stockfish { get; set; }

        public TestStockfishMethods(ITestOutputHelper testOutputHelper)
        {
            string path = Utils.GetStockfishDir();
            Stockfish = new Core.Stockfish(path, depth: 2);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetBestMoveFirstMove()
        {
            string bestMove = Stockfish.GetBestMove();
            Assert.Contains(bestMove, new List<string>
            {
                "e2e4",
            });
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetBestMoveTimeFirstMove()
        {
            string bestMove = Stockfish.GetBestMoveTime();
            Assert.Contains(bestMove, new List<string>
            {
                "e2e4",
            });
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetBestMoveNotFirstMove()
        {
            Stockfish.SetPosition(
                "e2e4", "e7e6"
            );
            string bestMove = Stockfish.GetBestMove();
            Assert.Contains(bestMove, new List<string>
            {
                "g1f3"
            });
        }


        [Fact(Timeout = 2000)]
        public async Task TestGetBestMoveTimeNotFirstMove()
        {
            Stockfish.SetPosition(
                "e2e4", "e7e6"
            );
            string bestMove = Stockfish.GetBestMoveTime();
            Assert.Contains(bestMove, new List<string>
            {
                "g1f3"
            });
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetBestMoveMate()
        {
            Stockfish.SetPosition(
                "f2f3", "e7e5", "g2g4", "d8h4"
            );
            string bestMove = Stockfish.GetBestMove();
            Assert.Null(bestMove);
        }

        [Fact(Timeout = 2000)]
        public async Task TestSetFenPosition()
        {
            Stockfish.SetFenPosition("7r/1pr1kppb/2n1p2p/2NpP2P/5PP1/1P6/P6K/R1R2B2 w - - 1 27");
            bool move1 = Stockfish.IsMoveCorrect("f4f5");
            bool move2 = Stockfish.IsMoveCorrect("a1c1");
            Assert.True(move1);
            Assert.False(move2);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetBoardVisual()
        {
            Stockfish.SetPosition("e2e4", "e7e6", "d2d4", "d7d5");
            string expected = " +---+---+---+---+---+---+---+---+\n" +
                           " | r | n | b | q | k | b | n | r | 8\n" +
                           " +---+---+---+---+---+---+---+---+\n" +
                           " | p | p | p |   |   | p | p | p | 7\n" +
                           " +---+---+---+---+---+---+---+---+\n" +
                           " |   |   |   |   | p |   |   |   | 6\n" +
                           " +---+---+---+---+---+---+---+---+\n" +
                           " |   |   |   | p |   |   |   |   | 5\n" +
                           " +---+---+---+---+---+---+---+---+\n" +
                           " |   |   |   | P | P |   |   |   | 4\n" +
                           " +---+---+---+---+---+---+---+---+\n" +
                           " |   |   |   |   |   |   |   |   | 3\n" +
                           " +---+---+---+---+---+---+---+---+\n" +
                           " | P | P | P |   |   | P | P | P | 2\n" +
                           " +---+---+---+---+---+---+---+---+\n" +
                           " | R | N | B | Q | K | B | N | R | 1\n" +
                           " +---+---+---+---+---+---+---+---+\n";
            string board = Stockfish.GetBoardVisual();
            Assert.Equal(expected, board);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetFenPosition()
        {
            string defaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string fen = Stockfish.GetFenPosition();
            Assert.Equal(defaultFen, fen);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetFenPositionAfterMoves()
        {
            Stockfish.SetPosition(
                "e2e4", "e7e6"
            );
            string fen = "rnbqkbnr/pppp1ppp/4p3/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
            string fenPosition = Stockfish.GetFenPosition();
            Assert.Equal(fen, fenPosition);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetEvaluationMate()
        {
            string wrongFen = "6k1/p4p1p/6p1/5r2/3b4/6PP/4qP2/5RK1 b - - 14 36";
            Stockfish.SetFenPosition(wrongFen);
            double evaluation = Stockfish.GetEvaluation();
            Assert.Equal(-7.32, evaluation);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetEvaluationCP()
        {
            string wrongFen = "r4rk1/pppb1p1p/2nbpqp1/8/3P4/3QBN2/PPP1BPPP/R4RK1 w - - 0 11";
            Stockfish.SetFenPosition(wrongFen);
            double evaluation = Stockfish.GetEvaluation();
            Assert.Equal(0.90, evaluation);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetEvaluationStalemate()
        {
            string wrongFen = "1nb1kqn1/pppppppp/8/6r1/5b1K/6r1/8/8 w - - 2 2";
            Stockfish.SetFenPosition(wrongFen);
            double evalutation = Stockfish.GetEvaluation();
            Assert.Equal(-33.3, evalutation);
        }

        [Fact(Timeout = 2000)]
        public async Task TestGetBestMoveWrongPositon()
        {
            string wrongFen = "3kk3/8/8/8/8/8/8/3KK3 w - - 0 0";
            Stockfish.SetFenPosition(wrongFen);
            string bestMove = Stockfish.GetBestMove();
            Assert.Contains(bestMove, new List<string>
            {
                "d1e2",
                "d1c1"
            });

        }
    }
}