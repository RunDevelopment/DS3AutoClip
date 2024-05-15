using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DS3AutoClip
{
    public class SymbolData
    {
        private string name;
        private byte?[] bytePattern;
        private int offset;

        public string Name { get => name; }
        public byte?[] BytePattern { get => bytePattern; }
        public int Offset { get => offset; }

        public SymbolData(string name, string aob, int offset = 0)
        {
            this.name = name;
            this.offset = offset;

            bytePattern = aob.Split(' ').Select(s =>
            {
                byte? result = null;
                if (s != "??")
                    result = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
                return result;
            }).ToArray();
        }

        public bool MatchesPattern(byte[] data, int at)
        {
            if (at + bytePattern.Length >= data.Length)
                return false;

            for (var i = 0; i < bytePattern.Length; i++)
            {
                var b = bytePattern[i];
                if (b != null && data[at + i] != b.Value)
                    return false;
            }
            return false;
        }

        public bool Search(byte[] bytes, out int index)
        {
            var firstByte = bytePattern[0].Value;
            var lastByte = bytePattern[bytePattern.Length - 1].Value;

            var currentIndex = 0;
            while (currentIndex < bytes.Length)
            {
                currentIndex = Array.IndexOf(bytes, firstByte, startIndex: currentIndex);
                if (currentIndex == -1 || currentIndex + bytePattern.Length >= bytes.Length)
                    break;

                if (bytes[currentIndex + bytePattern.Length - 1] == lastByte && MatchesPattern(bytes, currentIndex))
                {
                    index = currentIndex;
                    return true;
                }

                currentIndex++;
            }

            index = -1;
            return false;
        }
    }

    public class GameProcess
    {
        public Process Process;
        public ProcessMemory Memory;

        private (MemoryBasicInformation[] pages, DateTime time)? lastPages = null;

        public bool IsAlive { get => !Process.HasExited; }

        public GameProcess(Process process)
        {
            Process = process;
            Memory = new ProcessMemory(process);
        }

        public MemoryBasicInformation[] GetPages()
        {
            if (lastPages != null)
            {
                var (pages, time) = lastPages.Value;
                var timeSinceLastQuery = Math.Abs((time - DateTime.UtcNow).TotalMilliseconds);
                const double TimeToLive = 50;

                if (timeSinceLastQuery < TimeToLive)
                    return pages;
            }

            var freshPages = Memory.QueryPages();
            lastPages = (freshPages, DateTime.UtcNow);
            return freshPages;
        }

        public static readonly SymbolData GameDataMan = new SymbolData("GameDataMan", aob: "48 8B 05 ?? ?? ?? ?? 48 85 C0 ?? ?? 48 8B 40 ?? C3");
        public static readonly SymbolData GameMan = new SymbolData("GameMan", aob: "48 8B ?? ?? ?? ?? 04 89 48 28 C3");
        public static readonly SymbolData FieldArea = new SymbolData("FieldArea", aob: "48 8B 0D ?? ?? ?? ?? 48 85 C9 74 26 44 8B");
        public static readonly SymbolData FrpgNetMan = new SymbolData("FrpgNetMan", aob: "48 8B 05 ?? ?? ?? ?? 40 0F B6 FF");
        public static readonly SymbolData LockTgtMan = new SymbolData("LockTgtMan", aob: "48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B D8 48 85 C0 0F 84 ?? ?? ?? ?? C7");
        public static readonly SymbolData NearOnlyDraw = new SymbolData("NearOnlyDraw", aob: "0F 84 ?? ?? ?? ?? 4C 8D 0D ?? ?? ?? ?? 4C 8D 05 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 48 8B C8 E8", offset: 20);
        public static readonly SymbolData WorldChrManDbg_Flags = new SymbolData("WorldChrManDbg_Flags", aob: "4C 8D 05 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 48 8B CB E8 ?? ?? ?? ?? 48 83 3D ?? ?? ?? ?? 00");
        public static readonly SymbolData FdpClient = new SymbolData("FdpClient", aob: "48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 41 8B D4");
        public static readonly SymbolData NewMenuSystem = new SymbolData("NewMenuSystem", aob: "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 ?? 80 B8 ?? ?? ?? ?? ?? 0F 95 C0 C3");
        public static readonly SymbolData MenuMan = new SymbolData("MenuMan", aob: "48 8B 0D ?? ?? ?? ?? 33 C0 48 39 81 ?? ?? ?? ?? 0F 95 C0 C3");
        public static readonly SymbolData MsgRepository = new SymbolData("MsgRepository", aob: "44 8B F8 44 0F B6 A7 ?? ?? ?? ?? 48 8B 1D ?? ?? ?? ?? 48 85 DB 75", offset: 11);
        public static readonly SymbolData MapItemMan = new SymbolData("MapItemMan", aob: "48 8B 0D ?? ?? ?? ?? BB ?? ?? ?? ?? 41 BC");
        public static readonly SymbolData ThrowMan = new SymbolData("ThrowMan", aob: "48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B CE E8 ?? ?? ?? ?? 48 8B 9E");
        public static readonly SymbolData DamageMan = new SymbolData("DamageMan", aob: "8B 57 5C 48 8B 0D ?? ?? ?? ?? 4C 8D 44 24 20 E8 ?? ?? ?? ?? 48 8B 9C 24", offset: 3);
    }
}
