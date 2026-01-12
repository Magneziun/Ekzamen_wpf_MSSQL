namespace Ekzamen_wpf_MSSQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.authors",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.books",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 255),
                        pages = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        publish_date = c.DateTime(nullable: false),
                        author_id = c.Int(nullable: false),
                        theme_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.authors", t => t.author_id, cascadeDelete: true)
                .ForeignKey("dbo.themes", t => t.theme_id, cascadeDelete: true)
                .Index(t => t.author_id)
                .Index(t => t.theme_id);
            
            CreateTable(
                "dbo.themes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.books", "theme_id", "dbo.themes");
            DropForeignKey("dbo.books", "author_id", "dbo.authors");
            DropIndex("dbo.books", new[] { "theme_id" });
            DropIndex("dbo.books", new[] { "author_id" });
            DropTable("dbo.themes");
            DropTable("dbo.books");
            DropTable("dbo.authors");
        }
    }
}
