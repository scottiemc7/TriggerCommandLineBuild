using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TriggerCommandLineConsole;

namespace TriggerCommandLineConsoleTest
{
    [TestClass]
    public class JSONValueSwapperTest
    {
        private const string JSON = "{\"config_version\":\"4\",\"author\":\"AuthorValue\",\"modules\":{\"name\":\"testName\",\"geolocation\":{\"version\":\"2.0\"}}}";

        [TestMethod]
        public void TestSwap()
        {
            string keyLevel1 = "config_version",
                newValueLevel1 = "5",
                keyLevel2 = "modules\\name",
                newValueLevel2 = "newName",
                keyLevel3 = "modules\\geolocation\\version",
                newValueLevel3 = "6.1",
                JSONAfter = "{\"config_version\":\"5\",\"author\":\"AuthorValue\",\"modules\":{\"name\":\"newName\",\"geolocation\":{\"version\":\"6.1\"}}}";

            IJSONValueSwapper swapper = new JSONValueSwapper(JSON);
            swapper.Swap(keyLevel1, newValueLevel1);
            swapper.Swap(keyLevel2, newValueLevel2);
            swapper.Swap(keyLevel3, newValueLevel3);

            Assert.AreEqual(JSONAfter, swapper.ToString());
        }

        [TestMethod]
        public void PrimativeTypeValuesAreNotWrappedInQuotes()
        {
            string jsonBefore = "{\"test_string\":\"beforeValue\",\"test_number\":1,\"test_bool\":true}",
                jsonAfter = "{\"test_string\":\"afterValue\",\"test_number\":2,\"test_bool\":false}";

            IJSONValueSwapper swapper = new JSONValueSwapper(jsonBefore);
            swapper.Swap("test_string", "afterValue");
            swapper.Swap("test_number", 2);
            swapper.Swap("test_bool", false);

            Assert.AreEqual(jsonAfter, swapper.ToString());
        }


        [TestMethod]
        public void TestSwapFail()
        {
            IJSONValueSwapper swapper = new JSONValueSwapper(JSON);
            try
            {
                swapper.Swap("NoExist", "value");
                Assert.Fail("Swapper should throw exception for undefined key");
            }
            catch (ArgumentException)
            {
            }

            try
            {
                swapper.Swap("NoExist\\Another\\Level", "value");
                Assert.Fail("Swapper should throw exception for undefined key");
            }
            catch (ArgumentException)
            {
            }

        }
    }
}
