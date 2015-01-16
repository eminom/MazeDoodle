using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    public class RowColumPos
    {
        public RowColumPos(int r, int c)
        {
            jobj_ = new JObject();
            InitializeSetPosFields();
            jobj_[ROW] = r.ToString();
            jobj_[COLUMN] = c.ToString();
        }

        public RowColumPos(JObject entry)
        {
            jobj_ = entry.DeepClone().ToObject<JObject>();
            InitializeSetPosFields();
        }

        public override String ToString()
        {
            return jobj_.ToString();
        }

        public String FormatSubName()
        {
            return jobj_.ToString();
        }

        public int GetRow()
        {
            return jobj_[ROW].ToObject<int>();
        }

        public int GetColumn()
        {
            return jobj_[COLUMN].ToObject<int>();
        }

        private void InitializeSetPosFields()
        {
            if (jobj_.Property(ROW) == null)
            {
                jobj_[ROW] = "-1";
            }
            if (jobj_.Property(COLUMN) == null)
            {
                jobj_[COLUMN] = "-1";
            }
        }

        public JObject CloneObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        private readonly JObject jobj_;
        private static readonly String ROW = "row";
        private static readonly String COLUMN = "column";
    }

    public class StatusSetPos
    {
        public StatusSetPos(int r,int c,int s)
        {
            jobj_ = new JObject();
            InitializeStatusSetPosFields();
            jobj_[ROW] = r.ToString();
            jobj_[COLUMN] = c.ToString();
            jobj_[STATUS] = s.ToString();
        }

        public StatusSetPos(JObject o)
        {
            jobj_ = o.DeepClone().ToObject<JObject>();
            InitializeStatusSetPosFields();
        }

        public String FormatSubName()
        {
            return jobj_.ToString();
        }

        public int GetRow()
        {
            return jobj_[ROW].ToObject<int>();
        }

        public int GetColumn()
        {
            return jobj_[COLUMN].ToObject<int>();
        }

        private void InitializeStatusSetPosFields()
        {
            if (jobj_.Property(ROW) == null)
            {
                jobj_[ROW] = "-1";
            }
            if (jobj_.Property(COLUMN) == null)
            {
                jobj_[COLUMN] = "-1";
            }
            if (jobj_.Property(STATUS) == null)
            {
                jobj_[STATUS] = "0";
            }
        }

        public JObject CloneObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        private JObject jobj_;
        private static readonly String ROW = "ROW";
        private static readonly String COLUMN = "COLUMN";
        private static readonly String STATUS = "STATUS";
    }

    internal class RowColumnPosSet : IEnumerable, IEnumerable<RowColumPos>
    {
        public String FormatEqSetString
        {
            get
            {
                String rv = "";
                foreach (RowColumPos sp in set_)
                {
                    rv += sp.FormatSubName();
                    rv += ", ";
                }
                return rv;
            }
        }

        public void SetValue(JArray jarr)
        {
            foreach (JObject o in jarr)
            {
                set_.Add(new RowColumPos(o));
            }
        }

        public JArray CloneJObject()
        {
            JArray arr = new JArray();
            foreach (RowColumPos sp in set_)
            {
                arr.Add(sp.CloneObject());
            }
            return arr;
        }

        public IEnumerator<RowColumPos> GetEnumerator()
        {
            return set_.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return set_.GetEnumerator();
        }

        public void AddSubPos(int r, int c)
        {
            set_.Add(new RowColumPos(r, c));
        }

        public readonly List<RowColumPos> set_ = new List<RowColumPos>();
    }

    internal class PosWithStatusSet : IEnumerable, IEnumerable<StatusSetPos>
    {
        public String FormatEqSetString
        {
            get
            {
                String rv = "";
                foreach (StatusSetPos sp in set_)
                {
                    rv += sp.FormatSubName();
                    rv += ", ";
                }
                return rv;
            }
        }

        public void SetValue(JArray jarr)
        {
            foreach (JObject o in jarr)
            {
                set_.Add(new StatusSetPos(o));
            }
        }

        public JArray CloneJArray()
        {
            JArray arr = new JArray();
            foreach (StatusSetPos sp in set_)
            {
                arr.Add(sp.CloneObject());
            }
            return arr;
        }

        public PosWithStatusSet(JArray ar)
        {
            SetValue(ar);
        }

        public IEnumerator<StatusSetPos> GetEnumerator()
        {
            return set_.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return set_.GetEnumerator();
        }

        public void AddSubPos(int r, int c)
        {
            set_.Add(new StatusSetPos(r, c,0));
        }

        public readonly List<StatusSetPos> set_ = new List<StatusSetPos>();
    }
}
