using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using GameBang_Bot.Database.Models;
using GameBang_Bot.Properties;

#nullable disable

namespace GameBang_Bot.Database
{
    public partial class PostgresContext : DbContext
    {
        public PostgresContext()
        {
        }

        public PostgresContext(DbContextOptions<PostgresContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LolTeam> LolTeams { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserBet> UserBets { get; set; }
        public virtual DbSet<UserPoint> UserPoints { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql($"Host={PropertiesContext.Database.Host};Database={PropertiesContext.Database.Database};Username={PropertiesContext.Database.Username};Password={PropertiesContext.Database.Password};");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("adminpack")
                .HasAnnotation("Relational:Collation", "Korean_Korea.949");

            modelBuilder.Entity<LolTeam>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("nextval('lol_teams_id'::regclass)");
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.Property(e => e.Date).HasDefaultValueSql("now()");

                entity.Property(e => e.IsBetable).HasDefaultValueSql("true");

                entity.HasOne(d => d.Team1Navigation)
                    .WithMany(p => p.MatchTeam1Navigations)
                    .HasForeignKey(d => d.Team1)
                    .HasConstraintName("matches_team1_fkey");

                entity.HasOne(d => d.Team2Navigation)
                    .WithMany(p => p.MatchTeam2Navigations)
                    .HasForeignKey(d => d.Team2)
                    .HasConstraintName("matches_team2_fkey");

                entity.HasOne(d => d.WinNavigation)
                    .WithMany(p => p.MatchWinNavigations)
                    .HasForeignKey(d => d.Win)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("matches_win_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToView("users_wp");
                entity.Property(e => e.Date).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<UserBet>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("nextval('users_bets_id'::regclass)");

                entity.Property(e => e.Date).HasDefaultValueSql("now()");

                entity.Property(e => e.Earned).HasDefaultValueSql("0");

                entity.Property(e => e.MatchId).ValueGeneratedOnAdd();

                entity.Property(e => e.UserId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.UserBets)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("user_bets_match_id_fkey");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.UserBets)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("user_bets_team_id_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserBets)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_bets_user_id_fkey");
            });

            modelBuilder.Entity<UserPoint>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("nextval('users_points_id'::regclass)");

                entity.Property(e => e.Date).HasDefaultValueSql("now()");

                entity.Property(e => e.UserId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPoints)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_points_user_id_fkey");
            });

            modelBuilder.HasSequence("lol_teams_id");

            modelBuilder.HasSequence("users_bets_id");

            modelBuilder.HasSequence("users_points_id");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
