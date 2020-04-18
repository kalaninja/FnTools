using System.Runtime.InteropServices;

namespace FnTools.Types
{
    /// <summary>
    /// Nothing is the return type for methods while the final type is not known.
    /// Another usage of Nothing is to indicate that the parameter is bypassed in Partial Application.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Nothing
    {
        public override string ToString() => nameof(Nothing);
    }
}