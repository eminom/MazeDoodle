using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    public class ShadowPoint
    {
        public static List<ShadowPoint> Load(JObject info)
        {
            List<ShadowPoint> lrv = new List<ShadowPoint>();
            if (info.Property(SHADOW_LIST_NAME)!=null)
            {
                JArray ar = (JArray)info[SHADOW_LIST_NAME];
                foreach(JArray jo in ar)
                {
                    lrv.Add(new ShadowPoint(jo.ToObject<JArray>()));
                }
            }
            return lrv;
        }

        public static void SaveToJObj(JObject root, List<ShadowPoint> points)
        {
            root[SHADOW_LIST_NAME] = ShadowPoint.CloneJArray(points);
        }

        private static JArray CloneJArray(List<ShadowPoint> points)
        {
            JArray jar = new JArray();
            foreach (ShadowPoint sp in points)
            {
                jar.Add(sp.CloneObejct());
            }
            return jar;
        }

        public JArray CloneObejct()
        {
            return jar_.DeepClone().ToObject<JArray>();
        }

        public override String ToString()
        {
            return String.Format("[{0},{1}]", Row, Column);
        }

        public bool PositionEqual(ShadowPoint rhs)
        {
            return Row == rhs.Row && Column == rhs.Column;
        }

        public static ShadowPoint Point(int x, int y)
        {
            return new ShadowPoint(y, x);
        }

        private ShadowPoint(int r, int c)
        {
            jar_ = new JArray();
            jar_.Add(r.ToString());
            jar_.Add(c.ToString());
        }

        private ShadowPoint(JArray jo)
        {
            jar_ = jo.DeepClone().ToObject<JArray>();
            while (jar_.Count < 2)
            {
                jar_.Add("-1");
            }
        }

        public int Row
        {
            get
            {
                return jar_[0].ToObject<int>();
            }
        }

        public int Column
        {
            get
            {
                return jar_[1].ToObject<int>();
            }
        }

        private readonly JArray jar_;
        private static readonly String SHADOW_LIST_NAME="shadow";

    }
}
