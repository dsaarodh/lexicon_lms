namespace LMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetUsers", name: "Courses_Id", newName: "CourseId");
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_Courses_Id", newName: "IX_CourseId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_CourseId", newName: "IX_Courses_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "CourseId", newName: "Courses_Id");
        }
    }
}
