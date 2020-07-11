using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace HanewichDataRelay
{
    public class DataPackageStitcher
    {
        private List<DataPackage> ReceivedDataPackages {get; set;}

        public DataPackageStitcher()
        {
            ReceivedDataPackages = new List<DataPackage>();
        }

        public void ReceiveDataPackage(DataPackage package)
        {
            //If we have some in there already, do some checks
            if (ReceivedDataPackages.Count > 0)
            {
                //Check every one
                foreach (DataPackage dp in ReceivedDataPackages)
                {
                
                    //Check identifier
                    if (dp.StreamIdentifier.ToString() != package.StreamIdentifier.ToString())
                    {
                        throw new Exception("The data package stream identifier of the provided package does not match the current repository.");
                    }

                    //Check # of packages
                    if (dp.StreamPackageCount != package.StreamPackageCount)
                    {
                        throw new Exception("The number of packages that the provided package describes should be in the stream does not equal that of the repository of received data packages.");
                    }

                    //Check id (it cannot be an over-write)
                    if (dp.PackageNumber == package.PackageNumber)
                    {
                        throw new Exception("We already received package #" + package.PackageNumber.ToString() + "!");
                    }

                }
            }

            ReceivedDataPackages.Add(package);
        }

        public bool AllPackagesReceived()
        {
            int should_be_count = ReceivedDataPackages[0].StreamPackageCount;
            if (ReceivedDataPackages.Count < should_be_count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public byte[] GetCombinedContent()
        {
            if (ReceivedDataPackages.Count == 0)
            {
                List<byte> bs = new List<byte>();
                return bs.ToArray();
            }
            else
            {
                return DataPackage.GatherCombinedContent(ReceivedDataPackages.ToArray());
            }
        }
    
        public void Reset()
        {
            ReceivedDataPackages.Clear();
        }
    }
}