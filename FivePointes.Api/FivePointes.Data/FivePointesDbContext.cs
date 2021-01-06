using FivePointes.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;

namespace FivePointes.Data
{
    public class FivePointesDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Commitment> Commitments { get; set; }

        public DbSet<TimeOffEntry> TimeOffEntries { get; set; }

        public FivePointesDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            var boolValueComparer = new ValueComparer<bool>(
                (c1, c2) => c1 == c2,
                c => c.GetHashCode(),
                c => c);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                /*if (entityType.IsQueryType)
                {
                    continue;
                }*/

                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                    else if (property.ClrType == typeof(bool))
                    {
                        property.SetValueComparer(boolValueComparer);
                    }
                }
            }

            modelBuilder.Entity<Account>(builder =>
            {
                builder.ToTable("Account");
                builder.HasKey(x => x.Id);
                //builder.HasMany(x => x.Expenses).WithOne(x => x.Account);
            });

            modelBuilder.Entity<Expense>(builder =>
            {
                builder.ToTable("Expense");
                builder.HasKey(x => x.Id);
                builder.HasOne(x => x.Account).WithMany(x => x.Expenses).HasForeignKey(x => x.AccountId);
                builder.HasOne(x => x.Category).WithMany(x => x.Expenses).HasForeignKey(x => x.CategoryId);
                builder.Property(x => x.AccountId).HasColumnName("Account_ID");
                builder.Property(x => x.CategoryId).HasColumnName("ExpenseCategory_ID");
                builder.Property(x => x.DatePaid).HasColumnType("Date");
            });

            modelBuilder.Entity<ExpenseCategory>(builder => {
                builder.ToTable("ExpenseCategory");
                builder.HasKey(x => x.Id);
                //builder.HasMany(x => x.Expenses).WithOne(x => x.Category);
            });

            modelBuilder.Entity<User>(builder =>
            {
                builder.ToTable("User");
                builder.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Commitment>(builder =>
            {
                builder.ToTable("Commitment");
                builder.HasKey(x => x.ClientId);
            });

            modelBuilder.Entity<TimeOffEntry>(builder =>
            {
                builder.ToTable("TimeOffEntry");
                builder.HasKey(x => x.Id);
            });
        }
    }
}
