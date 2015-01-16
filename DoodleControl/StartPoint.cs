using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DoodleControls
{
    internal class StartPoint
    {
        public JObject CloneJObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }

        public static StartPoint LoadFromJson(JObject info)
        {
            StartPoint born = new StartPoint(new JObject());
            if (info.Property(START_POINT)!=null)
            {
                born = new StartPoint(info[START_POINT].ToObject<JObject>());
            }
            return born;
        }

        public static void SaveToJObj(JObject root, StartPoint sp)
        {
            root[START_POINT] = sp.CloneJObject();
        }

        public StartPoint(JObject o)
        {
            jobj_ = o.DeepClone().ToObject<JObject>();
            if (jobj_.Property(ROW) == null)
            {
                jobj_[ROW] = "1";
            }
            if (jobj_.Property(COLUMN) == null)
            {
                jobj_[COLUMN] = "1";
            }
        }

        public StartPoint()
            : this(new JObject())
        {
        }

        private void SetValue(int row, int col)
        {
            Row = row;
            Column = col;
        }

        public int Row
        {
            get
            {
                return jobj_[ROW].ToObject<int>();
            }
            set
            {
                jobj_[ROW] = value.ToString();
            }

        }

        public int Column
        {
            get
            {
                return jobj_[COLUMN].ToObject<int>();
            }
            set
            {
                jobj_[COLUMN] = value.ToString();
            }
        
        }

        public void SetRow(int r)
        {
            Row = r;
        }

        public void SetColumn(int c)
        {
            Column = c;
        }

        private readonly JObject jobj_;

        private static readonly String ROW = "row";
        private static readonly String COLUMN = "column";
        private static readonly String START_POINT ="start_point";
    }
}
