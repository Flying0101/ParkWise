namespace pragueParkingFilePathsV2
{
    public class JsonPaths
    {
        private static string GetSolutionDirectory()
        {
            // Start from the current directory
            string currentDirectory = Directory.GetCurrentDirectory();
            // Navigate up to find the 'prague-v2' solution directory
            while (currentDirectory != null && !currentDirectory.EndsWith("prague-v2"))
            {
                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            }
            return currentDirectory ?? throw new DirectoryNotFoundException("Could not find solution directory");
        }

        private static string GetJsonFilesPath()
        {
            // Navigate to the pragueParkingFilePathsV2 project where JSON files are stored
            return Path.Combine(GetSolutionDirectory(), "pragueParkingFilePathsV2");
        }

        // Public properties for the JSON file paths
        public static string parkingDataPath => Path.Combine(GetJsonFilesPath(), "parking_data.json");
        public static string configPath => Path.Combine(GetJsonFilesPath(), "config.json");


        //keep, ensure if file exists, optimize, use later.
        public static bool EnsureFileExists(string fullPath)
        {

            if (!File.Exists(fullPath))
            {
                return false;
            }
            return true;
        }


        //below methods runs file read and write for JsonDataAccess for current project.
        public static string LoadParkData()
        {
            return File.ReadAllText(parkingDataPath);
        }

        public static void SaveParkData(string jsonString)
        {
            File.WriteAllText(parkingDataPath, jsonString);
        }

        public static string LoadConfigData()
        {
            return File.ReadAllText(configPath);
        }

        public static void SaveConfig(string json)
        {
            File.WriteAllText(configPath, json);
        }
    }
}
