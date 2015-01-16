using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DoodleControls
{
    public class KKbLayerItem
    {
        public KKbLayerItem(JObject jo)
        {
            jobj_ = jo.DeepClone().ToObject<JObject>();
            if (jobj_.Property("r") == null)
            {
                jobj_["r"] = "-1";
            }
            if (jobj_.Property("c") == null)
            {
                jobj_["c"] = "-1";
            }
            if (jobj_.Property("tex") == null)
            {
                jobj_["tex"] = "0";
            }
        }

        public KKbLayerItem(int r,int c,int t)
        {
            jobj_ = new JObject();
            jobj_["r"] = r.ToString();
            jobj_["c"] = c.ToString();
            jobj_["tex"] = t.ToString();
        }

        public static KKbLayerItem Load(JObject info)
        {
            KKbLayerItem item = new KKbLayerItem(info);
            return item;
        }

        public JObject CloneJObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        public bool InPlace(int r, int c)
        {
            return Row == r && Colum == c;
        }

        public int Tex
        {
            get
            {
                return jobj_["tex"].ToObject<int>();
            }
            set
            {
                jobj_["tex"] = value.ToString();
            }
        }

        public int Row
        {
            get
            {
                return jobj_["r"].ToObject<int>();
            }
        }

        public int Colum
        {
            get
            {
                return jobj_["c"].ToObject<int>();
            }
        }

        private readonly JObject jobj_;
    }

    public class KkbStandardLayer
    {
        public KkbStandardLayer(JObject info)
        {
            jobj_ = info.DeepClone().ToObject<JObject>();
            if (jobj_.Property("details") == null)
            {
                jobj_["details"] = new JArray();
            }

            JArray ar = (JArray)jobj_["details"];
            foreach (JObject o in ar)
            {
                items_.Add(new KKbLayerItem(o));
            }
        }

        private void UpdateToItems()
        {
            JArray new_items = new JArray();
            foreach (KKbLayerItem ki in items_)
            {
                new_items.Add(ki.CloneJObject());
            }
            jobj_["details"] = new_items;
        }

        public String Name
        {
            get
            {
                return jobj_["name"].ToObject<String>();
            }
            set
            {
                jobj_["name"] = value;
            }
        }

        public JObject CloneJObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        public void SetTexture(int x, int y, int t)
        {
            if (RemoveTexId == t)
            {
                while(true)
                {
                    KKbLayerItem target = null;
                    foreach(KKbLayerItem it in items_)
                    {
                        if (it.InPlace(y, x))
                        {
                            target = it;
                            break;
                        }
                    }
                    if (null == target)
                        break;
                    items_.Remove(target);
                }
            }
            else
            {
                KKbLayerItem target = null;
                foreach (KKbLayerItem it in items_)
                {
                    if (it.InPlace(y, x))
                    {
                        target = it;
                        break;
                    }
                }
                if (target != null)
                {
                    target.Tex = t;
                }
                else
                {
                    items_.Add(new KKbLayerItem(y, x, t));
                }
            }

            UpdateToItems();
        }

        public List<KKbLayerItem> Items
        {
            get
            {
                return items_;
            }
        }
        
        private JObject jobj_;  //~ with name
        private readonly List<KKbLayerItem> items_=new List<KKbLayerItem>();   //~ 
        public const int RemoveTexId = -1;
    }


    public class KkbLayers
    {
        public static KkbLayers Load(JObject info)
        {
            KkbLayers kl = new KkbLayers();
            if (info.Property(KKB_LAYERS_NAME)!=null)
            {
                JArray jar = (JArray)info[KKB_LAYERS_NAME];
                foreach (JObject o in jar)
                {
                    kl.layers_.Add(new KkbStandardLayer(o));
                }
            }
            return kl;
        }

        public static void SaveToJObj(JObject root, KkbLayers theLayers)
        {
            root[KKB_LAYERS_NAME] = theLayers.CloneJArray();
        }
    
        private JArray CloneJArray()
        {
            JArray jar = new JArray();
            foreach (KkbStandardLayer layer in layers_)
            {
                jar.Add(layer.CloneJObject());
            }
            return jar;
        }

        public KkbStandardLayer Find(String name)
        {
            KkbStandardLayer rv = null;
            foreach (KkbStandardLayer layer in layers_)
            {
                if (String.CompareOrdinal(layer.Name, name) == 0)
                {
                    rv = layer;
                    break;
                }
            }
            return rv;
        }

        public KkbStandardLayer FindOrCreate(String name)
        {
            KkbStandardLayer rv = Find(name);
            if (null == rv)
            {
                rv = new KkbStandardLayer(new JObject());
                rv.Name = name;
                layers_.Add(rv);
            }
            return rv;
        }

        private readonly List<KkbStandardLayer> layers_ = new List<KkbStandardLayer>();

        private static readonly String KKB_LAYERS_NAME = "layers";
    }
}
