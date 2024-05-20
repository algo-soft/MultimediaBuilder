using Engine;
using Engine.Input;
using Engine.UI;

class Prog
{
    //Some values
    static float vel;

    //Declaring main vars outside because we want to use it in other methods
    static Window window = new Window();
    static Camera cam = new Camera(window);
    static World main = new World();

    static void Main(string[] args)
    {
        //Setting starter world and camera
        window.SetCamera(cam);
        window.SetActiveWorld(main);

        //Init entity
        Entity entity = new Entity(EntitySample.Button);

        entity.transform.scale = new Vector2(10, 10);
        entity.transform.position = new Vector2(-150, 0);

        SpriteRenderer sprR = entity.GetComponent<SpriteRenderer>();
        Sprite spr = sprR.sprite;
        spr = new Sprite(window, new Image("C:\\Users\\user\\Desktop\\button1.png"));
        spr.is9Sliced = true;

        spr.left_bottom = new Image("C:\\Users\\user\\Desktop\\corner2.png");
        spr.right_bottom = new Image("C:\\Users\\user\\Desktop\\corner3.png");
        spr.left_top = new Image("C:\\Users\\user\\Desktop\\corner1.png");
        spr.right_top = new Image("C:\\Users\\user\\Desktop\\corner4.png");
        spr.left = new Image("C:\\Users\\user\\Desktop\\lr1.png");
        spr.right = new Image("C:\\Users\\user\\Desktop\\lr2.png");
        spr.bottom = new Image("C:\\Users\\user\\Desktop\\tb2.png");
        spr.top = new Image("C:\\Users\\user\\Desktop\\tb1.png");
        spr.center = new Image("C:\\Users\\user\\Desktop\\center.png");

        sprR.sprite = spr;

        entity.Init();

        entity.Update += EntityUpdate;
        entity.GetComponent<Button>().OnClick += OnButtonClick;

        Entity e = new Entity(EntitySample.Default);
        e.GetComponent<SpriteRenderer>().sprite = new Sprite(window, new Image("C:\\Users\\user\\Desktop\\Playerr1.png"));

        main.AddEntity(entity);
        main.AddEntity(e);

        //Run window
        window.Init();

        //Everything here wont work
    }

    static void EntityUpdate(Entity entity)
    {
        Button b = entity.GetComponent<Button>();

        if (b.isHolded)
        {
            
        }
    }

    static void OnButtonClick(Button b)
    {
        
    }
}
