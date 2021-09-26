using System;
using CustomerManagement.Datastore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

#nullable disable

namespace CustomerManagement.Datastore
{
    public partial class CustomerManagementContext : DbContext
    {
        static CustomerManagementContext() => NpgsqlConnection.GlobalTypeMapper.MapEnum<CardType>();
        public CustomerManagementContext()
        {
        }

        public CustomerManagementContext(DbContextOptions<CustomerManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum(null, "card_type", new[] { "Amex", "Visa", "MasterCard" })
                .HasAnnotation("Relational:Collation", "en_US.UTF-8");
            
            modelBuilder.Entity<Card>(entity =>
            {
                entity.ToTable("cards");
                entity.HasKey(e => e.Number);


                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.Cvv)
                    .IsRequired()
                    .HasColumnName("cvv");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("date")
                    .HasColumnName("expiry_date");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasColumnName("number");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type");

                //entity.HasOne(d => d.Customer)
                //    .WithMany(p => p.Cards)
                //    .HasForeignKey(d => d.CustomerId)
                //    .HasConstraintName("cards_customer_id_fkey");              
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
