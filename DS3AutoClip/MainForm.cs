using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

namespace DS3AutoClip
{
    public partial class MainForm : Form
    {
        public GameProcess game = null;
        public GameState gameState = GameState.NoGame;
        public readonly DS3GameValues DS3 = new DS3GameValues();

        private readonly List<string> logLines = new List<string>();
        private readonly List<string> pendingLogLines = new List<string>();

        private readonly List<EventRecord> eventHistory = new List<EventRecord>();
        private readonly struct EventRecord
        {
            public readonly GameEvent Event;
            public readonly DateTime Time;

            public EventRecord(GameEvent e)
            {
                Event = e;
                Time = DateTime.UtcNow;
            }

            public static implicit operator EventRecord(GameEvent e) => new EventRecord(e);
            public static implicit operator GameEvent(EventRecord e) => e.Event;
        }

        public MainForm()
        {
            InitializeComponent();
            InitUI();
            UpdateUI();
        }

        private void InitUI()
        {
            startEventComboBox.Items.Clear();
            startEventComboBox.Items.AddRange(GameEvents.Labels.Values.ToArray());
            startEventComboBox.SelectedItem = GameEvent.EnteredAnotherWorld.GetLabel();

            stopEventComboBox.Items.Clear();
            stopEventComboBox.Items.AddRange(GameEvents.Labels.Values.ToArray());
            stopEventComboBox.SelectedItem = GameEvent.LeavingAnotherWorld.GetLabel();
        }

        private void FireEvent(GameEvent gameEvent)
        {
            Log($"Event: {gameEvent}");

            eventHistory.Add(gameEvent);
        }

        private void UpdateUI()
        {
            var stateLabels = new Dictionary<GameState, string>()
            {
                [GameState.NoGame] = "No Game",
                [GameState.TitleScreen] = "Title screen",
                [GameState.LoadingAnotherWorld] = "Loading another world",
                [GameState.LoadingOwnWorld] = "Loading",
                [GameState.GamingAnotherWorld] = "Gaming in another world",
                [GameState.GamingOwnWorld] = "Gaming",
            };
            stateLabel.Text = stateLabels[gameState];
        }

        private void UpdateGameValues()
        {
            if (game != null && !game.IsAlive)
            {
                // unload game instance and free resources
                game = null;
                GC.Collect();
            }

            if (game == null)
            {
                var process = Process.GetProcessesByName("DarkSoulsIII").FirstOrDefault();
                if (process != null)
                    game = new GameProcess(process);
            }

            if (game != null)
            {
                DS3.Update(game);
                game.ClearOldCaches();
            }
        }
        private GameState GetNextGameState()
        {
            if (game == null)
                return GameState.NoGame;

            // Log($"Player Health: {DS3.PlayerHP.Value}");
            // Log($"Player Char t: {DS3.PlayerCharacterType.Value}");
            // Log($"Player Team t: {DS3.PlayerTeamType.Value}");
            // Log($"Player hollowing: {DS3.PlayerHollowing.Value}");
            // Log($"Invasion Type: {DS3.InvasionType.Value}");
            // Log($"Area for online: {DS3.AreaForOnlineActivity.Value}");
            // Log($"collision: {DS3.IsCollisionEnabled.Value}");
            // Log($"Level loaded: {DS3.IsLevelLoaded.Value}");


            var chrType = DS3.PlayerCharacterType.Value;
            // var isHost = chrType == 0;
            var isPhantom = chrType != null && chrType != 0;

            if (DS3.IsLevelLoaded.Value)
            {
                return isPhantom ? GameState.GamingAnotherWorld : GameState.GamingOwnWorld;
            }
            else if (DS3.IsTitleScreen.Value)
            {
                return GameState.TitleScreen;
            }
            else
            {
                return isPhantom ? GameState.LoadingAnotherWorld : GameState.LoadingOwnWorld;
            }
        }
        private IEnumerable<GameEvent> GetEvents(GameState prev, GameState next)
        {
            if (next == GameState.LoadingAnotherWorld && prev != next)
                yield return GameEvent.LoadingAnotherWorld;

            if (next == GameState.GamingAnotherWorld && prev != next)
                yield return GameEvent.EnteredAnotherWorld;
            if (next == GameState.GamingOwnWorld && prev != next)
                yield return GameEvent.EnteredOwnWorld;

            if (prev == GameState.GamingAnotherWorld && next != prev)
                yield return GameEvent.LeavingAnotherWorld;
        }
        private void mainTimer_Tick(object sender, EventArgs e)
        {
            UpdateGameValues();

            var prevState = gameState;
            gameState = GetNextGameState();

            // fire events
            foreach (var ev in GetEvents(prevState, gameState))
                FireEvent(ev);

            UpdateUI();
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

                const int MaxHistory = 1000;
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

    public enum GameState
    {
        NoGame,
        TitleScreen,
        LoadingOwnWorld,
        LoadingAnotherWorld,
        GamingOwnWorld,
        GamingAnotherWorld,
    }
    public enum GameEvent
    {
        LoadingAnotherWorld,
        EnteredAnotherWorld,
        LeavingAnotherWorld,
        EnteredOwnWorld,
    }
    public static class GameEvents
    {
        public static readonly Dictionary<GameEvent, string> Labels = new Dictionary<GameEvent, string>()
        {
            [GameEvent.LoadingAnotherWorld] = "Loading another world",
            [GameEvent.EnteredAnotherWorld] = "Entered another world",
            [GameEvent.LeavingAnotherWorld] = "Leaving another world",
            [GameEvent.EnteredOwnWorld] = "Entered own world",
        };

        public static GameEvent ParseLabel(string s)
        {
            foreach (var kv in Labels)
            {
                if (s == kv.Value)
                    return kv.Key;
            }
            throw new Exception($"Cannot parse action event from string: {s}");
        }
        public static string GetLabel(this GameEvent e)
        {
            return Labels[e];
        }
    }
}
