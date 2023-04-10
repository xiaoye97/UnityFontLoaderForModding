using TMPro;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace xiaoye97
{
    [BepInPlugin("UnityFontLoader", "UnityFontLoader", "1.0.0")]
    public class UnityFontLoader : BaseUnityPlugin
    {
        public FontManager FontManager;

        public static ConfigEntry<bool> SearchSystemFont;
        public static ConfigEntry<bool> PatchTextFont;
        public static ConfigEntry<bool> PatchTextMeshProFont;
        public static ConfigEntry<string> MainFontConfig;
        public static ConfigEntry<string> FallbackFontsConfig;
        public static string[] FallbackFontNames;

        public static FontData MainFont;

        private void Awake()
        {
            SearchSystemFont = Config.Bind<bool>("config", "SearchSystemFont", true, "是否搜索系统字体");
            PatchTextFont = Config.Bind<bool>("config", "PatchTextFont", false, "是否Patch UnityEngine.UI.Text");
            PatchTextMeshProFont = Config.Bind<bool>("config", "PatchTextMeshProFont", true, "是否Patch TextMeshPro");
            MainFontConfig = Config.Bind<string>("config", "MainFont", "msyh.ttc", "主字体，如果没有主字体，则使用第一个后备字体作为主字体");
            FallbackFontsConfig = Config.Bind<string>("config", "FallbackFonts", "arial.ttf", "后备字体列表，使用;分隔，后备字体仅对TextMeshPro生效");
            FallbackFontNames = FallbackFontsConfig.Value.Split(';');
            FontManager = new FontManager();
            FontManager.SearchSystemFont = SearchSystemFont.Value;
            FontManager.CustomFontDirPath = $"{Paths.PluginPath}/Fonts";
            FontManager.Init();
            SetFont();
            if (MainFont != null)
            {
                if (PatchTextFont.Value)
                {
                    Harmony.CreateAndPatchAll(typeof(TextFontPatchs));
                }
                if (PatchTextMeshProFont.Value)
                {
                    Harmony.CreateAndPatchAll(typeof(TMPFontPatchs));
                }
            }
            else
            {
                Debug.LogError("[UnityFontLoader]初始化完毕，主字体为空，将不会进行字体替换");
            }
        }

        public void SetFont()
        {
            if (FontManager.FontDict.ContainsKey(MainFontConfig.Value))
            {
                MainFont = FontManager.FontDict[MainFontConfig.Value];
                Debug.Log($"[UnityFontLoader]将{MainFontConfig.Value}作为主字体");
            }
            else
            {
                Debug.Log($"[UnityFontLoader]找不到后主字体 {MainFontConfig.Value}，忽略");
            }
            if (FallbackFontNames.Length > 0)
            {
                foreach (string fontName in FallbackFontNames)
                {
                    if (FontManager.FontDict.ContainsKey(fontName))
                    {
                        if (MainFont == null)
                        {
                            MainFont = FontManager.FontDict[fontName];
                            Debug.Log($"[UnityFontLoader]将后备字体{fontName}作为主字体");
                        }
                        else
                        {
                            if (MainFont.TMPFont.fallbackFontAssetTable == null)
                            {
                                MainFont.TMPFont.fallbackFontAssetTable = new List<TMP_FontAsset>();
                            }
                            MainFont.TMPFont.fallbackFontAssetTable.Add(FontManager.FontDict[fontName].TMPFont);
                            Debug.Log($"[UnityFontLoader]将后备字体{fontName}添加到后备字体列表");
                        }
                    }
                    else
                    {
                        Debug.Log($"[UnityFontLoader]找不到后备字体 {fontName}，忽略");
                    }
                }
            }
            if (MainFont == null)
            {
                if (FontManager.Fonts.Count > 0)
                {
                    MainFont = FontManager.Fonts[0];
                    Debug.Log($"[UnityFontLoader]由于既没有主字体，也没有后备字体，所以将扫描到的第一个字体[{FontManager.Fonts[0].FontFileName}]作为主字体，其他字体作为后备字体");
                    for (int i = 1; i < FontManager.Fonts.Count; i++)
                    {
                        if (MainFont.TMPFont.fallbackFontAssetTable == null)
                        {
                            MainFont.TMPFont.fallbackFontAssetTable = new List<TMP_FontAsset>();
                        }
                        MainFont.TMPFont.fallbackFontAssetTable.Add(FontManager.Fonts[i].TMPFont);
                        Debug.Log($"[UnityFontLoader]将字体{FontManager.Fonts[i].TMPFont}添加到后备字体列表");
                    }
                }
            }
        }
    }
}