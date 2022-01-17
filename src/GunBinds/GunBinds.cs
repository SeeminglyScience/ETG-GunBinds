using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BindingAPI;
using MonoMod.RuntimeDetour;
using TranslationAPI;
using UnityEngine;
using UnityEngine.UI;

namespace GunBinds;

public class GunBinds : ETGModule
{
    public static readonly string MOD_NAME = "GunBinds";

    public static readonly string VERSION = "1.0.0";

    public static readonly string TEXT_COLOR = "#00FFFF";

    public static Text Text;

    public static Text GunName;

    public override void Start()
    {
        global::GunBinds.Log.Init();
        InitTranslations();
        BindingBuilder.Init("GunBinds");

        ETGModMainBehaviour.Instance.gameObject.AddComponent<GunBindsBehaviours>();
        GUI.Init();
        Text = GUI.CreateText(null, new Vector2(-300f, 150), "", TextAnchor.LowerRight, 20);
        GunName = GUI.CreateText(null, new Vector2(-300f, 100), "", TextAnchor.LowerRight, 20);
    }

    public static void Log(string text, string color="FFFFFF")
    {
        ETGModConsole.Log($"<color={color}>{text}</color>");
    }

    public override void Exit() { }
    public override void Init() { }

    private static void InitTranslations()
    {
        TranslationManager.Init();
        string translationsDirectory = Path.Combine(
            ETGMod.ResourcesDirectory,
            Path.Combine("GunBinds", "translations"));

        string englishFile = Path.Combine(translationsDirectory, "english.json");

        Directory.CreateDirectory(translationsDirectory);
        if (!File.Exists(englishFile))
        {
            string contents = JsonUtility.ToJson(GunBindsTranslations.Default);

            File.WriteAllText(englishFile, contents);
        }

        foreach (string file in Directory.GetFiles(translationsDirectory))
        {
            if (Path.GetExtension(file) is not ".json")
            {
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();
            StringTableManager.GungeonSupportedLanguages language;
            try
            {
                language = (StringTableManager.GungeonSupportedLanguages)Enum.Parse(
                    typeof(StringTableManager.GungeonSupportedLanguages),
                    fileName);
            }
            catch
            {
                ETGModConsole.Log($"File '{file}' does not match a gungeon supported language.");
                continue;
            }

            static void AddOrDefault(
                StringTableManager.GungeonSupportedLanguages language,
                string name,
                string value,
                string defaultValue,
                StringTableType type = StringTableType.Core)
            {
                if (value is null or { Length: 0 })
                {
                    value = defaultValue;
                }

                TranslationManager.AddTranslation(language, name, value, type);
            }

            string jsonContent = File.ReadAllText(file);
            GunBindsTranslations translation;
            try
            {
                translation = JsonUtility.FromJson<GunBindsTranslations>(jsonContent);
            }
            catch (Exception e)
            {
                ETGModConsole.Log($"Translation file for '{language}' was not formed properly. Message: {e.Message}");
                continue;
            }

            ref readonly GunBindsTranslations backup = ref GunBindsTranslations.Default;
            AddOrDefault(language, "#GUNBINDS_FAVORITE", translation.favorite, backup.favorite);
            AddOrDefault(language, "#GUNBINDS_FAVORITE1", translation.favorite1, backup.favorite1);
            AddOrDefault(language, "#GUNBINDS_FAVORITE2", translation.favorite2, backup.favorite2);
            AddOrDefault(language, "#GUNBINDS_FAVORITE3", translation.favorite3, backup.favorite3);
            AddOrDefault(language, "#GUNBINDS_FAVORITE4", translation.favorite4, backup.favorite4);
            AddOrDefault(language, "#GUNBINDS_FAVORITE5", translation.favorite5, backup.favorite5);
            AddOrDefault(language, "#GUNBINDS_FAVORITE6", translation.favorite6, backup.favorite6);
            AddOrDefault(language, "#GUNBINDS_FAVORITE7", translation.favorite7, backup.favorite7);
            AddOrDefault(language, "#GUNBINDS_FAVORITE8", translation.favorite8, backup.favorite8);
            AddOrDefault(language, "#GUNBINDS_FAVORITE9", translation.favorite9, backup.favorite9);
            AddOrDefault(language, "#GUNBINDS_PRIMARY_TOGGLE", translation.primaryToggle, backup.primaryToggle);
            AddOrDefault(language, "#GUNBINDS_PRIMARY", translation.primary, backup.primary);
            AddOrDefault(language, "#GUNBINDS_SECONDARY", translation.secondary, backup.secondary);
            AddOrDefault(language, "#GUNBINDS_STARTER", translation.starter, backup.starter);
            AddOrDefault(language, "#GUNBINDS_SET", translation.set, backup.set);
        }

        foreach (dfLanguageManager languageManager in GameObject.FindObjectsOfType<dfLanguageManager>())
        {
            AddAllLocalizationKeys(languageManager);
        }

        const BindingFlags flags = BindingFlags.NonPublic
            | BindingFlags.Public
            | BindingFlags.Instance
            | BindingFlags.Static;

        new Hook(
            typeof(dfLanguageManager).GetMethod("parseDataFile", flags),
            typeof(GunBinds).GetMethod(nameof(GunBinds.ParseDataFileHook), flags));
    }

    private static void AddAllLocalizationKeys(dfLanguageManager languageManager)
    {
        Dictionary<string, string> strings = languageManager.GetStrings();
        foreach (string key in Strings.AllKeys)
        {
            strings.Add(key, StringTableManager.GetString(key));
        }
    }

    private static void ParseDataFileHook(Action<dfLanguageManager> original, dfLanguageManager self)
    {
        original(self);
        AddAllLocalizationKeys(self);
    }
}
