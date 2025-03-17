using BCnEncoder.Decoder;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using HNIdesu.DDS;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KastViewer.Core
{
    public static class TextureInternalToImageConverter
    {
        private static int ToInt32(this ColorRgba32 color) =>
            (color.a << 24) | (color.r << 16) | (color.g << 8) | color.b;
        public static Image Convert(TextureInternal textureInternal)
        {
            var ddsFile = TextureInternalToDDSConverter.Convert(textureInternal);
            var decoder = new BcDecoder();
            using var ms = new MemoryStream();
            DDSSerializer.Serialize(ddsFile, ms);
            ms.Seek(0, SeekOrigin.Begin);
            var matrix = decoder.Decode2D(DdsFile.Load(ms)).ToArray();
            var bitmap = new Bitmap(textureInternal.Width, textureInternal.Height,PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var colorBuffer = new int[bitmap.Width * bitmap.Height]; ;
            for (int i = 0; i < textureInternal.Height; i++)
                for (int j = 0; j < textureInternal.Width; j++)
                    colorBuffer[i * bitmap.Width + j] = matrix[textureInternal.Height - 1 - i, j].ToInt32();
            Marshal.Copy(colorBuffer, 0, bitmapData.Scan0, colorBuffer.Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
