using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DoodleControls;


namespace MazeDoodle
{
    public partial class SelectTextureForm : Form
    {
        public SelectTextureForm()
        {
            InitializeComponent();

            CenterToParent();

            tex_ = new Bitmap(TerranceRender.GetTextureWithGridFilePath());

            SetClientSizeCore(client_width, client_height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics dc = e.Graphics;

            RectangleF dstRect = new RectangleF(0, 0, client_width, client_height);
            RectangleF srcRect = dstRect;
            dc.DrawImage(tex_, dstRect, srcRect, GraphicsUnit.Pixel);
        }

        protected override void  OnMouseClick(MouseEventArgs e)
        {
            //base.OnClick(e);

            int rr = e.Y / TerranceRender.GridPx;
            int cc = e.X / TerranceRender.GridPx;

            if (click_row_ >= 0 && click_row_ < TerranceRender.MaxTerranceRow &&
                click_col_ >= 0 && click_row_ < TerranceRender.MaxTerranceColumn)
            {
                click_row_ = rr;
                click_col_ = cc;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public int ClickRow
        {
            get
            {
                return click_row_;
            }
        }

        public int ClickCol
        {
            get
            {
                return click_col_;
            }
        }

        private int click_row_;

        private int click_col_;
        
        private readonly Bitmap tex_;

        private int client_width = TerranceRender.MaxTerranceColumn * TerranceRender.GridPx;

        private int client_height = TerranceRender.MaxTerranceRow * TerranceRender.GridPx;

        [DllImport("kernel32")]
        private static extern Boolean IsDebuggerPresent();
    }
}
