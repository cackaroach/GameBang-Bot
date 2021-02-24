using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GameBang_Bot.Database.Models
{
    [Table("matches")]
    public partial class Match
    {
        public Match()
        {
            UserBets = new HashSet<UserBet>();
        }

        [Key]
        [Column("id")]
        public ulong Id { get; set; }
        [Column("team1")]
        public int Team1 { get; set; }
        [Column("team2")]
        public int Team2 { get; set; }
        [Required]
        [Column("is_betable")]
        public bool? IsBetable { get; set; }
        [Column("win")]
        public int? Win { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(Team1))]
        [InverseProperty(nameof(LolTeam.MatchTeam1Navigations))]
        public virtual LolTeam Team1Navigation { get; set; }
        [ForeignKey(nameof(Team2))]
        [InverseProperty(nameof(LolTeam.MatchTeam2Navigations))]
        public virtual LolTeam Team2Navigation { get; set; }
        [ForeignKey(nameof(Win))]
        [InverseProperty(nameof(LolTeam.MatchWinNavigations))]
        public virtual LolTeam WinNavigation { get; set; }
        [InverseProperty(nameof(UserBet.Match))]
        public virtual ICollection<UserBet> UserBets { get; set; }
    }
}
