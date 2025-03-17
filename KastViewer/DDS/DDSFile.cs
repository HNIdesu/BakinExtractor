using DDS;
using System.Text;

namespace HNIdesu.DDS
{
    public sealed class DDSFile(DDSHeader header, byte[] data)
    {
        public const string Signature = "DDS ";
        public readonly DDSHeader DDSHeader = header;
        public readonly byte[] TextureData = data;
        public sealed class Builder()
        {
            private int? mWidth;
            private int? mHeight;
            private byte[]? mTextureData;
            private string? mFourCC;
            private DDSPixelFormatFlags mDDSPixelFormatFlags;
            private int? mByteCountPerPixel;
            private uint mRedMask;
            private uint mGreenMask;
            private uint mBlueMask;
            private uint mAlphaMask;
            public Builder SetPixelFormatFlags(DDSPixelFormatFlags flags)
            {
                mDDSPixelFormatFlags = flags;
                return this;
            }
            public Builder SetFourCC(string fourCC)
            {
                mFourCC = fourCC;
                return this;
            }
            public Builder SetRGBABitMask(uint red,uint green,uint blue,uint alpha)
            {
                mRedMask = red;
                mGreenMask = green;
                mBlueMask = blue;
                mAlphaMask = alpha;
                return this;
            }
            public Builder SetWidth(int width)
            {
                mWidth = width;
                return this;
            }
            public Builder SetHeight(int height)
            {
                mHeight = height;
                return this;
            }
            public Builder SetTextureData(byte[] data)
            {
                mTextureData = data;
                return this;
            }
            public Builder SetByteCountPerPixel(int byteCountPerPixel)
            {
                mByteCountPerPixel = byteCountPerPixel;
                return this;
            }

            public DDSFile Build()
            {
                if (mWidth == null || mHeight == null || mByteCountPerPixel == null)
                    throw new InvalidOperationException("Texture width, height, or byte count per pixel is not set.");
                if (mTextureData == null)
                    throw new InvalidOperationException("Texture data is missing.");
                var isCompressedTexture = mByteCountPerPixel * mWidth * mHeight > mTextureData.Length;
                if (((mDDSPixelFormatFlags & DDSPixelFormatFlags.FourCC) != 0) && mFourCC == null)
                    throw new InvalidOperationException("FourCC code is missing.");
                var header = new DDSHeader
                {
                    ddspf = new DDSPixelFormat()
                    {
                        dwSize = 32,
                        dwABitMask = mAlphaMask,
                        dwRBitMask = mRedMask,
                        dwGBitMask = mGreenMask,
                        dwBBitMask = mBlueMask,
                        dwFlags = mDDSPixelFormatFlags
                    },
                    dwSize = 124,
                    dwWidth = (uint)mWidth,
                    dwHeight = (uint)mHeight
                };
                if (mFourCC != null)
                    header.ddspf.dwFourCC = BitConverter.ToUInt32(Encoding.ASCII.GetBytes(mFourCC[..4]));
                if (isCompressedTexture)
                {;
                    header.dwFlags = DDSFlags.WIDTH | DDSFlags.HEIGHT | DDSFlags.CAPS | DDSFlags.PIXELFORMAT;
                }
                else
                {
                    header.dwFlags = DDSFlags.WIDTH | DDSFlags.HEIGHT | DDSFlags.CAPS | DDSFlags.PIXELFORMAT | DDSFlags.PITCH;
                    header.dwPitchOrLinearSize = (uint)(mWidth * mByteCountPerPixel);
                    header.ddspf.dwRGBBitCount = (uint)(mByteCountPerPixel * 8);
                }
                return new DDSFile(header, mTextureData);
            }
        }
    }
}
