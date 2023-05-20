using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Windows_App_WinUI3.FileHandlers
{
    public class JsonManager
    {
        // Variable to keep track of current lighting mode
        public string currentLightingMode;
        public string LightUpPattern;
        public string Brightness;
        public string FrameColor1;
        public string FrameColor2;
        public string ButtonColor1;
        public string ButtonColor2;

        // Create a dictionary to map button content to lighting modes
        public Dictionary<string, string> lightingModeMapping = new Dictionary<string, string>
        {
            {"Idle Lighting", "IdleLighting"},
            {"Button Press Lighting", "ButtonPressLighting"},
            {"Lid Lift Lighting", "LidLiftLighting"},
        };

        public void EnsureDefaultSettingsExist()
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, "Data");
            string settingsFilePath = Path.Combine(dataDirectory, "settings.json");

            // Ensure the Data directory exists
            Directory.CreateDirectory(dataDirectory);

            // Check if the settings file exists
            if (!File.Exists(settingsFilePath))
            {
                // Define your default settings here
                string defaultSettingsJson =
                "{" +
                  "'Settings': {" +
                    "'LightingSettings': {" +
                      "'IdleLighting': {" +
                        "'LightUpPattern': 'None'," +
                        "'Brightness': 50," +
                        "'PatternSpeed': 50," +
                        "'FrameColor1': '#FFFFFFFF'," +
                        "'FrameColor2': '#FFFFFFFF'," +
                        "'ButtonColor1': '#FFFFFFFF'," +
                        "'ButtonColor2': '#FFFFFFFF'" +
                      "}," +
                      "'ButtonPressLighting': {" +
                        "'LightUpPattern': 'None'," +
                        "'Brightness': 50," +
                        "'PatternSpeed': 50," +
                        "'FrameColor1': '#FFFFFFFF'," +
                        "'FrameColor2': '#FFFFFFFF'," +
                        "'ButtonColor1': '#FFFFFFFF'," +
                        "'ButtonColor2': '#FFFFFFFF'" +
                      "}," +
                      "'LidLiftLighting': {" +
                        "'LightUpPattern': 'None'," +
                        "'Brightness': 50," +
                        "'PatternSpeed': 50," +
                        "'FrameColor1': '#FFFFFFFF'," +
                        "'FrameColor2': '#FFFFFFFF'," +
                        "'ButtonColor1': '#FFFFFFFF'," +
                        "'ButtonColor2': '#FFFFFFFF'" +
                      "}" +
                    "}" +
                  "}" +
                "}";

                // Write the default settings to the file
                File.WriteAllText(settingsFilePath, defaultSettingsJson);
            }
        }

        public void UpdateSetting(string category, string setting, string value)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(setting) || string.IsNullOrEmpty(value))
            {
                return;
            }

            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, "Data");
            string settingsFilePath = Path.Combine(dataDirectory, "settings.json");

            string jsonString = File.ReadAllText(settingsFilePath);

            if (string.IsNullOrEmpty(jsonString))
            {
                return;
            }

            // Parse JSON into a dictionary structure.
            jsonString = jsonString.Replace('\'', '\"');
            var settingsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

            // Access and modify the setting.
            if (settingsDict.ContainsKey("Settings"))
            {
                var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(settingsDict["Settings"].ToString());

                if (settings.ContainsKey("LightingSettings"))
                {
                    var lightingSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(settings["LightingSettings"].ToString());

                    if (lightingSettings.ContainsKey(category))
                    {
                        var categorySettings = JsonSerializer.Deserialize<Dictionary<string, object>>(lightingSettings[category].ToString());

                        if (categorySettings.ContainsKey(setting))
                        {
                            categorySettings[setting] = value;
                        }

                        // Put the updated dictionary back to its parent.
                        lightingSettings[category] = categorySettings;
                    }

                    settings["LightingSettings"] = lightingSettings;
                }

                settingsDict["Settings"] = settings;
            }

            // Convert the dictionary back to JSON.
            string updatedJsonString = JsonSerializer.Serialize(settingsDict);

            // Save the updated JSON.
            File.WriteAllText(settingsFilePath, updatedJsonString);
        }

        public string ReadSetting(string category, string setting)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(setting))
            {
                return null;
            }

            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, "Data");
            string settingsFilePath = Path.Combine(dataDirectory, "settings.json");

            string jsonString = File.ReadAllText(settingsFilePath);

            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            // Parse JSON into a dictionary structure.
            jsonString = jsonString.Replace('\'', '\"');
            var settingsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

            // Access the setting.
            if (settingsDict.ContainsKey("Settings"))
            {
                var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(settingsDict["Settings"].ToString());

                if (settings.ContainsKey("LightingSettings"))
                {
                    var lightingSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(settings["LightingSettings"].ToString());

                    if (lightingSettings.ContainsKey(category))
                    {
                        var categorySettings = JsonSerializer.Deserialize<Dictionary<string, object>>(lightingSettings[category].ToString());

                        if (categorySettings.ContainsKey(setting))
                        {
                            return categorySettings[setting].ToString();
                        }
                    }
                }
            }

            return null;
        }


    }
}
