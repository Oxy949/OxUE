using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace OxUE
{
    /// <summary>
    /// Useful functions
    /// </summary>
    public static class Helpers
    {
        public static Texture2D FlipTexture(Texture2D original)
        {
            if (original != null)
            {
                Texture2D flipped = new Texture2D(original.width, original.height);

                int xN = original.width;
                int yN = original.height;


                for (int i = 0; i < xN; i++)
                {
                    for (int j = 0; j < yN; j++)
                    {
                        flipped.SetPixel(i, yN - j - 1, original.GetPixel(i, j));
                    }
                }

                flipped.Apply();

                return flipped;
            }
            else
            {
                return new Texture2D(2, 2);
            }
        }

        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (!task.IsFaulted) yield break;
            if (task.Exception != null) throw task.Exception;
        }

        public static string GetProjectDirectory(string url, bool createMissing = false)
        {
            url = Application.dataPath + "/../" + url;
            if (createMissing)
                CheckDirectory(url);
            //Debug.Log(url);
            return url;
        }

        public static void CheckDirectory(string dirName)
        {
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
        }

        public static string GetGameVersion()
        {
            return Application.version;
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static string ColorToHex(Color32 color)
        {
            string hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
            return hex;
        }

        public static float DbToNormal(float bd)
        {
            return bd <= -80 ? 0 : (float)Math.Pow(Math.E, (bd / 20.0d));
        }

        public static float NormalToDb(float normal)
        {
            return normal > 0 ? (Mathf.Log(normal) * 20) : -80;
        }

        public static string RemoveHTMLTagsFromText(string inputString)
        {
            string HTML_TAG_PATTERN = "<.*?>";
            return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }
        
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        
        public static object CreateInstance(string strFullyQualifiedName, object[] paramArray = null)
        {
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
            {
                if (paramArray != null && paramArray.Length > 0)
                    return Activator.CreateInstance(type, args: paramArray);
                return Activator.CreateInstance(type);
            }

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                {
                    if (paramArray != null && paramArray.Length > 0)
                        return Activator.CreateInstance(type, args: paramArray);
                    return Activator.CreateInstance(type);
                }
            }
            return null;
        }

        public static Color ColorGradient(Color from, Color to, float progress)
        {
            Gradient g;
            GradientColorKey[] gck;
            GradientAlphaKey[] gak;
            g = new Gradient();
            gck = new GradientColorKey[2];
            gck[0].color = to;
            gck[0].time = 0.0F;
            gck[1].color = from;
            gck[1].time = 1.0F;
            gak = new GradientAlphaKey[2];
            gak[0].alpha = 1.0F;
            gak[0].time = 0.0F;
            gak[1].alpha = 1.0F;
            gak[1].time = 1.0F;
            g.SetKeys(gck, gak);

            return g.Evaluate(progress);
        }
    }
}