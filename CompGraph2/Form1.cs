// Form1.cs
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CompGraph2
{
    public partial class Form1 : Form
    {
        private GLControl gl;
        private DateTime _lastFrameTime;
        private float _angle = 0f;
        private float _speed = 90f; // градусов в секунду
        private bool _isRunning = true;

        private float rotX = 20f;
        private float rotY = 30f;

        private Point lastMouse;
        private bool dragging = false;

        private List<Primitive> _buildingParts = new List<Primitive>();

        private float _cameraDistance = 8f;
        private float _minDistance = 2f;
        private float _maxDistance = 20f;

        public Form1()
        {
            InitializeComponent();

            gl = new GLControl();
            gl.Dock = DockStyle.Fill;

            gl.Load += Gl_Load;
            gl.Paint += Gl_Paint;
            gl.Resize += Gl_Resize;

            gl.MouseDown += Gl_MouseDown;
            gl.MouseUp += Gl_MouseUp;
            gl.MouseMove += Gl_MouseMove;
            gl.MouseWheel += GlControl1_MouseWheel;

            Controls.Add(gl);

            _lastFrameTime = DateTime.Now;
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += (s, e) => gl.Invalidate();
            timer.Start();
        }

        private void Gl_Load(object sender, EventArgs e)
        {
            gl.MakeCurrent();
            GL.ClearColor(0.2f, 0.2f, 0.3f, 1f);
            GL.Enable(EnableCap.DepthTest);

            // Включаем освещение
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);

            float[] lightPos = { 5.0f, 10.0f, 5.0f, 1.0f };
            float[] lightAmbient = { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] lightDiffuse = { 0.8f, 0.8f, 0.8f, 1.0f };

            GL.Light(LightName.Light0, LightParameter.Position, lightPos);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightAmbient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse);
        }

        private void Gl_Resize(object sender, EventArgs e)
        {
            gl.MakeCurrent();

            GL.Viewport(0, 0, gl.Width, gl.Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)gl.Width / gl.Height,
                0.1f,
                100f
            );

            GL.LoadMatrix(ref proj);

            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void GlControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            _cameraDistance -= e.Delta * 0.01f;
            _cameraDistance = MathHelper.Clamp(_cameraDistance, _minDistance, _maxDistance);
            gl.Invalidate();
        }

        private void Gl_Paint(object sender, PaintEventArgs e)
        {
            // Вычисляем deltaTime для анимации
            var currentTime = DateTime.Now;
            double deltaTime = (currentTime - _lastFrameTime).TotalSeconds;
            _lastFrameTime = currentTime;

            // Обновляем анимацию
            UpdateAnimation(deltaTime);

            // Перестраиваем модель с новыми параметрами
            BuildAll();

            gl.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4 lookat = Matrix4.LookAt(
                _cameraDistance, _cameraDistance, _cameraDistance,
                0, 0, 0,
                0, 1, 0);
            GL.LoadMatrix(ref lookat);

            GL.Rotate(rotX, 1, 0, 0);
            GL.Rotate(rotY, 0, 1, 0);

            Draw3DModel();

            gl.SwapBuffers();
        }

        private void Gl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                lastMouse = e.Location;
            }
        }

        private void Gl_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Gl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging) return;

            float dx = e.X - lastMouse.X;
            float dy = e.Y - lastMouse.Y;

            rotY += dx * 0.5f;
            rotX += dy * 0.5f;

            lastMouse = e.Location;
        }

        private void Draw3DModel()
        {
            foreach (var part in _buildingParts)
            {
                part.Draw();
            }
        }

        private void BuildAll()
        {
            _buildingParts.Clear();

            // Параметры механизма
            float rad = MathHelper.DegreesToRadians(_angle);
            float crankRadius = 0.8f;  // радиус кривошипа
            float rodLength = 2.5f;     // длина шатуна

            // Позиция оси коленвала (неподвижная точка вращения)
            Vector3 crankAxisPos = new Vector3(0, -1.5f, 0);

            // Позиция шейки кривошипа
            Vector3 crankPinPos = crankAxisPos + new Vector3(
                MathF.Sin(rad) * crankRadius,
                0,
                MathF.Cos(rad) * crankRadius
            );

            // Вертикальное движение поршня (ось Y)
            // Поршень движется вертикально вверх-вниз
            float pistonY = crankAxisPos.Y + MathF.Sqrt(rodLength * rodLength -
                (crankPinPos.X - crankAxisPos.X) * (crankPinPos.X - crankAxisPos.X) -
                (crankPinPos.Z - crankAxisPos.Z) * (crankPinPos.Z - crankAxisPos.Z));

            // Позиция поршневого пальца
            Vector3 pistonPinPos = new Vector3(crankAxisPos.X, pistonY, crankAxisPos.Z);

            // Позиция поршня (центр)
            Vector3 pistonPos = new Vector3(pistonPinPos.X, pistonPinPos.Y - 0.3f, pistonPinPos.Z);

            // ===== ЦИЛИНДР =====
            // Корпус цилиндра
            float cylinderBottom = crankAxisPos.Y + 0.3f;
            float cylinderHeight = 4.0f;
            float cylinderCenterY = cylinderBottom + cylinderHeight / 2;

            //_buildingParts.Add(new Primitive
            //{
            //    Type = Primitive.PrimitiveType.Cylinder,
            //    Position = new Vector3(0, cylinderCenterY, 0),
            //    Scale = new Vector3(1.5f, cylinderHeight, 1.5f),
            //    Color = new Vector3(0.7f, 0.7f, 0.8f),
            //    Rotation = Vector3.Zero
            //});
            //
            //// Головка цилиндра (верхняя крышка)
            //_buildingParts.Add(new Primitive
            //{
            //    Type = Primitive.PrimitiveType.Cylinder,
            //    Position = new Vector3(0, cylinderBottom + cylinderHeight + 0.3f, 0),
            //    Scale = new Vector3(1.6f, 0.6f, 1.6f),
            //    Color = new Vector3(0.6f, 0.6f, 0.7f),
            //    Rotation = Vector3.Zero
            //});

            // ===== КАРТЕР =====
            //_buildingParts.Add(new Primitive
            //{
            //    Type = Primitive.PrimitiveType.Cube,
            //    Position = new Vector3(0, crankAxisPos.Y, 0),
            //    Scale = new Vector3(2.5f, 1.5f, 2.5f),
            //    Color = new Vector3(0.5f, 0.5f, 0.6f),
            //    Rotation = Vector3.Zero
            //});

            // ===== ПОРШЕНЬ =====
            // Головка поршня
            _buildingParts.Add(new Primitive
            {
                Type = Primitive.PrimitiveType.Cylinder,
                Position = pistonPinPos + new Vector3(0, 0.2f, 0),
                Scale = new Vector3(1.3f, 0.4f, 1.3f),
                Color = new Vector3(0.8f, 0.3f, 0.3f),
                Rotation = Vector3.Zero
            });

            // Юбка поршня
            _buildingParts.Add(new Primitive
            {
                Type = Primitive.PrimitiveType.Cylinder,
                Position = pistonPinPos - new Vector3(0, 0.5f, 0),
                Scale = new Vector3(1.3f, 0.8f, 1.3f),
                Color = new Vector3(0.7f, 0.3f, 0.3f),
                Rotation = Vector3.Zero
            });

            // ===== ШАТУН =====
            Vector3 rodDirection = pistonPinPos - crankPinPos;
            float currentRodLength = rodDirection.Length;
            Vector3 rodMiddle = (crankPinPos + pistonPinPos) / 2f;

            // Вычисляем углы поворота шатуна
            float rodAngleX = (float)Math.Atan2(rodDirection.Y, Math.Sqrt(rodDirection.X * rodDirection.X + rodDirection.Z * rodDirection.Z));
            float rodAngleZ = (float)Math.Atan2(rodDirection.X, rodDirection.Z);

            _buildingParts.Add(new Primitive
            {
                Type = Primitive.PrimitiveType.Cylinder,
                Position = rodMiddle,
                Scale = new Vector3(0.3f, currentRodLength, 0.3f),
                Color = new Vector3(0.6f, 0.6f, 0.7f),
                Rotation = new Vector3(
                    MathHelper.RadiansToDegrees(-rodAngleX),
                    0,
                    MathHelper.RadiansToDegrees(rodAngleZ)
                )
            });

            // ===== КОЛЕНЧАТЫЙ ВАЛ =====
            // Основная ось
            _buildingParts.Add(new Primitive
            {
                Type = Primitive.PrimitiveType.Cylinder,
                Position = crankAxisPos,
                Scale = new Vector3(0.3f, 2.5f, 0.3f),
                Color = new Vector3(0.5f, 0.5f, 0.6f),
                Rotation = new Vector3(0, 0, 90)
            });

            // Кривошип (рычаг от оси до шейки)
            Vector3 crankArmDir = crankPinPos - crankAxisPos;
            Vector3 crankArmMiddle = (crankAxisPos + crankPinPos) / 2f;
            float crankArmAngleX = (float)Math.Atan2(crankArmDir.Y, Math.Sqrt(crankArmDir.X * crankArmDir.X + crankArmDir.Z * crankArmDir.Z));
            float crankArmAngleZ = (float)Math.Atan2(crankArmDir.X, crankArmDir.Z);

            _buildingParts.Add(new Primitive
            {
                Type = Primitive.PrimitiveType.Cylinder,
                Position = crankArmMiddle,
                Scale = new Vector3(0.25f, crankRadius, 0.25f),
                Color = new Vector3(0.4f, 0.4f, 0.5f),
                Rotation = new Vector3(
                    MathHelper.RadiansToDegrees(-crankArmAngleX),
                    0,
                    MathHelper.RadiansToDegrees(crankArmAngleZ)
                )
            });

            // Шейка кривошипа
            _buildingParts.Add(new Primitive
            {
                Type = Primitive.PrimitiveType.Cylinder,
                Position = crankPinPos,
                Scale = new Vector3(0.35f, 0.8f, 0.35f),
                Color = new Vector3(0.6f, 0.6f, 0.7f),
                Rotation = new Vector3(0, 0, 90)
            });

            // Противовес
            Vector3 counterWeightOffset = -crankArmDir.normalized() * 0.4f;
            Vector3 counterWeightPos = crankAxisPos + new Vector3(counterWeightOffset.X, 0, counterWeightOffset.Z);

            _buildingParts.Add(new Primitive
            {
                Type = Primitive.PrimitiveType.Cube,
                Position = counterWeightPos,
                Scale = new Vector3(1.0f, 0.4f, 0.8f),
                Color = new Vector3(0.3f, 0.3f, 0.4f),
                Rotation = Vector3.Zero
            });
        }

        private void UpdateAnimation(double deltaTime)
        {
            if (!_isRunning) return;

            _angle += (float)(_speed * deltaTime);
            if (_angle >= 360) _angle -= 360;
        }

        // Обработчики клавиш для управления анимацией
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Space:
                    _isRunning = !_isRunning;
                    break;
                case Keys.Up:
                    _speed += 10f;
                    break;
                case Keys.Down:
                    _speed = Math.Max(0, _speed - 10f);
                    break;
                case Keys.R:
                    _angle = 0;
                    break;
            }
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 normalized(this Vector3 vector)
        {
            float length = vector.Length;
            if (length > 0)
                return vector / length;
            return vector;
        }
    }
}