using System;

namespace GameBang_Bot.Properties {
	[AttributeUsage(AttributeTargets.Property)]
	class KeyNameAttribute : Attribute {
		public KeyNameAttribute(string name) {
			this.Name = name;
		}

		public string Name { get; set; }
		public bool Required { get; set; }
	}
}
