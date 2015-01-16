using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    public class Gate
    {
        public Gate(JObject jo)
        {
            jobj_ = jo.DeepClone().ToObject<JObject>();
            InitializeFields();
        }

        private Gate():this(new JObject())
        {
        }

        public static Gate CreateInstance()
        {
            return new Gate();
        }

        public int Row
        {
            get
            {
                return jobj_[ROW_FIELD].ToObject<int>();
            }
            set
            {
                jobj_[ROW_FIELD] = value.ToString();
            }
        }

        public int Column
        {
            get
            {
                return jobj_[COL_FIELD].ToObject<int>();
            }
            set
            {
                jobj_[COL_FIELD] = value.ToString();
            }
        }

        public String Permutation
        {
            get
            {
                return jobj_[PERM_FIELD].ToObject<String>();
            }
            set
            {
                jobj_[PERM_FIELD] = value;
            }
        }

        //public void SetValue(JObject ginfo)
        //{
        //    jobj_ = new JObject(ginfo.ToString());
        //    InitializeFields();
        //}

        public JObject CloneObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        private void InitializeFields()
        {
            if (null == jobj_.Property(ROW_FIELD))
            {
                jobj_[ROW_FIELD] = "-1";
            }
            if (null == jobj_.Property(COL_FIELD))
            {
                jobj_[COL_FIELD] = "-1";
            }
            if (null == jobj_.Property(PERM_FIELD))
            {
                jobj_[PERM_FIELD] = "";
            }
        }

        public override String ToString()
        {
            //~ FORMAT !
            return jobj_.ToString();
        }

        private readonly JObject jobj_;

        private static readonly String ROW_FIELD = "row";
        private static readonly String COL_FIELD = "column";
        private static readonly String PERM_FIELD = "perm";
    }
}
