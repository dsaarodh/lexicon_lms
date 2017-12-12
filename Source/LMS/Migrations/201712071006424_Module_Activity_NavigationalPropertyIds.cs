namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Module_Activity_NavigationalPropertyIds : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Activities", "ActivityType_Id", "dbo.ActivityTypes");
            DropForeignKey("dbo.Activities", "Module_Id", "dbo.Modules");
            DropForeignKey("dbo.Modules", "Course_Id", "dbo.Courses");
            DropIndex("dbo.Activities", new[] { "ActivityType_Id" });
            DropIndex("dbo.Activities", new[] { "Module_Id" });
            DropIndex("dbo.Modules", new[] { "Course_Id" });
            RenameColumn(table: "dbo.Activities", name: "ActivityType_Id", newName: "ActivityTypeId");
            RenameColumn(table: "dbo.Activities", name: "Module_Id", newName: "ModuleId");
            RenameColumn(table: "dbo.Modules", name: "Course_Id", newName: "CourseId");
            AlterColumn("dbo.Activities", "ActivityTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Activities", "ModuleId", c => c.Int(nullable: false));
            AlterColumn("dbo.Modules", "CourseId", c => c.Int(nullable: false));
            CreateIndex("dbo.Activities", "ActivityTypeId");
            CreateIndex("dbo.Activities", "ModuleId");
            CreateIndex("dbo.Modules", "CourseId");
            AddForeignKey("dbo.Activities", "ActivityTypeId", "dbo.ActivityTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Activities", "ModuleId", "dbo.Modules", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Modules", "CourseId", "dbo.Courses", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Modules", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Activities", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Activities", "ActivityTypeId", "dbo.ActivityTypes");
            DropIndex("dbo.Modules", new[] { "CourseId" });
            DropIndex("dbo.Activities", new[] { "ModuleId" });
            DropIndex("dbo.Activities", new[] { "ActivityTypeId" });
            AlterColumn("dbo.Modules", "CourseId", c => c.Int());
            AlterColumn("dbo.Activities", "ModuleId", c => c.Int());
            AlterColumn("dbo.Activities", "ActivityTypeId", c => c.Int());
            RenameColumn(table: "dbo.Modules", name: "CourseId", newName: "Course_Id");
            RenameColumn(table: "dbo.Activities", name: "ModuleId", newName: "Module_Id");
            RenameColumn(table: "dbo.Activities", name: "ActivityTypeId", newName: "ActivityType_Id");
            CreateIndex("dbo.Modules", "Course_Id");
            CreateIndex("dbo.Activities", "Module_Id");
            CreateIndex("dbo.Activities", "ActivityType_Id");
            AddForeignKey("dbo.Modules", "Course_Id", "dbo.Courses", "Id");
            AddForeignKey("dbo.Activities", "Module_Id", "dbo.Modules", "Id");
            AddForeignKey("dbo.Activities", "ActivityType_Id", "dbo.ActivityTypes", "Id");
        }
    }
}
