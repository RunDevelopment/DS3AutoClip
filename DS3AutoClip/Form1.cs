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
    public enum GameState
    {
        NoGame,
        Title,
        Loading,
        Playing,
    }

    public partial class Form1 : Form
    {
        public GameState currentState = GameState.NoGame;
        public GameProcess game = null;

        public Form1()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            // TODO: icon

            switch (currentState)
            {
                case GameState.NoGame:
                    stateLabel.Text = "DS3 not running";
                    break;
                case GameState.Title:
                    stateLabel.Text = "Title screen";
                    break;
                case GameState.Loading:
                    stateLabel.Text = "Loading";
                    break;
                case GameState.Playing:
                    stateLabel.Text = "Gaming";
                    break;
            }
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

            Log($"Player health: {GetPlayerHealth()}");


            if (game == null)
            {
                currentState = GameState.NoGame;
            }
            else
            {
                currentState = GameState.Title;
            }

            UpdateUI();
            game?.ClearOldCaches();
        }

        private int? GetPlayerHealth()
        {
            if (game == null)
                return null;

            var addr = game
                .AddressOf(GameProcess.WorldChrMan)
                .Deref()
                .Deref(offset: 0x80)
                .Deref(offset: 0x1f90)
                .Deref(offset: 0x18)
                .Offset(0xd8);

            if (!addr.IsValid)
                return null;

            try
            {
                return addr.Read<int>();
            }
            catch
            {
                return null;
            }
        }

        private static Process FindDS3Process()
        {
            return Process.GetProcessesByName("DarkSoulsIII").FirstOrDefault();
        }

        private void Log(string message)
        {
            var now = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace("T", " ");
            richTextBox1.AppendText("[" + now + "] " + message + "\n");
            richTextBox1.ScrollToCaret();
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
