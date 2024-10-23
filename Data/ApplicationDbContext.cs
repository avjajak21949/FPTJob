using System;
using System.Collections.Generic;
using System.Text;
using FPTJob.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FPTJob.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedUserRole(builder);
            // Cấu hình quan hệ JobCV
            builder.Entity<JobCV>()
                .HasOne(jc => jc.CV)
                .WithMany(c => c.JobCV)
                .HasForeignKey(jc => jc.CVID);
            builder.Entity<JobCV>()
                .HasOne(jc => jc.Job)
                .WithMany(j => j.JobCV)
                .HasForeignKey(jc => jc.JobId);
        }
        private void SeedUserRole(ModelBuilder builder)
        {
            var adminAccount = new IdentityUser
            {
                Id = "user0",
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true
            };
            var jobseekerAccount = new IdentityUser
            {
                Id = "user1",
                UserName = "jobseeker@gmail.com",
                Email = "jobseeker@gmail.com",
                NormalizedUserName = "JOBSEEKER@GMAIL.COM",
                NormalizedEmail = "JOBSEEKER@GMAIL.COM",
                EmailConfirmed = true
            };
            var employerAccount = new IdentityUser
            {
                Id = "user2",
                UserName = "employer@gmail.com",
                Email = "employer@gmail.com",
                NormalizedUserName = "EMPLOYER@GMAIL.COM",
                NormalizedEmail = "EMPLOYER@GMAIL.COM",
                EmailConfirmed = true
            };
            var hasher = new PasswordHasher<IdentityUser>();
            adminAccount.PasswordHash = hasher.HashPassword(adminAccount, "123456");
            jobseekerAccount.PasswordHash = hasher.HashPassword(jobseekerAccount, "123456");
            employerAccount.PasswordHash = hasher.HashPassword(employerAccount, "123456");
            builder.Entity<IdentityUser>().HasData(adminAccount, jobseekerAccount, employerAccount);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "role0",
                    Name = "Adminstator",
                    NormalizedName = "ADMINSTRATOR"
                },
                new IdentityRole
                {
                    Id = "role1",
                    Name = "Jobseeker",
                    NormalizedName = "JOBSEEKER"
                },
                new IdentityRole
                {
                    Id = "role2",
                    Name = "Employer",
                    NormalizedName = "EMPLOYER"
                });
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = "user0",
                    RoleId = "role0"
                },
                new IdentityUserRole<string>
                {
                    UserId = "user1",
                    RoleId = "role1"
                },
                new IdentityUserRole<string>
                {
                    UserId = "user2",
                    RoleId = "role2"
                }
            );
        }
        public DbSet<FPTJob.Models.Category> Category { get; set; }
        public DbSet<FPTJob.Models.CV> CV { get; set; }
        public DbSet<FPTJob.Models.Job> Job { get; set; }
        public DbSet<FPTJob.Models.JobCV> JobCV { get; set; }
    }
}
