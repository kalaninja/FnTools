using System.Runtime.InteropServices;

namespace FnTools.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Nothing
    {
        public override string ToString() => "Nothing";
    }
}