using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Drawing;

namespace DS3AutoClip
{
    public partial class MainForm : Form
    {
        public GameProcess game = null;
        public GameState gameState = GameState.NoGame;
        public readonly DS3GameValues DS3 = new DS3GameValues();

        public static readonly string ObsPath = @"C:\Program Files\obs-studio\bin\64bit\obs64.exe";
        public static readonly string ObsProcessName = @"obs64";
        private bool obsPathExists = false;

        public string targetProcessName = ObsProcessName;

        private Properties.Settings settings = Properties.Settings.Default;

        public MainForm()
        {
            InitializeComponent();

            obsPathExists = File.Exists(ObsPath);

            InitUI();
            UpdateUI();

            FormClosing += (_, e) => settings.Save();
        }

        private void InitUI()
        {
            GameEvent ParseEvent(string s, GameEvent orElse)
            {
                try
                {
                    return GameEvents.ParseLabel(s);
                }
                catch (Exception)
                {
                    return orElse;
                }
            }

            startEventComboBox.Items.Clear();
            startEventComboBox.Items.AddRange(GameEvents.Labels.Values.ToArray());
            startEventComboBox.SelectedItem = ParseEvent(settings.StartEvent, GameEvent.EnteredAnotherWorld).GetLabel();
            startEventComboBox.SelectedValueChanged += (_, e) => settings.StartEvent = (string)startEventComboBox.SelectedItem;

            stopEventComboBox.Items.Clear();
            stopEventComboBox.Items.AddRange(GameEvents.Labels.Values.ToArray());
            stopEventComboBox.SelectedItem = ParseEvent(settings.StopEvent, GameEvent.LeavingAnotherWorld).GetLabel();
            stopEventComboBox.SelectedValueChanged += (_, e) => settings.StopEvent = (string)stopEventComboBox.SelectedItem;

            Action SendKey(VK key)
            {
                return () =>
                {
                    var processes = Process.GetProcessesByName(targetProcessName);
                    Log($"Sending {key} to {targetProcessName} ({processes.Length})");
                    foreach (var process in processes)
                        Keyboard.SendKey(process.MainWindowHandle, key);
                };
            }

            var actions = new EventAction[]
            {
                new EventAction("None", () => {}),
                new EventAction("Key F2", SendKey(VK.F2)),
                new EventAction("Key F3", SendKey(VK.F3)),
                new EventAction("Key F4", SendKey(VK.F4)),
                new EventAction("Key F5", SendKey(VK.F5)),
                new EventAction("Key F6", SendKey(VK.F6)),
                new EventAction("Key F7", SendKey(VK.F7)),
                new EventAction("Key F8", SendKey(VK.F8)),
                new EventAction("Key F9", SendKey(VK.F9)),
                new EventAction("Key F10", SendKey(VK.F10)),
                new EventAction("Key F11", SendKey(VK.F11)),
                new EventAction("Key F12", SendKey(VK.F12)),
            };
            int ParseActionIndex(string s, int orElse = 0)
            {
                var index = actions.ToList().FindIndex(e => e.Label == s);
                return index == -1 ? orElse : index;
            }

            startActionComboBox.Items.Clear();
            startActionComboBox.Items.AddRange(actions);
            startActionComboBox.SelectedIndex = ParseActionIndex(settings.StartAction);
            startActionComboBox.SelectedValueChanged += (_, e) => settings.StartAction = ((EventAction)startActionComboBox.SelectedItem).Label;

            stopActionComboBox.Items.Clear();
            stopActionComboBox.Items.AddRange(actions);
            stopActionComboBox.SelectedIndex = ParseActionIndex(settings.StopAction);
            stopActionComboBox.SelectedValueChanged += (_, e) => settings.StopAction = ((EventAction)stopActionComboBox.SelectedItem).Label;

            UpdateProcessSelector();
        }

        #region Process Selector

