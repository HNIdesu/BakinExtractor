using DDS;
using HNIdesu.DDS;

namespace KastViewer.Core
{
    public class TextureInternalToDDSConverter
    {
        public static DDSFile Convert(TextureInternal textureInternal)
        {
            var textureType = textureInternal.TextureType;
            if (textureType != "2D__")
                throw new NotSupportedException();
            var format = textureInternal.Format;
            byte[] textureData;
            using (var br = new BinaryReader(textureInternal.OpenRead()))
                textureData = br.ReadBytes((int)textureInternal.Size);
            var builder = new DDSFile.Builder()
                .SetWidth(textureInternal.Width)
                .SetHeight(textureInternal.Height)
                .SetTextureData(textureData);
            switch (format)
            {
                case 1://GL_RGB
                    {
                        builder.SetByteCountPerPixel(3)
                            .SetPixelFormatFlags(DDSPixelFormatFlags.RGB)
                            .SetRGBABitMask(0x00FF0000, 0x0000FF00, 0x000000FF, 0);
                        break;
                    }
                case 2://GL_R8
                    {
                        builder.SetByteCountPerPixel(1)
                            .SetPixelFormatFlags(DDSPixelFormatFlags.RGB)
                            .SetRGBABitMask(0xFF000000,0,0,0);
                        break;
                    }
                case 3://GL_R16
                    {
                        throw new NotImplementedException();
                    }
                case 4://GL_R16F
                    {
                        builder.SetByteCountPerPixel(2)
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC)
                            .SetFourCC("o\x00\x00\x00");
                        break;
                    }
                case 5:
                case 7://GL_RGBA16F
                    {
                        builder.SetByteCountPerPixel(8)
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC)
                            .SetFourCC("q\x00\x00\x00");
                        break;
                    }
                case 6://GL_RGB16F
                    {
                        throw new NotImplementedException();
                    }
                case 8:
                case 10://GL_DEPTH_COMPONENT
                    {
                        throw new NotImplementedException();
                    }
                case 9://GL_DEPTH24_STENCIL8
                    {
                        throw new NotImplementedException();
                    }

                case 12://GL_SRGB8_EXT
                    {
                        throw new NotImplementedException();
                    }
                case 14://GL_COMPRESSED_RGB_S3TC_DXT1_EXT
                    {
                        builder.SetByteCountPerPixel(3)
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC)
                            .SetFourCC("DXT1");
                        break;
                    }
                case 15://GL_COMPRESSED_SRGB_S3TC_DXT1_EXT
                    {
                        builder.SetByteCountPerPixel(4)
                            .SetFourCC("DXT1")
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC);
                        break;
                    }
                case 20:
                case 16://GL_COMPRESSED_RGBA_S3TC_DXT1_EXT
                    {
                        builder.SetByteCountPerPixel(4)
                            .SetFourCC("DXT1")
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC);
                        break;
                    }
                case 17://GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT
                case 21:
                    {
                        builder.SetByteCountPerPixel(4)
                            .SetFourCC("DXT1")
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC);
                        break;
                    }
                case 18://GL_COMPRESSED_RGBA_S3TC_DXT5_EXT
                case 22:
                    {
                        builder.SetByteCountPerPixel(4)
                            .SetFourCC("DXT5")
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC);
                        break;
                    }
                case 19://GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT
                case 23:
                    {
                        builder.SetByteCountPerPixel(4)
                            .SetFourCC("DXT5")
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC);
                        break;
                    }
                case 24://GL_COMPRESSED_RG_RGTC2
                    {
                        builder.SetByteCountPerPixel(2)
                            .SetFourCC("ATI2")
                            .SetPixelFormatFlags(DDSPixelFormatFlags.FourCC);
                        break;
                    }
                case 25://GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT
                    {
                        throw new NotImplementedException();
                    }
                case 11://GL_SRGB8_ALPHA8_EXT
                case 13://GL_RGBA
                default://GL_RGBA
                    {
                        builder.SetByteCountPerPixel(4)
                            .SetPixelFormatFlags(
                            DDSPixelFormatFlags.RGB
                            | DDSPixelFormatFlags.AlphaPixels)
                            .SetRGBABitMask(0x000000FF, 0x0000FF00,0x00FF0000, 0xFF000000);
                        break;
                    }
            }
            return builder.Build();
        }
    }
}
