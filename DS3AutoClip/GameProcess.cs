using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DS3AutoClip
{
    public class SymbolData
    {
        public readonly string Name;
        public readonly byte?[] BytePattern;
        public readonly int Offset;

        public SymbolData(string name, string aob, int offset = 0)
        {
            Name = name;
            Offset = offset;

            BytePattern = aob.Split(' ').Select(s =>
            {
                byte? result = null;
                if (s != "??")
                    result = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
                return result;
            }).ToArray();
        }

        public bool MatchesPattern(byte[] data, int at)
        {
            if (at + BytePattern.Length > data.Length)
                return false;

            for (var i = 0; i < BytePattern.Length; i++)
            {
                var b = BytePattern[i];
                if (b != null && data[at + i] != b.Value)
                    return false;
            }
            return true;
        }

        public bool Find(byte[] bytes, out int index)
        {
            var firstByte = BytePattern[0].Value;
            var lastByte = BytePattern[BytePattern.Length - 1].Value;

            var currentIndex = 0;
            while (currentIndex < bytes.Length)
            {
                currentIndex = Array.IndexOf(bytes, firstByte, startIndex: currentIndex);
                if (currentIndex == -1 || currentIndex + BytePattern.Length >= bytes.Length)
                    break;

                if (bytes[currentIndex + BytePattern.Length - 1] == lastByte && MatchesPattern(bytes, currentIndex))
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


        const double CacheTTL = 50;
        private (MemoryBasicInformation[] pages, DateTime time)? pagesCache = null;

        private struct SymbolEntry
        {
            public IntPtr patternAddr;
            public IntPtr address;
            public DateTime lastValidated;
        }
        private Dictionary<SymbolData, SymbolEntry> symbolCache = new Dictionary<SymbolData, SymbolEntry>();

        private Dictionary<ProcessModule, (byte[] data, DateTime time)> moduleMemoryCache
            = new Dictionary<ProcessModule, (byte[] data, DateTime time)>();

        public bool IsAlive { get => !Process.HasExited; }

        public GameProcess(Process process)
        {
            Process = process;
            Memory = new ProcessMemory(process);
        }

        public MemoryBasicInformation[] GetPages()
        {
            if (pagesCache != null)
            {
                var (pages, time) = pagesCache.Value;
                var timeSinceLastQuery = Math.Abs((time - DateTime.UtcNow).TotalMilliseconds);
                if (timeSinceLastQuery < CacheTTL)
                    return pages;
            }

            var freshPages = Memory.QueryPages();
            pagesCache = (freshPages, DateTime.UtcNow);
            return freshPages;
        }

        public byte[] GetModuleMemory(ProcessModule module)
        {
            if (moduleMemoryCache.TryGetValue(module, out var entry))
            {
                var timeSinceLastQuery = Math.Abs((entry.time - DateTime.UtcNow).TotalMilliseconds);
                if (timeSinceLastQuery < CacheTTL)
                    return entry.data;
            }

            // read memory
            var memory = Memory.ReadBytes(module.BaseAddress, module.ModuleMemorySize);

            moduleMemoryCache[module] = (memory, DateTime.UtcNow);
            return memory;
        }

        public void ClearOldCaches()
        {
            var cleared = 0;

            {
                // moduleMemoryCache
                var toRemove = new List<ProcessModule>();
                var now = DateTime.UtcNow;
                foreach (var kv in moduleMemoryCache)
                {
                    var timeSinceLastQuery = Math.Abs((kv.Value.time - now).TotalMilliseconds);
                    if (timeSinceLastQuery < CacheTTL)
                        toRemove.Add(kv.Key);
                }

                foreach (var item in toRemove)
                    moduleMemoryCache.Remove(item);

                cleared += toRemove.Count;
            }

            if (cleared > 0)
                GC.Collect();
        }

        public bool FindSymbol(SymbolData symbol, out IntPtr address)
        {
            if (symbolCache.TryGetValue(symbol, out var entry))
            {
                var timeSinceLastQuery = Math.Abs((entry.lastValidated - DateTime.UtcNow).TotalMilliseconds);
                if (timeSinceLastQuery < CacheTTL)
                {
                    address = entry.address;
                    return true;
                }

                // instead of re-scanning, just check whether the symbol's pattern still matches
                try
                {
                    var bytes = Memory.ReadBytes(entry.patternAddr, symbol.BytePattern.Length);
                    if (symbol.MatchesPattern(bytes, at: 0))
                    {
                        address = entry.address;
                        return true;
                    }
                }
                catch
                {
                    // memory no longer accessible, full re-scan necessary
                }

                symbolCache.Remove(symbol);
            }

            // scan main module
            try
            {
                var module = Process.MainModule;
                var memory = GetModuleMemory(module);
                if (symbol.Find(memory, out var symbolIndex))
                {
                    var instructionBytes = memory.Slice(symbolIndex, length: 16);
                    var instructionAddress = (IntPtr)(module.BaseAddress.ToInt64() + symbolIndex + symbol.Offset);
                    var symbolAddress = Disassembler.GetAddressFromInstruction(instructionBytes, instructionAddress);

                    symbolCache[symbol] = new SymbolEntry()
                    {
                        address = symbolAddress,
                        patternAddr = (IntPtr)(module.BaseAddress.ToInt64() + symbolIndex),
                        lastValidated = DateTime.UtcNow,
                    };

                    address = symbolAddress;
                    return true;
                }
            }
            catch
            {
                // just ignore error
            }

            address = default;
            return false;
        }

        public Addr AddressOf(SymbolData symbol)
        {
            if (FindSymbol(symbol, out var address))
            {
                return new Addr(Memory, address);
            }
            return Addr.Invalid(Memory);
        }

        public struct Addr
        {
            public readonly ProcessMemory Memory;
            public readonly IntPtr Address;

            public bool IsValid { get => Address != IntPtr.Zero; }

            public Addr(ProcessMemory memory, IntPtr address)
            {
                Memory = memory;
                Address = address;
            }

            public static Addr Invalid(ProcessMemory memory) => new Addr(memory, IntPtr.Zero);
            public Addr Invalidate() => Invalid(Memory);

            public Addr Offset(long by)
            {
                if (!IsValid)
                    return this;
                return new Addr(Memory, (IntPtr)(Address.ToInt64() + by));
            }
            public Addr Deref(long offset = 0)
            {
                if (!IsValid)
                    return this;

                try
                {
                    var at = (IntPtr)(Address.ToInt64() + offset);
                    var deref = Memory.ReadMemory<IntPtr>(at);
                    return new Addr(Memory, deref);
                }
                catch
                {
                    return Invalidate();
                }
            }

            public T Read<T>() where T : struct
            {
                if (!IsValid)
                    throw new AccessViolationException("Cannot read from an invalid address");
                return Memory.ReadMemory<T>(Address);
            }
        }

        public static readonly SymbolData GameDataMan = new SymbolData("GameDataMan", aob: "48 8B 05 ?? ?? ?? ?? 48 85 C0 ?? ?? 48 8B 40 ?? C3");
        public static readonly SymbolData WorldChrMan = new SymbolData("WorldChrMan", aob: "48 8B 1D ?? ?? ?? 04 48 8B F9 48 85 DB ?? ?? 8B 11 85 D2 ?? ?? 8D");
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
