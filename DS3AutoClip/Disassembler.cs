using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS3AutoClip
{
    internal static class Disassembler
    {
        /// <summary>
        /// Returns a static address from the instruction in the given machine code.
        /// </summary>
        /// <param name="code">The mchine code to disassemble.</param>
        /// <param name="codeAddress">The address of the machine code.</param>
        /// <returns></returns>
        public static IntPtr GetAddressFromInstruction(byte[] code, IntPtr codeAddress)
        {
            // 48 8B 05 xx xx xx xx
            // mov rax, [xxxxxxxx]
            if (code.Length >= 7 && code.StartsWith(0x48, 0x8B, 0x05))
            {
                var relative = BitConverter.ToInt32(code, 3);
                return (IntPtr)(codeAddress.ToInt64() + 7 + relative);
            }

            // 48 8B 0D xx xx xx xx
            // mov rcx, [xxxxxxxx]
            if (code.Length >= 7 && code.StartsWith(0x48, 0x8B, 0x0D))
            {
                var relative = BitConverter.ToInt32(code, 3);
                return (IntPtr)(codeAddress.ToInt64() + 7 + relative);
            }

            // 48 8B 0D xx xx xx xx
            // mov rbx, [xxxxxxxx]
            if (code.Length >= 7 && code.StartsWith(0x48, 0x8B, 0x1D))
            {
                var relative = BitConverter.ToInt32(code, 3);
                return (IntPtr)(codeAddress.ToInt64() + 7 + relative);
            }

            throw new Exception("Unknown instruction");
        }
    }
}
