namespace MultimediaBuilder.Graphics
{
    public interface IRenderable
    {
        public Sprite Sprite { get; set; }
        public Transform Transform { get; set; }

        public int Order { get; set; }
    }
}