using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GameBang_Bot.Database.Models {
	[Table("users_wp")]
	public partial class User {
		public User() {
			UserBets = new HashSet<UserBet>();
			UserPoints = new HashSet<UserPoint>();
		}

		[Key]
		[Column("id")]
		public ulong Id { get; set; }
		[Column("date")]
		public DateTime Date { get; set; }
		[Column("point")]
		public long Point { get; set;  }

		[InverseProperty(nameof(UserBet.User))]
		public virtual ICollection<UserBet> UserBets { get; set; }
		[InverseProperty(nameof(UserPoint.User))]
		public virtual ICollection<UserPoint> UserPoints { get; set; }
	}
}
