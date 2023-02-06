using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SunPerfume.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTriggerAutoID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE TRIGGER Auto_Increase_ProductId\r\nON Products\r\ninstead of INSERT\r\nAS\r\nBEGIN\r\n\t\t\r\n\t\t\tDECLARE @ID VARCHAR(5)\r\n\t\t\tIF (SELECT COUNT(ProductId) FROM dbo.Products ) = 0\r\n\t\t\t\tBEGIN\r\n\t\t\t\tSET @ID = '0'\r\n\t\t\t\tEND\r\n\t\t\tELSE\r\n\t\t\t\tBEGIN\r\n\t\t\t\tSELECT @ID = MAX(RIGHT(ProductId, 3)) FROM dbo.Products\r\n\t\t\t\tSELECT @ID = CASE\r\n\t\t\t\t\tWHEN @ID >= 0 and @ID < 9 THEN 'L00' + CONVERT(VARCHAR, CONVERT(INT, @ID) + 1)\r\n\t\t\t\t\tWHEN @ID >= 9 THEN 'L0' + CONVERT(VARCHAR, CONVERT(INT, @ID) + 1)\r\n\t\t\t\tEND\r\n\t\t\t\tEND\r\n\t\t\tINSERT INTO Products\r\n\t\t\tSELECT Name, Description, Price, ImageUrl, CategoryId, BrandId, Quantity, ProductId = @ID\r\n\t\t\tFROM inserted\r\nEND");
            migrationBuilder.Sql("CREATE TRIGGER Auto_Increase_CategoryId\r\nON Categories\r\ninstead of INSERT\r\nAS\r\nBEGIN\r\n\t\t\r\n\t\t\tDECLARE @ID VARCHAR(5)\r\n\t\t\tIF (SELECT COUNT(CategoryId) FROM dbo.Categories ) = 0\r\n\t\t\t\tBEGIN\r\n\t\t\t\tSET @ID = '0'\r\n\t\t\t\tEND\r\n\t\t\tELSE\r\n\t\t\t\tBEGIN\r\n\t\t\t\tSELECT @ID = MAX(RIGHT(CategoryId, 3)) FROM dbo.Categories\r\n\t\t\t\tSELECT @ID = CASE\r\n\t\t\t\t\tWHEN @ID >= 0 and @ID < 9 THEN 'L00' + CONVERT(VARCHAR, CONVERT(INT, @ID) + 1)\r\n\t\t\t\t\tWHEN @ID >= 9 THEN 'L0' + CONVERT(VARCHAR, CONVERT(INT, @ID) + 1)\r\n\t\t\t\tEND\r\n\t\t\t\tEND\r\n\t\t\tINSERT INTO Categories\r\n\t\t\tSELECT Name, Note, ImageUrl, CategoryId = @ID\r\n\t\t\tFROM inserted\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER Auto_Increase_ProductId");
            migrationBuilder.Sql("DROP TRIGGER Auto_Increase_CategoryId");
        }
    }
}
