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
using System.Reflection;

namespace DS3AutoClip
{
    public partial class Form1 : Form
    {
        public GameProcess game = null;
        DS3GameValues DS3 = new DS3GameValues();

        private List<string> logLines = new List<string>();
        private List<string> pendingLogLines = new List<string>();

        public Form1()
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
            else if (DS3.IsLevelLoaded)
            {
                stateText = "Gaming";
            }
            else
            {
                stateText = "Other";
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

    public class DS3GameValues
    {
        // Player stats

        public readonly ObvservedValue<int> PlayerHP = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Deref(offset: 0x1f90)
                .Deref(offset: 0x18)
                .Offset(0xd8)
        );
        public readonly ObvservedValue<int> PlayerCharacterType = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Offset(0x70)
        );
        public readonly ObvservedValue<int> PlayerTeamType = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Offset(0x74)
        );

        // Misc

        public readonly ObvservedValue<byte> IsCollisionEnabled = new ObvservedValue<byte>(
            GameProcess.FieldArea,
            addr => addr.Deref()
                .Deref(offset: 0x60)
                .Offset(0x48)
        );
        public bool IsLevelLoaded { get => IsCollisionEnabled.Value != null; }

        // Multiplayer

        public readonly ObvservedValue<int> AreaForOnlineActivity = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Offset(0x1abc)
        );
        public readonly ObvservedValue<int> InvasionType = new ObvservedValue<int>(
            GameProcess.GameMan,
            addr => addr.Deref()
                .Offset(0xC54)
        );

        public void Update(GameProcess game)
        {
            var valueFields = typeof(DS3GameValues).GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in valueFields)
            {
                var obsValue = field.GetValue(this);
                var ovType = obsValue.GetType();
                var updateMethod = ovType.GetMethod("Update");
                updateMethod.Invoke(obsValue, new object[] { game });
            }
        }
    }

    public class ObvservedValue<T>
        where T : struct
    {
        public T? Value = null;
        public T? Prev = null;

        private readonly SymbolData symbol;
        private readonly Func<GameProcess.Addr, GameProcess.Addr> addrFn;

        public ObvservedValue(SymbolData symbol, Func<GameProcess.Addr, GameProcess.Addr> addrFn)
        {
            this.symbol = symbol;
            this.addrFn = addrFn;
        }

        private T? GetFreshValue(GameProcess game)
        {
            if (game == null)
                return null;

            var addr = addrFn(game.AddressOf(symbol));
            if (!addr.IsValid)
                return null;

            try
            {
                return addr.Read<T>();
            }
            catch
            {
                return null;
            }
        }

        public void Update(GameProcess game)
        {
            var newValue = GetFreshValue(game);
            Prev = Value;
            Value = newValue;
        }
    }
}
