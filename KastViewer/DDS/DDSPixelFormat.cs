using System.Runtime.InteropServices;

namespace DDS
{
    [Flags]
    public enum DDSPixelFormatFlags : uint
    {
        None = 0x0,
        AlphaPixels = 0x1,
        Alpha = 0x2,
        FourCC = 0x4,
        RGB = 0x40,
        YUV = 0x200,
        Luminance = 0x20000
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DDSPixelFormat
    {
        public uint dwSize;
        public DDSPixelFormatFlags dwFlags;
        public uint dwFourCC;
        public uint dwRGBBitCount;
        public uint dwRBitMask;
        public uint dwGBitMask;
        public uint dwBBitMask;
        public uint dwABitMask;
    }
}
