using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemberDataIntegration.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceSpecificNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwardForceName",
                table: "Members",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailchimpName",
                table: "Members",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberPressName",
                table: "Members",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE Members
                SET MemberPressName = TRIM(COALESCE(FirstName, '') || ' ' || COALESCE(LastName, ''))
                WHERE IsMemberPress = 1
                    AND (COALESCE(FirstName, '') <> '' OR COALESCE(LastName, '') <> '')
                """
            );

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Members");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                table: "Members",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_Email",
                table: "Members");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Members",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Members",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE Members
                SET FirstName = MemberPressName
                WHERE MemberPressName IS NOT NULL
                """
            );

            migrationBuilder.DropColumn(
                name: "AwardForceName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MailchimpName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MemberPressName",
                table: "Members");
        }
    }
}
