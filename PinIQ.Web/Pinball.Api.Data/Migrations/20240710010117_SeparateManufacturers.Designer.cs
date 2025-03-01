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
    [Migration("20240710010117_SeparateManufacturers")]
    partial class SeparateManufacturers
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Pinball.Entities.Core.Entities.CatalogChangelog", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.Property<string>("PinballMachineGroups")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PinballMachines")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PinballManufacturers")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.HasKey("Id");

                    b.ToTable("CatalogChangelogs");
                });

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

                    b.Property<int>("KeywordCount")
                        .HasColumnType("int");

                    b.Property<int>("MachineCount")
                        .HasColumnType("int");

                    b.Property<int>("MachineGroupCount")
                        .HasColumnType("int");

                    b.Property<string>("MachineGroupJsonResponse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MachineJsonResponse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ManufacturerCount")
                        .HasColumnType("int");

                    b.Property<string>("NewestMachine")
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

                    b.Property<int>("CatalogId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Created")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NewOpdbId")
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)");

                    b.Property<string>("OpdbId")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)");

                    b.Property<DateTimeOffset>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("sysdatetimeoffset()");

                    b.HasKey("Id");

                    b.ToTable("OpdbChangelogs");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CommonName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("IpdbId")
                        .HasColumnType("int");

                    b.Property<int?>("MachineGroupId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ManufactureDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ManufacturerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("OpdbId")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)")
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");

                    b.Property<short>("PlayerCount")
                        .HasColumnType("smallint");

                    b.Property<string>("TypeId")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.HasKey("Id");

                    b.HasIndex("MachineGroupId");

                    b.HasIndex("ManufacturerId");

                    b.HasIndex("TypeId");

                    b.ToTable("PinballMachines", "pinball");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineFeature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("PinballFeatures", "pinball");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineFeatureMapping", b =>
                {
                    b.Property<Guid>("FeatureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MachineId")
                        .HasColumnType("int");

                    b.HasKey("FeatureId", "MachineId");

                    b.HasIndex("MachineId");

                    b.ToTable("PinballMachineFeatureMapping", "pinball");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("OpdbId")
                        .IsRequired()
                        .HasMaxLength(18)
                        .HasColumnType("nvarchar(18)")
                        .UseCollation("SQL_Latin1_General_CP1_CS_AS");

                    b.Property<string>("ShortName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("PinballMachineGroups", "pinball");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineKeyword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.ToTable("PinballKeywords", "pinball");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineKeywordMapping", b =>
                {
                    b.Property<Guid>("KeywordId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MachineId")
                        .HasColumnType("int");

                    b.HasKey("KeywordId", "MachineId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("MachineId");

                    b.ToTable("PinballMachineKeywordMapping", "pinball");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineManufacturer", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("FullName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("PinballManufacturers", "pinball");
                });

            modelBuilder.Entity("Pinball.Entities.Data.PinballMachines.PinballMachineType", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("PinballTypes", "pinball");

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

            modelBuilder.Entity("Pinball.Entities.Data.Opdb.OpdbCatalogSnapshot", b =>
                {
                    b.OwnsMany("Pinball.Entities.Opdb.Manufacturer", "Manufacturers", b1 =>
                        {
                            b1.Property<int>("OpdbCatalogSnapshotId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("datetime2")
                                .HasAnnotation("Relational:JsonPropertyName", "created_at");

                            b1.Property<string>("FullName")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "full_name");

                            b1.Property<int>("ManufacturerId")
                                .HasColumnType("int")
                                .HasAnnotation("Relational:JsonPropertyName", "manufacturer_id");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "name");

                            b1.Property<DateTime>("UpdatedAt")
                                .HasColumnType("datetime2")
                                .HasAnnotation("Relational:JsonPropertyName", "updated_at");

                            b1.HasKey("OpdbCatalogSnapshotId", "Id");

                            b1.ToTable("CatalogSnapshots");

                            b1.ToJson("Manufacturers");

                            b1.WithOwner()
                                .HasForeignKey("OpdbCatalogSnapshotId");
                        });

                    b.OwnsMany("Pinball.Entities.Opdb.Machine", "Machines", b1 =>
                        {
                            b1.Property<int>("OpdbCatalogSnapshotId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<string>("CommonName")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "common_name");

                            b1.Property<string>("Description")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "description");

                            b1.Property<string>("Display")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "display");

                            b1.Property<int>("EqualityHash")
                                .HasColumnType("int");

                            b1.Property<string>("Features")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "features");

                            b1.Property<int?>("IpdbId")
                                .HasColumnType("int")
                                .HasAnnotation("Relational:JsonPropertyName", "ipdb_id");

                            b1.Property<string>("Keywords")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "keywords");

                            b1.Property<string>("MachineType")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "type");

                            b1.Property<DateTime?>("ManufactureDate")
                                .HasColumnType("datetime2")
                                .HasAnnotation("Relational:JsonPropertyName", "manufacture_date");

                            b1.Property<int?>("ManufacturerId")
                                .HasColumnType("int");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "name");

                            b1.Property<string>("OpdbId")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "opdb_id");

                            b1.Property<int>("PhysicalMachine")
                                .HasColumnType("int")
                                .HasAnnotation("Relational:JsonPropertyName", "physical_machine");

                            b1.Property<int>("PlayerCount")
                                .HasColumnType("int")
                                .HasAnnotation("Relational:JsonPropertyName", "player_count");

                            b1.Property<string>("Shortname")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "shortname");

                            b1.HasKey("OpdbCatalogSnapshotId", "Id");

                            b1.ToTable("CatalogSnapshots");

                            b1.ToJson("Machines");

                            b1.WithOwner()
                                .HasForeignKey("OpdbCatalogSnapshotId");

                            b1.OwnsOne("Pinball.Entities.Opdb.Manufacturer", "Manufacturer", b2 =>
                                {
                                    b2.Property<int>("MachineOpdbCatalogSnapshotId")
                                        .HasColumnType("int");

                                    b2.Property<int>("MachineId")
                                        .HasColumnType("int");

                                    b2.Property<DateTime>("CreatedAt")
                                        .HasColumnType("datetime2")
                                        .HasAnnotation("Relational:JsonPropertyName", "created_at");

                                    b2.Property<string>("FullName")
                                        .HasColumnType("nvarchar(max)")
                                        .HasAnnotation("Relational:JsonPropertyName", "full_name");

                                    b2.Property<int>("Id")
                                        .HasColumnType("int");

                                    b2.Property<int>("ManufacturerId")
                                        .HasColumnType("int")
                                        .HasAnnotation("Relational:JsonPropertyName", "manufacturer_id");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)")
                                        .HasAnnotation("Relational:JsonPropertyName", "name");

                                    b2.Property<DateTime>("UpdatedAt")
                                        .HasColumnType("datetime2")
                                        .HasAnnotation("Relational:JsonPropertyName", "updated_at");

                                    b2.HasKey("MachineOpdbCatalogSnapshotId", "MachineId");

                                    b2.ToTable("CatalogSnapshots");

                                    b2
                                        .ToJson("Manufacturer")
                                        .HasAnnotation("Relational:JsonPropertyName", "manufacturer");

                                    b2.WithOwner()
                                        .HasForeignKey("MachineOpdbCatalogSnapshotId", "MachineId");
                                });

                            b1.Navigation("Manufacturer");
                        });

                    b.OwnsMany("Pinball.Entities.Opdb.MachineGroup", "MachineGroups", b1 =>
                        {
                            b1.Property<int>("OpdbCatalogSnapshotId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("datetime2")
                                .HasAnnotation("Relational:JsonPropertyName", "created_at");

                            b1.Property<string>("Description")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "description");

                            b1.Property<int>("EqualityHash")
                                .HasColumnType("int");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "name");

                            b1.Property<string>("OpdbId")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "opdb_id");

                            b1.Property<string>("Shortname")
                                .HasColumnType("nvarchar(max)")
                                .HasAnnotation("Relational:JsonPropertyName", "shortname");

                            b1.Property<DateTime>("UpdatedAt")
                                .HasColumnType("datetime2")
                                .HasAnnotation("Relational:JsonPropertyName", "updated_at");

                            b1.HasKey("OpdbCatalogSnapshotId", "Id");

                            b1.ToTable("CatalogSnapshots");

                            b1.ToJson("MachineGroups");

                            b1.WithOwner()
                                .HasForeignKey("OpdbCatalogSnapshotId");
                        });

                    b.Navigation("MachineGroups");

                    b.Navigation("Machines");

                    b.Navigation("Manufacturers");
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
