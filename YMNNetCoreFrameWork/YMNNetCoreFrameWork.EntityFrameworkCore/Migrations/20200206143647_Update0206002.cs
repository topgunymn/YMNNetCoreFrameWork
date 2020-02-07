using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YMNNetCoreFrameWork.EntityFrameworkCore.Migrations
{
    public partial class Update0206002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YMNPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(nullable: true),
                    CreateUserId = table.Column<string>(maxLength: 255, nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifiterUserId = table.Column<string>(maxLength: 255, nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<string>(maxLength: 255, nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(maxLength: 50, nullable: true),
                    RoleId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YMNPermissions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YMNPermissions");
        }
    }
}
