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


            if (game == null)
            {
                currentState = GameState.NoGame;
            }
            else
            {
                currentState = GameState.Title;
            }

            UpdateUI();
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

            ulong x = 0x7FF4911a0aa0;
            for (var i = 0; i < game.Process.Modules.Count; i++)
            {
                var m = game.Process.Modules[i];
                if ((ulong)m.BaseAddress <= x && (ulong)m.BaseAddress + (ulong)m.ModuleMemorySize >= x)
                {
                    int asda = 0;
                }
            }

            var module = game.Process.MainModule;
            var data = game.Memory.ReadBytes(module.BaseAddress, module.ModuleMemorySize);
            GameProcess.GameDataMan.Search(data, out var foundIndex);

            var pages = game.Memory.QueryPages();
            var found = pages.Where(p => p.BaseAddress <= x && x < p.BaseAddress + p.RegionSize).ToArray();
            var totalMemory = pages.Where(p => p.Protect == MemProtect.PAGE_READWRITE).Select(p => (long)p.RegionSize).Sum();

            //var health = mem.ReadMemory<int>((IntPtr)0x7FF4A6F79FD8);
            //Log($"Current health is {health}");

        }
    }
}
