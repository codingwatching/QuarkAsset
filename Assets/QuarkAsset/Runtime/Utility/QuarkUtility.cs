﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
namespace Quark
{
    public partial class QuarkUtility
    {
        #region Properties
        /// <summary>
        /// 标准的UTF-8是不含BOM的；
        /// 构造的UTF8Encoding，排除掉UTF8-BOM的影响；
        /// </summary>
        static UTF8Encoding utf8Encoding = new UTF8Encoding(false);
        [ThreadStatic]//每个静态类型字段对于每一个线程都是唯一的
        static StringBuilder stringBuilderCache = new StringBuilder(1024);
        static char[] stringConstant ={
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
            };
        #endregion

        #region Assembly
        public static T GetTypeInstance<T>(string typeName)
        {
            T inst = default;
            var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in domainAssemblies)
            {
                var dstType = a.GetType(typeName);
                if (dstType != null)
                {
                    inst = (T)Activator.CreateInstance(dstType);
                    break;
                }
            }
            return inst;
        }
        public static Type GetTypeFromAllAssemblies(string typeName)
        {
            Type dstType = null;
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var length = asms.Length;
            for (int i = 0; i < length; i++)
            {
                var asm = asms[i];
                dstType = asm.GetType(typeName);
                if (dstType != null)
                {
                    break;
                }
            }
            return dstType;
        }

        public static string[] GetDerivedTypeNames<T>()
    where T : class
        {
            return GetDerivedTypeNames(typeof(T), AppDomain.CurrentDomain.GetAssemblies());
        }
        /// <summary>
        /// 获取某类型在指定程序集的所有派生类完全限定名数组；
        /// </summary>
        /// <param name="type">基类</param>
        /// <param name="assemblies">查询的程序集集合</param>
        /// <returns>非抽象派生类完全限定名</returns>
        public static string[] GetDerivedTypeNames(Type type, params System.Reflection.Assembly[] assemblies)
        {
            List<string> types;
            if (assemblies == null)
                return type.Assembly.GetTypes().Where(t => { return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract; }).Select(t => t.FullName).ToArray();
            else
            {
                types = new List<string>();
                foreach (var a in assemblies)
                {
                    var dstTypes = a.GetTypes().Where(t => { return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract; }).Select(t => t.FullName);
                    types.AddRange(dstTypes);
                }
            }
            return types.ToArray();
        }
        #endregion

