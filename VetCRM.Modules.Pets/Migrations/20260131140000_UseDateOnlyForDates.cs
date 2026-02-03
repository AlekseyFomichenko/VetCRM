using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetCRM.Modules.Pets.Migrations
{
    public partial class UseDateOnlyForDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE pets ALTER COLUMN ""BirthDate"" TYPE date USING ""BirthDate""::date;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE pets ALTER COLUMN ""BirthDate"" TYPE timestamp with time zone USING ""BirthDate""::timestamp with time zone;");
        }
    }
}
