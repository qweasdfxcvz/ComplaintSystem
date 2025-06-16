using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplaintSystem.Migrations
{
    /// <inheritdoc />
    public partial class ComplaintNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComplaintNumber",
                table: "Complaints",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComplaintNumber",
                table: "Complaints");
        }
    }
}
