using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DX.Common
{
    class CommandManager
    {
        static CommandManager()
        {
            var fileName = "Command.csv";
            try
            {
                if (File.Exists(fileName))
                {
                    Commands = File.ReadAllLines(fileName)
                        .Where((c) => !string.IsNullOrWhiteSpace(c)).ToList();
                }
                else
                {
                    Commands = new List<string>();
                }
            }
            catch
            {
#if DEBUG
                throw;
#else
                MessageBox.Show($"{fileName} is wrong.");
                Commands = new List<string>();
#endif
            }
        }

        public static List<string> Commands { get; private set; }
    }
}
