using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace HanewichDataRelay
{
    public class DataPackage
    {
        public Guid StreamIdentifier {get; set;}
        public int StreamPackageCount {get; set;}
        public int PackageNumber {get; set;}
        public byte[] Content {get; set;}

        public static DataPackage Deserialize(byte[] bytes)
        {
            //The byte array length has to be at least 24 bytes
            if (bytes.Length < 24)
            {
                throw new Exception("The byte array for this type of data package should be at least 24 bytes. You passed an array with " + bytes.Length.ToString() + " bytes.");
            }

            DataPackage dp = new DataPackage();

            //Get guid (Stream Identifier)
            List<byte> siguid = new List<byte>();
            siguid.Add(bytes[0]);
            siguid.Add(bytes[1]);
            siguid.Add(bytes[2]);
            siguid.Add(bytes[3]);
            siguid.Add(bytes[4]);
            siguid.Add(bytes[5]);
            siguid.Add(bytes[6]);
            siguid.Add(bytes[7]);
            siguid.Add(bytes[8]);
            siguid.Add(bytes[9]);
            siguid.Add(bytes[10]);
            siguid.Add(bytes[11]);
            siguid.Add(bytes[12]);
            siguid.Add(bytes[13]);
            siguid.Add(bytes[14]);
            siguid.Add(bytes[15]);
            dp.StreamIdentifier = new Guid(siguid.ToArray());

            //Get how many packets are part of this stream
            List<byte> spc = new List<byte>();
            spc.Add(bytes[16]);
            spc.Add(bytes[17]);
            spc.Add(bytes[18]);
            spc.Add(bytes[19]);
            dp.StreamPackageCount = BitConverter.ToInt32(spc.ToArray(), 0);


            //Get what package number this is
            List<byte> pn = new List<byte>();
            pn.Add(bytes[20]);
            pn.Add(bytes[21]);
            pn.Add(bytes[22]);
            pn.Add(bytes[23]);
            dp.PackageNumber = BitConverter.ToInt32(pn.ToArray(), 0);

            //Get the remainder
            List<byte> package_content = new List<byte>();
            int t = 0;
            for (t=24;t<bytes.Length;t++)
            {
                package_content.Add(bytes[t]);
            }
            dp.Content = package_content.ToArray();


            return dp;
        }

        public byte[] Serialize()
        {
            List<byte> ser = new List<byte>();
            
            //Put in guid
            ser.AddRange(StreamIdentifier.ToByteArray());

            //Put in Stream Package count
            ser.AddRange(BitConverter.GetBytes(StreamPackageCount));

            //Put in my package number
            ser.AddRange(BitConverter.GetBytes(PackageNumber));

            //Put in the content
            ser.AddRange(Content);

            return ser.ToArray();
        }
    
        public static DataPackage[] CreateDataPackages(byte[] content, int max_bytes_per_package = 1024)
        {
            double d = (double)content.Length / (double)max_bytes_per_package;
            int packages_needed = (int)Math.Ceiling(d);
            MemoryStream ms = new MemoryStream(content);
            ms.Position = 0;

            List<DataPackage> ToReturn = new List<DataPackage>();
            int t = 0;
            for (t=0;t<packages_needed;t++)
            {
                //Basic data
                DataPackage dp = new DataPackage();
                dp.StreamPackageCount = packages_needed;
                dp.PackageNumber = t + 1;


                //Get content
                List<byte> cont = new List<byte>();
                int tb = 0;
                for (tb=0;tb<max_bytes_per_package;tb++)
                {
                    if (ms.Position < ms.Length)
                    {
                        cont.Add((byte)ms.ReadByte());
                    }   
                }
                dp.Content = cont.ToArray();

                ToReturn.Add(dp);

            }
        
            return ToReturn.ToArray();
        }

        public static byte[] GatherCombinedContent(DataPackage[] packages)
        {


            //First arrange by package number
            List<DataPackage> fo = packages.ToList();
            List<DataPackage> f1 = new List<DataPackage>();
            while (fo.Count > 0)
            {
                DataPackage winner = fo[0];
                foreach (DataPackage dp in fo)
                {
                    if (dp.PackageNumber < winner.PackageNumber)
                    {
                        winner = dp;
                    }
                }
                f1.Add(winner);
                fo.Remove(winner);
            }

            //Get the bytes
            List<byte> ToReturn = new List<byte>();
            foreach (DataPackage dp in f1)
            {
                ToReturn.AddRange(dp.Content);
            }

            return ToReturn.ToArray();
        }

        #region "Utility Functions"
        public static byte[] StringToBytes(string str)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(str);
            sw.Flush();
            ms.Position = 0;

            List<byte> ToReturn = new List<byte>();
            while (ms.Position < ms.Length)
            {
                ToReturn.Add((byte)ms.ReadByte());
            }

            return ToReturn.ToArray();         
        }
    
        public static string BytesToString(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            StreamReader sr = new StreamReader(ms);
            string s = sr.ReadToEnd();
            return s;
        }
    
        #endregion
    }

}