# StockNET

[![NuGet version](https://badge.fury.io/nu/StockNET.svg)](https://badge.fury.io/nu/StockNET)

StockNET is a .NET 9 library that leverages the power of Stockfish, the strongest open-source chess engine, to analyze chess positions and provide the best possible moves. This library simplifies the integration of Stockfish into your .NET applications.

---

## Features

- **Easy Stockfish Integration:** Interact with the Stockfish engine through a simple API.
- **Best Move Calculation:** Get the strongest move recommendation for any given position.
- **Multi-Platform Support:** Works seamlessly on Windows, macOS, and Linux.
- **.NET 9 Ready:** Leverages the latest features and performance improvements in .NET 9.
- **Extensible:** Easily customize and expand to suit your chess-related projects.

---

## Installation

Install the package via NuGet:

```bash
Install-Package StockNET
```

Or using the .NET CLI:

```bash
dotnet add package StockNET
```

---

## Getting Started

### Prerequisites

- .NET 9 SDK
- Stockfish executable (download it from [official Stockfish page](https://stockfishchess.org/download/))

### Basic Usage

```csharp
using StockNET;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize StockNET with the path to the Stockfish executable
        var stockfishPath = "path/to/stockfish.exe";
        using var stockNET = new StockNET(stockfishPath);

        // Start the Stockfish engine
        await stockNET.Start();

        // Provide the FEN string for the current board position
        string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        // Get the best move
        var bestMove = await stockNET.GetBestMove(fen);

        Console.WriteLine($"Best move: {bestMove}");

        // Stop the engine
        await stockNET.StopAsync();
    }
}
```

---

## API Reference

### Methods

#### `Task StartAsync()`
Starts the Stockfish engine.

#### `Task<string> GetBestMoveAsync(string fen)`
Gets the best move for the given board position (in FEN format).

- **Parameters:**
  - `fen`: The FEN string representing the board position.
- **Returns:**
  - `string`: The best move in UCI format.

#### `Task StopAsync()`
Stops the Stockfish engine.

---

## Contributing

Contributions are welcome! Feel free to submit issues or pull requests on the [GitHub repository](https://github.com/JeremyCPE/StockNET).

---

## License

StockNET is licensed under the [MIT License](LICENSE).

---

## Acknowledgements

- [Stockfish](https://stockfishchess.org/) for the incredible open-source chess engine.

---

## Contact

For questions or support, please reach out to [JeremyCPE](https://github.com/JeremyCPE).
