namespace StockfishNET.Tests
{
    public class Utils
    {
        public static string GetStockfishDir()
        {
            System.Reflection.Assembly? assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .SingleOrDefault(a => a.GetName().Name == "StockNET");
            string? location = assembly?.Location;
            DirectoryInfo? dir = Directory.GetParent(Directory.GetParent(Directory
                .GetParent(Directory.GetParent(Directory.GetParent(location).ToString())
                    .ToString()).ToString()).ToString());
            Console.WriteLine(dir);
            string path = null;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                path = $@"{dir}\StockNET.Tests\Stockfish\win\stockfish_17_win_x64\stockfish-windows-x86-64.exe";
            }
            else
            {
                path = "/usr/games/stockfish";
            }
            return path;
        }
    }
}