using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DoodleControls
{
    public class TrapSetPoint
    {
        public TrapSetPoint(int r,int c)
        {
            jar_ = new JArray();
            while (jar_.Count < 2)
            {
                jar_.Add("-1");
            }
        }

        public TrapSetPoint(JArray jar)
        {
            jar_ = jar.DeepClone().ToObject<JArray>();
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
            set
            {
                jar_[0] = value.ToString();
            }
        }

        public int Column
        {
            get
            {
                return jar_[1].ToObject<int>();
            }
            set
            {
                jar_[1] = value.ToString();
            }
        }

        public override string ToString()
        {
            return String.Format("[{0},{1}]", Row, Column);
        }

        public JArray CloneJArray()
        {
            return jar_.DeepClone().ToObject<JArray>();
        }

        private JArray jar_;
    }

    public class TrapSet
    {
        public static TrapSet Load(JObject info)
        {
            TrapSet ts = new TrapSet();
            if (info.Property(KKB_TRAPS_NAME)!=null)
            {
                JArray jar = (JArray)info[KKB_TRAPS_NAME];
                foreach (Object o in jar)
                {
                    List<TrapSetPoint> group = new List<TrapSetPoint>();
                    foreach (Object so in (JArray)o)
                    {
                        JArray element = (JArray)so;
                        group.Add(new TrapSetPoint(element));
                    }
                    ts.list_.Add(group);
                }
            }

            while(ts.list_.Count < 2)
            {
                ts.list_.Add(new List<TrapSetPoint>());
            }

            return ts;
        }

        public static void SaveToJObj(JObject root, TrapSet ts)
        {
            root[KKB_TRAPS_NAME] = ts.CloneJArray();
        }

        public static TrapSet DefaultTrap()
        {
            TrapSet ts = new TrapSet();
            while (ts.list_.Count < 2)
                ts.list_.Add(new List<TrapSetPoint>());
            return ts;
        }

        public int SetCount
        {
            get
            {
                return list_.Count;
            }
        }

        public void AddTrapAt(int r, int c, int g)
        {
            list_[g].Add(new TrapSetPoint(r, c));
        }

        public void RemoveAll()
        {
            foreach (List<TrapSetPoint> ls in list_)
            {
                ls.Clear();
            }
        }

        public void SwitchTrapGroupIn(String name)
        {
            int g;
            TrapSetPoint tsp = ExtractPoint(name, out g);
            if (tsp != null)
            {
                g = (g+1)%list_.Count;
                list_[g].Add(tsp);
            }
        }

        private TrapSetPoint ExtractPoint(String name,out int current_group)
        {
            current_group = -1;
            TrapSetPoint tsp = null;
            for(int i=0;i<list_.Count;++i)
            {
                foreach (TrapSetPoint it in list_[i])
                {
                    if (String.CompareOrdinal(it.ToString(), name) == 0)
                    {
                        tsp = it;
                        break;
                    }
                }
                if (tsp != null)
                {
                    list_[i].Remove(tsp);
                    current_group = i;
                    break;
                }
            }
            return tsp;
        }

        public void RemoveTrapByName(String name)
        {
            for (int i = 0; i < list_.Count; ++i)
            {
                RemoveTrapByNameInGroup(i, name);
            }
        }

        private void RemoveTrapByNameInGroup(int i, String name)
        {
            TrapSetPoint tsp = null;
            do
            {
                tsp = null;
                foreach (TrapSetPoint it in list_[i])
                {
                    if (String.CompareOrdinal(it.ToString(), name) == 0)
                    {
                        tsp = it;
                        break;
                    }
                }
                list_[i].Remove(tsp);
            } while (tsp!=null);
        }

        public List<TrapSetPoint> GetSet(int c)
        {
            return list_[c];
        }

        private JArray CloneJArray()
        {
            JArray jar = new JArray();
            foreach (List<TrapSetPoint> ls in list_)
            {
                JArray group = new JArray();
                foreach (TrapSetPoint tsp in ls)
                {
                    group.Add(tsp.CloneJArray());
                }
                jar.Add(group);
            }
            return jar;
        }

        private readonly List<List<TrapSetPoint>> list_ = new List<List<TrapSetPoint>>();
        private static readonly String KKB_TRAPS_NAME = "traps";
    }
}
