using System;
using System.IO;
using UnityEngine;

namespace OxUE
{
    public class Security: Singleton<Security>
    {
        [SerializeField] private string securityKey;
        
        public static string Encrypt(string originalString)
        {
            if (String.IsNullOrEmpty(originalString))
            {
                return "";
            }

            return Crypto.EncryptStringAES(originalString, GetSecurityKey()); ;
        }

        public static string Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                return "";
            }
            return Crypto.DecryptStringAES(cryptedString, GetSecurityKey()); ;
        }

        public static byte[] Decrypt(byte[] data)
        {
            return Crypto.DecryptBytesAES(data, GetSecurityKey()); ;
        }

        public static byte[] Encrypt(byte[] data)
        {
            return Crypto.EncryptBytesAES(data, GetSecurityKey()); ;
        }

        public static string GetSecurityKey()
        {
#if UNITY_EDITOR
            return File.ReadAllText(Helpers.GetProjectDirectory("") + "key.txt");
#else
            return Instance.securityKey;
#endif
        }   
    }
}