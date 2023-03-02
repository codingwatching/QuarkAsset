﻿using Quark.Asset;

namespace Quark
{
    public class QuarkManifestSerializer
    {
        public static string Serialize(QuarkManifest manifest, string aesKey)
        {
            var manifestJson = QuarkUtility.ToJson(manifest);
            var aesKeyBytes = QuarkUtility.GenerateBytesAESKey(aesKey);
            var encryptedManifestJson = QuarkUtility.AESEncryptStringToString(manifestJson, aesKeyBytes);
            return encryptedManifestJson;
        }
        /// <summary>
        /// 使用配置的aesKey反序列化manifest；
        /// </summary>
        /// <param name="manifestContext">读取到的文本内容</param>
        /// <returns>反序列化后的内容</returns>
        public static QuarkManifest Deserialize(string manifestContext)
        {
            return Deserialize(manifestContext, QuarkDataProxy.QuarkEncrytionData.QuarkAesEncryptionKey);
        }
        /// <summary>
        /// 使用自定义的aesKey反序列化manifest；
        /// </summary>
        /// <param name="manifestContext">读取到的文本内容</param>
        /// <param name="aesKey">对称加密密钥</param>
        /// <returns>反序列化后的内容</returns>
        public static QuarkManifest Deserialize(string manifestContext, string aesKey)
        {
            QuarkManifest quarkAssetManifest = null;
            try
            {
                var aesKeyBytes = QuarkUtility.GenerateBytesAESKey(aesKey);
                var encrypted = aesKeyBytes.Length > 0 ? true : false;
                var unencryptedManifest = manifestContext;
                if (encrypted)
                {
                    unencryptedManifest = QuarkUtility.AESDecryptStringToString(manifestContext, aesKeyBytes);
                }
                quarkAssetManifest = QuarkUtility.ToObject<QuarkManifest>(unencryptedManifest);
            }
            catch { }
            return quarkAssetManifest;
        }
    }
}