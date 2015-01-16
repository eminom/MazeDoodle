using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    public class DoodleItem
    {
        public static List<DoodleItem> LoadItems(JObject root)
        {
            List<DoodleItem> dis = new List<DoodleItem>();
            if( root.Property(DOODLE_ITEM_NAME) != null )
            {
                JArray arr = root[DOODLE_ITEM_NAME].ToObject<JArray>();
                object[][] items = arr.ToObject<object[][]>();
                foreach(JToken jo in arr)
                {
                    dis.Add(new DoodleItem(jo.ToObject<JArray>()));
                }
            }
            return dis;
        }

        public static void SaveToJObj(JObject root, List<DoodleItem> items)
        {
            root[DOODLE_ITEM_NAME] = DoodleItem.CloneJArray(items);
        }

        private static JArray CloneJArray(List<DoodleItem> items)
        {
            JArray ar = new JArray();
            foreach(DoodleItem it in items)
            {
                ar.Add(it.CloneJArray());
            }
            return ar;
        }

        public JArray CloneJArray()
        {
            return jar_.DeepClone().ToObject<JArray>();
        }

        public DoodleItem(JArray jar)
        {
            jar_ = jar.DeepClone().ToObject<JArray>();
            if (jar_.Count < 4)
            {
                int left = 4 - jar_.Count;
                for (int i = 0; i < left; ++i)
                {
                    jar_.Add("0");
                }
            }
        }

        public DoodleItem(int r, int c, int v, String specialty)
        {
            jar_ = new JArray();
            jar_.Add(r.ToString());
            jar_.Add(c.ToString());
            jar_.Add(v.ToString());
            jar_.Add(specialty);
        }

        public int Item
        {
            get
            {
                if (jar_.Count >= 3)
                    return jar_[2].ToObject<int>();
                return 0;
            }
        }

        public String Specialty
        {
            get
            {
                if (jar_.Count >= 3)
                    return jar_[3].ToObject<String>();
                return "";
            }
        }

        public int Row
        {
            get
            {
                if (jar_.Count >= 1)
                    return jar_[0].ToObject<int>();
                return -1;
            }
        }

        public int Column
        {
            get
            {
                if (jar_.Count >= 2)
                    return jar_[1].ToObject<int>();
                return -1;
            }
        }

        public void setItemValue(int v)
        {
            while(jar_.Count < 3 )
            {
                jar_.Add("-1");
            }
            jar_[2] = v.ToString();
        }

        public bool inPos(int x, int y)
        {
            return Row == y && Column == x;
        }

        private readonly JArray jar_;

        private static readonly String DOODLE_ITEM_NAME = "item_list";
    }
}
