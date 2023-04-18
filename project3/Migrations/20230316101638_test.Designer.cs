﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using project3.Data;

#nullable disable

namespace project3.Migrations
{
    [DbContext(typeof(FelixDbContext))]
    [Migration("20230316101638_test")]
    partial class test
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("project3.Models.Application", b =>
                {
                    b.Property<int>("app_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("app_id"), 1L, 1);

                    b.Property<string>("CV")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("company_id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("date_of_app")
                        .HasColumnType("datetime2");

                    b.Property<int?>("job_id")
                        .HasColumnType("int");

                    b.Property<int?>("user_id")
                        .HasColumnType("int");

                    b.HasKey("app_id");

                    b.HasIndex("company_id");

                    b.HasIndex("job_id");

                    b.HasIndex("user_id");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("project3.Models.Category", b =>
                {
                    b.Property<int>("cat_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("cat_id"), 1L, 1);

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("cat_id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("project3.Models.company", b =>
                {
                    b.Property<int>("company_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("company_id"), 1L, 1);

                    b.Property<string>("company_email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("company_image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("company_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("company_id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("project3.Models.job", b =>
                {
                    b.Property<int>("job_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("job_id"), 1L, 1);

                    b.Property<string>("age_range")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("cat_id")
                        .HasColumnType("int");

                    b.Property<string>("city")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("company_id")
                        .HasColumnType("int");

                    b.Property<int?>("experience_years")
                        .HasColumnType("int");

                    b.Property<string>("job_age")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("job_image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("job_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("work_hours")
                        .HasColumnType("int");

                    b.HasKey("job_id");

                    b.HasIndex("cat_id");

                    b.HasIndex("company_id");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("project3.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("project3.Models.user", b =>
                {
                    b.Property<int>("user_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("user_id"), 1L, 1);

                    b.Property<string>("gender")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<int>("user_age")
                        .HasColumnType("int");

                    b.Property<string>("user_email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("user_image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("user_name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("user_password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("user_phone")
                        .HasColumnType("int");

                    b.Property<int>("user_role")
                        .HasColumnType("int");

                    b.HasKey("user_id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("project3.Models.Application", b =>
                {
                    b.HasOne("project3.Models.company", "company")
                        .WithMany()
                        .HasForeignKey("company_id");

                    b.HasOne("project3.Models.job", "job")
                        .WithMany()
                        .HasForeignKey("job_id");

                    b.HasOne("project3.Models.user", "user")
                        .WithMany()
                        .HasForeignKey("user_id");

                    b.Navigation("company");

                    b.Navigation("job");

                    b.Navigation("user");
                });

            modelBuilder.Entity("project3.Models.job", b =>
                {
                    b.HasOne("project3.Models.Category", "Category")
                        .WithMany("jobs")
                        .HasForeignKey("cat_id");

                    b.HasOne("project3.Models.company", "company")
                        .WithMany("Jobs")
                        .HasForeignKey("company_id");

                    b.Navigation("Category");

                    b.Navigation("company");
                });

            modelBuilder.Entity("project3.Models.Category", b =>
                {
                    b.Navigation("jobs");
                });

            modelBuilder.Entity("project3.Models.company", b =>
                {
                    b.Navigation("Jobs");
                });
#pragma warning restore 612, 618
        }
    }
}