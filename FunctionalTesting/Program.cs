using System;
using HanewichDataRelay;

namespace FunctionalTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            test2();
        }

        static void test1()
        {
            string msg = "Hello world!";
            DataPackage[] packages = DataPackage.CreateDataPackages(DataPackage.StringToBytes(msg), 5);

            DataPackageStitcher dps = new DataPackageStitcher();
            foreach (DataPackage dp in packages)
            {
                dps.ReceiveDataPackage(dp);
            }

            Console.WriteLine(dps.AllPackagesReceived().ToString());
            Console.WriteLine(DataPackage.BytesToString(dps.GetCombinedContent()));
        }

        static void test2()
        {
            string path = "C:\\Users\\tihanewi\\Downloads\\Formula 1 Games Telemetry\\Codemasters F1 2019 4-5-2020.txt";
            string content = System.IO.File.ReadAllText(path);
            DataPackage[] packs = DataPackage.CreateDataPackages(DataPackage.StringToBytes(content));
            string s = DataPackage.BytesToString(DataPackage.GatherCombinedContent(packs));
            Console.WriteLine(s);
        }

    }
}
