﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pinball.Api.Data;

#nullable disable

namespace Pinball.Api.Data.Migrations
{
    [DbContext(typeof(PinballDbContext))]
    [Migration("20240112173916_TimedEntities")]
    partial class TimedEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Pinball.Entities.Data.Opdb.OpdbCatalogSnapshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.Property<DateTimeOffset>("Imported")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("MachineGroupJsonResponse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MachineJsonResponse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("Published")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.HasKey("Id");

                    b.ToTable("CatalogSnapshots");
                });

            modelBuilder.Entity("Pinball.Entities.Data.Opdb.OpdbChangelog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Action")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NewOpdbId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OpdbId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.HasKey("Id");

                    b.ToTable("OpdbChangelogs");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachine", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");

                    b.Property<string>("CommonName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("IpdbId")
                        .HasColumnType("int");

                    b.Property<string>("MachineGroupId")
                        .HasColumnType("nvarchar(450)")
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");

                    b.Property<DateTime?>("ManufactureDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ManufacturerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("PlayerCount")
                        .HasColumnType("smallint");

                    b.Property<string>("TypeId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("MachineGroupId");

                    b.HasIndex("ManufacturerId");

                    b.HasIndex("TypeId");

                    b.ToTable("PinballMachines");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineFeature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PinballFeatures");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineFeatureMapping", b =>
                {
                    b.Property<Guid>("FeatureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MachineId")
                        .HasColumnType("nvarchar(450)")
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");

                    b.HasKey("FeatureId", "MachineId");

                    b.HasIndex("MachineId");

                    b.ToTable("PinballMachineFeatureMapping");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineGroup", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PinballMachineGroups");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineKeyword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PinballKeywords");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineKeywordMapping", b =>
                {
                    b.Property<Guid>("KeywordId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MachineId")
                        .HasColumnType("nvarchar(450)")
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");

                    b.HasKey("KeywordId", "MachineId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("MachineId");

                    b.ToTable("PinballMachineKeywordMapping");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineManufacturer", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PinballManufacturers");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PinballTypes");

                    b.HasData(
                        new
                        {
                            Id = "ss",
                            Name = "Solid-State"
                        },
                        new
                        {
                            Id = "em",
                            Name = "Electro-Mechanical"
                        },
                        new
                        {
                            Id = "me",
                            Name = "Mechanical"
                        },
                        new
                        {
                            Id = "dmd",
                            Name = "Dot-Matrix Display"
                        });
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachine", b =>
                {
                    b.HasOne("Pinball.Entities.Data.PinballMachines.PinballMachineGroup", "MachineGroup")
                        .WithMany("Machines")
                        .HasForeignKey("MachineGroupId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Pinball.Entities.Data.PinballMachines.PinballMachineManufacturer", "Manufacturer")
                        .WithMany("Machines")
                        .HasForeignKey("ManufacturerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Pinball.Entities.Data.PinballMachines.PinballMachineType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("MachineGroup");

                    b.Navigation("Manufacturer");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineFeatureMapping", b =>
                {
                    b.HasOne("Pinball.Entities.Data.PinballMachines.PinballMachineFeature", "Feature")
                        .WithMany("Mappings")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pinball.Entities.Data.PinballMachines.PinballMachine", "Machine")
                        .WithMany("FeatureMappings")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feature");

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineKeywordMapping", b =>
                {
                    b.HasOne("Pinball.Entities.Data.PinballMachines.PinballMachineKeyword", "Keyword")
                        .WithMany("Mappings")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pinball.Entities.Data.PinballMachines.PinballMachine", "Machine")
                        .WithMany("KeywordMappings")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Keyword");

                    b.Navigation("Machine");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachine", b =>
                {
                    b.Navigation("FeatureMappings");

                    b.Navigation("KeywordMappings");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineFeature", b =>
                {
                    b.Navigation("Mappings");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineGroup", b =>
                {
                    b.Navigation("Machines");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineKeyword", b =>
                {
                    b.Navigation("Mappings");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineManufacturer", b =>
                {
                    b.Navigation("Machines");
                });
#pragma warning restore 612, 618
        }
    }
}
