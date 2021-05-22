using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToConfigMap
{
    public static class ConfigMapUtility
    {
        public static void ConvertToCM(string JsonFile, string YamlFile, string CMName, bool Tokenize)
        {

            fillCM(sb, CMName);
            using (StreamReader sr = new StreamReader(JsonFile))
            {
                string json = sr.ReadToEnd();
                Json4(json, 0,Tokenize);
                using (StreamWriter sw = new StreamWriter(YamlFile))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private static void fillCM(StringBuilder sb, string cmName)
        {
            sb.AppendLine("apiVersion: v1");
            sb.AppendLine("kind: ConfigMap");
            sb.AppendLine("metadata:");
            sb.AppendLine($"  name: {cmName}");
            sb.AppendLine("  labels:");
            sb.AppendLine($"    name: {cmName}");
            sb.AppendLine("data:");
        }

        static StringBuilder sb = new StringBuilder();
        static List<string> rootlevels = new List<string>();
        static bool Json4(string json, int level, bool tokenize)
        {
            if (json == "") return false;
            Dictionary<string, object> jsonDoc;
            try
            {
                jsonDoc = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
            catch (Exception)
            {
                if (tokenize)
                {
                    sb.AppendLine($"   \"{rootlevels[level - 1]}\": \"$${rootlevels[level - 1]}$$\"");
                }
                else
                {
                    sb.AppendLine($"   \"{rootlevels[level - 1]}\": \"{json}\"");
                }

                rootlevels.RemoveAt(level - 1);
                return false;
            }


            foreach (string strKey in jsonDoc.Keys)
            {
                if (level == 0)
                {
                    rootlevels.Clear();
                    rootlevels.Add(strKey);
                }
                else
                {
                    rootlevels.Add(rootlevels[level - 1] + "__" + strKey);
                }
                Json4(jsonDoc[strKey].ToString(), level + 1,tokenize);
            }
            return true;
        }
    }
}
