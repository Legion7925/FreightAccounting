﻿// <auto-generated />
using System;
using FreightAccounting.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FreightAccounting.Core.Migrations
{
    [DbContext(typeof(FreightAccountingContext))]
    partial class FreightAccountingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FreightAccounting.Core.Entities.Debtor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("DebtAmount")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Destination")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DriverFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DriverLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlateNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DriverFirstName");

                    b.HasIndex("DriverLastName");

                    b.ToTable("Debtors");
                });

            modelBuilder.Entity("FreightAccounting.Core.Entities.Expense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ExpensesAmount")
                        .HasColumnType("bigint");

                    b.Property<long>("Income")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("FreightAccounting.Core.Entities.OperatorUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Family")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OperatorUsers");
                });

            modelBuilder.Entity("FreightAccounting.Core.Entities.Remittance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("InsurancePayment")
                        .HasColumnType("bigint");

                    b.Property<long>("NetProfit")
                        .HasColumnType("bigint");

                    b.Property<int>("OperatorUserId")
                        .HasColumnType("int");

                    b.Property<long>("OrganizationPayment")
                        .HasColumnType("bigint");

                    b.Property<long>("ProductInsuranceNumber")
                        .HasColumnType("bigint");

                    b.Property<long>("ReceviedCommission")
                        .HasColumnType("bigint");

                    b.Property<string>("RemittanceNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("SubmitDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubmittedUsername")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TaxPayment")
                        .HasColumnType("bigint");

                    b.Property<long>("TransforPayment")
                        .HasColumnType("bigint");

                    b.Property<long>("UserCut")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("OperatorUserId");

                    b.HasIndex("RemittanceNumber");

                    b.ToTable("Remittances");
                });

            modelBuilder.Entity("FreightAccounting.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("NameAndFamily")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            NameAndFamily = "root",
                            Password = "113765b146867037b814a8ef1c2ec35d73bfb77c8d27a5f1a520099f5949cfaa",
                            Username = "root"
                        },
                        new
                        {
                            Id = 2,
                            NameAndFamily = "kaveh",
                            Password = "1b19dec984a63114b8061b5a7d9a7be7d3515876f9d59763afdefe288be0b700",
                            Username = "kaveh"
                        });
                });

            modelBuilder.Entity("FreightAccounting.Core.Entities.Remittance", b =>
                {
                    b.HasOne("FreightAccounting.Core.Entities.OperatorUser", "OperatorUser")
                        .WithMany("Remittances")
                        .HasForeignKey("OperatorUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OperatorUser");
                });

            modelBuilder.Entity("FreightAccounting.Core.Entities.OperatorUser", b =>
                {
                    b.Navigation("Remittances");
                });
#pragma warning restore 612, 618
        }
    }
}
