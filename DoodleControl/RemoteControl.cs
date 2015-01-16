using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DoodleControls
{
    public class RemoteControl:IEnumerable,IEnumerable<RowColumPos>
    {
        private RemoteControl()
        {
            gate_ = Gate.CreateInstance();
            eq_set_ = new RowColumnPosSet();
        }

        public RemoteControl(JObject jobj)
        {
            gate_ = new Gate(jobj["gate"].ToObject<JObject>());
            eq_set_ = new RowColumnPosSet();

            eq_set_.SetValue(jobj["set"].ToObject<JArray>());
        }

        public static RemoteControl MakeTmp()
        {
            return new RemoteControl();
        }
        
        public static List<RemoteControl> Load(JObject info)
        {
            List<RemoteControl> remotes = new List<RemoteControl>();
            if( info.Property(REMOTE_CONTROLS_NAME)!=null )
            {
                JArray remote_arr = info[REMOTE_CONTROLS_NAME].ToObject<JArray>();
                for (int i = 0; i < remote_arr.Count; ++i)
                {
                    RemoteControl rc = new RemoteControl(remote_arr[i].ToObject<JObject>());
                    remotes.Add(rc);
                }
            }
            return remotes;
        }

        public static void SaveToJObj(JObject root, List<RemoteControl> ls)
        {
            root[REMOTE_CONTROLS_NAME] = RemoteControl.CloneJArray(ls);
        }

        private static JArray CloneJArray(List<RemoteControl> list)
        {
            JArray ar = new JArray();
            foreach (RemoteControl rc in list)
            {
                ar.Add(rc.CloneObject());
            }
            return ar;
        }

        public JObject CloneObject(List<RemoteControl> remotes)
        {
            JObject rv = new JObject();
            JArray ar = new JArray();
            rv[REMOTE_CONTROLS_NAME] = ar;
            foreach (RemoteControl rc in remotes)
            {
                ar.Add(new JObject(rc.ToString()));
            }
            return rv;
        }

        public int GateRow
        {
            get
            {
                return gate_.Row;
            }
        }

        public int GateColumn
        {
            get
            {
                return gate_.Column;
            }
        }

        public void SetGatePos(int r, int c)
        {
            gate_.Row = r;
            gate_.Column = c;
        }

        public void AddSub(int r, int c)
        {
            eq_set_.AddSubPos(r,c);
        }

        public String FormatGateName
        {
            get
            {
                return gate_.ToString();
            }
        }

        public String FormatEqSetString
        {
            get
            {
                return eq_set_.FormatEqSetString;
            }
        }

        public override String ToString()
        {
            return CloneObject().ToString();
        }

        public JObject CloneObject()
        {
            JObject jo = new JObject();
            jo["gate"] = gate_.CloneObject();
            jo["set"] = eq_set_.CloneJObject();
            return jo;
        }

        IEnumerator<RowColumPos> IEnumerable<RowColumPos>.GetEnumerator()
        {
            return eq_set_.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return eq_set_.GetEnumerator();
        }

        private readonly Gate gate_;
        private readonly RowColumnPosSet eq_set_;

        private static readonly String REMOTE_CONTROLS_NAME = "remote_controls";
        //private static readonly String REMOTE_SINGLE = "remote_control";
    }
}
