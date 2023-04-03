﻿namespace Quark.Editor
{
    public class QuarkEditorConstant
    {
        /// <summary>
        /// Quark可识别的文件后缀名；
        /// 大小写不敏感；
        /// </summary>
        public static string[] Extensions { get { return extensions; } }
        readonly static string[] extensions = new string[]
        {
            ".3ds",".bmp",".blend",".eps",".exif",".gif",".icns",".ico",".jpeg",
            ".jpg",".ma",".max",".mb",".pcx",".png",".psd",".svg",".controller",
            ".wav",".txt",".prefab",".xml",".shadervariants",".shader",".anim",
            ".unity",".mat",".mask",".overrideController",".tif",".spriteatlas",
            ".mp3",".ogg",".aiff",".tga",".dds",".bytes",".json",".asset",".mp4",
            ".xls",".xlsx",".docx",".doc",".mov",".renderTexture",".csv",".fbx",".mixer",
            ".flare",".playable",".physicMaterial",".signal",".guiskin",".dds",".otf"
        };
        public const int DetailIconPreviewSize = 34;
    }
}
