using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CompGraph2
{
    public class Cylinder : Primitive
    {
        public float Radius { get; set; }
        public float Height { get; set; }
        public int Segments { get; set; } = 24; // количество сегментов по окружности

        public Cylinder(float radius, float height, int segments = 24)
        {
            Radius = radius;
            Height = height;
            Segments = segments;
        }

        public override void Draw()
        {
            GL.PushMatrix();
            ApplyTransformations();
            GL.Color3(Color);

            float h = Height / 2f;

            // Боковая поверхность
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < Segments; i++)
            {
                double angle1 = 2 * Math.PI * i / Segments;
                double angle2 = 2 * Math.PI * (i + 1) / Segments;

                float x1 = (float)(Radius * Math.Cos(angle1));
                float z1 = (float)(Radius * Math.Sin(angle1));
                float x2 = (float)(Radius * Math.Cos(angle2));
                float z2 = (float)(Radius * Math.Sin(angle2));

                // Нормаль для каждого квадрата
                GL.Normal3(x1, 0, z1);
                GL.Vertex3(x1, -h, z1);
                GL.Vertex3(x2, -h, z2);
                GL.Vertex3(x2, h, z2);
                GL.Vertex3(x1, h, z1);
            }
            GL.End();

            // Верхняя крышка
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(0, h, 0);
            for (int i = 0; i <= Segments; i++)
            {
                double angle = 2 * Math.PI * i / Segments;
                float x = (float)(Radius * Math.Cos(angle));
                float z = (float)(Radius * Math.Sin(angle));
                GL.Vertex3(x, h, z);
            }
            GL.End();

            // Нижняя крышка
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Normal3(0, -1, 0);
            GL.Vertex3(0, -h, 0);
            for (int i = 0; i <= Segments; i++)
            {
                double angle = 2 * Math.PI * i / Segments;
                float x = (float)(Radius * Math.Cos(angle));
                float z = (float)(Radius * Math.Sin(angle));
                GL.Vertex3(x, -h, z);
            }
            GL.End();

            GL.PopMatrix();
        }
    }
}
