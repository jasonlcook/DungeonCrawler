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
                        Adventurer_Id = c.Guid(),
                        Level_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Adventurers", t => t.Adventurer_Id)
                .ForeignKey("dbo.Levels", t => t.Level_Id)
                .Index(t => t.Adventurer_Id)
                .Index(t => t.Level_Id);
            
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
                        Type = c.Int(nullable: false),
                        Current = c.Boolean(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                        Selectable = c.Boolean(nullable: false),
                        Level_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Levels", t => t.Level_Id)
                .Index(t => t.Level_Id);
            
            CreateTable(
                "dbo.Monsters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TypeName = c.String(),
                        Health = c.Int(nullable: false),
                        Damage = c.Int(nullable: false),
                        Protection = c.Int(nullable: false),
                        Tile_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tiles", t => t.Tile_Id)
                .Index(t => t.Tile_Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Index = c.Int(nullable: false),
                        Text = c.String(),
                        Dungeon_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dungeons", t => t.Dungeon_Id)
                .Index(t => t.Dungeon_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Dungeon_Id", "dbo.Dungeons");
            DropForeignKey("dbo.Dungeons", "Level_Id", "dbo.Levels");
            DropForeignKey("dbo.Tiles", "Level_Id", "dbo.Levels");
            DropForeignKey("dbo.Monsters", "Tile_Id", "dbo.Tiles");
            DropForeignKey("dbo.Dungeons", "Adventurer_Id", "dbo.Adventurers");
            DropIndex("dbo.Messages", new[] { "Dungeon_Id" });
            DropIndex("dbo.Monsters", new[] { "Tile_Id" });
            DropIndex("dbo.Tiles", new[] { "Level_Id" });
            DropIndex("dbo.Dungeons", new[] { "Level_Id" });
            DropIndex("dbo.Dungeons", new[] { "Adventurer_Id" });
            DropTable("dbo.Messages");
            DropTable("dbo.Monsters");
            DropTable("dbo.Tiles");
            DropTable("dbo.Levels");
            DropTable("dbo.Dungeons");
            DropTable("dbo.Adventurers");
        }
    }
}
