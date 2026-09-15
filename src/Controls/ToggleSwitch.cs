using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NetworkSwitcher
{
    /// <summary>
    /// Кастомный переключатель (тумблер) с анимированным бегунком и поддержкой
    /// светлой/тёмной темы через <see cref="ApplyTheme(bool)"/>.
    /// </summary>
    public partial class ToggleSwitch : Control
    {
        /// <summary>Текущее состояние переключателя.</summary>
        private bool isChecked;

        /// <summary>Возникает при изменении состояния <see cref="Checked"/>.</summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Возвращает или задаёт состояние переключателя.
        /// При изменении вызывает <see cref="CheckedChanged"/> и перерисовывает контрол.
        /// </summary>
        public bool Checked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    OnCheckedChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        /// <summary>Защищённый вызов события <see cref="CheckedChanged"/>.</summary>
        protected virtual void OnCheckedChanged(EventArgs e) => CheckedChanged?.Invoke(this, e);

        /// <summary>Цвет дорожки в выключенном состоянии.</summary>
        public Color TrackColorOff { get; set; } = Color.FromArgb(200, 200, 200);

        /// <summary>Цвет дорожки во включённом состоянии.</summary>
        public Color TrackColorOn { get; set; } = Color.FromArgb(123, 47, 190);

        /// <summary>Цвет бегунка (кружка).</summary>
        public Color ThumbColor { get; set; } = Color.White;

        /// <summary>Ширина дорожки переключателя в пикселях.</summary>
        private const int ToggleWidth = 50;

        /// <summary>Высота дорожки переключателя в пикселях.</summary>
        private const int ToggleHeight = 26;

        /// <summary>
        /// Создаёт новый экземпляр <see cref="ToggleSwitch"/> с двойной буферизацией
        /// и поддержкой прозрачного фона.
        /// </summary>
        public ToggleSwitch()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            Size = new Size(220, 30);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        /// <summary>Переключает состояние по клику мыши.</summary>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Checked = !Checked;
        }

        /// <summary>Рисует дорожку, бегунок и текст переключателя.</summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int trackX = 2;
            int trackY = (Height - ToggleHeight) / 2;
            Rectangle trackRect = new Rectangle(trackX, trackY, ToggleWidth, ToggleHeight);

            using (GraphicsPath trackPath = GetCapsulePath(trackRect))
            using (Brush trackBrush = new SolidBrush(Checked ? TrackColorOn : TrackColorOff))
            {
                g.FillPath(trackBrush, trackPath);
            }

            int margin = 3;
            int thumbDiameter = ToggleHeight - (margin * 2);
            int thumbX = Checked ? (trackRect.Right - margin - thumbDiameter) : (trackRect.Left + margin);
            int thumbY = trackY + margin;
            Rectangle thumbRect = new Rectangle(thumbX, thumbY, thumbDiameter, thumbDiameter);

            using (Brush thumbBrush = new SolidBrush(ThumbColor))
            {
                g.FillEllipse(thumbBrush, thumbRect);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                int textX = trackRect.Right + 12;
                Rectangle textRect = new Rectangle(textX, 0, Width - textX, Height);
                using (Brush textBrush = new SolidBrush(ForeColor))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(Text, Font, textBrush, textRect, sf);
                }
            }
        }

        /// <summary>
        /// Строит замкнутый путь в форме капсулы (скруглённого прямоугольника)
        /// для дорожки переключателя.
        /// </summary>
        private GraphicsPath GetCapsulePath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = rect.Height;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            path.AddArc(arc, 90, 180);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 180);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Переключает цветовую схему переключателя между тёмной и светлой темой.
        /// </summary>
        /// <param name="isDark"><c>true</c> — тёмная тема, <c>false</c> — светлая.</param>
        public void ApplyTheme(bool isDark)
        {
            if (isDark)
            {
                TrackColorOff = Color.FromArgb(70, 70, 70);
                TrackColorOn = Color.FromArgb(147, 51, 234);
                ThumbColor = Color.FromArgb(230, 230, 230);
            }
            else
            {
                TrackColorOff = Color.FromArgb(210, 210, 215);
                TrackColorOn = Color.FromArgb(123, 47, 190);
                ThumbColor = Color.White;
            }
            Invalidate();
        }
    }
}