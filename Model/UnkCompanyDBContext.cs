﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ImportUtility.Model
{
    public partial class UnkCompanyDBContext : DbContext
    {
        public UnkCompanyDBContext()
        {
        }

        public UnkCompanyDBContext(DbContextOptions<UnkCompanyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Position> Positions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=DESKTOP-CBSCN3M;database=UnkCompanyDB;integrated security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.IdDepartment);

                entity.Property(e => e.IdDepartment).HasColumnName("idDepartment");

                entity.Property(e => e.IdDirector).HasColumnName("idDirector");

                entity.Property(e => e.IdParentDepartment).HasColumnName("idParentDepartment");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdDirectorNavigation)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.IdDirector)
                    .HasConstraintName("FK_Departments_Employees");

                entity.HasOne(d => d.IdParentDepartmentNavigation)
                    .WithMany(p => p.InverseIdParentDepartmentNavigation)
                    .HasForeignKey(d => d.IdParentDepartment)
                    .HasConstraintName("FK_Departments_Departments");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.IdEmployee);

                entity.Property(e => e.IdEmployee).HasColumnName("idEmployee");

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.IdDepartment).HasColumnName("idDepartment");

                entity.Property(e => e.IdPosition).HasColumnName("idPosition");

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdDepartmentNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.IdDepartment)
                    .HasConstraintName("FK_Employees_Departments");

                entity.HasOne(d => d.IdPositionNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.IdPosition)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Positions");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.IdPosition);

                entity.Property(e => e.IdPosition).HasColumnName("idPosition");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
