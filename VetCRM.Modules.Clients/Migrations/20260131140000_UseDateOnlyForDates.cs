using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetCRM.Modules.Clients.Migrations
{
    public partial class UseDateOnlyForDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Clients"" ALTER COLUMN ""CreatedAt"" TYPE date USING ""CreatedAt""::date;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Clients"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone USING ""CreatedAt""::timestamp with time zone;");
        }
    }
}
