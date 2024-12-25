using System.Diagnostics;

namespace StockNET.Core
{
    /// <summary>
    /// https://github.com/official-stockfish/Stockfish/wiki/UCI-&-Commands
    /// </summary>
    internal class StockfishProcess
    {
        /// <summary>
        /// Default process info for Stockfish process
        /// </summary>
        private ProcessStartInfo _processStartInfo { get; set; }

        /// <summary>
        /// Stockfish process
        /// </summary>
        private Process _process { get; set; }

        /// <summary>
        /// Stockfish process constructor
        /// </summary>
        /// <param name="path">Path to usable binary file from stockfish site</param>
        public StockfishProcess(string path)
        {
            //TODO: need add method which should be depended on os version
            _processStartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            _process = new Process { StartInfo = _processStartInfo };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecond"></param>
        public void Wait(int millisecond)
        {
            this._process.WaitForExit(millisecond);
        }

        /// <summary>
        /// This method is writing in stdin of Stockfish process
        /// </summary>
        /// <param name="command"></param>
        public void WriteLine(string command)
        {
            if (_process.StandardInput == null)
            {
                throw new NullReferenceException();
            }
            _process.StandardInput.WriteLine(command);
            _process.StandardInput.Flush();
        }

        /// <summary>
        /// This method is allowing to read stdout of Stockfish process
        /// </summary>
        /// <returns></returns>
        public string? ReadLine()
        {
            if (_process.StandardOutput == null)
            {
                throw new NullReferenceException();
            }
            return _process.StandardOutput.ReadLine();
        }

        /// <summary>
        /// Read all lines from Stockfish process until bestmove command
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public IEnumerable<string> ReadAllLines()
        {
            if (_process.StandardInput == null)
            {
                throw new NullReferenceException();
            }
            while (true)
            {
                string? line = _process.StandardOutput.ReadLine();
                if (line == null)
                {
                    yield break;
                }

                yield return line;

                if (line.Contains("bestmove", StringComparison.OrdinalIgnoreCase))
                {
                    yield return line;
                    yield break;
                }
            }
        }

        /// <summary>
        /// Start stock
        /// fish process
        /// </summary>
        public void Start()
        {
            _process.Start();
        }
        /// <summary>
        /// This method is allowing to close Stockfish process
        /// </summary>
        ~StockfishProcess()
        {
            //When process is going to be destructed => we are going to close stockfish process
            _process.Close();
        }
    }
}