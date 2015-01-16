using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleControls;

namespace TestCake
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = @"E:\GDJuno\KingkongBondM\data\res\level\";
            int tot = 0;
            int passed = 0;
            foreach (String f in files)
            {
                tot++;
                String full = path + f;
                try
                {
                    String tmp="d:\\temp_2013Feb5th2253.json";
                    DoodleCake dc = new DoodleCake(full);
                    String content0 = dc.MakeJObject().ToString();
                    dc.WriteToJson(tmp);
                    dc = new DoodleCake(tmp);
                    String content1 = dc.MakeJObject().ToString();
                    if (String.CompareOrdinal(content0, content1) == 0)
                    {
                        passed++;
                        Console.WriteLine(f + " passed");
                    }
                    else
                    {
                        throw new Exception(f + " different after write twice !");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(full + " has ocurred a problem");
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("\nPassed / All = {0} / {1}", passed, tot);
            Console.WriteLine(passed == tot ? "All is Done !" : "Some problems remain");
        }

        private static String[] files = new String[]{
            "flirping_guard.json",
            "light_control_demo.json",
            "maze0.json",
            "maze1.json",
            "maze10.json",
            "maze11.json",
            "maze12.json",
            "maze13.json",
            "maze14.json",
            "maze15.json",
            "maze16.json",
            "maze17.json",
            "maze18.json",
            "maze19.json",
            "maze2.json",
            "maze20.json",
            "maze3.json",
            "maze4.json",
            "maze5.json",
            "maze6.json",
            "maze7.json",
            "maze8.json",
            "maze9.json",
            "maze_test_a.json",
            "palace_control_demo.json",
            "puzzle_control_demo.json",
            "remote_control_demo.json"
        };
    }
}
