using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GameBang_Bot.Database.Models
{
    [Table("user_bets")]
    public partial class UserBet
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public ulong UserId { get; set; }
        [Column("match_id")]
        public ulong MatchId { get; set; }
        [Column("team_id")]
        public int TeamId { get; set; }
        [Column("point")]
        public int Point { get; set; }
        [Column("earned")]
        public int? Earned { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(MatchId))]
        [InverseProperty("UserBets")]
        public virtual Match Match { get; set; }
        [ForeignKey(nameof(TeamId))]
        [InverseProperty(nameof(LolTeam.UserBets))]
        public virtual LolTeam Team { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserBets")]
        public virtual User User { get; set; }
    }
}
