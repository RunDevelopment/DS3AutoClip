using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

// Based on: https://github.com/C0reTheAlpaca/C0reExternal-Base-v2/blob/master/Memory.cs

namespace DS3AutoClip
{
    public class ProcessMemory
    {
        public Process process;
        public IntPtr pHandle;

        public IntPtr numberOfBytesRead = default;
        public IntPtr numberOfBytesWritten = default;

        public ProcessMemory(Process process)
        {
            this.process = process;
            pHandle = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, this.process.Id); // Sets Our ProcessHandle
        }

        public int GetModuleAdress(string ModuleName)
        {
            try
            {
                foreach (ProcessModule ProcMod in process.Modules)
                {
                    if (!ModuleName.Contains(".dll"))
                        ModuleName = ModuleName.Insert(ModuleName.Length, ".dll");

                    if (ModuleName == ProcMod.ModuleName)
                    {
                        return (int)ProcMod.BaseAddress;
                    }
                }
            }
            catch { }
            return -1;
        }

        public T ReadMemory<T>(IntPtr Adress) where T : struct
        {
            int ByteSize = Marshal.SizeOf<T>(); // Get ByteSize Of DataType
            byte[] buffer = new byte[ByteSize]; // Create A Buffer With Size Of ByteSize
            ReadProcessMemory(pHandle, Adress, buffer, buffer.Length, out numberOfBytesRead); // Read Value From Memory

            return ByteArrayToStructure<T>(buffer); // Transform the ByteArray to The Desired DataType
        }

        public float[] ReadMatrix<T>(IntPtr Adress, int MatrixSize) where T : struct
        {
            int ByteSize = Marshal.SizeOf<T>();
            byte[] buffer = new byte[ByteSize * MatrixSize]; // Create A Buffer With Size Of ByteSize * MatrixSize
            ReadProcessMemory(pHandle, Adress, buffer, buffer.Length, out numberOfBytesRead);

            return ConvertToFloatArray(buffer); // Transform the ByteArray to A Float Array (PseudoMatrix ;P)
        }

        public byte[] ReadBytes(IntPtr Adress, int length)
        {
            byte[] buffer = new byte[length];
            ReadProcessMemory(pHandle, Adress, buffer, buffer.Length, out numberOfBytesRead);
            return buffer;
        }

        public void WriteMemory<T>(IntPtr Adress, object Value)
        {
            byte[] buffer = StructureToByteArray(Value); // Transform Data To ByteArray

            WriteProcessMemory(pHandle, Adress, buffer, buffer.Length, out numberOfBytesWritten);
        }

        public void WriteMemory<T>(IntPtr Adress, char[] Value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Value);

            WriteProcessMemory(pHandle, Adress, buffer, buffer.Length, out numberOfBytesWritten);
        }

        public MemoryBasicInformation[] QueryPages()
        {
            var pages = new List<MemoryBasicInformation>();

            var sizeOf = (UIntPtr)Marshal.SizeOf<MemoryBasicInformation>();

            var p = IntPtr.Zero;
            while (VirtualQueryEx(pHandle, p, out var info, sizeOf) == sizeOf)
            {
                if (info.State == MemState.COMMIT)
                {
                    pages.Add(info);
                }

                p = (IntPtr)(info.BaseAddress + info.RegionSize);
            }

            return pages.ToArray();
        }

        #region Transformation
        public static float[] ConvertToFloatArray(byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            float[] floats = new float[bytes.Length / 4];

            for (int i = 0; i < floats.Length; i++)
                floats[i] = BitConverter.ToSingle(bytes, i * 4);

            return floats;
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        private static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }
        #endregion

        #region DllImports

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead
        );

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] buffer,
            int size,
            out IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll")]
        private static extern UIntPtr VirtualQueryEx(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out MemoryBasicInformation buffer,
            UIntPtr dwLength
        );
        #endregion

        #region Constants

        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;

        #endregion
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-memory_basic_information?redirectedfrom=MSDN
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct MemoryBasicInformation
    {
        public ulong BaseAddress;
        public ulong AllocationBase;
        public MemProtect AllocationProtect;
        public int __alignment1;
        public ulong RegionSize;
        public MemState State;
        public MemProtect Protect;
        public MemType Type;
        public int __alignment2;
    }

    public enum MemState : int
    {
        COMMIT = 0x1000,
        FREE = 0x10000,
        RESERVE = 0x2000,
    }
    public enum MemType : int
    {
        IMAGE = 0x1000000,
        MAPPED = 0x40000,
        PRIVATE = 0x20000,
    }
    /// <summary>
    ///  https://learn.microsoft.com/en-us/windows/win32/memory/memory-protection-constants
    /// </summary>
    public enum MemProtect : int
    {
        PAGE_EXECUTE = 0x10,
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READWRITE = 0x40,
        PAGE_EXECUTE_WRITECOPY = 0x80,

        PAGE_NOACCESS = 0x01,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,

        PAGE_TARGETS_INVALID = 0x40000000,
        PAGE_TARGETS_NO_UPDATE = 0x40000000,
    }
}
