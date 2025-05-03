using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public abstract class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasDefaultSchema("dbo");  

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<UserProfile>()
                .Property(p => p.UserId)
                .IsRequired();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r=> r.User)
                .WithMany()
                .HasForeignKey(r=>r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
             
            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailVerificationToken>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
             
            modelBuilder.Entity<OtpCode>()
               .HasOne(o => o.User)
               .WithMany()
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
