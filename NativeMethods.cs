using System.Runtime.InteropServices;
using System.Text;

namespace AtomTableDumper
{
    public class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClipboardFormatName(uint format, [Out] StringBuilder lpszFormatName, int cchMaxCount);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GlobalGetAtomName(ushort nAtom, StringBuilder lpBuffer, int nSize);

    }
}
