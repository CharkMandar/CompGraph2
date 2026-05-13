using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CompGraph2
{
    public partial class Form1 : Form
    {
        private GLControl gl;

        private float rotX = 20f;
        private float rotY = 30f;

        private Point lastMouse;
        private bool dragging = false;

        private List<Primitive> _buildingParts = new List<Primitive>();

        private float _cameraDistance = 15f;  // Расстояние камеры от центра (по умолчанию 15)
        private float _minDistance = 5f;       // Минимальное приближение
        private float _maxDistance = 40f;      // Максимальное отдаление

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

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 16;
            timer.Tick += (s, e) => gl.Invalidate();
            timer.Start();
        }

        // ================= INIT =================
        private void Gl_Load(object sender, EventArgs e)
        {
            gl.MakeCurrent();

            GL.ClearColor(0.15f, 0.15f, 0.2f, 1f);
            GL.Enable(EnableCap.DepthTest);
        }

        // ================= RESIZE =================
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
            // Изменяем расстояние в зависимости от направления колёсика
            // e.Delta > 0 - крутим вперёд (приближение)
            // e.Delta < 0 - крутим назад (отдаление)
            _cameraDistance -= e.Delta * 0.01f;

            // Ограничиваем минимальное и максимальное расстояние
            _cameraDistance = MathHelper.Clamp(_cameraDistance, _minDistance, _maxDistance);

            // Запрашиваем перерисовку
            gl.Invalidate();
        }

        // ================= RENDER =================
        private void Gl_Paint(object sender, PaintEventArgs e)
        {
            gl.MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // камера
            GL.LoadIdentity();

            Matrix4 lookat = Matrix4.LookAt(
                _cameraDistance, _cameraDistance, _cameraDistance,  // позиция камеры (меняется)
                0, 0, 0,                                             // точка, на которую смотрим
                0, 1, 0);                                            // вектор "вверх"
            GL.LoadMatrix(ref lookat);

            // вращение сцены мышью
            GL.Rotate(rotX, 1, 0, 0);
            GL.Rotate(rotY, 0, 1, 0);

            //DrawAxes();
            //DrawCube();
            Draw3DModel();

            gl.SwapBuffers();
        }

        // ================= MOUSE =================
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
            if (_buildingParts.Count == 0)
                BuildAll();

            foreach (var part in _buildingParts)
            {
                part.Draw();
            }
        }

        private void BuildAll()
        {

        }
    }
}
