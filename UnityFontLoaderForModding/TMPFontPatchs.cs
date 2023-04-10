using TMPro;
using HarmonyLib;
using UnityEngine;

namespace xiaoye97
{
    public class TMPFontPatchs
    {
        /// <summary>
        /// 修改TMP字体
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(TextMeshProUGUI), "OnEnable")]
        public static void TMPFontPatch(TextMeshProUGUI __instance)
        {
            if (UnityFontLoader.MainFont.TMPFont == null) return;
            if (__instance.font.name != UnityFontLoader.MainFont.TMPFont.name)
            {
                __instance.font = UnityFontLoader.MainFont.TMPFont;
            }
        }

        /// <summary>
        /// 修改TMP字体
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(TextMeshPro), "OnEnable")]
        public static void TMPFontPatch2(TextMeshPro __instance)
        {
            if (UnityFontLoader.MainFont.TMPFont == null) return;
            if (__instance.font.name != UnityFontLoader.MainFont.TMPFont.name)
            {
                __instance.font = UnityFontLoader.MainFont.TMPFont;
            }
        }

        /// <summary>
        /// 如果有不显示的文本，则设置显示方式为溢出
        /// </summary>
        [HarmonyPostfix, HarmonyPatch(typeof(TextMeshProUGUI), "InternalUpdate")]
        public static void TMPFontPatch3(TextMeshProUGUI __instance)
        {
            if (UnityFontLoader.MainFont.TMPFont == null) return;
            if (__instance.font == UnityFontLoader.MainFont.TMPFont)
            {
                if (__instance.overflowMode != TextOverflowModes.Overflow)
                {
                    if (__instance.preferredWidth > 1 && __instance.bounds.extents == Vector3.zero)
                    {
                        __instance.overflowMode = TextOverflowModes.Overflow;
                    }
                }
            }
        }
    }
}