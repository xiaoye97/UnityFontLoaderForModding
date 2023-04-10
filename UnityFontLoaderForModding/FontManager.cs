using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace xiaoye97
{
    /// <summary>
    /// 字体管理器
    /// </summary>
    public class FontManager
    {
        public List<FontData> Fonts;
        public Dictionary<string, FontData> FontDict;
        public bool SearchSystemFont;

        public string CustomFontDirPath;

        public void Init()
        {
            Debug.Log("[UnityFontLoader]FontManager开始初始化");
            Fonts = new List<FontData>();
            FontDict = new Dictionary<string, FontData>();
            if (SearchSystemFont)
            {
                LoadSystemFonts();
            }
            if (Directory.Exists(CustomFontDirPath))
            {
                LoadFonts(CustomFontDirPath);
            }
            Debug.Log("[UnityFontLoader]FontManager初始化完毕");
        }

        public void LoadFonts(string dirPath)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            // 查找文件夹下的所有字体
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            int count = 0;
            if (dir.Exists)
            {
                List<string> paths = new List<string>();
                paths.AddRange(dir.GetFiles("*.ttf").Select(f => f.FullName));
                paths.AddRange(dir.GetFiles("*.otf").Select(f => f.FullName));

                foreach (var path in paths)
                {
                    try
                    {
                        FontData font = new FontData(path);
                        Fonts.Add(font);
                        FontDict[font.FontFileName] = font;
                        Debug.Log($"[UnityFontLoader]加载了字体:{font.FontFileName}，路径:{path}");
                        count++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[UnityFontLoader]加载字体文件 {path} 时出现异常，忽略此文件:\n{e}");
                    }
                }
            }
            else
            {
                Debug.Log($"[UnityFontLoader]目标字体文件夹不存在 {dir}");
            }
            sw.Stop();
            Debug.Log($"从[{dir}]加载了{count}个字体, 耗时{sw.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// 加载系统字体
        /// </summary>
        public void LoadSystemFonts()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var paths = Font.GetPathsToOSFonts();
            int count = 0;
            foreach (var path in paths)
            {
                try
                {
                    FontData font = new FontData(path);
                    Fonts.Add(font);
                    FontDict[font.FontFileName] = font;
                    Debug.Log($"[UnityFontLoader]加载了字体:{font.FontFileName}，路径:{path}");
                    count++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[UnityFontLoader]加载字体文件 {path} 时出现异常，忽略此文件:\n{e}");
                }
            }

            sw.Stop();
            Debug.Log($"[UnityFontLoader]从系统目录加载了{count}个字体, 耗时{sw.ElapsedMilliseconds}ms");
        }
    }
}