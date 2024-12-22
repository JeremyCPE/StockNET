using StockNET.Exceptions;
using StockNET.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StockNET.Core
{
    public class Stockfish : IStockfish
    {
        #region private variables

        /// <summary>
        /// 
        /// </summary>
        private const int MAX_TRIES = 200;

        /// <summary>
        /// 
        /// </summary>
        private int _skillLevel;

        #endregion

        # region private properties

        /// <summary>
        /// 
        /// </summary>
        private StockfishProcess _stockfish { get; set; }

        #endregion

        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SkillLevel
        {
            get => _skillLevel;
            set
            {
                _skillLevel = value;
                Settings.SkillLevel = SkillLevel;
                SetOption("Skill level", SkillLevel.ToString());
            }
        }

        #endregion

        # region constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="depth"></param>
        /// <param name="settings"></param>
        public Stockfish(
            string path,
            int depth = 2,
            Settings settings = null)
        {
            Depth = depth;
            _stockfish = new StockfishProcess(path);
            _stockfish.Start();
            _stockfish.ReadLine();

            if (settings == null)
            {
                Settings = new Settings();
            }
            else
            {
                Settings = settings;
            }

            SkillLevel = Settings.SkillLevel;
            foreach (KeyValuePair<string, string> property in Settings.GetPropertiesAsDictionary())
            {
                SetOption(property.Key, property.Value);
            }

            StartNewGame();
        }

        #endregion

        #region private

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="estimatedTime"></param>
        private void Send(string command, int estimatedTime = 100)
        {
            _stockfish.WriteLine(command);
            _stockfish.Wait(estimatedTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
		private bool IsReady()
        {
            Send("isready");
            int tries = 0;
            while (tries < MAX_TRIES)
            {
                ++tries;

                if (_stockfish.ReadLine() == "readyok")
                {
                    return true;
                }
            }
            throw new MaxTriesException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ApplicationException"></exception>
        private void SetOption(string name, string value)
        {
            Send($"setoption name {name} value {value}");
            if (!IsReady())
            {
                throw new ApplicationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moves"></param>
        /// <returns></returns>
        private string movesToString(string[] moves)
        {
            return string.Join(" ", moves);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private void StartNewGame()
        {
            Send("ucinewgame");
            if (!IsReady())
            {
                throw new ApplicationException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Go()
        {
            Send($"go depth {Depth}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        private void GoTime(int time)
        {
            Send($"go movetime {time}", estimatedTime: time + 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<string> ReadLineAsList()
        {
            string data = _stockfish.ReadLine();
            return data.Split(' ').ToList();
        }

        #endregion

        #region public

        /// <summary>
        /// Setup current position
        /// </summary>
        /// <param name="moves"></param>
        public void SetPosition(params string[] moves)
        {
            StartNewGame();
            Send($"position startpos moves {movesToString(moves)}");
        }

        /// <summary>
        /// Get visualisation of current position
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetBoardVisual()
        {
            Send("d");
            string board = "";
            int lines = 0;
            int tries = 0;
            while (lines < 17)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                string data = _stockfish.ReadLine();
                if (data.Contains("+") || data.Contains("|"))
                {
                    lines++;
                    board += $"{data}\n";
                }

                tries++;
            }

            return board;
        }

        /// <summary>
        /// Get position in fen format
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetFenPosition()
        {
            Send("d");
            int tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                List<string> data = ReadLineAsList();
                if (data[0] == "Fen:")
                {
                    return string.Join(" ", data.GetRange(1, data.Count - 1));
                }

                tries++;
            }
        }

        /// <summary>
        /// Set position in fen format
        /// </summary>
        /// <param name="fenPosition"></param>
        public void SetFenPosition(string fenPosition)
        {
            StartNewGame();
            Send($"position fen {fenPosition}");
        }

        /// <summary>
        /// Getting best move of current position
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetBestMove()
        {
            Go();
            int tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                List<string> data = ReadLineAsList();

                if (data[0] == "bestmove")
                {
                    if (data[1] == "(none)")
                    {
                        return null;
                    }

                    return data[1];
                }

                tries++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public string GetBestMoveTime(int time = 1000)
        {
            GoTime(time);
            int tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                List<string> data = ReadLineAsList();
                if (data[0] == "bestmove")
                {
                    if (data[1] == "(none)")
                    {
                        return null;
                    }

                    return data[1];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveValue"></param>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public bool IsMoveCorrect(string moveValue)
        {
            Send($"go depth 1 searchmoves {moveValue}");
            int tries = 0;
            while (true)
            {
                if (tries > MAX_TRIES)
                {
                    throw new MaxTriesException();
                }

                List<string> data = ReadLineAsList();
                if (data[0] == "bestmove")
                {
                    if (data[1] == "(none)")
                    {
                        return false;
                    }

                    return true;
                }

                tries++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MaxTriesException"></exception>
        public double GetEvaluation()
        {
            Send($"eval");
            while (true)
            {
                string? data = _stockfish?.ReadLine();
                if (data == null)
                {
                    throw new MaxTriesException();
                }
                if (data.Contains("Final evaluation"))
                {
                    Regex regex = new(@"-?\d+(\.\d+)?");
                    Match match = regex.Match(data);

                    if (match.Success)
                    {
                        if (double.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
                        {
                            return number;
                        }
                    }
                    // Failed to parse number
                    return 0;
                }
            }
        }
        #endregion
    }
}
