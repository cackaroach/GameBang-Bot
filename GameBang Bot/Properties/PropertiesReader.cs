using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GameBang_Bot.Properties {
	class PropertiesReader {
		// Pattern Match : Int32, Int64, String
		public static T ReadAsObject<T>(string filename) where T : IProperties, new() {
			T obj = new T();

			var pros = obj.GetType().GetProperties();
			var dic = ReadProperties(filename);

			foreach (var p in pros) {
				var attr = p.GetCustomAttribute(typeof(KeyNameAttribute)) as KeyNameAttribute;
				if (dic.TryGetValue((attr == null) ? p.Name : attr.Name, out string value)) {
					if (p.PropertyType.Equals(typeof(Int32)))
						p.SetValue(obj, Int32.Parse(value));
					else if (p.PropertyType.Equals(typeof(UInt64)))
						p.SetValue(obj, UInt64.Parse(value));
					else
						p.SetValue(obj, value);
				} else
					throw new Exception($"{filename} Properties is not found: {p.Name}");
			}

			return obj;
		}

		private static Dictionary<string, string> ReadProperties(string filename) {
			var line = string.Empty;
			var dic = new Dictionary<string, string>();

			using (var reader = new StreamReader(filename)) {
				while ((line = reader.ReadLine()) != null) {
					if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
						continue;

					var splits = line.Split('=');
					var content = String.Concat(splits[1..]);
					dic.Add(splits[0].Trim(), content.Trim());
				}
			}

			return dic;
		}
	}
}
