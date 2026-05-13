using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CompGraph2
{
    public class Box : Primitive
    {
        public float Width { get; set; }   // по оси X
        public float Height { get; set; }  // по оси Y
        public float Depth { get; set; }   // по оси Z

        public Box(float width, float height, float depth)
        {
            Width = width;
            Height = height;
            Depth = depth;
        }

        public override void Draw()
        {
            GL.PushMatrix();
            ApplyTransformations();
            GL.Color3(Color);

            float w = Width / 2f;
            float h = Height / 2f;
            float d = Depth / 2f;

            GL.Begin(PrimitiveType.Quads);

            // Передняя грань (Z = d)
            GL.Normal3(0, 0, 1);
            GL.Vertex3(-w, -h, d);
            GL.Vertex3(w, -h, d);
            GL.Vertex3(w, h, d);
            GL.Vertex3(-w, h, d);

            // Задняя грань (Z = -d)
            GL.Normal3(0, 0, -1);
            GL.Vertex3(-w, -h, -d);
            GL.Vertex3(-w, h, -d);
            GL.Vertex3(w, h, -d);
            GL.Vertex3(w, -h, -d);

            // Верхняя грань (Y = h)
            GL.Normal3(0, 1, 0);
            GL.Vertex3(-w, h, -d);
            GL.Vertex3(-w, h, d);
            GL.Vertex3(w, h, d);
            GL.Vertex3(w, h, -d);

            // Нижняя грань (Y = -h)
            GL.Normal3(0, -1, 0);
            GL.Vertex3(-w, -h, -d);
            GL.Vertex3(w, -h, -d);
            GL.Vertex3(w, -h, d);
            GL.Vertex3(-w, -h, d);

            // Левая грань (X = -w)
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(-w, -h, -d);
            GL.Vertex3(-w, -h, d);
            GL.Vertex3(-w, h, d);
            GL.Vertex3(-w, h, -d);

            // Правая грань (X = w)
            GL.Normal3(1, 0, 0);
            GL.Vertex3(w, -h, -d);
            GL.Vertex3(w, h, -d);
            GL.Vertex3(w, h, d);
            GL.Vertex3(w, -h, d);

            GL.End();
            GL.PopMatrix();
        }
    }
}
