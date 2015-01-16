using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    internal class LightControl
    {
        private LightControl(int r,int c,int item_req)
        {
            jobj_ = new JObject();
            jobj_[ITEM_REQUIRED] = item_req.ToString();

            //~
            jobj_[SET] = JObject.Parse("{}");
            set_ = (JObject)jobj_[SET];
            set_["row"] = r.ToString();
            set_["column"] = c.ToString();
            //~
        }

        private LightControl(JObject jo)
        {
            jobj_ = jo.DeepClone().ToObject<JObject>();
            InitializeLightControlFields();
            set_ = (JObject)jobj_[SET];
        }

        private void InitializeLightControlFields()
        {
            if (jobj_.Property(ITEM_REQUIRED)==null)
            {
                jobj_[ITEM_REQUIRED] = "128";
            }
            if (jobj_.Property(SET) == null)
            {
                jobj_[SET] = JObject.Parse("{\"row\":\"-1\",\"column\":\"-1\"}");
            }
        }

        public static LightControl Load(JObject info)
        {
            LightControl lc = null;
            if (info.Property(LIGHT_CONTROL_NAME)!=null)
            {
                lc = new LightControl(info[LIGHT_CONTROL_NAME].ToObject<JObject>());
            }

            return lc;
        }

        public static void SaveToJObj(JObject root, LightControl lc)
        {
            if (lc != null)
            {
                root[LIGHT_CONTROL_NAME] = lc.CloneJObject();
            }
        }

        public JObject CloneJObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        private readonly JObject jobj_;
        private readonly JObject set_;
        private static readonly String LIGHT_CONTROL_NAME = "light_control";
        private static readonly String ITEM_REQUIRED = "item_required";
        private static readonly String SET = "set";
    }
}
