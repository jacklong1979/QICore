using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace QICore.NIdentityServer4.Common
{
    public class RSAUtils
    {
        /// <summary>
        /// 从本地文件中读取用来签发 Token 的 RSA Key
        /// </summary>
        /// <param name="directoryPath">存放密钥的文件夹路径</param>
        /// <param name="withPrivate">用【私有密钥】，如果是空的就用【公有密钥】</param>
        /// <param name="keyParameters"></param>
        /// <returns></returns>
        public static bool TryGetKeyParameters(string directoryPath, bool withPrivate, out RSAParameters keyParameters)
        {
            string filename = withPrivate ? "key.private.rsa" : "key.public.rsa";
            keyParameters = default(RSAParameters);
            if (Directory.Exists(directoryPath) == false)
            {
                return false;
            }
            keyParameters = JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(Path.Combine(directoryPath, filename)));
            return true;
        }

        /// <summary>
        /// 生成并保存 RSA 公钥与私钥
        /// </summary>
        /// <param name="directoryPath">存放密钥的文件夹路径</param>
        /// <returns></returns>
        public static RSAParameters GenerateAndSaveKey(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            RSAParameters publicKeys, privateKeys;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    privateKeys = rsa.ExportParameters(true);
                    publicKeys = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            File.WriteAllText(Path.Combine(directoryPath, "key.private.rsa"), JsonConvert.SerializeObject(privateKeys));
            File.WriteAllText(Path.Combine(directoryPath, "key.public.rsa"), JsonConvert.SerializeObject(publicKeys));
            return privateKeys;
        }
    }
}

