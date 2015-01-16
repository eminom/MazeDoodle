using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace DoodleControls
{

    public class SecurityCameraEquip
    {
        public SecurityCameraEquip(int r, int c,String period)
        {
            jobj_ = JObject.Parse("{\"start_point\":{\"row\":\"-1\",\"column\":\"-1\"},\"period\":\"\"");
            jobj_[PERIODSTR] = period;
            start_point_ = (JObject)jobj_[START_POINT];
            start_point_["row"] = r.ToString();
            start_point_["column"] = c.ToString();
        }

        public SecurityCameraEquip(JObject jo)
        {
            jobj_ = jo.DeepClone().ToObject<JObject>();
            if (jobj_.Property(PERIODSTR) == null)
            {
                jobj_[PERIODSTR] = "";
            }
            if (jobj_.Property("start_point") == null)
            {
                jobj_["start_point"] = JObject.Parse("{\"row\":\"-1\",\"column\":\"-1\"}");
            }
            start_point_ = (JObject)jobj_["start_point"];
        }

        public int Row
        {
            get
            {
                return start_point_["row"].ToObject<int>();
            }
        }

        public int Column
        {
            get
            {
                return start_point_["column"].ToObject<int>();
            }
        }

        public String Period
        {
            get
            {
                return jobj_[PERIODSTR].ToObject<String>();
            }
            set
            {
                jobj_[PERIODSTR] = value;
            }
        }

        public override bool Equals(Object rhs)
        {
            if (Object.ReferenceEquals(this, rhs))
                return true;

            if (null == rhs)
                return false;

            if (rhs.GetType() != typeof(SecurityCameraEquip))
            {
                return false;
            }

            SecurityCameraEquip rhs_o = (SecurityCameraEquip)rhs;
            return this.Row == rhs_o.Row && this.Column == rhs_o.Column;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static SecurityCameraEquip Parse(String str)
        {
            Match mc = rex.Match(str);
            if (mc.Success)
            {
                int r = int.Parse(mc.Groups[1].ToString());
                int c = int.Parse(mc.Groups[2].ToString());
                return new SecurityCameraEquip(r, c,"");
            }
            return null;
        }

        public override String ToString()
        {
            return String.Format("\\{0},{1}\\", Row, Column);
        }

        public JObject CloneJObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        private readonly JObject jobj_;
        private readonly JObject start_point_;
        private readonly static Regex rex = new Regex("^\\\\(\\d+),(\\d+)\\\\$", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly static String PERIODSTR = "period";
        private readonly static String START_POINT = "start_point";
    }

    public class SecurityCamera:IEnumerable,IEnumerable<SecurityCameraEquip>
    {
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return equip_list_.GetEnumerator();
        }

        System.Collections.Generic.IEnumerator<SecurityCameraEquip> System.Collections.Generic.IEnumerable<SecurityCameraEquip>.GetEnumerator()
        {
            return equip_list_.GetEnumerator();
        }

        protected SecurityCamera() { }

        public static SecurityCamera Load(JObject dc)
        {
            SecurityCamera sc = new SecurityCamera();
            if (dc.Property(_CAMREA_KEY)!=null)
            {
                JArray karr = (JArray)dc[_CAMREA_KEY];
                JObject[] keepers = karr.ToObject<JObject[]>();

                foreach (JObject jo in keepers)
                {
                    sc.equip_list_.Add(new SecurityCameraEquip(jo));
                }
            }

            return sc;
        }

        public static void SaveToJObj(JObject root, SecurityCamera cam)
        {
            root[_CAMREA_KEY] = cam.CloneJArray();
        }
        
        private JArray CloneJArray()
        {
            JArray ar = new JArray();
            foreach (SecurityCameraEquip sce in equip_list_)
            {
                ar.Add(sce.CloneJObject());
            }
            return ar;
        }

        public int Count
        {
            get
            {
                return equip_list_.Count;
            }
        }

        public SecurityCameraEquip GetByIndex(int i)
        {
            return equip_list_[i];
        }

        public int RemoveObject(SecurityCameraEquip sce)
        {
            if (null == sce)
                return 0;
            int removed = 0;
            bool should_continue;
            do
            {
                should_continue = false;
                for (int i = 0; i < equip_list_.Count; ++i)
                {
                    if (sce.Equals(equip_list_[i]))
                    {
                        equip_list_.RemoveAt(i);
                        removed++;
                        should_continue = true;
                        break;
                    }
                }
            } while (should_continue);
            return removed;
        }

        public void RemoveAll()
        {
            equip_list_.Clear();
        }

        public void AddOne(int r,int c)
        {
            SecurityCameraEquip sce = new SecurityCameraEquip(r, c,"");
            if (!equip_list_.Exists(delegate(SecurityCameraEquip rhs) { return sce.Equals(rhs); }))
            {
                equip_list_.Add(sce);
            }
        }

        public static SecurityCamera Default()
        {
            return new SecurityCamera();
        }

        private readonly List<SecurityCameraEquip> equip_list_ = new List<SecurityCameraEquip>();

        public const String _CAMREA_KEY = "camera";
    }
}
