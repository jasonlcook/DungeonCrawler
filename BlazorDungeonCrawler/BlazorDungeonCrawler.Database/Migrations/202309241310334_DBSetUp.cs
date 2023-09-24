namespace BlazorDungeonCrawler.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBSetUp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adventurers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        HealthBase = c.Int(nullable: false),
                        HealthInitial = c.Int(nullable: false),
                        AuraPotion = c.Int(nullable: false),
                        AuraPotionDuration = c.Int(nullable: false),
                        DamageBase = c.Int(nullable: false),
                        DamagePotion = c.Int(nullable: false),
                        DamagePotionDuration = c.Int(nullable: false),
                        ProtectionBase = c.Int(nullable: false),
                        ShieldPotion = c.Int(nullable: false),
                        ShieldPotionDuration = c.Int(nullable: false),
                        Weapon = c.Int(nullable: false),
                        ArmourHelmet = c.Int(nullable: false),
                        ArmourBreastplate = c.Int(nullable: false),
                        ArmourGauntlet = c.Int(nullable: false),
                        ArmourGreave = c.Int(nullable: false),
                        ArmourBoots = c.Int(nullable: false),
                        IsAlive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Dungeons",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MacGuffinFound = c.Boolean(nullable: false),
                        AdventurerId = c.Guid(nullable: false),
                        LevelId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Adventurers", t => t.AdventurerId, cascadeDelete: true)
                .ForeignKey("dbo.Levels", t => t.LevelId, cascadeDelete: true)
                .Index(t => t.AdventurerId)
                .Index(t => t.LevelId);
            
            CreateTable(
                "dbo.Levels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Depth = c.Int(nullable: false),
                        Rows = c.Int(nullable: false),
                        Columns = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tiles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Row = c.Int(nullable: false),
                        Column = c.Int(nullable: false),
                        Level_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Levels", t => t.Level_Id)
                .Index(t => t.Level_Id);
            
            CreateTable(
                "dbo.Monsters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LevelStart = c.Int(nullable: false),
                        LevelEnd = c.Int(nullable: false),
                        DiceHealth = c.Int(nullable: false),
                        DiceDamage = c.Int(nullable: false),
                        DiceProtection = c.Int(nullable: false),
                        Documentation = c.String(),
                        Tile_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tiles", t => t.Tile_Id)
                .Index(t => t.Tile_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Dungeons", "LevelId", "dbo.Levels");
            DropForeignKey("dbo.Tiles", "Level_Id", "dbo.Levels");
            DropForeignKey("dbo.Monsters", "Tile_Id", "dbo.Tiles");
            DropForeignKey("dbo.Dungeons", "AdventurerId", "dbo.Adventurers");
            DropIndex("dbo.Monsters", new[] { "Tile_Id" });
            DropIndex("dbo.Tiles", new[] { "Level_Id" });
            DropIndex("dbo.Dungeons", new[] { "LevelId" });
            DropIndex("dbo.Dungeons", new[] { "AdventurerId" });
            DropTable("dbo.Monsters");
            DropTable("dbo.Tiles");
            DropTable("dbo.Levels");
            DropTable("dbo.Dungeons");
            DropTable("dbo.Adventurers");
        }
    }
}
