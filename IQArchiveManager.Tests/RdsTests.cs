using IQArchiveManager.Client.RDS.Parser.Modes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace IQArchiveManager.Tests
{
    [TestClass]
    public class RdsTests
    {
        private void PerformSingleTest(string rt, string challengeTitle, string challengeArtist)
        {
            Assert.IsTrue(new RdsPatchNative().TryParse(rt, out string title, out string artist, out string station, false));
            Assert.AreEqual(title, challengeTitle);
            Assert.AreEqual(artist, challengeArtist);
        }

        private void PerformTest(string rt, string challengeTitle, string challengeArtist)
        {
            PerformSingleTest(rt, challengeTitle, challengeArtist);
            PerformSingleTest(rt.ToLower(), challengeTitle.ToLower(), challengeArtist.ToLower());
            PerformSingleTest(rt.ToUpper(), challengeTitle.ToUpper(), challengeArtist.ToUpper());
        }

        [TestMethod]
        public void TestKXXR()
        {
            PerformTest("INTERSTATE LOVE SONG by STONE TEMPLE PILOTS On 93X", "INTERSTATE LOVE SONG", "STONE TEMPLE PILOTS");
        }
        
        [TestMethod]
        public void TestKQRS()
        {
            PerformTest("Never Been Any Reason - Head East On KQRS", "Never Been Any Reason", "Head East");
        }

        [TestMethod]
        public void TestKPFX()
        {
            PerformTest("Stairway to Heaven by Led Zeppelin on 107.9 The FOX!", "Stairway to Heaven", "Led Zeppelin");
            PerformTest("Stairway to Heaven - Led Zeppelin - 107.9 The FOX", "Stairway to Heaven", "Led Zeppelin");
        }

        [TestMethod]
        public void TestWHMH()
        {
            PerformTest("Shimmer - FUEL", "Shimmer", "FUEL");
        }
        
        [TestMethod]
        public void TestKZJK()
        {
            PerformTest("Now playing Hysteria by Def Leppard on JACK FM", "Hysteria", "Def Leppard");
        }
    }
}
