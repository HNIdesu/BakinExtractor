using System.Runtime.InteropServices;

namespace DDS
{
    [Flags]
    public enum DDSFlags : uint
    {
        CAPS = 0x00000001,
        HEIGHT = 0x00000002,
        WIDTH = 0x00000004,
        PITCH = 0x00000008,
        PIXELFORMAT = 0x00001000,
        MIPMAPCOUNT = 0x00020000,
        LINEARSIZE = 0x00080000,
        DEPTH = 0x00800000
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DDSHeader
    {
        public uint dwSize;
        public DDSFlags dwFlags;
        public uint dwHeight;
        public uint dwWidth;
        public uint dwPitchOrLinearSize;
        public uint dwDepth;
        public uint dwMipMapCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public uint[] dwReserved1;
        public DDSPixelFormat ddspf;
        public uint dwCaps;
        public uint dwCaps2;
        public uint dwCaps3;
        public uint dwCaps4;
        public uint dwReserved2;
    }
}
