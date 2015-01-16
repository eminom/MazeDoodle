using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    public class DoodleCake
    {
        public DoodleCake()
        {
            layers_ = new KkbLayers();
        }

        public DoodleCake(String file)
        {
            LoadFromJson(file);
        }

        public int Row
        {
            get
            {
                return row_;
            }
        }

        public void SetRow(int r)
        {
            row_ = r;
        }

        public int Column
        {
            get
            {
                return column_;
            }
        }

        public void SetColumn(int c)
        {
            column_ = c;
        }

        public void GetSize(out int r, out int c)
        {
            r = row_;
            c = column_;
        }

        public int[,] Map
        {
            get
            {
                return map_;
            }
        }

        public int GetTexture(int r, int c)
        {
            int lo, hi, rlo;
            Extract(map_[r, c], out lo, out hi, out rlo);
            return hi;
        }

        public void SetMap(int[,] m)
        {
            map_ = (int[,])m.Clone();
        }

        public List<DoodleItem> ItemList
        {
            get
            {
                return item_list_;
            }
        }

        public List<StairPort> StairPorts
        {
            get
            {
                return sp_list_;
            }
        }

        public List<ShadowPoint> ShadowPoints
        {
            get
            {
                return shadow_list_;
            }
        }

        public SecurityCamera SecurityCamera
        {
            get
            {
                return security_camera_;
            }
        }

        public bool AddOneRemoteControlSet()
        {
            bool rv = false;
            if (r_ctrl_ != null && r_ctrl_.GateRow >= 0 && r_ctrl_.GateColumn >= 0)
            {
                remote_control_.Add(r_ctrl_);
                r_ctrl_ = null;
                rv = true;
            }
            return rv;
        }

        public void ResetTmpRemoteControl()
        {
            r_ctrl_ = RemoteControl.MakeTmp();
        }

        public RemoteControl TmpRemoteControl
        {
            get
            {
                return r_ctrl_;
            }
        }

        public void SetTmpRemoteControlGate(int r, int c)
        {
            if (r_ctrl_ != null && CheckPosValidForRemoteControl(r, c))
            {
                r_ctrl_.SetGatePos(r, c);
            }
        }

        public void AddSubToTmpRemoteControl(int r, int c)
        {
            if (r_ctrl_ != null && CheckPosValidForRemoteControl(r, c))
            {
                r_ctrl_.AddSub(r, c);
            }
        }

        private bool CheckPosValidForRemoteControl(int r, int c)
        {
            bool rv = true;
            foreach (RemoteControl rc in remote_control_)
            {
                if (rc.GateRow == r && rc.GateColumn == c)
                {
                    Console.WriteLine("Click on one other gate(remote-control)");
                    rv = false;
                    break;
                }
                foreach (RowColumPos sp in rc)
                {
                    if (sp.GetRow() == r && sp.GetColumn() == c)
                    {
                        rv = false;
                        break;
                    }
                }
                if (!rv)
                {
                    break;
                }
            }
            return rv;
        }


        public DoodleKeeper GetKeeperByIndex(int index)
        {
            return keeper_list_[index];
        }

        public void AddCameraAt(int row, int col)
        {
            security_camera_.AddOne(row, col);
        }

        public bool UpdateCameraPeriod(int i, String period)
        {
            bool rv = false;
            SecurityCameraEquip sce = security_camera_.GetByIndex(i);
            if (sce != null)
            {
                if (sce.Period != period)
                {
                    sce.Period = period;
                    rv = true;
                }
            }
            return rv;
        }

        public void RemoveAllShadowPoints()
        {
            Console.WriteLine("DoodleCake.RemoveAllShadow-Points");
            shadow_list_.Clear();
        }

        public void ClearAtPos(int x, int y)
        {
            foreach (DoodleItem di in item_list_)
            {
                if (di.inPos(x, y))
                {
                    item_list_.Remove(di);
                    break;
                }
            }

            foreach (StairPort sp in sp_list_)
            {
                if (sp.inPos(x, y))
                {
                    sp_list_.Remove(sp);
                    break;
                }
            }
        }

        public void SetStairPortAt(int x, int y, int to_stair, int item_acq, bool clear_item)
        {
            if (map_.GetLength(0) <= y || map_.GetLength(1) <= x || x < 0 || y < 0)
            {
                return;
            }

            StairPort dp = null;
            foreach (StairPort sp in sp_list_)
            {
                if (sp.inPos(x, y))
                {
                    dp = sp;
                    break;
                }
            }

            if (dp != null)
                dp.setToPort(to_stair, item_acq);
            else
                sp_list_.Add(new StairPort(y, x, to_stair, item_acq));

            if (clear_item)
            {
                DoodleItem target_item = null;
                foreach (DoodleItem di in item_list_)
                {
                    if (di.inPos(x, y))
                    {
                        target_item = di;
                        break;
                    }
                }
                item_list_.Remove(target_item);
            }
        }

        public void PutItemAt(int x, int y, int item,String specialty, bool clear_stair_port)
        {
            if (map_.GetLength(0) <= y || map_.GetLength(1) <= x || x < 0 || y < 0)
            {
                Console.WriteLine("invalid x or invalid y");
                return;
            }

            DoodleItem any = null;
            foreach (DoodleItem di in item_list_)
            {
                if (di.inPos(x, y))
                {
                    any = di;
                    break;
                }
            }

            if (null == any)
                item_list_.Add(new DoodleItem(y, x, item,specialty));
            else
                any.setItemValue(item);

            if (clear_stair_port)
            {
                StairPort target_port = null;
                foreach (StairPort sp in sp_list_)
                {
                    if (sp.inPos(x, y))
                    {
                        target_port = sp;
                        break;
                    }
                }
                sp_list_.Remove(target_port);
            }
        }

        private void AndWallAt(int x, int y, int v)
        {
            if (y < map_.GetLength(0) && x < map_.GetLength(1) &&
                y >= 0 && x >= 0)
            {
                int lo, hi, rlo;
                Extract(map_[y, x], out lo, out hi, out rlo);
                lo &= v;
                map_[y, x] = Combine(lo, hi, rlo);
            }
        }

        private void OrWallAt(int x, int y, int v)
        {
            if (y < map_.GetLength(0) && x < map_.GetLength(1) &&
                y >= 0 && x >= 0)
            {
                int lo, hi, rlo;
                Extract(map_[y, x], out lo, out hi, out rlo);
                lo |= v;
                map_[y, x] = Combine(lo, hi, rlo);
            }
        }

        public void SetRockAt(int x, int y)
        {
            OrWallAt(x, y, 3);
            OrWallAt(x, y + 1, 1);
            OrWallAt(x + 1, y, 2);
        }

        public void RemoveRockAt(int x, int y)
        {
            AndWallAt(x, y, 0);
            AndWallAt(x, y + 1, (0x0E));
            AndWallAt(x + 1, y, (0x0D));
        }

        private void SetTextureToSpecificLayer(int x, int y, int t, String name)
        {
            KkbStandardLayer layer = layers_.FindOrCreate(name);
            if (layer != null)
            {
                layer.SetTexture(x, y, t);
            }
        }

        public void SetTexture(int x, int y, int t,int layer_mask)
        {
            if ((layer_mask & LayerBackground) == layer_mask)
            {
                int lo, hi, rlo;
                Extract(map_[y, x], out lo, out hi, out rlo);
                hi = t;
                map_[y, x] = Combine(lo, hi, rlo);
            }
            else if ((layer_mask & LayerDecoration) == layer_mask)
            {
                SetTextureToSpecificLayer(x, y, t, "decoration");
            }
            else if ((layer_mask & LayerRoleAndItem) == layer_mask)
            {
                SetTextureToSpecificLayer(x,y,t,"roleanditem");
            }
            else if ((layer_mask & LayerFix) == layer_mask)
            {
                SetTextureToSpecificLayer(x, y, t, "fix");
            }
            else
            {
                Console.WriteLine("cannot draw to this layer");
            }
        }

        public void RemoveTexture(int x, int y,int tex_view_mask)
        {
            SetTexture(x, y, KkbStandardLayer.RemoveTexId,tex_view_mask);
        }

        public void ToggleWallAt(int x, int y)
        {
            if (map_.GetLength(0) == y || map_.GetLength(1) == x || x < 0 || y < 0)
            {
                if (map_.GetLength(0) == y)
                    Console.WriteLine("y exceeded !");
                else if (map_.GetLength(1) == x)
                    Console.WriteLine("x exceeded !");
                else
                    Console.WriteLine("invalid x or y ({0},{1})", x, y);
                return;
            }

            int lo, hi, rlo;
            Extract(map_[y, x], out lo, out hi, out rlo);

            lo = (lo + 1) % 4;

            //~
            map_[y, x] = Combine(lo, hi, rlo);
        }

        public void ResetSize(int r, int c)
        {
            if (r != row_ || c != column_)
            {
                row_ = r;
                column_ = c;
                map_ = new int[r, c];
            }
        }

        public void Enclose()
        {
            for (int i = 0; i < column_ - 1; i++)
            {
                map_[0, i] |= 1;
                map_[row_ - 1, i] |= 1;
            }
            for (int i = 0; i < row_ - 1; i++)
            {
                map_[i, 0] |= 2;
                map_[i, column_ - 1] |= 2;
            }
            map_[0, 0] |= 3;
        }

        public void Disclose()
        {
            for (int i = 0; i < column_ - 1; i++)
            {
                map_[0, i] &= (~1);
                map_[row_ - 1, i] &= (~1);
            }
            for (int i = 0; i < row_ - 1; i++)
            {
                map_[i, 0] &= (~2);
                map_[i, column_ - 1] &= (~2);
            }
        }

        public void ResetMap(int r, int c, int[][] vals)
        {
            row_ = r;
            column_ = c;

            map_ = new int[r, c];
            for (int i = 0; i < r; ++i)
            {
                for (int j = 0; j < c; ++j)
                {
                    map_[i, j] = vals[i][j];
                }
            }
        }

        private void Extract(int v, out int lo, out int hi, out int rlo)
        {
            lo = (v & 0xFF);
            hi = ((v >> 8) & 0xFF);
            rlo = ((v >> 16) & 0xFF);
        }

        private int Combine(int lo, int hi, int rlo)
        {
            return (lo | (hi << 8) | (rlo << 16));
        }

        private void LoadFromJson(String fileName)
        {
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(fileName);
                JsonTextReader jtr = new JsonTextReader(sr);

                JsonSerializer js = JsonSerializer.Create(null);
                //Dictionary<String, object> info = (Dictionary<String, object>)js.Deserialize(jtr, typeof(Dictionary<string, object>));
                JObject root = (JObject)js.Deserialize(jtr, typeof(JObject));
                if (null == root)
                {
                    //~ 
                    throw new LevelJsonNullException("Level Json corrupted");
                }

                int r = root["row"].ToObject<int>();
                int c = root["column"].ToObject<int>();
                JArray arr = root["maze"].ToObject<JArray>();
                int[][] map_dat = arr.ToObject<int[][]>();
                ResetMap(r, c, map_dat);

                item_list_.Clear();
                item_list_.AddRange(DoodleItem.LoadItems(root));

                sp_list_.Clear();
                sp_list_.AddRange(StairPort.LoadStairPorts(root));

                keeper_list_.Clear();
                keeper_list_.AddRange(DoodleKeeper.Load(root));

                shadow_list_.Clear();
                shadow_list_.AddRange(ShadowPoint.Load(root));

                start_point_ = StartPoint.LoadFromJson(root);
                trap_ = TrapSet.Load(root);
                remote_control_.AddRange(RemoteControl.Load(root));
                light_control_ = LightControl.Load(root);
                palace_control_ = PalaceControl.Load(root);
                security_camera_ = SecurityCamera.Load(root);

                layers_ = KkbLayers.Load(root);
            }
            catch (LevelJsonNullException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }

        private JArray MakeMazeJArray()
        {
            JArray jar = new JArray();
            for (int i = 0; i < map_.GetLength(0); ++i)
            {
                JArray line = new JArray();
                for (int j = 0; j < map_.GetLength(1); ++j)
                {
                    line.Add(map_[i, j]);
                }
                jar.Add(line);
            }
            return jar;
        }

        public JObject MakeJObject()
        {
            JObject root;
            try
            {
                root = new JObject();
                root["row"] = map_.GetLength(0).ToString();
                root["column"] = map_.GetLength(1).ToString();
                root["maze"] = MakeMazeJArray();

                DoodleItem.SaveToJObj(root, item_list_);
                StairPort.SaveToJObj(root, sp_list_);
                DoodleKeeper.SaveToJObj(root, keeper_list_);
                ShadowPoint.SaveToJObj(root, shadow_list_);
                TrapSet.SaveToJObj(root, trap_);
                StartPoint.SaveToJObj(root, start_point_);
                RemoteControl.SaveToJObj(root, remote_control_);

                LightControl.SaveToJObj(root, light_control_);
                PalaceControl.SaveToJObj(root, palace_control_);
                SecurityCamera.SaveToJObj(root, security_camera_);
                KkbLayers.SaveToJObj(root, layers_);
            }
            catch (Exception)
            {
                throw;
            }
            return root;
        }

        public void WriteToJson(string fileName)
        {
            StreamWriter sw = null;
            try
            {
                JObject obj = MakeJObject();
                sw = new StreamWriter(fileName);
                sw.WriteLine(obj.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }

        public void SetStartPoint(int r, int c)
        {
            start_point_.SetRow(r);
            start_point_.SetColumn(c);
        }

        public bool SetShadowPointAt(int x, int y)
        {
            ShadowPoint np = ShadowPoint.Point(x, y);
            bool found = false;
            foreach (ShadowPoint shadow in shadow_list_)
            {
                if (shadow.PositionEqual(np))
                {
                    found = true;
                    break;
                }
            }
            if( ! found )
                shadow_list_.Add(np);
            return !found;
        }

        public void GetStartPoint(out int r, out int c)
        {
            r = start_point_.Row;
            c = start_point_.Column;
        }

        public void AddTrapAt(int r, int c)
        {
            trap_.AddTrapAt(r, c,0);
        }

        public void RemoveTrapByName(String name)
        {
            trap_.RemoveTrapByName(name);
        }

        public void RemoveAllTrap()
        {
            trap_.RemoveAll();
        }

        public void SwitchTrapGroup(String name)
        {
            trap_.SwitchTrapGroupIn(name);
        }

        public void EraseRemoteControlByIndex(int i)
        {
            if (i >= 0 && i < remote_control_.Count)
            {
                remote_control_.RemoveAt(i);
            }
        }

        public void EraseAllRemoteControls()
        {
            remote_control_.Clear();
        }

        public TrapSet Trap
        {
            get
            {
                return trap_;
            }
        }

        public int StartRow
        {
            get
            {
                return start_point_.Row;
            }
        }

        public int StartColumn
        {
            get
            {
                return start_point_.Column;
            }
        }

        public List<DoodleKeeper> KeepersList
        {
            get
            {
                return keeper_list_;
            }
        }

        public void AddKeeper(DoodleKeeper dk)
        {
            if (dk == null)
                throw new Exception("What the fuck");
            keeper_list_.Add(dk);
        }

        public KkbLayers Layers
        {
            get
            {
                return layers_;
            }
        }

        public IEnumerator<RemoteControl> RemoteControlEnumerator
        {
            get
            {
                return remote_control_.GetEnumerator();
            }
        }

        //DATA FIELD
        private int row_ = 0;
        private int column_ = 0;
        private int[,] map_;

        private StartPoint start_point_ = new StartPoint();

        private readonly List<DoodleItem> item_list_ = new List<DoodleItem>();
        private readonly List<StairPort> sp_list_ = new List<StairPort>();
        private readonly List<DoodleKeeper> keeper_list_ = new List<DoodleKeeper>();
        private readonly List<ShadowPoint> shadow_list_ = new List<ShadowPoint>();
        private TrapSet trap_ = TrapSet.DefaultTrap();
        private readonly List<RemoteControl> remote_control_ = new List<RemoteControl>();
        private RemoteControl r_ctrl_ = null;
        private LightControl light_control_;
        private PalaceControl palace_control_;
        private SecurityCamera security_camera_ = SecurityCamera.Default();
        private KkbLayers layers_;

        private const int Layer0 = 1;
        private const int Layer1 = 2;
        private const int Layer2 = 4;
        private const int Layer3 = 8;
        private const int Layer4 = 16;
        private const int Layer5 = 32;

        public const int LayerBackground = Layer0;
        public const int LayerDecoration = Layer1;
        public const int LayerRoleAndItem = Layer3;
        public const int LayerFix = Layer5;

        public const int AllLayerMask = (Layer0 | Layer1 | Layer2 | Layer3 | Layer4 | Layer5);
    }


    class LevelJsonNullException : Exception
    {
        public LevelJsonNullException(String msg)
            : base(msg)
        {

        }
    }
}