        private static Process[] GetWindowProcesses()
        {
            var ignore = new string[] {
                // windows stuff
                "explorer",
                "conhost",
                "svchost",
                "TextInputHost",
                // game stuff
                "steamwebhelper",
                "DarkSoulsIII",
                // this process
                Process.GetCurrentProcess().ProcessName
            };

            return Process.GetProcesses()
                .Where(p =>
                {
                    if (ignore.Contains(p.ProcessName))
                        return false;
                    return p.MainWindowHandle != IntPtr.Zero;
                })
                .ToArray();
        }
        private void UpdateProcessSelector()
        {
            var processes = GetWindowProcesses()
                .GroupBy(p => p.ProcessName)
                .Select(g =>
                {
                    var p = g.First();
                    return new ProcessRecord(p.ProcessName, p.MainWindowTitle);
                })
                .ToList();

            var obsRunning = processes.Any(p => p.Name == ObsProcessName);

            if (!processes.Any(p => p.Name == targetProcessName))
            {
                processes.Add(new ProcessRecord(targetProcessName));
            }

            processes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
            var selectedIndex = processes.FindIndex(p => p.Name == targetProcessName);

            targetProcessComboBox.Items.Clear();
            targetProcessComboBox.Items.AddRange(processes.ToArray());
            targetProcessComboBox.SelectedIndex = selectedIndex;

            startObsButton.Enabled = obsPathExists && !obsRunning;
        }
        private void processTimer_Tick(object sender, EventArgs e)
        {
            UpdateProcessSelector();
        }
        private void targetProcessComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = (ProcessRecord)targetProcessComboBox.SelectedItem;
            if (selected.Name != targetProcessName)
            {
                targetProcessName = selected.Name;
                UpdateProcessSelector();
            }
        }

        private void targetProcessComboBox_DropDown(object sender, EventArgs e)
        {
            processTimer.Stop();
        }
        private void targetProcessComboBox_DropDownClosed(object sender, EventArgs e)
        {
            SetTimeout(() =>
            {
                processTimer.Start();
                UpdateProcessSelector();
            }, 10);
        }

        private static void SetTimeout(Action action, int timeout)
        {
            var timer = new Timer();
            timer.Interval = timeout;
            timer.Tick += delegate (object sender, EventArgs args)
            {
                action();
                timer.Stop();
            };
            timer.Start();
        }

        private void startObsButton_Click(object sender, EventArgs e)
        {
            try
            {
                var info = new ProcessStartInfo(ObsPath)
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(ObsPath),
                };
                Process.Start(info);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        #endregion

        #region Tick Logic

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

        private void FireEvent(GameEvent gameEvent)
        {
            Log($"Event: {gameEvent}");

            var startEvent = GameEvents.ParseLabel((string)startEventComboBox.SelectedItem);
            var startAction = (EventAction)startActionComboBox.SelectedItem;
            if (startEvent == gameEvent) startAction.Action();

            var stopEvent = GameEvents.ParseLabel((string)stopEventComboBox.SelectedItem);
            var stopAction = (EventAction)stopActionComboBox.SelectedItem;
            if (stopEvent == gameEvent) stopAction.Action();

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

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = $"v{version.Major}.{version.Minor}.{version.Build}";
            this.Text = $"DS3 Auto Clip {versionString}  -  " + stateLabels[gameState];
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

        #endregion

        #region Logging

        private readonly List<string> logLines = new List<string>();
        private readonly List<string> pendingLogLines = new List<string>();

        private bool showLogs = false;
        private int originalHeight = 0;

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

        private void logToggleLabel_Click(object sender, EventArgs e)
        {
            showLogs = !showLogs;

            if (showLogs)
            {
                logToggleLabel.Text = "Hide logs";

                var newHeight = 400;
                originalHeight = Height;
                Height = newHeight;
                richTextBox1.Visible = true;
                richTextBox1.Height = newHeight - originalHeight - 9;
            }
            else
            {
                logToggleLabel.Text = "Show logs";
                Height = originalHeight;
                richTextBox1.Visible = false;
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            FireEvent(GameEvent.EnteredAnotherWorld);

            if (game == null)
            {
                Log("No game found");
                return;
            }


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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/RunDevelopment/DS3AutoClip")
            {
                UseShellExecute = true
            };
            Process.Start(sInfo);
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

    public class EventAction
    {
        public readonly string Label;
        public readonly Action Action;

        public EventAction(string label, Action action)
        {
            Label = label;
            Action = action;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public class ProcessRecord : IEquatable<ProcessRecord>
    {
        public readonly string Name;
        public readonly string Title = null;

        public ProcessRecord(string name, string title = null)
        {
            Name = name;
            Title = title;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProcessRecord other)
                return Equals(other);
            return base.Equals(obj);
        }
        public bool Equals(ProcessRecord other)
        {
            return Name == other.Name && Title == other.Title;
        }

        public override int GetHashCode()
        {
            if (Title == null)
                return Name.GetHashCode();
            return Name.GetHashCode() ^ Title.GetHashCode();
        }

        public override string ToString()
        {
            if (Title == null)
                return $"{Name}: not running";
            return $"{Name}: {Title}";
        }
    }
}
