using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Common
{
    public static class DataPersistence
    {
        private static readonly string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + $"\\{Assembly.GetEntryAssembly().GetName().Name}\\settings_data.json";

        public static void SaveData<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            //encrypt the json string with aes
            //json = EncryptionHelper.Encrypt(json);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // 确保目录存在
            File.WriteAllText(filePath, json);
        }

        public static T? LoadData<T>()
        {
            if (!File.Exists(filePath))
                return default;

            var json = File.ReadAllText(filePath);
            //decrypt the json string with aes
            //json = EncryptionHelper.Decrypt(json);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
