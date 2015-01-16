using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace DoodleControls
{
    public delegate void DoodleMouseMove(int grid_x,int grid_y);
    public delegate void DoodleMouseClick(int grid_x,int grid_y);
    public delegate void DoodleTrapSetChanged();
    public delegate void DoodleCameraSetChanged();
    public delegate void DoodleControlStateChanged(DoodleControlState ds);
    public delegate void DoodleRemoteControlChanged();
    public delegate void DoodleLogicWallViewPropertyChanged(bool enabled);

    public enum KeeperPickState
    {
        Idle,
        PickingStartPoint,
        PickingRoutePoint,
    }

    public enum DoodleControlState{
        Idle,
        PickingTrap,
        PickingCamera,
        PickingRemoteControl
    }

    public class DoodleControl:Control
    {

        public DoodleControl()
        {
            ResetData(20,16);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
        }

        public List<DoodleKeeper> GetKeeperList()
        {
            return cake_.KeepersList;
        }

        public DoodleKeeper GetKeeperByIndex(int index)
        {
            return cake_.GetKeeperByIndex(index);
        }

        public List<ShadowPoint> GetShadowPointList()
        {
            return cake_.ShadowPoints;
        }

        public void ClearShadowPoints()
        {
            cake_.RemoveAllShadowPoints();
            Invalidate();
        }

        public bool SetShadowPointAt(int x, int y)
        {
            bool rv = cake_.SetShadowPointAt(x, y);
            Invalidate();
            return rv;
        }

        public void SetActiveKeeper(int new_index)
        {
            if (new_index != active_keeper_)
            {
                active_keeper_ = new_index;
                Invalidate();
            }
        }

        public void SetStartPoint(int row, int column)
        {
            cake_.SetStartPoint(row, column);
            Invalidate();
        }

        public void GetStartPoint(out int row, out int column)
        {
            cake_.GetStartPoint(out row, out column);
        }

        public void ClearAtPos(int x, int y)
        {
            cake_.ClearAtPos(x, y);
            Invalidate();
        }

        public void SetStairPortAt(int x, int y, int to_stair,int item_acq, bool clear_item)
        {
            cake_.SetStairPortAt(x, y, to_stair, item_acq, clear_item);
            Invalidate();
        }

        public void PutItemAt(int x, int y, int item, String specialty)
        {
            //~
            PutItemAt(x, y, item,specialty, true);
            //~ no invalidate
        }

        public void PutItemAt(int x, int y,int item,String specialty,bool clear_stair_port)
        {
            cake_.PutItemAt(x, y, item,specialty, clear_stair_port);
            Invalidate();
        }

        public void SetRockAt(int x, int y)
        {
            cake_.SetRockAt(x, y);
            Invalidate();
        }

        public void RemoveRockAt(int x, int y)
        {
            cake_.RemoveRockAt(x, y);
            Invalidate();
        }

        public void SetTexture(int x, int y, int t)
        {
            cake_.SetTexture(x, y, t, tex_edit_mask_);
            Invalidate();
        }

        public void RemoveTexture(int x, int y)
        {
            cake_.RemoveTexture(x, y,tex_edit_mask_);
            Invalidate();
        }

        public void ToggleWallAt(int x, int y)
        {
            cake_.ToggleWallAt(x, y);
            Invalidate();
        }

        //public void ResetSize(int r,int c)
        //{
        //    cake_.ResetSize(r, c);
        //    Invalidate();
        //}

        public void Enclose()
        {
            cake_.Enclose();
            Invalidate();
        }

        public void Disclose()
        {
            cake_.Disclose();
            Invalidate();
        }

        public void ResetMap(int r, int c, int[][] vals)
        {
            cake_.ResetMap(r, c, vals);
            Invalidate();
        }

        public void LoadFromJson(String fileName)
        {
            cake_ = new DoodleCake(fileName);
            active_keeper_ = -1;
            Invalidate();
        }

        public void ResetData(int r,int c)
        {
            cake_ = new DoodleCake();
            active_keeper_ = -1;
            cake_.ResetSize(r, c);
            Invalidate();
        }

        //~ tmd, manually exports to json format.
        //~ wtf
        public void WriteToJson(string fileName)
        {
            cake_.WriteToJson(fileName);
        }

        public void GetSize(out int r, out int c)
        {
            cake_.GetSize(out r, out c);
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        private void DrawTexture(Graphics dc)
        {
            Rectangle rc = ClientRectangle;
            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            for (int i = cake_.Row - 1; i >= 0; i--)
            {
                for (int j = cake_.Column - 1; j >= 0; j--)
                {
                    int t = cake_.GetTexture(i, j);
                    try
                    {
                        //if (t > 0)
                        //{
                        //    Brush t_brush = PredeinedBrush[t];
                        //    float x = hs * j;
                        //    float y = (cake_.Row - i) * vs;
                        //    //dc.FillRectangle(t_brush, x, y - vs, hs, vs);
                        //    tex_render_.DrawTerrance(dc, t,x, y - vs, hs, vs);
                        //}

                        float x = hs * j;
                        float y = (cake_.Row - i) * vs;
                        tex_render_.DrawTerrance(dc, t, x, y - vs, hs, vs);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        MessageBox.Show(ex.Message+"\r\n"+ex.TargetSite.ToString());
                    }
                }
            }
        }

        private void DrawStarBuck(Graphics dc, int r, int c)
        {
            Rectangle rc = ClientRectangle;
            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            float center_x = (c + 0.5f) * hs;
            float center_y = ((cake_.Row-1-r) + 0.5f) * vs;

            float start_a = 90;

            float toRad = (float)(Math.PI/180.0);
            float rad = 10f;
            float pre_x = (float)(Math.Cos(start_a * toRad) * rad);
            float pre_y = (float)(Math.Sin(start_a * toRad) * rad);

            for (int i = 1; i < 6; ++i)
            {
                float angle = start_a+144 * i;
                float this_x = (float)(Math.Cos(angle * toRad) * rad);
                float this_y = (float)(Math.Sin(angle * toRad) * rad);
                dc.DrawLine(keeper_star_buck_pen, center_x + pre_x, center_y + pre_y, center_x + this_x, center_y + this_y);
                pre_x = this_x;
                pre_y = this_y;
            }

            dc.DrawEllipse(keeper_star_buck_pen, center_x-rad, center_y-rad, rad*2, rad*2);

        }

        private delegate void DrawShape(Graphics dc,float vs, float hs,int r,int c,Pen pen);

        private void DrawCross(Graphics dc, float vs, float hs, int r, int c,Pen pen)
        {
            float center_x = (c + 0.5f) * hs;
            float center_y = ((cake_.Row - 1 - r) + 0.5f) * vs;

            dc.DrawLine(pen, center_x - hs / 2, center_y - vs / 2, center_x + hs / 2, center_y + vs / 2);
            dc.DrawLine(pen, center_x - hs / 2, center_y + vs / 2, center_x + hs / 2, center_y - vs / 2);
        }

        private void DrawCircle(Graphics dc, float vs, float hs, int r, int c,Pen pen)
        {
            float center_x = (c + 0.5f) * hs;
            float center_y = ((cake_.Row-1-r) + 0.5f) * vs;
            dc.DrawEllipse(pen, center_x-hs/2,center_y-vs/2,hs,vs);
        }

        private void DrawTraps(Graphics dc)
        {
            Rectangle rc = ClientRectangle;
            float w = rc.Width;
            float h = rc.Height;
            float vs = h/cake_.Row;
            float hs = w / cake_.Column;

            TrapSet ts = cake_.Trap;
            if (ts != null && ts.SetCount == 2)
            {
                DrawShape []ds=new DrawShape[]{new DrawShape(DrawCross),new DrawShape(DrawCircle)};
                
                for(int i=0;i<2;i++)
                {
                    List<TrapSetPoint> os = ts.GetSet(i);
                    foreach (TrapSetPoint tsp in os)
                    {
                        ds[i](dc, vs, hs, tsp.Row, tsp.Column,trap_pen);
                    }
                }

            }
            else
            {
                // UNKNOWN TRAP SET TYPE

            }
        }

        private void DrawOneKeeper(Graphics dc, DoodleKeeper dk, float vs,float hs,bool is_active_keeper)
        {
            String[] routes = dk.Routes;

            int pre_pos_r = dk.StartRow;        // -1;
            int pre_pos_c = dk.StartColumn;     // -1;
            bool is_first = true;
            
            int pos_r = -1;
            int pos_c = -1;
            for (int i = 0; i < routes.Length; ++i)
            {
                String[] parts = routes[i].Split(":,".ToCharArray());
                if (0 == String.CompareOrdinal("stop", parts[0]))
                {

                }
                else if (0 == String.CompareOrdinal("point", parts[0]))
                {
                    pos_r = int.Parse(parts[1]);
                    pos_c = int.Parse(parts[2]);

                    float x2 = (pos_c + 0.5f) * hs;
                    float y2 = ((cake_.Row - 1 - pos_r) + 0.5f) * vs;

                    if (pre_pos_r >= 0 && pre_pos_c >= 0)
                    {
                        float x1 = (pre_pos_c + 0.5f) * hs;
                        float y1 = ((cake_.Row - 1 - pre_pos_r) + 0.5f) * vs;

                        Pen pen_for_keeper_line = null;
                        if (is_first)
                        {
                            pen_for_keeper_line = from_starting_point_pen;
                            is_first = false;
                        }
                        else if (is_active_keeper)
                            pen_for_keeper_line = active_keeper_route_pen;
                        else
                            pen_for_keeper_line = keeper_route_pen;
                        dc.DrawLine(pen_for_keeper_line, x1, y1, x2, y2);
                    }

                    float rad = 5.5f;
                    dc.FillEllipse(Brushes.LightSalmon, x2 - rad, y2 - rad, rad * 2, rad * 2);
                    pre_pos_r = pos_r;
                    pre_pos_c = pos_c;
                }
            }
            DrawStarBuck(dc, dk.StartRow, dk.StartColumn);
        }

        private void DrawKeepers(Graphics dc)
        {
            Rectangle rc = ClientRectangle;
            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;


            int current_index = 0;
            foreach (DoodleKeeper dk in cake_.KeepersList)
            {
                DrawOneKeeper(dc,dk,vs,hs,active_keeper_ == current_index);
                current_index++;
            }
        }

        private void DrawShadowPoint(Graphics dc)
        {
            Rectangle rc = ClientRectangle;
            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            foreach(ShadowPoint spt in cake_.ShadowPoints)
            {
                float x = spt.Column * hs + 0.1f * hs;
                float y = (cake_.Row - spt.Row) * vs - 0.9f * vs;
                dc.FillEllipse(Brushes.DarkKhaki, x, y, hs*0.8f, vs*0.8f);
            }
        }

        private void DrawGrids(Graphics dc)
        {
            if (!draw_grids_)
                return;

            //~
            Rectangle rc = ClientRectangle;

            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            for (int i = cake_.Row - 1 ; i >=0 ; --i)
            {
                Pen grayPen = Pens.Gray;
                dc.DrawLine(grayPen, new Point(0, (int)(i * vs)), new Point((int)w, (int)(i * vs)));
            }

            for (int j = cake_.Column - 1; j >=0 ; --j)
            {
                Pen grayPen = Pens.Gray;
                dc.DrawLine(grayPen, new Point((int)(j * hs), 0), new Point((int)(j * hs), (int)h));
            }
        }

        private void DrawMaze(Graphics dc)
        {
            Rectangle rc = ClientRectangle;

            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;


            if (draw_logic_wall_)
            {
                for (int i = cake_.Row - 1; i >= 0; i--)
                {
                    for (int j = cake_.Column - 1; j >= 0; j--)
                    {
                        Pen bluePen = Pens.Blue;
                        DrawWallAt(dc, j, i, w, h, hs, vs);
                    }
                }
            }

            foreach (DoodleItem di in cake_.ItemList)
            {
                DrawItemAt(dc, di,w,h, hs, vs);
            }

            foreach (StairPort sp in cake_.StairPorts)
            {
                DrawStairAt(dc, sp.Column, sp.Row, sp.Port,w, h, hs, vs);
            }
        }

        private void DrawItemAt(Graphics dc, DoodleItem di,float w,float h,float hs,float vs)
        {
            int x = di.Column;
            int y = di.Row;
            int item_value = di.Item;
            String specialty = di.Specialty;

            Point pt = new Point((int)(x * hs + 0.2f * hs),(int)((cake_.Row - y) * vs - 0.8f * vs));
            if (item_value > 0)
            {
                switch(specialty.ToLower())
                {
                    case "accesscontroller":
                        dc.DrawRectangle(accesscontroller_pen_,new Rectangle(pt,new Size(12,12)));
                        break;
                    case "emitter":
                        dc.DrawEllipse(emitter_pen_,new Rectangle(pt,new Size(12,12)));
                        break;
                    case "box":
                        dc.DrawRectangle(box_pen_,new Rectangle(pt,new Size(12,12)));
                        break;
                    case "nightglass":
                        dc.DrawEllipse(nightglass_pen_, new Rectangle(pt, new Size(16, 16)));
                        break;
                    case "star":
                        dc.DrawEllipse(star_pen_,new Rectangle(pt,new Size(20,20)));
                        break;
                    default:
                    case "common":
                        //dc.DrawEllipse(common_pen_, new Rectangle(pt, new Size(10, 10)));
                        break;
                }
                dc.DrawString(String.Format("({0})", item_value), consolasFont, blueBrush, pt);
            }
            else
                Console.WriteLine("Warning !!!!");
        }

        private void DrawStairAt(Graphics dc, int x, int y, int to,float w, float h, float hs, float vs)
        {
            Point pt = new Point((int)(x * hs + 0.2f * hs), (int)((cake_.Row - y) * vs - 0.8f * vs));
            if (to > 0)
                dc.DrawString(String.Format("*{0}", to), consolasFont, blueBrush, pt);
            else
                Console.WriteLine("Warning !!!!!");
        }

        private void DrawWallAt(Graphics dc, int x, int y, float w, float h, float hs, float vs)
        {
            const int y_off = -2;
            const int x_off = 2;

            if ((cake_.Map[y, x] & 1) != 0)
            {
                dc.DrawLine(bluePen, new Point((int)(x * hs), (int)((cake_.Row - y) * vs) + y_off), new Point((int)((x + 1) * hs), (int)((cake_.Row - y) * vs) + y_off));
            }
            if ((cake_.Map[y, x] & 2) != 0)
            {
                dc.DrawLine(bluePen, new Point((int)(x * hs) + x_off, (int)((cake_.Row - y) * vs) + y_off), new Point((int)(x * hs) + x_off, (int)((cake_.Row - y - 1) * vs)));
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int grid_x;
            int grid_y;
            CalculateGridPos(e.X, e.Y, out grid_x, out grid_y);


            switch (control_state_)
            {
                default:
                case DoodleControlState.PickingRemoteControl:
                case DoodleControlState.PickingCamera:
                case DoodleControlState.Idle:
                    EvtDoodleMm_(grid_x, grid_y);
                    break;
                case DoodleControlState.PickingTrap:
                    last_x_ = e.X;
                    last_y_ = e.Y;
                    Invalidate();
                    //Console.WriteLine("Picking trap ....");
                    break;
            }
        }

        private void CalculateGridPos(int x,int y,out int grid_x, out int grid_y)
        {
            Rectangle rc = ClientRectangle;
            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            grid_x = (int)(x / hs);
            grid_y = (int)((rc.Height-y) / vs);
        }

        private void DrawStartPoint(Graphics dc)
        {
            Rectangle rc = ClientRectangle;

            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            int r = cake_.StartRow;
            int c = cake_.StartColumn;
            if (r >= 0 && c >= 0 && r < cake_.Row && c < cake_.Column)
            {
                float x = c * hs + 0.5f * hs;
                float y = (cake_.Row - 1 - r) * vs + 0.5f * vs;
                float rw = hs*0.3f,rh = vs * 0.3f;
                Rectangle target =new Rectangle((int)(x - rw /2) ,(int)(y- rh/2),(int)rw,(int)rh);
                
                //pen.Width = 5;
                dc.DrawEllipse(Pens.Purple, target);
                dc.FillEllipse(Brushes.Purple, target);
            }
        }


        private void ProcessKeeperEditing(MouseEventArgs e, out bool done)
        {
            if (KeeperPickState.Idle == keeper_pick_state)
            {
                done = false;
                return;
            }

            int grid_x, grid_y;
            CalculateGridPos(e.X, e.Y, out grid_x, out grid_y);
            KeeperPickState new_state = keeper_pick_state;
            switch (keeper_pick_state)
            {
                case KeeperPickState.PickingStartPoint:
                    current_temp.SetStartPoint(grid_y, grid_x);
                    new_state = KeeperPickState.PickingRoutePoint;
                    Invalidate();
                    break;
                case KeeperPickState.PickingRoutePoint:
                    current_temp.AddOneRoutePoint(grid_y, grid_x);
                    Invalidate();
                    break;
                default:
                    throw new Exception("What the fuck site 4");
            }
            
            keeper_pick_state = new_state;
            done = true;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int grid_x;
            int grid_y;
            CalculateGridPos(e.X, e.Y, out grid_x, out grid_y);

            switch (control_state_)
            {
                default:
                case DoodleControlState.Idle:
                    {
                        bool done;
                        ProcessKeeperEditing(e, out done);
                        if (done)
                            return;
                        EvtDoodleMc_(grid_x, grid_y);
                    }
                    break;
                case DoodleControlState.PickingTrap:
                    cake_.AddTrapAt(grid_y,grid_x);
                    Invalidate();
                    EvtTrapSetChanged_();
                    break;
                case DoodleControlState.PickingCamera:
                    cake_.AddCameraAt(grid_y, grid_x);
                    Invalidate();
                    EvtCameraSetChanged_();

                    //SetControlState(DoodleControlState.Idle);       //~ Reset automatically !
                    break;
                case DoodleControlState.PickingRemoteControl:
                    if ((GetAsyncKeyState(0xA2) & 0x8000) == 0)
                    {
                        cake_.SetTmpRemoteControlGate(grid_y, grid_x);
                    }
                    else
                    {
                        cake_.AddSubToTmpRemoteControl(grid_y, grid_x);
                    }
                    Invalidate();
                    break;
            }
        }

        public void EnableTexViewMask(int mask_bit)
        {
            int new_mask = (tex_view_mask_|mask_bit);
            if (tex_view_mask_ != new_mask)
            {
                tex_view_mask_ = new_mask;
                Console.WriteLine("TexViewMask has been changed, reschedule-paint");
                Invalidate();
            }
        }

        public void DisableTexViewMask(int mask_bit)
        {
            int new_mask = (tex_view_mask_ & (~mask_bit));
            if (tex_view_mask_ != new_mask)
            {
                tex_view_mask_ = new_mask;
                Invalidate();
            }
        }

        public void SetTexEditMask(int new_mask)
        {
            if (tex_edit_mask_ != new_mask)
            {
                tex_edit_mask_ = new_mask;
                Console.WriteLine("TexEditMask has been changed.");
            }
        }

        private void DrawRoleAndItemLayer(Graphics dc)
        {
            DrawLayerWithName(dc,"roleanditem");
        }

        private void DrawDecorationLayer(Graphics dc)
        {
            DrawLayerWithName(dc,"decoration");
        }

        private void DrawFixLayer(Graphics dc)
        {
            DrawLayerWithName(dc,"fix");
        }

        private void DrawLayerWithName(Graphics dc, String name)
        {
            KkbLayers layers = cake_.Layers;
            if (layers != null)
            {
                KkbStandardLayer layer = layers.Find(name);
                if (layer != null)
                {
                    Rectangle rc = ClientRectangle;
                    float w = rc.Width;
                    float h = rc.Height;
                    float vs = h / cake_.Row;
                    float hs = w / cake_.Column;

                    foreach (KKbLayerItem item in layer.Items)
                    {
                        int r = item.Row;
                        int c = item.Colum;
                        int tx = item.Tex;

                        float x = hs * c;
                        float y = (cake_.Row - r) * vs;
                        tex_render_.DrawTerrance(dc, tx, x, y - vs, hs, vs);
                    }
                }
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics dc = e.Graphics;

            Rectangle rc = ClientRectangle;
            rc.Inflate(-1, -1);
            dc.DrawRectangle(circlePen, rc);

            if( (tex_view_mask_ & DoodleCake.LayerBackground) != 0 )
                DrawTexture(dc);

            if ((tex_view_mask_ & DoodleCake.LayerDecoration) != 0)
                DrawDecorationLayer(dc);

            if( (tex_view_mask_ & DoodleCake.LayerRoleAndItem ) != 0 )
                DrawRoleAndItemLayer(dc);

            if ((tex_view_mask_ & DoodleCake.LayerFix) != 0)
                DrawFixLayer(dc);
            
            DrawGrids(dc);
            DrawMaze(dc);
            DrawStartPoint(dc);
            DrawShadowPoint(dc);
            DrawKeepers(dc);
            DrawTraps(dc);
            DrawCamera(dc);

            DrawRemoteControls(dc);
            DrawTempKeeperTrack(dc);
            DrawAimPoint(dc);
        }

        protected void DrawCamera(Graphics dc)
        {
            if (cake_.SecurityCamera != null)
            {
                Rectangle rc = ClientRectangle;

                float w = rc.Width;
                float h = rc.Height;
                float vs = h / cake_.Row;
                float hs = w / cake_.Column;

                foreach (SecurityCameraEquip sce in cake_.SecurityCamera)
                {
                    int r = sce.Row;
                    int c = sce.Column;
                    float x = c * hs + 0.5f * hs;
                    float y = (cake_.Row - 1 - r) * vs + 0.5f * vs;
                    float rw = hs * 0.56f, rh = vs * 0.45f;
                    Rectangle target = new Rectangle((int)(x - rw / 2), (int)(y - rh / 2), (int)rw, (int)rh);
                    //pen.Width = 5;
                    dc.DrawEllipse(Pens.Orange, target);
                    dc.FillEllipse(Brushes.Yellow, target);
                }
            }
        }

        private void DrawRemoteControls(Graphics dc)
        {
            Rectangle rc = ClientRectangle;

            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            int i = 0;
            IEnumerator<RemoteControl> enumerator = cake_.RemoteControlEnumerator;
            while (enumerator.MoveNext())
            {
                RemoteControl ctrl = enumerator.Current;
                DrawOneRemoteControl(ctrl, dc, selected_index_of_rctrl_ == i ? RemoteControlDrawSelect.Selected:RemoteControlDrawSelect.Normal);
                ++i;
            }
            if (cake_.TmpRemoteControl != null)
                DrawOneRemoteControl(cake_.TmpRemoteControl, dc, RemoteControlDrawSelect.Tmp);
        }

        private void DrawOneRemoteControl(RemoteControl ctrl, Graphics dc, RemoteControlDrawSelect mode)
        {

            Pen frame_line_pen;
            switch (mode)
            {
                default:
                case RemoteControlDrawSelect.Normal:
                    frame_line_pen = null;
                    break;
                case RemoteControlDrawSelect.Selected:
                    frame_line_pen = Pens.White;
                    break;
                case RemoteControlDrawSelect.Tmp:
                    frame_line_pen = Pens.Bisque;
                    break;
            }


            //~
            Rectangle rc = ClientRectangle;

            float w = rc.Width;
            float h = rc.Height;
            float vs = h / cake_.Row;
            float hs = w / cake_.Column;

            float width = hs * 0.75f;
            float height = vs * 0.75f;

            float x = ctrl.GateColumn * hs + (hs - width) / 2;
            float y = (cake_.Row - 1 - ctrl.GateRow) * vs + (vs - height) / 2;
            dc.FillRectangle(Brushes.Red, x, y, width, height);
            if (frame_line_pen != null)
            {
                dc.DrawRectangle(frame_line_pen, x, y, width, height);
            }

            foreach (RowColumPos sp in ctrl)
            {
                float s_width = hs * 0.6f;
                float s_height = vs * 0.6f;

                float sx = sp.GetColumn() * hs + (hs - s_width) / 2;
                float sy = (cake_.Row - 1 - sp.GetRow()) * vs + (vs - s_height) / 2;
                dc.FillRectangle(Brushes.SeaGreen, sx, sy, s_width, s_height);
                if (frame_line_pen != null)
                {
                    dc.DrawRectangle(frame_line_pen, sx, sy, s_width, s_height);
                }
            }
        }

        protected void DrawTempKeeperTrack(Graphics dc)
        {
            if (null != current_temp)
            {
                Rectangle rc = ClientRectangle;
                float w = rc.Width;
                float h = rc.Height;
                float vs = h / cake_.Row;
                float hs = w / cake_.Column;

                DrawOneKeeper(dc, current_temp, vs, hs, true);
            }
        }

        protected void DrawAimPoint(Graphics dc)
        {
            if (DoodleControlState.PickingTrap == control_state_)
            {
                Rectangle rc = ClientRectangle;
                dc.DrawLine(Pens.Red, 0, last_y_, rc.Width,last_y_);
                dc.DrawLine(Pens.Red, last_x_, 0, last_x_, rc.Height);
            }
        }

        public void SetPickState(KeeperPickState new_state)
        {
            switch (new_state)
            {
                case KeeperPickState.PickingStartPoint:
                    if (current_temp != null)
                        throw new Exception("What the fuck Site 3");
                    current_temp = DoodleKeeper.MakeTempKeeper();
                    break;
                case KeeperPickState.Idle:
                    cake_.AddKeeper(current_temp);
                    current_temp = null;
                    break;
            }

            //~
            keeper_pick_state = new_state;
        }

        public void SelectRemoteControlByIndex(int index)
        {
            selected_index_of_rctrl_ = index;
            Invalidate();   //~
        }

        public void RemoveTrapByName(String name)
        {
            cake_.RemoveTrapByName(name);
            Invalidate();
        }

        public void RevemoAllTrap()
        {
            cake_.RemoveAllTrap();
            Invalidate();
        }

        public void SwitchTrapGroup(String name)
        {
            cake_.SwitchTrapGroup(name);
            Invalidate();
        }

        public int TrapSetCount
        {
            get
            {
                return cake_.Trap.SetCount;
            }
        }

        public int SecurityCameraCount
        {
            get
            {
                if (cake_.SecurityCamera != null)
                    return cake_.SecurityCamera.Count;
                return 0;
            }
        }

        public void UpdateCameraPeriod(int i,String period)
        {
            if( cake_.UpdateCameraPeriod(i,period))
            {
                EvtCameraSetChanged_();
            }
        }

        public SecurityCameraEquip GetCameraByIndex(int i)
        {
            if (cake_.SecurityCamera != null)
            {
                return cake_.SecurityCamera.GetByIndex(i);
            }
            return null;
        }

        public void RemoveRemoteControlByIndex(int i)
        {
            cake_.EraseRemoteControlByIndex(i);
            Invalidate();
            EvtRemoteControlChanged_();
        }

        public void RemoveAllRemoteControls()
        {
            cake_.EraseAllRemoteControls();
            Invalidate();
            EvtRemoteControlChanged_();
        }

        public void RemoveAllCameras()
        {
            if (cake_.SecurityCamera != null)
            {
                cake_.SecurityCamera.RemoveAll();
                Invalidate();
            }
        }

        public int RemoveCamera(SecurityCameraEquip sce)
        {
            int rv = 0;
            if (cake_.SecurityCamera != null)
            {
                rv = cake_.SecurityCamera.RemoveObject(sce);
                Invalidate();
            }
            return rv;
        }

        public void FlipPickingCamera()
        {
            if (control_state_ != DoodleControlState.PickingCamera)
            {
                SetControlState(DoodleControlState.PickingCamera);
            }
            else
            {
                SetControlState(DoodleControlState.Idle);
            }
        }

        public void FlipPickingTrap()
        {
            if (control_state_ != DoodleControlState.PickingTrap)
            {
                SetControlState(DoodleControlState.PickingTrap);
            }
            else
            {
                SetControlState(DoodleControlState.Idle);
            }
        }

        public void FlipAddRemoteControl()
        {
            if (control_state_ != DoodleControlState.PickingRemoteControl)
            {
                selected_index_of_rctrl_ = -1;
                SetControlState(DoodleControlState.PickingRemoteControl);
            }
            else
            {
                if (cake_.AddOneRemoteControlSet())
                {
                    Invalidate();
                    EvtRemoteControlChanged_();
                }
                SetControlState(DoodleControlState.Idle);
            }
        }

        public void SetControlState(DoodleControlState new_state)
        {
            if (new_state != control_state_)
            {
                control_state_ = new_state;
                switch (control_state_)
                {
                    case DoodleControlState.PickingRemoteControl:
                        cake_.ResetTmpRemoteControl();
                        break;
                }

                Invalidate();
                EvtControlStateChanged_(new_state);
            }
        }

        public List<TrapSetPoint> GetTrapGroupByIndex(int i)
        {
            return cake_.Trap.GetSet(i);
        }

        public KeeperPickState PickState
        {
            get
            {
                return keeper_pick_state;
            }
        }

        public IEnumerator<RemoteControl> RemoteControlEnumerator
        {
            get
            {
                return cake_.RemoteControlEnumerator;
            }
        }

        public void SetEnableGrids(bool grids_on)
        {
            if (draw_grids_ ^ grids_on)
            {
                draw_grids_ = grids_on;
                Invalidate();
            }
        }

        public void SetEnableLogicWall(bool logic_wall_enabled)
        {
            if (draw_logic_wall_ ^ logic_wall_enabled)
            {
                draw_logic_wall_ = logic_wall_enabled;
                Invalidate();
                EvtLogicWallViewPropertyChanged(logic_wall_enabled);
            }
        }

        public DoodleMouseMove EvtDoodleMm_;
        public DoodleMouseClick EvtDoodleMc_;
        public DoodleTrapSetChanged EvtTrapSetChanged_;
        public DoodleCameraSetChanged EvtCameraSetChanged_;
        public DoodleControlStateChanged EvtControlStateChanged_;
        public DoodleRemoteControlChanged EvtRemoteControlChanged_;
        public DoodleLogicWallViewPropertyChanged EvtLogicWallViewPropertyChanged;

        private Pen trap_pen = new Pen(Color.DarkOrange, 3f);
        private Pen keeper_star_buck_pen = new Pen(Color.Purple, 2f);
        private Pen keeper_route_pen = new Pen(Color.OrangeRed, 4.5f);
        private Pen active_keeper_route_pen = new Pen(Color.LightSeaGreen, 5.0f);
        private Pen from_starting_point_pen = new Pen(Color.LightSlateGray, 4.5f);

        private Pen circlePen = new Pen(Color.Orange, 1);
        private Pen bluePen = new Pen(Color.Blue,2);//Pens.Blue;
        private Brush blueBrush = Brushes.Blue;
        private Font consolasFont = new Font("Consolas", 12);
        private Font blodTimesFont = new Font("Times New Roman", 10, FontStyle.Bold);
        private Font italicFixedsysFont = new Font("Fixedsys", 11, FontStyle.Italic | FontStyle.Underline);

        private readonly Brush[] PredeinedBrush = new Brush[]{
            Brushes.Gray,
            Brushes.Aqua,
            Brushes.Brown
        };

        private int tex_view_mask_ = DoodleCake.AllLayerMask;

        private int tex_edit_mask_ = DoodleCake.LayerBackground;            //~ Exclusive

        private DoodleCake cake_ = new DoodleCake();

        private readonly TerranceRender tex_render_ = new TerranceRender();

        private int active_keeper_ = -1;

        private KeeperPickState keeper_pick_state = KeeperPickState.Idle;

        private DoodleKeeper current_temp = null;

        private DoodleControlState control_state_ = DoodleControlState.Idle;

        private int last_x_, last_y_;

        private Pen accesscontroller_pen_ = new Pen(Brushes.Silver, 5);
        private Pen emitter_pen_ = new Pen(Brushes.SeaShell, 5);
        private Pen box_pen_ = new Pen(Brushes.RosyBrown, 5);
        private Pen common_pen_ = new Pen(Brushes.Silver, 3);
        private Pen star_pen_ = new Pen(Brushes.Yellow, 4);
        private Pen nightglass_pen_ = new Pen(Brushes.LightGreen, 5);

        private enum RemoteControlDrawSelect
        {
            Normal,
            Selected,
            Tmp,
        }

        private bool draw_grids_ = true;

        private bool draw_logic_wall_ = true;

        private int selected_index_of_rctrl_ = -1;

        [DllImport("user32")]
        public static extern short GetAsyncKeyState(int vKey);
    }
}

