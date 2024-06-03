using Silk.NET.OpenGL;
using System.Numerics;
using MultimediaBuilder;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace MultimediaBuilder.Graphics
{
    public static class Renderer
    {
        private const uint stride = (3 * sizeof(float)) + (2 * sizeof(float));
        private static uint vao, vbo, ebo;

        public static unsafe void Init(Window window)
        {
            GL gl = window.gl;

            vao = gl.GenVertexArray();
            vbo = gl.GenBuffer();
            ebo = gl.GenBuffer();

            gl.Enable(EnableCap.Blend);
            gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public static unsafe void Render2D(Window window, Sprite sprite, Transform transform)
        {
            GL gl = window.gl;

            gl.BindVertexArray(vao);

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (void* p = sprite.vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(sprite.vertices.Length * sizeof(float)), p, BufferUsageARB.StaticDraw);
            }

            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            fixed (void* p = sprite.indices)
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(sprite.indices.Length * sizeof(uint)), p, BufferUsageARB.StaticDraw);
            }

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, GLEnum.Float, false, stride, (void*)0);

            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, (void*)(3 * sizeof(float)));

            // Setting model (sprite transform) uniform
            Maths.Vector2 pos = transform.Position;
            Maths.Vector2 scale = transform.Scale;
            float angle = transform.Rotation;

            Matrix4x4 model = Matrix4x4.CreateScale(scale.X * sprite.texture.Width, scale.Y * sprite.texture.Height, 1) *
                Matrix4x4.CreateRotationZ(Maths.MathHelper.DegToRad(angle)) *
                Matrix4x4.CreateTranslation(pos.X, pos.Y, 0);
            int modelLoc = gl.GetUniformLocation(sprite.program, "model");
            if (modelLoc != -1)
            {
                gl.UniformMatrix4(modelLoc, 1, false, (float*)&model);
            }
            else
            {
                Console.WriteLine("NO UNIFORM FOUND!");
            }

            // Setting projection (camera) uniform
            float hh = window.Size.X / 2;
            float wh = window.Size.Y / 2;

            Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(-hh, hh, -wh, wh, -1, 1);

            int projectionLoc = gl.GetUniformLocation(sprite.program, "projection");
            if (projectionLoc != -1)
            {
                gl.UniformMatrix4(projectionLoc, 1, false, (float*)&projection);
            }
            else
            {
                Console.WriteLine("NO UNIFORM FOUND!");
            }

            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, sprite.texture.textureID);
            int textureLoc = gl.GetUniformLocation(sprite.program, "uTexture");
            if (textureLoc != -1)
            {
                gl.Uniform1(textureLoc, 0);
            }
            else
            {
                Console.WriteLine("NO UNIFORM FOUND!");
            }

            gl.UseProgram(sprite.program);

            gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);

            gl.BindTexture(TextureTarget.Texture2D, 0);

            gl.BindVertexArray(0);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        }

        //Render 9 sliced sprite
        public static void Render9Sliced(Window window, Sprite sliced, Transform transform)
        {
            if (sliced.is9Sliced)
            {
                List<(Sprite sprite, Transform transform)> objectsToRender = new List<(Sprite sprite, Transform transform)>();

                Transform[] cornerTr = new Transform[4] { new Transform(), new Transform(), new Transform(), new Transform() };
                Transform[] sideTr = new Transform[4] { new Transform(), new Transform(), new Transform(), new Transform() };
                Transform centerTr = new Transform();

                //Side metrics
                int sideW = sliced.side.texture.Width;

                //Center metrics
                int centerW = sliced.center.texture.Width;
                int centerH = sliced.center.texture.Height;

                //Original 1x1 scaled sprite whole metrics
                int origW = sideW * 2 + centerW;
                int origH = sideW * 2 + centerH;

                //Scaling for center sprite
                float centerScalingX = (origW * transform.Scale.X - sideW * 2) / centerW;
                float centerScalingY = (origH * transform.Scale.Y - sideW * 2) / centerH;

                //Positions
                float PosX = centerW * centerScalingX / 2 + sideW / 2 + transform.Position.X;
                float PosY = centerH * centerScalingY / 2 + sideW / 2 + transform.Position.Y;

                //Set corner positions
                //Left-Top
                cornerTr[0].Position = new Maths.Vector2(-PosX, PosY);

                //Left-Bottom
                cornerTr[1].Position = new Maths.Vector2(-PosX, -PosY);
                cornerTr[1].Rotation = 90;

                //Right-Top
                cornerTr[2].Position = new Maths.Vector2(PosX, PosY);
                cornerTr[2].Rotation = 270;

                //Right-Bottom
                cornerTr[3].Position = new Maths.Vector2(PosX, -PosY);
                cornerTr[3].Rotation = 180;

                //Set side positions
                //Left
                sideTr[0].Position = new Maths.Vector2(-PosX, transform.Position.Y);
                sideTr[0].Scale.Y = centerScalingY;
                sideTr[0].Rotation = 0;

                //Right
                sideTr[1].Position = new Maths.Vector2(PosX, transform.Position.Y);
                sideTr[1].Rotation = 180;
                sideTr[1].Scale.Y = centerScalingY;

                //Top
                sideTr[2].Position = new Maths.Vector2(transform.Position.X, PosY);
                sideTr[2].Rotation = 270;
                sideTr[2].Scale.Y = centerScalingX;

                //Bottom
                sideTr[3].Position = new Maths.Vector2(transform.Position.X, -PosY);
                sideTr[3].Rotation = 90;
                sideTr[3].Scale.Y = centerScalingX;

                //Set center scale
                centerTr.Position = transform.Position;
                centerTr.Scale = new Maths.Vector2(centerScalingX, centerScalingY);

                // Add to render list
                // center
                objectsToRender.Add((sliced.center, centerTr));

                // Sides
                objectsToRender.Add((sliced.side, sideTr[0]));
                objectsToRender.Add((sliced.side, sideTr[1]));
                objectsToRender.Add((sliced.side, sideTr[2]));
                objectsToRender.Add((sliced.side, sideTr[3]));

                // Corners
                objectsToRender.Add((sliced.corner, cornerTr[0]));
                objectsToRender.Add((sliced.corner, cornerTr[1]));
                objectsToRender.Add((sliced.corner, cornerTr[2]));
                objectsToRender.Add((sliced.corner, cornerTr[3]));

                // Rendering all objects
                foreach (var obj in objectsToRender)
                {
                    Render2D(window, obj.sprite, obj.transform);
                }
            }
            else
            {
                Console.WriteLine("Given sprite is not 9 sliced!");
            }
        }

    }
}
