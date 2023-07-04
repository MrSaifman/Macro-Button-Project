using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Windows_App_WinUI3.FileHandlers
{
    public class JsonManager
    {
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
                        "}," +
                        "'AppSettings': {" +
                            "'FocusedAppOnly': true" +
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
                 // Handle the 'AppSettings' category.
                if (category == "AppSettings")
                {
                    if (settings.ContainsKey("AppSettings"))
                    {
                        var appSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(settings["AppSettings"].ToString());

                        if (appSettings.ContainsKey(setting))
                        {
                            appSettings[setting] = value;
                            settings["AppSettings"] = appSettings;
                        }
                    }

                    settingsDict["Settings"] = settings;
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
                
                if (category == "AppSettings" && settings.ContainsKey("AppSettings"))
                {
                    var appSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(settings["AppSettings"].ToString());

                    if (appSettings.ContainsKey(setting))
                    {
                        return appSettings[setting].ToString();
                    }
                }
            }

            return null;
        }

        public List<Application> ReadApplicationList(string fileName)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, "Data");
            string appsFilePath = Path.Combine(dataDirectory, fileName);

            string jsonString = File.ReadAllText(appsFilePath);

            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<Application>>(jsonString);
        }
    }
    public class Icon
    {
        public bool AutoPlay { get; set; }
        public int CreateOptions { get; set; }
        public int DecodePixelHeight { get; set; }
        public int DecodePixelType { get; set; }
        public int DecodePixelWidth { get; set; }
        public bool IsAnimatedBitmap { get; set; }
        public bool IsPlaying { get; set; }
        public string UriSource { get; set; }
        public int PixelHeight { get; set; }
        public int PixelWidth { get; set; }
        public DispatcherQueue DispatcherQueue { get; set; }
    }

    public class DispatcherQueue
    {
        public bool HasThreadAccess { get; set; }
    }

    public class Application
    {
        public string Name { get; set; }
        public Icon Icon { get; set; }
        public string Path { get; set; }
    }

    public class LightingMode
    {
        [JsonPropertyName("LightUpPattern")]
        public string LightUpPattern { get; set; }

        [JsonPropertyName("Brightness")]
        public string Brightness { get; set; }

        [JsonPropertyName("PatternSpeed")]
        public string PatternSpeed { get; set; }

        [JsonPropertyName("FrameColor1")]
        public string FrameColor1 { get; set; }

        [JsonPropertyName("FrameColor2")]
        public string FrameColor2 { get; set; }

        [JsonPropertyName("ButtonColor1")]
        public string ButtonColor1 { get; set; }

        [JsonPropertyName("ButtonColor2")]
        public string ButtonColor2 { get; set; }
    }

    public class LightingSettings
    {
        [JsonPropertyName("IdleLighting")]
        public LightingMode IdleLighting { get; set; }

        [JsonPropertyName("ButtonPressLighting")]
        public LightingMode ButtonPressLighting { get; set; }

        [JsonPropertyName("LidLiftLighting")]
        public LightingMode LidLiftLighting { get; set; }
    }

    public class AppSettings
    {
        [JsonPropertyName("FocusedAppOnly")]
        public string FocusedAppOnly { get; set; }
    }

    public class Settings
    {
        [JsonPropertyName("LightingSettings")]
        public LightingSettings LightingSettings { get; set; }

        [JsonPropertyName("AppSettings")]
        public AppSettings AppSettings { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("Settings")]
        public Settings Settings { get; set; }
    }

}
