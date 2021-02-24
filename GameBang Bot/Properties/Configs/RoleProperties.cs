namespace GameBang_Bot.Properties.Configs {
	class RoleProperties : IProperties {
		public ulong SoleIron { get; set; }
		public ulong SoleBronze { get; set; }
		public ulong SoleSilver { get; set; }
		public ulong SoleGold { get; set; }
		public ulong SolePlatinum { get; set; }
		public ulong SoleDiamond { get; set; }
		public ulong SoleMaster { get; set; }
		public ulong SoleGrandMaster { get; set; }
		public ulong SoleChallenger { get; set; }
		public ulong TeamIron { get; set; }
		public ulong TeamBronze { get; set; }
		public ulong TeamSilver { get; set; }
		public ulong TeamGold { get; set; }
		public ulong TeamPlatinum { get; set; }
		public ulong TeamDiamond { get; set; }
		public ulong TeamMaster { get; set; }
		public ulong TeamGrandMaster { get; set; }
		public ulong TeamChallenger { get; set; }

		public ulong ToId(string name) {
			var properties = this.GetType().GetProperties();

			foreach (var p in properties)
				if (p.Name == name)
					return (ulong)p.GetValue(this);

			throw new System.Exception("해당하는 역할을 찾지 못했습니다.");
		}

		public bool IsRankRole(ulong id) {
			var properties = this.GetType().GetProperties();

			foreach (var p in properties)
				if ((ulong)p.GetValue(this) == id)
					return true;

			return false;
		}
	}
}
