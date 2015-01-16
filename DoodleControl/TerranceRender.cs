using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace DoodleControls
{
    public class TerranceRender
    {
        public TerranceRender()
        {
            tex_ = new Bitmap(GetTextureFilePath());

            String str = String.Format("Map Texture Width = {0}, Height = {1}", tex_.Width, tex_.Height);
            Console.WriteLine(str);
        }
        
        public void DrawTerrance(Graphics dc,int tex_id,float x, float y, float width, float height)
        {
            int r, c;
            DecodeTextureId(tex_id, out r, out c);

            if( r >= 0 && r < MaxTerranceRow && c >= 0 && c < MaxTerranceColumn ){
                RectangleF srcRc = new RectangleF( c * GridPx, r * GridPx, GridPx, GridPx );

                float dx = (float)((double)x - Math.Floor(x));
                float dy = (float)((double)y - Math.Floor(y));

                float dst_x = (float)Math.Floor(x);
                float dst_y = (float)Math.Floor(y);
                RectangleF dstRc = new RectangleF(dst_x,dst_y,width+dx+0.5f,height+dy+0.5f);
                dc.DrawImage(tex_, dstRc, srcRc, GraphicsUnit.Pixel);
            }
        }


        //public static String GetTextureFilePath()
        //{
        //    String file_name = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        //    String dir_name = Path.GetDirectoryName(file_name);
        //    return dir_name + TextureFileName;
        //}

        public static String GetTextureFilePath()
        {
            return TextureFileName;
        }

        //public static String GetTextureWithGridFilePath()
        //{
        //    String file_name = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        //    String dir_name = Path.GetDirectoryName(file_name);
        //    return dir_name + TextureWithGridFileName;
        //}

        public static String GetTextureWithGridFilePath()
        {
            return TextureWithGridFileName;
        }


        public static int EncodeTextureId(int tr, int tc)
        {
            return ((tr & 0xF) + ((tc & 0xF) << 4));
        }

        public static void DecodeTextureId(int tid, out int tr, out int tc)
        {
            tr = (tid & 0xF);
            tc = ((tid>>4)&0xF);
        }

        private readonly Bitmap tex_;

        public const int MaxTerranceRow = 12;

        public const int MaxTerranceColumn = 12;

        public const int GridPx = 40;

        private const String TextureFileName = @"texture.png";

        private const String TextureWithGridFileName = @"texture_grid.jpg";
    }
}