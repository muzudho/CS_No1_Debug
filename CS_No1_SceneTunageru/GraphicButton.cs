using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace Gs_No1
{
    public class GraphicButton
    {

        /// <summary>
        /// コンテンツに割り当てられる通し番号です。物を特定するために重複させない数字です。
        /// アプリケーション起動時～終了までの間だけ変わりません。
        /// </summary>
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }




        /// <summary>
        /// 後景色。
        /// </summary>
        private Color backColor;

        /// <summary>
        /// 選択中。
        /// </summary>
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
            }
        }

        /// <summary>
        /// マウスカーソルが合わさっています。
        /// </summary>
        private bool isMouseOvered;
        public bool IsMouseOvered
        {
            get
            {
                return this.isMouseOvered;
            }
            set
            {
                this.isMouseOvered = value;
            }
        }

        /// <summary>
        /// 境界線。
        /// </summary>
        private Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return this.bounds;
            }
            set
            {
                this.bounds = value;
            }
        }

        /// <summary>
        /// 画像ファイルパス。
        /// </summary>
        private string filePath;
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }

        /// <summary>
        /// 画像。
        /// </summary>
        private Image image;
        public Image Image
        {
            get
            {
                return this.image;
            }
        }

        /// <summary>
        /// 実行プログラム。スイッチオン時。
        /// </summary>
        public delegate void ButtonAction();
        private ButtonAction mouseUpAction;
        public ButtonAction SwitchOnAction
        {
            get
            {
                return this.mouseUpAction;
            }
            set
            {
                this.mouseUpAction = value;
            }
        }

        /// <summary>
        /// 実行プログラム。ラジオボタンがスイッチオフになった時。
        /// </summary>
        private ButtonAction radioReleaseAction;
        public ButtonAction RadioOffAction
        {
            get
            {
                return this.radioReleaseAction;
            }
            set
            {
                this.radioReleaseAction = value;
            }
        }

        /// <summary>
        /// 他のボタンと識別するために使う名前を入れます。
        /// </summary>
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// 表示の有無。
        /// </summary>
        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.isVisible = value;
            }
        }

        public GraphicButton(int id)
        {
            this.Id = id;

            this.name = "";
            this.backColor = Color.White;
            this.bounds = new Rectangle(0,0,50,50);
            this.mouseUpAction = () => {};
            this.radioReleaseAction = () => {};
            this.filePath = "";
            this.isVisible = true;
        }

        public void Load()
        {
            try
            {
                this.image = System.Drawing.Image.FromFile(this.filePath);
            }
            catch (Exception)
            {
                // ビジュアルエディターでは、フォルダー階層が異なることにより、画像ファイルパスが変わっていて読込みに失敗することがあります。
            }
        }

        public void Paint(Graphics g)
        {
            if (this.isVisible)
            {
                // 画像
                if (null != this.image)
                {
                    g.DrawImage(this.image, this.Bounds);
                }

                // 枠線の太さ
                float weight;
                if (this.isMouseOvered)
                {
                    weight = 4.0f;
                }
                else
                {
                    weight = 2.0f;
                }

                // 枠線の色
                Pen pen;
                if (this.isSelected)
                {
                    pen = new Pen(Color.Green, weight);
                }
                else
                {
                    pen = new Pen(Color.Black, weight);
                }

                // 枠線
                Rectangle bounds2 = new Rectangle(
                        this.bounds.X,
                        this.bounds.Y,
                        this.bounds.Width - 2,
                        this.bounds.Height - 2
                        );
                g.DrawRectangle(pen, bounds2);

                // 半透明の緑色で塗りつぶし
                if (this.isSelected)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 255, 0)), bounds2);
                }

                // デバッグ設定
                if (UiMain.VisibleId)
                {
                    g.DrawString(this.Id.ToString(), new Font(UiMain.FontName, 12.0f), Brushes.Red, this.Bounds.Location);
                }
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            this.mouseUpAction();
        }

        public void OnRadioRelease(object sender, MouseEventArgs e)
        {
            this.radioReleaseAction();
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (this.isVisible)
            {
                if (this.Bounds.Contains(e.Location))
                {
                    //ystem.Console.WriteLine("範囲内。mouse(" + e.X + "," + e.Y + ") bounds(" + this.Bounds.X + "," + this.Bounds.Y + "," + this.Bounds.Width + "," + this.Bounds.Height + ")");
                    this.isSelected = true;
                }
                else
                {
                    //ystem.Console.WriteLine("境界外。");
                    this.isSelected = false;
                }
            }
        }

        /// <summary>
        /// 表示しているとき、指定座標が境界内なら真。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsHit(Point location)
        {
            return this.IsVisible && this.Bounds.Contains(location);
        }

        /// <summary>
        /// マウスが合わさっているかどうかを判定し、状態変更します。
        /// </summary>
        /// <param name="location"></param>
        public void CheckMouseOver(Point location, ref bool forcedOff)
        {
            if (forcedOff)
            {
                this.IsMouseOvered = false;
            }
            else
            {
                if (this.IsHit(location))
                {
                    this.IsMouseOvered = true;
                    forcedOff = true;
                }
                else
                {
                    this.IsMouseOvered = false;
                }
            }
        }

    }
}
