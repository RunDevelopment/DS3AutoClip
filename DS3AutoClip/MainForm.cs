using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

namespace DS3AutoClip
{
    public partial class MainForm : Form
    {
        public GameProcess game = null;
        public readonly DS3GameValues DS3 = new DS3GameValues();

        private readonly List<string> logLines = new List<string>();
        private readonly List<string> pendingLogLines = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            string stateText;
            if (game == null)
            {
                stateText = "DS3 not running";
            }
            else if (DS3.IsLevelLoaded.Value)
            {
                stateText = "Gaming";
            }
            else if (DS3.IsTitleScreen.Value)
            {
                stateText = "Title screen";
            }
            else
            {
                stateText = "Loading";
            }


            stateLabel.Text = stateText;
        }

        private void mainTimer_Tick(object sender, EventArgs e)
        {
            if (game != null && !game.IsAlive)
                game = null;

            if (game == null)
            {
                var process = FindDS3Process();
                if (process != null)
                    game = new GameProcess(process);
            }

            if (game != null)
            {
                DS3.Update(game);

                Log($"Player Health: {DS3.PlayerHP.Value}");
                Log($"Player Char t: {DS3.PlayerCharacterType.Value}");
                Log($"Player Team t: {DS3.PlayerTeamType.Value}");
                Log($"Player hollowing: {DS3.PlayerHollowing.Value}");
                Log($"Invasion Type: {DS3.InvasionType.Value}");
                Log($"Area for online: {DS3.AreaForOnlineActivity.Value}");
                Log($"collision: {DS3.IsCollisionEnabled.Value}");
                Log($"Level loaded: {DS3.IsLevelLoaded}");
            }

            UpdateUI();
            game?.ClearOldCaches();
        }

        private static Process FindDS3Process()
        {
            return Process.GetProcessesByName("DarkSoulsIII").FirstOrDefault();
        }

        private void Log(string message)
        {
            var now = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace("T", " ");
            pendingLogLines.Add("[" + now + "] " + message + "\n");
        }
        private void logTimer_Tick(object sender, EventArgs e)
        {
            if (pendingLogLines.Count > 0)
            {
                logLines.AddRange(pendingLogLines);
                pendingLogLines.Clear();

                const int MaxHistory = 100;
                if (logLines.Count > MaxHistory)
                    logLines.RemoveRange(0, logLines.Count - MaxHistory);

                var text = string.Concat(logLines);
                richTextBox1.Text = text;
                richTextBox1.Select(text.Length, 0);
                richTextBox1.ScrollToCaret();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (game == null)
            {
                Log("No game found");
                return;
            }

            ulong x = 0x7FF718A57290;
            var symbol = GameProcess.GameMan;
            var found = game.FindSymbol(symbol, out var symbolAddr);

            var levelAddr = game
                .AddressOf(GameProcess.GameDataMan)
                .Deref()
                .Deref(offset: 0x10)
                .Offset(0x70);
            var level = levelAddr.Read<int>();

            /*var module = game.Process.MainModule;
            var mBytes = game.Memory.ReadBytes(module.BaseAddress, module.ModuleMemorySize);
            if (symbol.Find(mBytes, out var symbolIndex))
            {
                var instructionBytes = mBytes.Slice(symbolIndex, length: 16);
                var instructionAddress = (IntPtr)(module.BaseAddress.ToInt64() + symbolIndex);
                var symbolAddress = Disassembler.GetAddressFromInstruction(instructionBytes, instructionAddress);
            }*/



            // var addrFound = game.FindSymbol(symbol, out var address);

            //var health = mem.ReadMemory<int>((IntPtr)0x7FF4A6F79FD8);
            //Log($"Current health is {health}");
            GC.Collect();
        }
    }
}
