using MultimediaBuilder.Maths;
using Silk.NET.OpenGL;
using System.Reflection;

namespace MultimediaBuilder.Graphics
{
    public class Sprite
    {
        //our texture
        public Texture texture = new Texture();

        //Sprite data
        public float[] vertices = {
            // positions       // texture coordinates
            0.5f,  0.5f, 0.0f, 1f, 1f,
            0.5f, -0.5f, 0.0f, 1f, 0f,
            -0.5f, -0.5f, 0.0f, 0f, 0f,
            -0.5f,  0.5f, 0.0f, 0f, 1f
        };

        public uint[] indices = {
            0, 1, 3, // first triangle
            1, 2, 3  // second triangle
        };
        
        //shader program
        public uint program;

        //variables for 9 slicing
        public bool is9Sliced = false;

        /// <summary>
        /// Left-Top corner sprite
        /// </summary>
        public Sprite corner;

        /// <summary>
        /// Left side sprite
        /// </summary>
        public Sprite side;

        /// <summary>
        /// Center sprite
        /// </summary>
        public Sprite center;

        //Constructors
        public Sprite() { }

        //Add sprite to load lists so texture image/shader program can be loaded when window is created
        public Sprite(Texture texture, Window window)
        {
            this.texture = texture;

            window.loadShader.Add(this);
        }

        public Sprite(Window window, string path)
        {
            window.loadShaderAndTexture.Add(this, path);
        }

        //Create shader program
        public void Init(GL gl)
        {
            program = CreateShaderProgram(gl);
        }

        //Methods for loading shader
        private uint LoadShader(GL gl,ShaderType type, string source)
        {
            // Create shader object
            uint shader = gl.CreateShader(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);

            // Check for compilation errors
            gl.GetShader(shader, GLEnum.CompileStatus, out var status);
            if (status == 0)
            {
                var infoLog = gl.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling shader: {infoLog}");
            }

            return shader;
        }

        private uint CreateShaderProgram(GL gl)
        {
            //Getting current assembly
            var assembly = Assembly.GetExecutingAssembly();
            //Declaring shaders
            uint vertex;
            uint fragment;

            //Loding shaders
            using (Stream streamVert = assembly.GetManifestResourceStream("MultimediaBuilder.Core.Graphics.Shaders.VertexShader.glsl"))
            using (StreamReader readerV = new StreamReader(streamVert))
            {
                vertex = LoadShader(gl, ShaderType.VertexShader, readerV.ReadToEnd());
            }

            using (Stream streamFrag = assembly.GetManifestResourceStream("MultimediaBuilder.Core.Graphics.Shaders.FragmentShader.glsl"))
            using (StreamReader readerF = new StreamReader(streamFrag))
            {
                fragment = LoadShader(gl, ShaderType.FragmentShader, readerF.ReadToEnd());
            }

            uint program = gl.CreateProgram();
            gl.AttachShader(program, vertex);
            gl.AttachShader(program, fragment);
            gl.LinkProgram(program);

            // Check for linking errors
            gl.GetProgram(program, GLEnum.LinkStatus, out var isLinked);
            if (isLinked == 0)
            {
                var infoLog = gl.GetProgramInfoLog(program);
                throw new Exception($"Error linking program: {infoLog}");
            }

            gl.DetachShader(program, vertex);
            gl.DetachShader(program, fragment);
            gl.DeleteShader(vertex);
            gl.DeleteShader(fragment);

            return program;
        }
    }
}