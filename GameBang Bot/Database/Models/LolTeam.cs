using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GameBang_Bot.Database.Models
{
    [Table("lol_teams")]
    [Index(nameof(Name), Name = "lol_teams_name_key", IsUnique = true)]
    public partial class LolTeam
    {
        public LolTeam()
        {
            MatchTeam1Navigations = new HashSet<Match>();
            MatchTeam2Navigations = new HashSet<Match>();
            MatchWinNavigations = new HashSet<Match>();
            UserBets = new HashSet<UserBet>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(20)]
        public string Name { get; set; }

        [InverseProperty(nameof(Match.Team1Navigation))]
        public virtual ICollection<Match> MatchTeam1Navigations { get; set; }
        [InverseProperty(nameof(Match.Team2Navigation))]
        public virtual ICollection<Match> MatchTeam2Navigations { get; set; }
        [InverseProperty(nameof(Match.WinNavigation))]
        public virtual ICollection<Match> MatchWinNavigations { get; set; }
        [InverseProperty(nameof(UserBet.Team))]
        public virtual ICollection<UserBet> UserBets { get; set; }
    }
}
