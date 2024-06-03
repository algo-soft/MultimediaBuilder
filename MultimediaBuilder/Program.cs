using MultimediaBuilder.Graphics;
using MultimediaBuilder;
using MultimediaBuilder.Maths;

class Program
{
    static void Main(string[] args)
    {
        //Our window
        Window window = new Window();

        //Add simple object so it can be rendered
        SimpleObject simple = new SimpleObject();

        Sprite spr = new Sprite(window, "C:\\Users\\user\\Desktop\\Buttons\\none.png");
        spr.is9Sliced = true;
        spr.corner = new Sprite(window, "C:\\Users\\user\\Desktop\\Playerr1.png");
        spr.side = new Sprite(window, "C:\\Users\\user\\Desktop\\Buttons\\Default\\side.png");
        spr.center = new Sprite(window, "C:\\Users\\user\\Desktop\\Buttons\\Default\\center.png");

        simple.Sprite = spr;
        simple.Transform.Scale = new MultimediaBuilder.Maths.Vector2(1, 1);

        SimpleObject simple2 = new SimpleObject();
        simple2.Sprite = new Sprite(window, "C:\\Users\\user\\Desktop\\Playerr1.png");
        simple2.Transform.Position.X = 90;

        window.AddRenderable(simple);
        window.AddRenderable(simple2);

        window.StartWindow();
    }
}

class SimpleObject : IRenderable
{
    public Sprite Sprite { get; set; }
    public Transform Transform { get; set; } = new Transform();
    public int Order { get; set; } = 0;
}