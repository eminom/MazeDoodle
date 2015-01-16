
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using DoodleControls;

namespace TextureChoose
{
    public class TextureChoose : Control
    {
        public TextureChoose()
        {
            if (IsDebuggerPresent())
                tex_ = new Bitmap(@"E:\texture.png");
            else
                tex_ = new Bitmap(TerranceRender.GetTextureFilePath());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle dstRc = new Rectangle(0, 0, TerranceRender.GridPx, TerranceRender.GridPx);
            Rectangle srcRc = new Rectangle(current_tex_col_ * TerranceRender.GridPx,
                current_tex_row_ * TerranceRender.GridPx, TerranceRender.GridPx, TerranceRender.GridPx);
            e.Graphics.DrawImage(tex_, dstRc, srcRc, GraphicsUnit.Pixel);

            e.Graphics.DrawRectangle(Pens.AliceBlue,ClientRectangle);
        }

        public void SetTexture(int tr, int tc)
        {
            current_tex_row_ = tr;
            current_tex_col_ = tc;
            Invalidate();
        }

        public int TexRow
        {
            get
            {
                return current_tex_row_;
            }
        }

        public int TexColum
        {
            get
            {
                return current_tex_col_;
            }
        }

        private readonly Bitmap tex_ = null;

        private int current_tex_row_;
        private int current_tex_col_;

        [DllImport("kernel32")]
        private static extern Boolean IsDebuggerPresent();
    }
}
