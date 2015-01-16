using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    internal class PalaceControl
    {
        private PalaceControl(JObject jo)
        {
            gate_ = new Gate(jo["gate"].ToObject<JObject>());
            eq_set_ = new PosWithStatusSet(jo["set"].ToObject<JArray>());
        }

        public static PalaceControl Load(JObject info)
        {
            PalaceControl pc = null;
            if (info.Property(PALACE_CONTROL_NAME)!=null)
            {
                pc = new PalaceControl(info[PALACE_CONTROL_NAME].ToObject<JObject>());
            }
            return pc;
        }

        public static void SaveToJObj(JObject root,PalaceControl pc)
        {
            if (pc != null)
            {
                root[PALACE_CONTROL_NAME] = pc.CloneObject();
            }
        }

        public JObject CloneObject()
        {
            JObject jo = new JObject();
            jo["gate"] = gate_.CloneObject();
            jo["set"] = eq_set_.CloneJArray();
            return jo;
        }

        private readonly Gate gate_;
        private readonly PosWithStatusSet eq_set_;

        private static readonly String PALACE_CONTROL_NAME = "palace_control";
    }

}
