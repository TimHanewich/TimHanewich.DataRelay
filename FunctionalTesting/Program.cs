using System;
using HanewichDataRelay;

namespace FunctionalTesting
{
    class Program
    {
        static void Main(string[] args)
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
    }
}
