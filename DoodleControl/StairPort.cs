using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    public class StairPort
    {
        public static List<StairPort> LoadStairPorts(JObject info)
        {
            List<StairPort> rs = new List<StairPort>();
            if (info.Property(STAIR_PORT_LIST_NAME)!=null)
            {
                JArray arr = info[STAIR_PORT_LIST_NAME].ToObject<JArray>();
                int[][] sps = arr.ToObject<int[][]>();
                for (int i = 0; i < sps.GetLength(0); ++i)
                {
                    rs.Add(new StairPort(sps[i][0], sps[i][1], sps[i][2], sps[i][3]));
                }
            }
            return rs;
        }

        public static void SaveToJObj(JObject root, List<StairPort> ports)
        {
            root[STAIR_PORT_LIST_NAME] = StairPort.CloneJArray(ports);
        }

        private static JArray CloneJArray(List<StairPort> ports)
        {
            JArray ar = new JArray();
            foreach (StairPort sp in ports)
            {
                ar.Add(sp.CloneJObject());
            }
            return ar;
        }

        public JArray CloneJObject()
        {
            return jar_.DeepClone().ToObject<JArray>();
        }

        public StairPort(int r, int c, int to,int items)
        {
            jar_ = new JArray();
            jar_.Add(r.ToString());
            jar_.Add(c.ToString());
            jar_.Add(to.ToString());
            jar_.Add(items.ToString());
        }

        public bool inPos(int x, int y)
        {
            return Row == y && Column == x;
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

        public int Port
        {
            get
            {
                return jar_[2].ToObject<int>();
            }
            set
            {
                jar_[2] = value.ToString();
            }
        }

        public int ItemsRequired
        {
            get
            {
                return jar_[3].ToObject<int>();
            }
            set
            {
                jar_[3] = value.ToString();
            }
        }

        public void setToPort(int new_port,int items)
        {
            Port = new_port;
            ItemsRequired = items;
        }


        private readonly JArray jar_;
        private static readonly String STAIR_PORT_LIST_NAME = "sp_list";
    }
}
