using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GameBang_Bot.Database.Models
{
    [Table("user_points")]
    public partial class UserPoint
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public ulong UserId { get; set; }
        [Column("point")]
        public int Point { get; set; }
        [Required]
        [Column("reason")]
        [StringLength(100)]
        public string Reason { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserPoints")]
        public virtual User User { get; set; }
    }
}
