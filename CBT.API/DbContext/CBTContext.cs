using CBT.API.Auth;
using CBT.API.Model;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;

namespace CBT.API.DbContext
{
    public class CBTContext : Microsoft.EntityFrameworkCore.DbContext 
    {
        public CBTContext(DbContextOptions<CBTContext> options) : base(options)
        {
        }

        //public DbSet<User> User { get; set; }
        public DbSet<UserManagement> UserManagement { get; set; }
        public DbSet<Question> Question { get; set; } 
        public DbSet<QuestionTopic> QuestionTopic { get; set; } 
        public DbSet<QuestionOption> QuestionOption { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<TestResult> TestResult { get; set; }
        public DbSet<TestResultHistory> TestResultHistory { get; set; }
        public DbSet<EmailNotificationModel> EmailNotificationModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionTopic>()
            .HasMany(qt => qt.questions) 
            .WithOne()
            .HasForeignKey(q => q.question_topic_id)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.question_options)
                .WithOne()
                .HasForeignKey(qo => qo.question_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestResult>()
                .HasMany(q => q.test_result_history)
                .WithOne()
                .HasForeignKey(qo => qo.test_result_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailNotificationModel>().HasNoKey();

            //modelBuilder.Entity<User>().HasData(
            //    new User
            //    {
            //        Id = 1,
            //        FirstName = "System",
            //        LastName = "",
            //        Username = "admin",
            //        Password = "admin",
            //    }
            //);

            //modelBuilder.Entity<TestResult>()
            //    .HasOne(q => q.user_management)
            //    .WithMany()
            //    .HasForeignKey(qo => qo.user_management_id)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<TestResult>()
            //    .HasOne(tr => tr.question_topic)
            //    .WithMany()
            //    .HasForeignKey(tr => tr.question_topic_id)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
