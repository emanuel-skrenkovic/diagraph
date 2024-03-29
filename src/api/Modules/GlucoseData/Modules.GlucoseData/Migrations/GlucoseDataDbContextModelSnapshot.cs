﻿// <auto-generated />
using System;
using Diagraph.Modules.GlucoseData.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Diagraph.Modules.GlucoseData.Migrations
{
    [DbContext(typeof(GlucoseDataDbContext))]
    partial class GlucoseDataDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Diagraph.Modules.GlucoseData.GlucoseMeasurement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at_utc");

                    b.Property<int>("ImportId")
                        .HasColumnType("integer")
                        .HasColumnName("import_id");

                    b.Property<float>("Level")
                        .HasColumnType("real")
                        .HasColumnName("level");

                    b.Property<DateTime>("TakenAt")
                        .HasColumnType("timestamp")
                        .HasColumnName("taken_at");

                    b.Property<int>("Unit")
                        .HasColumnType("integer")
                        .HasColumnName("unit");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at_utc");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("ImportId");

                    b.ToTable("glucose_measurement", (string)null);
                });

            modelBuilder.Entity("Diagraph.Modules.GlucoseData.Imports.Import", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at_utc");

                    b.Property<string>("Hash")
                        .HasColumnType("text")
                        .HasColumnName("hash");

                    b.Property<DateTime>("UpdatedAtUtc")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at_utc");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.ToTable("import", (string)null);
                });

            modelBuilder.Entity("Diagraph.Modules.GlucoseData.GlucoseMeasurement", b =>
                {
                    b.HasOne("Diagraph.Modules.GlucoseData.Imports.Import", null)
                        .WithMany("Measurements")
                        .HasForeignKey("ImportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Diagraph.Modules.GlucoseData.Imports.Import", b =>
                {
                    b.Navigation("Measurements");
                });
#pragma warning restore 612, 618
        }
    }
}
