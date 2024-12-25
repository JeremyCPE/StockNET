using StockfishNET.Tests;
using StockNET.Core;
using StockNET.Models;
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

        [Fact]
        public async Task TestGetBestMoveFirstMove()
        {
            string bestMove = Stockfish.GetNextBestMove();
            Assert.Contains(bestMove, new List<string>
            {
                "e2e4",
                "c2c3",
            });
        }


        [Fact]
        public async Task TestGetBestMoveNotFirstMove()
        {
            Stockfish.SetPosition(
                "e2e4", "e7e6"
            );
            string bestMove = Stockfish.GetNextBestMove();
            Assert.Contains(bestMove, new List<string>
            {
                "g1f3",
                "a2a3",
            });
        }



        [Fact]
        public async Task TestGetBestMoveMate()
        {
            Stockfish.SetPosition(
                "f2f3", "e7e5", "g2g4", "d8h4"
            );
            string bestMove = Stockfish.GetNextBestMove();
            Assert.Null(bestMove);
        }

        [Fact]
        public async Task TestSetFenPosition()
        {
            Stockfish.SetFenPosition("7r/1pr1kppb/2n1p2p/2NpP2P/5PP1/1P6/P6K/R1R2B2 w - - 1 27");
            // Check if the position is set correctly
            // TODO : Add tests
        }

        [Fact]
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

        [Fact]
        public async Task TestGetFenPosition()
        {
            string defaultFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string fen = Stockfish.GetFenPosition();
            Assert.Equal(defaultFen, fen);
        }

        [Fact]
        public async Task TestGetFenPositionAfterMoves()
        {
            Stockfish.SetPosition(
                "e2e4", "e7e6"
            );
            string fen = "rnbqkbnr/pppp1ppp/4p3/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2";
            string fenPosition = Stockfish.GetFenPosition();
            Assert.Equal(fen, fenPosition);
        }

        [Fact]
        public void TestGetEvaluationMate()
        {
            string wrongFen = "6k1/p4p1p/6p1/5r2/3b4/6PP/4qP2/5RK1 b - - 14 36";
            Stockfish.SetFenPosition(wrongFen);
            Evaluation evaluation = Stockfish.GetEvaluation();

            Assert.Equal(3, evaluation.Moves.Count);
            Assert.IsType<MateMove>(evaluation.Moves[0]);
            Assert.IsType<MateMove>(evaluation.Moves[1]);
            Assert.IsType<MateMove>(evaluation.Moves[2]);


            MateMove mateMove = (MateMove)evaluation.Moves[0];

            Assert.Equal(3, mateMove.MateInMoves);
            Assert.Equal("f5f2 f1f2 e2f2 g1h1 f2g1", mateMove.TopNextMoves);
            Assert.Equal(100, evaluation.Score);
        }

        [Fact]
        public void TestGetEvaluationCP()
        {
            string fen = "r4rk1/pppb1p1p/2nbpqp1/8/3P4/3QBN2/PPP1BPPP/R4RK1 w - - 0 11";
            Stockfish.SetFenPosition(fen);
            Evaluation evaluation = Stockfish.GetEvaluation();

            Assert.Equal(3, evaluation.Moves.Count);
            Assert.IsType<CentipawnMove>(evaluation.Moves[0]);
            Assert.IsType<CentipawnMove>(evaluation.Moves[1]);
            Assert.IsType<CentipawnMove>(evaluation.Moves[2]);

            CentipawnMove centipawnMove = (CentipawnMove)evaluation.Moves[0];

            Assert.Equal(57, centipawnMove.Centipawn);
            Assert.Equal(57, evaluation.Score);
        }

        [Fact]
        public async Task TestGetEvaluationStalemate()
        {
            string wrongFen = "1nb1kqn1/pppppppp/8/6r1/5b1K/6r1/8/8 w - - 2 2";
            Stockfish.SetFenPosition(wrongFen);

            Evaluation evaluation = Stockfish.GetEvaluation();


            Assert.Single(evaluation.Moves);
            Assert.IsType<DrawMove>(evaluation.Moves[0]);
            Assert.Equal(0, evaluation.Score);


            DrawMove drawMove = (DrawMove)evaluation.Moves[0];


            Assert.Null(drawMove.TopNextMoves);
            Assert.Equal(DrawType.Stalemate, drawMove.DrawType);
        }

        [Fact]
        public async Task TestGetBestMoveWrongPositon()
        {
            string wrongFen = "3kk3/8/8/8/8/8/8/3KK3 w - - 0 0";
            Stockfish.SetFenPosition(wrongFen);
            string bestMove = Stockfish.GetNextBestMove();
            Assert.Contains(bestMove, new List<string>
            {
                "d1e2",
                "d1c1",
                "d1c2",
            });

        }
    }
}