using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemberDataIntegration.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMemberFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Members",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Members");
        }
    }
}
