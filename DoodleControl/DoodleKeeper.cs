using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DoodleControls
{
    public class DoodleKeeper
    {
        public static List<DoodleKeeper> Load(JObject info)
        {
            List<DoodleKeeper> rs = new List<DoodleKeeper>();

            if (info.Property(KEEPER_LIST_NAME)!=null)
            {
                JArray karr = (JArray)info[KEEPER_LIST_NAME];
                JObject[] keepers = karr.ToObject<JObject[]>();
                foreach (JObject jo in keepers)
                {
                    rs.Add(new DoodleKeeper(jo));
                }
            }
            return rs;
        }

        public static void SaveToJObj(JObject root, List<DoodleKeeper> list)
        {
            root[KEEPER_LIST_NAME] = DoodleKeeper.CloneJArray(list);
        }
        
        private static JArray CloneJArray(List<DoodleKeeper> keepers)
        {
            JArray ar = new JArray();
            foreach (DoodleKeeper dk in keepers)
            {
                ar.Add(dk.CloneJObject());
            }
            return ar;
        }

        private DoodleKeeper(int r,int c,String[] routes,String type_of_route,int velo,int skin):this()
        {
            jobj_ = JObject.Parse("{\"start_point\":{\"row\":\"-1\",\"column\":\"-1\"},\"route\":[],\"type\":\"default\",\"velocity\":\"0\",\"skin\":\"0\"}");
            start_point_ = (JObject)jobj_["start_point"];
            routes_ = (JArray)jobj_["route"];

            //~
            start_point_["row"] = r;
            start_point_["column"] = c;
            foreach(String rte in routes)
            {
                routes_.Add(rte);
            }
            jobj_["type"] = type_of_route;
            jobj_["velocity"] = Velocity;
            jobj_["skin"] = skin;
        }

        private DoodleKeeper(JObject jobj)
        {
            jobj_ = jobj.DeepClone().ToObject<JObject>();
            if (jobj_.Property(KEEPER_START_POINT) == null)
            {
                jobj_[KEEPER_START_POINT] = JObject.Parse("{\"row\":\"1\",\"column\":\"1\"}");
            }

            if (jobj_.Property(KEEPER_ROUTE) == null)
            {
                jobj_[KEEPER_ROUTE] = new JArray();
            }

            start_point_ = (JObject)jobj_[KEEPER_START_POINT];
            routes_ = (JArray)jobj_[KEEPER_ROUTE];
        }

        private DoodleKeeper()
        {
            jobj_ = JObject.Parse("{\"start_point\":{\"row\":\"-1\",\"column\":\"-1\"},\"route\":[],\"type\":\"default\",\"velocity\":\"0\",\"skin\":\"0\"}");
            start_point_ = (JObject)jobj_["start_point"];
            routes_ = (JArray)jobj_["route"];
        }

        public JObject CloneJObject()
        {
            return jobj_.DeepClone().ToObject<JObject>();
        }


        public static DoodleKeeper MakeTempKeeper()
        {
            return new DoodleKeeper();
        }

        public int StartRow
        {
            get
            {
                return start_point_["row"].ToObject<int>();
            }
        }

        public int StartColumn
        {
            get
            {
                return start_point_["column"].ToObject<int>();
            }
        }

        public String RouteType
        {
            get
            {
                return jobj_[KEEPER_ROUTE_TYPE].ToObject<String>();
            }
        }

        public void SetRouteType(String t)
        {
            jobj_[KEEPER_ROUTE_TYPE] = t;
        }

        public void SetStartPoint(int row, int col)
        {
            Console.WriteLine("Start Point Set to {0}, {1}", row, col);
            start_point_["row"] = row.ToString();
            start_point_["column"] = col.ToString();
        }

        public String GetShortName()
        {
            return String.Format("R:{0}, C:{1}", StartRow, StartColumn);
        }

        public override string ToString()
        {
            String rv = String.Format("Row = {0}, Col = {1};\n{2}", StartRow, StartColumn, routes_.ToString());
            return rv;
        }

        public String[] Routes
        {
            get
            {
                return routes_.ToObject<String[]>();
            }
        }

        public int Velocity
        {
            get
            {
                return jobj_[KEEPER_VELOCITY].ToObject<int>();
            }
        }

        public int Skin
        {
            get
            {
                return jobj_[KEEPER_SKINSELECT].ToObject<int>();
            }
        }

        public void SetSkin(int newSkin)
        {
            jobj_[KEEPER_SKINSELECT] = newSkin.ToString();
        }

        public void SetVelocity(int newVelo)
        {
            jobj_[KEEPER_VELOCITY] = newVelo.ToString();
        }
        
        public void AddOneRoutePoint(int r, int c)
        {
            String pt = String.Format("point:{0},{1}", r, c);
            Console.WriteLine("Route Point {0} is added.", pt);
            routes_.Add(pt);
        }
        
        private readonly JObject jobj_;
        private readonly JObject start_point_;
        private readonly JArray routes_;

        private static readonly String KEEPER_LIST_NAME = "keepers";
        private static readonly String KEEPER_START_POINT = "start_point";
        private static readonly String KEEPER_ROUTE_TYPE = "type";
        private static readonly String KEEPER_VELOCITY = "velocity";
        private static readonly String KEEPER_SKINSELECT = "skin";
        private static readonly String KEEPER_ROUTE = "route";
    }
}
