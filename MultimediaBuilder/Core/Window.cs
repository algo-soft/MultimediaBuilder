using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using MultimediaBuilder.Maths;
using MultimediaBuilder.Graphics;
using System.Drawing;

namespace MultimediaBuilder
{
    public class Window
    {
        //private variables
        private IWindow window;
        private List<IRenderable> renderables = new List<IRenderable>();

        //public variables

        //OpenGL instance
        public GL gl;

        //Lists for loading shader/texture image
        public Dictionary<Sprite, string> loadShaderAndTexture = new Dictionary<Sprite, string>();
        public List<Sprite> loadShader = new List<Sprite>();

        //Window size
        public Vector2 Size = Vector2.Zero;

        //Window BG color
        public Color BGColor = Color.DarkGray;
        
        //public methods
        public void StartWindow()
        {
            // Create a new window with a given configuration
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "MultimediaBuilder";

            window = Silk.NET.Windowing.Window.Create(options);

            // Set up the event hooks
            window.Load += OnLoad;
            window.Render += OnRender;
            window.Closing += OnClose;
            window.Update += OnUpdate;
            window.FramebufferResize += OnFramebufferResize;

            // Run the window
            window.Run();
        }

        public void AddRenderable(IRenderable renderable)
        {
            renderables.Add(renderable);
        }

        public void AddRenderables(IEnumerable<IRenderable> renderables)
        {
            this.renderables.AddRange(renderables);
        }

        //Main events
        private void OnLoad()
        {
            gl = window.CreateOpenGL();
            Renderer.Init(this);

            foreach ((Sprite spr, string path) in loadShaderAndTexture)
            {
                spr.Init(gl);
                spr.texture.GenTexture(gl, path);
            }

            foreach (Sprite spr in loadShader)
            {
                spr.Init(gl);
            }

            Size = new Vector2(window.Size.X, window.Size.Y);
            gl.ClearColor(BGColor);
        }

        private void OnRender(double arg)
        {
            gl.Clear(ClearBufferMask.ColorBufferBit);

            renderables.Sort((r1, r2) => r2.Order.CompareTo(r1.Order));

            foreach (IRenderable rend in renderables)
            {
                if (rend.Sprite.is9Sliced)
                {
                    Renderer.Render9Sliced(this, rend.Sprite, rend.Transform);
                }
                else
                {
                    Renderer.Render2D(this, rend.Sprite, rend.Transform);
                }
            }
        }

        private void OnUpdate(double arg)
        {

        }

        private void OnFramebufferResize(Vector2D<int> newSize)
        {
            Size = new Vector2(newSize.X, newSize.Y);
            gl.Viewport(newSize);
        }

        private void OnClose()
        {
            gl?.Dispose();
        }
    }
}