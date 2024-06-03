using Silk.NET.OpenGL;
using StbImageSharp;

namespace MultimediaBuilder.Graphics
{
    public class Texture
    {
        public uint textureID;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public unsafe void GenTexture(GL gl, string path)
        {
            //Generate texture ID
            textureID = gl.GenTexture();

            //our image
            ImageResult image = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
            Width = image.Width;
            Height = image.Height;

            byte[] bytes = FlipImageVertically(image);

            gl.ActiveTexture(GLEnum.Texture0);
            gl.BindTexture(GLEnum.Texture2D, textureID);

            //We want the ability to create a texture using data generated from code aswell.
            fixed (byte* d = bytes)
            {
                //Setting the data of a texture.
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
            }

            gl.TextureParameter(textureID, GLEnum.TextureWrapS, (int)GLEnum.ClampToBorder);
            gl.TextureParameter(textureID, GLEnum.TextureWrapT, (int)GLEnum.ClampToBorder);
            gl.TextureParameter(textureID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            gl.TextureParameter(textureID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            gl.GenerateMipmap(GLEnum.Texture2D);

            gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        private byte[] FlipImageVertically(ImageResult image)
        {
            byte[] flippedPixels = new byte[image.Data.Length];
            int rowLength = image.Width * 4;  // Assuming RGBA
            for (int y = 0; y < image.Height; y++)
            {
                int originalRowStart = y * rowLength;
                int flippedRowStart = (image.Height - 1 - y) * rowLength;
                Array.Copy(image.Data, originalRowStart, flippedPixels, flippedRowStart, rowLength);
            }
            return flippedPixels;
        }
    }
}