        #region String
        public static string Append(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("Append is invalid.");
            }
            stringBuilderCache.Clear();
            int length = args.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilderCache.Append(args[i]);
            }
            return stringBuilderCache.ToString();
        }
        /// <summary>
        /// 格式化AB名称；
        /// 此方法Quark专供；
        /// </summary>
        /// <param name="srcStr">原始名称</param>
        /// <param name="replaceContext">替换的内容</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatAssetBundleName(string srcStr, string replaceContext = "_")
        {
            return Replace(srcStr, new string[] { "\\", "/", ".", " " }, replaceContext).ToLower();
        }
        /// <summary>
        /// 多字符替换；
        /// </summary>
        /// <param name="context">需要修改的内容</param>
        /// <param name="oldContext">需要修改的内容</param>
        /// <param name="newContext">修改的新内容</param>
        /// <returns>修改后的内容</returns>
        public static string Replace(string context, string[] oldContext, string newContext)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("context is invalid.");
            if (oldContext == null)
                throw new ArgumentNullException("oldContext is invalid.");
            if (string.IsNullOrEmpty(newContext))
                throw new ArgumentNullException("newContext is invalid.");
            var length = oldContext.Length;
            for (int i = 0; i < length; i++)
            {
                context = context.Replace(oldContext[i], newContext);
            }
            return context;
        }
        /// <summary>
        /// 合并路径；
        /// web类型地址合并， 获得的路径将以 / 作为分割符；
        /// 返回结果示例：github.com/DonnYep/CosmosFramework
        /// </summary>
        /// <param name="paths">路径</param>
        /// <returns>合并的路径</returns>
        public static string WebPathCombine(params string[] paths)
        {
            var pathResult = Path.Combine(paths);
            pathResult = pathResult.Replace("\\", "/");
            return pathResult;
        }
        #endregion

        #region Encryption
        public static byte[] GenerateBytesAESKey(string srckey)
        {
            if (string.IsNullOrEmpty(srckey))
                return new byte[0];
            var srcKeyLen = Encoding.UTF8.GetBytes(srckey).Length;
            int dstLen = 16;
            switch (srcKeyLen)
            {
                case 0:
                    return new byte[0];
                    break;
                case 16:
                    dstLen = 16;
                    break;
                case 24:
                    dstLen = 24;
                    break;
                case 32:
                    dstLen = 32;
                    break;
                default:
                    throw new Exception("Key should be 16,24 or 32 bytes long");
                    break;
            }
            var srcBytes = Encoding.UTF8.GetBytes(srckey);
            byte[] dstBytes = new byte[dstLen];
            var srcLen = srcBytes.Length;
            if (srcLen > dstLen)
            {
                Array.Copy(srcBytes, 0, dstBytes, 0, dstLen);
            }
            else
            {
                var diffLen = dstLen - srcLen;
                var diffBytes = new byte[diffLen];
                Array.Copy(srcBytes, 0, dstBytes, 0, srcLen);
                Array.Copy(diffBytes, 0, dstBytes, srcLen, diffLen);
            }
            return dstBytes;
        }
        /// <summary>
        /// AES对称加密string类型内容;
        /// 密钥的byte长度必须是16, 24, 32；
        /// </summary>
        /// <param name="context">需要加密的内容</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的内容</returns>
        public static string AESEncryptStringToString(string context, byte[] key)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("context is invalid ! ");
            if (key == null)
                throw new ArgumentNullException("key is invalid ! ");
            using (var aes = new AesCryptoServiceProvider())
            {
                var iv = aes.IV;
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length);
                    using (var cryptStream = new CryptoStream(ms, aes.CreateEncryptor(key, aes.IV), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptStream))
                        {
                            writer.Write(context);
                        }
                    }
                    var buf = ms.ToArray();
                    return Convert.ToBase64String(buf, 0, buf.Length);
                }
            }
        }
        /// <summary>
        /// AES对称解密string类型内容；
        /// 密钥的byte长度必须是16, 24, 32；
        /// </summary>
        /// <param name="context">需要解密的内容</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后的内容</returns>
        public static string AESDecryptStringToString(string context, byte[] key)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("context is invalid ! ");
            if (key == null)
                throw new ArgumentNullException("key is invalid ! ");
            var bytes = Convert.FromBase64String(context);
            using (var aes = new AesCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    var iv = new byte[16];
                    ms.Read(iv, 0, 16);
                    using (var cryptStream = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
        #endregion

        #region Debug
        public static void LogInfo(object msg)
        {
            UnityEngine.Debug.Log($"<b><color=cyan>{"[QUARK-INFO]-->>"} </color></b>{msg}");
        }
        public static void LogWarning(object msg)
        {
            UnityEngine.Debug.LogWarning($"<b><color=orange>{"[QUARK-WARNING]-->>" }</color></b>{msg}");
        }
        public static void LogError(object msg)
        {
            UnityEngine.Debug.LogError($"<b><color=red>{"[QUARK-ERROR]-->>"} </color></b>{msg}");
        }
        #endregion

        #region Json
        /// <summary>
        /// 将对象序列化为JSON字段
        /// </summary>
        /// <param name="obj">需要被序列化的对象</param>
        /// <returns>序列化后的JSON字符串</returns>xxxx
        public static string ToJson(object obj)
        {
            return LitJson.JsonMapper.ToJson(obj);
        }
        /// <summary>
        /// 将JSON反序列化为泛型对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <returns>反序列化后的泛型对象</returns>
        public static T ToObject<T>(string json)
        {
            return LitJson.JsonMapper.ToObject<T>(json);
        }
        #endregion

        #region IO
        /// <summary>
        /// 读取指定路径下某text类型文件的内容
        /// </summary>
        /// <param name="fileFullPath">文件的完整路径，包含文件名与扩展名</param>
        /// <returns>指定文件的包含的内容</returns>
        public static string ReadTextFileContent(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
                throw new IOException("ReadTextFileContent path not exist !" + fileFullPath);
            string result = string.Empty;
            using (FileStream stream = File.Open(fileFullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream, utf8Encoding))
                {
                    result = Append(reader.ReadToEnd());
                }
            }
            return result;
        }
        /// <summary>
        /// 获取文件大小；
        /// 若文件存在，则返回正确的大小；若不存在，则返回-1；
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns>文件long类型的长度</returns>
        public static long GetFileSize(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                return 0;
            }
            else if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }
            return 0;
        }
        /// <summary>
        /// 完全覆写；
        ///  使用UTF8编码；
        /// </summary>
        /// <param name="fileFullPath">文件完整路径</param>
        /// <param name="context">写入的信息</param>
        public static void OverwriteTextFile(string fileFullPath, string context)
        {
            var folderPath = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            using (FileStream stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                {
                    writer.WriteLine(context);
                    writer.Flush();
                }
            }
        }
        public static void DeleteFile(string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }
        }
        /// <summary>
        /// 获取文件夹中的文件数量；
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns>文件数量</returns>
        public static int FolderFileCount(string folderPath)
        {
            int count = 0;
            var files = Directory.GetFiles(folderPath); //String数组类型
            count += files.Length;
            var dirs = Directory.GetDirectories(folderPath);
            foreach (var dir in dirs)
            {
                count += FolderFileCount(dir);
            }
            return count;
        }
        /// <summary>
        /// 完全覆写；
        ///  使用UTF8编码；
        /// </summary>
        /// <param name="filePath">w文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="context">写入的信息</param>
        public static void OverwriteTextFile(string filePath, string fileName, string context)
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            var fileFullPath = Path.Combine(filePath, fileName);
            using (FileStream stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                {
                    writer.WriteLine(context);
                    writer.Flush();
                }
            }
        }
        /// <summary>
        /// 遍历文件夹下的文件；
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="handler">遍历到一个文件时的处理的函数</param>
        /// <exception cref="IOException">
        /// Folder path is invalid
        /// </exception>
        public static void TraverseFolderFile(string folderPath, Action<FileSystemInfo> handler)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);
            FileSystemInfo[] fsInfoArr = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsInfo in fsInfoArr)
            {
                if (fsInfo is DirectoryInfo)     //判断是否为文件夹
                {
                    TraverseFolderFile(fsInfo.FullName, handler);//递归调用
                }
                else
                {
                    handler(fsInfo);
                }
            }
        }
        /// <summary>
        /// 拷贝文件夹的内容到另一个文件夹；
        /// </summary>
        /// <param name="sourceDirectory">原始地址</param>
        /// <param name="targetDirectory">目标地址</param>
        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            CopyDirectoryRecursively(diSource, diTarget);
        }
        /// <summary>
        /// 拷贝文件夹的内容到另一个文件夹；
        /// </summary>
        /// <param name="source">原始地址</param>
        /// <param name="target">目标地址</param>
        public static void CopyDirectoryRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            //复制所有文件到新地址
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            //递归拷贝所有子目录
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectoryRecursively(diSourceSubDir, nextTargetSubDir);
            }
        }
        /// <summary>
        /// 删除文件夹下的所有文件以及文件夹
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        public static void DeleteFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo directory = Directory.CreateDirectory(folderPath);
                FileInfo[] files = directory.GetFiles();
                foreach (var file in files)
                {
                    file.Delete();
                }
                DirectoryInfo[] folders = directory.GetDirectories();
                foreach (var folder in folders)
                {
                    DeleteFolder(folder.FullName);
                }
                directory.Delete();
            }
        }
        public static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        /// <summary>
        /// 清空文件夹
        /// </summary>
        public static void EmptyFolder(string folderPath)
        {
            DeleteFolder(folderPath);
            CreateFolder(folderPath);
        }
        /// <summary>
        /// 使用UTF8编码；
        /// 写入文件信息；
        /// 若文件为空，则自动创建；
        /// 此方法为text类型文件写入；
        /// </summary>
        /// <param name="fileFullPath">文件完整路径</param>
        /// <param name="context">写入的信息</param>
        /// <param name="append">是否追加</param>
        public static void WriteTextFile(string fileFullPath, string context, bool append = false)
        {
            var folderPath = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            using (FileStream stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (append)
                    stream.Position = stream.Length;
                using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                {
                    writer.WriteLine(context);
                    writer.Flush();
                }
            }
        }
        /// <summary>
        /// 追加并完全写入所有bytes;
        /// </summary>
        /// <param name="path">写入的地址</param>
        /// <param name="bytesArray">数组集合</param>
        public static void AppendAndWriteAllBytes(string path, params byte[][] bytesArray)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var bytesArrayLength = bytesArray.Length;
                int size = 0;
                for (int i = 0; i < bytesArrayLength; i++)
                {
                    stream.Write(bytesArray[i], 0, bytesArray[i].Length);
                    size += bytesArray[i].Length;
                }
                File.WriteAllBytes(path, stream.ToArray());
                stream.Close();
            }
        }
        /// <summary>
        /// 检测资源与场景是否同处于一个AB包中；
        /// </summary>
        /// <param name="bundlePath">包地址</param>
        /// <returns>是否处于同一个包</returns>
        public static bool CheckAssetsAndScenesInOneAssetBundle(string bundlePath)
        {
            if (File.Exists(bundlePath))//若是文件
                return false;
            var col = Directory.GetFiles(bundlePath, ".", SearchOption.AllDirectories).Select(path => Path.GetExtension(path));
            var exts = new HashSet<string>(col);
            exts.Remove(".meta");
            if (exts.Contains(".unity"))
            {
                exts.Remove(".unity");
                return exts.Count != 0;
            }
            return false;
        }
        #endregion

        #region Platform
        public static string PlatformPerfix
        {
            get
            {
                string perfix = string.Empty;
#if UNITY_IOS|| UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                perfix=@"file://";
#endif
                return perfix;
            }
        }
        #endregion

        #region Converter
        public static string FormatBytes(long bytes, int decimals = 2)
        {
            string[] suffix = { "Byte", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;
            if (bytes > 1024)
                for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
                    dblSByte = bytes / 1024.0;
            return $"{Math.Round(dblSByte, decimals)}{suffix[i]}";
        }
        #endregion

        #region Text
        /// <summary>
        /// 生成指定长度的随机字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns>生成的随机字符串</returns>
        public static string GenerateRandomString(int length)
        {
            stringBuilderCache.Clear();
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                stringBuilderCache.Append(stringConstant[rd.Next(62)]);
            }
            return stringBuilderCache.ToString();
        }
        #endregion
    }
}