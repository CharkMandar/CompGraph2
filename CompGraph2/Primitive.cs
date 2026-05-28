// Primitive.cs
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CompGraph2
{
    public class Primitive
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; } // в градусах
        public Vector3 Scale { get; set; } = Vector3.One;
        public Vector3 Color { get; set; }
        public PrimitiveType Type { get; set; }

        public enum PrimitiveType
        {
            Cube,
            Cylinder,
            Sphere
        }

        public void Draw()
        {
            GL.PushMatrix();

            // Применяем трансформации
            GL.Translate(Position);
            GL.Rotate(Rotation.X, 1, 0, 0);
            GL.Rotate(Rotation.Y, 0, 1, 0);
            GL.Rotate(Rotation.Z, 0, 0, 1);
            GL.Scale(Scale);

            GL.Color3(Color);

            switch (Type)
            {
                case PrimitiveType.Cube:
                    DrawCube();
                    break;
                case PrimitiveType.Cylinder:
                    DrawCylinder();
                    break;
                case PrimitiveType.Sphere:
                    DrawSphere();
                    break;
            }

            GL.PopMatrix();
        }

        private void DrawCube()
        {
            // Используем TriangleStrip для куба
            float[] vertices = {
                // Передняя грань
                -0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                 // Задняя грань
                 0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f
            };

            int[] indices = {
                0,1,2, 1,3,2, // перед
                4,5,6, 5,7,6, // зад
                5,0,7, 0,2,7, // лево
                1,4,3, 4,6,3, // право
                2,3,7, 3,6,7, // верх
                5,4,0, 4,1,0  // низ
            };

            GL.Begin(BeginMode.Triangles);
            foreach (int index in indices)
            {
                int i = index * 3;
                GL.Vertex3(vertices[i], vertices[i + 1], vertices[i + 2]);
            }
            GL.End();
        }

        private void DrawCylinder()
        {
            int segments = 32;
            float radius = 0.5f;
            float height = 1.0f;

            // Боковая поверхность
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * 2.0f * MathF.PI / segments;
                float angle2 = (i + 1) * 2.0f * MathF.PI / segments;

                float x1 = MathF.Cos(angle1) * radius;
                float z1 = MathF.Sin(angle1) * radius;
                float x2 = MathF.Cos(angle2) * radius;
                float z2 = MathF.Sin(angle2) * radius;

                GL.Begin(BeginMode.TriangleStrip);
                GL.Normal3(MathF.Cos(angle1), 0, MathF.Sin(angle1));
                GL.Vertex3(x1, height / 2, z1);
                GL.Vertex3(x1, -height / 2, z1);

                GL.Normal3(MathF.Cos(angle2), 0, MathF.Sin(angle2));
                GL.Vertex3(x2, height / 2, z2);
                GL.Vertex3(x2, -height / 2, z2);
                GL.End();
            }

            // Верхняя крышка
            GL.Begin(BeginMode.TriangleFan);
            GL.Normal3(0, 1, 0);
            GL.Vertex3(0, height / 2, 0);
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2.0f * MathF.PI / segments;
                float x = MathF.Cos(angle) * radius;
                float z = MathF.Sin(angle) * radius;
                GL.Vertex3(x, height / 2, z);
            }
            GL.End();

            // Нижняя крышка
            GL.Begin(BeginMode.TriangleFan);
            GL.Normal3(0, -1, 0);
            GL.Vertex3(0, -height / 2, 0);
            for (int i = segments; i >= 0; i--)
            {
                float angle = i * 2.0f * MathF.PI / segments;
                float x = MathF.Cos(angle) * radius;
                float z = MathF.Sin(angle) * radius;
                GL.Vertex3(x, -height / 2, z);
            }
            GL.End();
        }

        private void DrawSphere()
        {
            int stacks = 16;
            int slices = 32;
            float radius = 0.5f;

            for (int i = 0; i < stacks; i++)
            {
                float lat0 = MathF.PI * (-0.5f + (float)i / stacks);
                float lat1 = MathF.PI * (-0.5f + (float)(i + 1) / stacks);

                float y0 = MathF.Sin(lat0) * radius;
                float yr0 = MathF.Cos(lat0) * radius;

                float y1 = MathF.Sin(lat1) * radius;
                float yr1 = MathF.Cos(lat1) * radius;

                GL.Begin(BeginMode.TriangleStrip);
                for (int j = 0; j <= slices; j++)
                {
                    float lng = 2 * MathF.PI * (float)j / slices;
                    float x = MathF.Cos(lng);
                    float z = MathF.Sin(lng);

                    GL.Normal3(x * MathF.Cos(lat0), MathF.Sin(lat0), z * MathF.Cos(lat0));
                    GL.Vertex3(x * yr0, y0, z * yr0);

                    GL.Normal3(x * MathF.Cos(lat1), MathF.Sin(lat1), z * MathF.Cos(lat1));
                    GL.Vertex3(x * yr1, y1, z * yr1);
                }
                GL.End();
            }
        }
    }
}