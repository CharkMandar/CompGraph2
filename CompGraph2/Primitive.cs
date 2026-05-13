using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CompGraph2
{
    public abstract class Primitive
    {
        public Vector3 Position { get; set; }  // Позиция в мире
        public Vector3 Rotation { get; set; }  // Вращение (углы в градусах)
        public Vector3 Scale { get; set; }     // Масштаб
        public Color Color { get; set; }      // Цвет (или потом заменим на материал)

        protected Primitive()
        {
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
            Color = Color.Gray; // серый по умолчанию
        }

        public abstract void Draw();

        // Применяем трансформации перед рисованием
        protected void ApplyTransformations()
        {
            GL.Translate(Position.X, Position.Y, Position.Z);
            GL.Rotate(Rotation.X, 1, 0, 0);
            GL.Rotate(Rotation.Y, 0, 1, 0);
            GL.Rotate(Rotation.Z, 0, 0, 1);
            GL.Scale(Scale.X, Scale.Y, Scale.Z);
        }
    }
}
