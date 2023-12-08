﻿// <auto-generated />
using System;
using BlazorDungeonCrawler.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazorDungeonCrawler.Server.Migrations
{
    [DbContext(typeof(DungeonDbContext))]
    [Migration("20231207201239_AddIsCurrentToFloor")]
    partial class AddIsCurrentToFloor
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Adventurer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ArmourBoots")
                        .HasColumnType("int");

                    b.Property<int>("ArmourBreastplate")
                        .HasColumnType("int");

                    b.Property<int>("ArmourGauntlet")
                        .HasColumnType("int");

                    b.Property<int>("ArmourGreave")
                        .HasColumnType("int");

                    b.Property<int>("ArmourHelmet")
                        .HasColumnType("int");

                    b.Property<int>("AuraPotion")
                        .HasColumnType("int");

                    b.Property<int>("AuraPotionDuration")
                        .HasColumnType("int");

                    b.Property<int>("DamageBase")
                        .HasColumnType("int");

                    b.Property<int>("DamagePotion")
                        .HasColumnType("int");

                    b.Property<int>("DamagePotionDuration")
                        .HasColumnType("int");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<int>("ExperienceLevel")
                        .HasColumnType("int");

                    b.Property<int>("HealthBase")
                        .HasColumnType("int");

                    b.Property<int>("HealthInitial")
                        .HasColumnType("int");

                    b.Property<bool>("IsAlive")
                        .HasColumnType("bit");

                    b.Property<int>("NextExperienceLevelCost")
                        .HasColumnType("int");

                    b.Property<int>("ProtectionBase")
                        .HasColumnType("int");

                    b.Property<int>("ShieldPotion")
                        .HasColumnType("int");

                    b.Property<int>("ShieldPotionDuration")
                        .HasColumnType("int");

                    b.Property<int>("Weapon")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Adventurers");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Dungeon", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AdventurerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApiVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("CombatInitiated")
                        .HasColumnType("bit");

                    b.Property<Guid>("CombatTile")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Depth")
                        .HasColumnType("int");

                    b.Property<bool>("GameOver")
                        .HasColumnType("bit");

                    b.Property<bool>("InCombat")
                        .HasColumnType("bit");

                    b.Property<bool>("MacGuffinFound")
                        .HasColumnType("bit");

                    b.Property<bool>("StairsDiscovered")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AdventurerId");

                    b.ToTable("Dungeons");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Floor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Columns")
                        .HasColumnType("int");

                    b.Property<int>("Depth")
                        .HasColumnType("int");

                    b.Property<Guid>("DungeonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsCurrent")
                        .HasColumnType("bit");

                    b.Property<int>("Rows")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DungeonId");

                    b.ToTable("Floors");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DangerDice")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Datestamp")
                        .HasColumnType("float");

                    b.Property<Guid?>("DungeonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SafeDice")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DungeonId");

                    b.HasIndex("MessageId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Monster", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ClientX")
                        .HasColumnType("int");

                    b.Property<int>("ClientY")
                        .HasColumnType("int");

                    b.Property<int>("Damage")
                        .HasColumnType("int");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<int>("Protection")
                        .HasColumnType("int");

                    b.Property<Guid>("TileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TileId");

                    b.ToTable("Monsters");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Tile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Column")
                        .HasColumnType("int");

                    b.Property<bool>("Current")
                        .HasColumnType("bit");

                    b.Property<bool>("FightWon")
                        .HasColumnType("bit");

                    b.Property<Guid>("FloorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Hidden")
                        .HasColumnType("bit");

                    b.Property<int>("Row")
                        .HasColumnType("int");

                    b.Property<bool>("Selectable")
                        .HasColumnType("bit");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FloorId");

                    b.ToTable("Tiles");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Dungeon", b =>
                {
                    b.HasOne("BlazorDungeonCrawler.Shared.Models.Adventurer", "Adventurer")
                        .WithMany()
                        .HasForeignKey("AdventurerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Adventurer");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Floor", b =>
                {
                    b.HasOne("BlazorDungeonCrawler.Shared.Models.Dungeon", null)
                        .WithMany("Floors")
                        .HasForeignKey("DungeonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Message", b =>
                {
                    b.HasOne("BlazorDungeonCrawler.Shared.Models.Dungeon", null)
                        .WithMany("Messages")
                        .HasForeignKey("DungeonId");

                    b.HasOne("BlazorDungeonCrawler.Shared.Models.Message", null)
                        .WithMany("Children")
                        .HasForeignKey("MessageId");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Monster", b =>
                {
                    b.HasOne("BlazorDungeonCrawler.Shared.Models.Tile", null)
                        .WithMany("Monsters")
                        .HasForeignKey("TileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Tile", b =>
                {
                    b.HasOne("BlazorDungeonCrawler.Shared.Models.Floor", null)
                        .WithMany("Tiles")
                        .HasForeignKey("FloorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Dungeon", b =>
                {
                    b.Navigation("Floors");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Floor", b =>
                {
                    b.Navigation("Tiles");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Message", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("BlazorDungeonCrawler.Shared.Models.Tile", b =>
                {
                    b.Navigation("Monsters");
                });
#pragma warning restore 612, 618
        }
    }
}
