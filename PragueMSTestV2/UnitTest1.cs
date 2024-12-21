using pragueParkingFilePathsV2;


namespace PragueMSTestV2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConfigPath_ShouldEndWithConfigJson()
        {
            // Act
            string configPath = JsonPaths.configPath;

            // Assert
            Assert.IsTrue(configPath.EndsWith("config.json"));
            Assert.IsTrue(File.Exists(configPath), "Config file should exist at the specified path");
        }

        [TestMethod]
        public void ParkingDataPath_ShouldEndWithParkingDataJson()
        {
            // Act
            string parkingDataPath = JsonPaths.parkingDataPath;

            // Assert
            Assert.IsTrue(parkingDataPath.EndsWith("parking_data.json"));
            Assert.IsTrue(File.Exists(parkingDataPath), "Parking data file should exist at the specified path");
        }
    }
}