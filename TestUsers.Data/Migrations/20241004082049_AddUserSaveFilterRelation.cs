using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestUsers.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSaveFilterRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilterValueJson",
                table: "UserSaveFilter");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "UserSaveFilter",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FromAmount",
                table: "UserSaveFilter",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Search",
                table: "UserSaveFilter",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ToAmount",
                table: "UserSaveFilter",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserSaveFilterRelation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCategoryParameterValueId = table.Column<int>(type: "int", nullable: false),
                    UserSaveFilterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSaveFilterRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSaveFilterRelation_ProductCategoryParameterValue_ProductCategoryParameterValueId",
                        column: x => x.ProductCategoryParameterValueId,
                        principalTable: "ProductCategoryParameterValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSaveFilterRelation_UserSaveFilter_UserSaveFilterId",
                        column: x => x.UserSaveFilterId,
                        principalTable: "UserSaveFilter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSaveFilter_CategoryId",
                table: "UserSaveFilter",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaveFilterRelation_ProductCategoryParameterValueId",
                table: "UserSaveFilterRelation",
                column: "ProductCategoryParameterValueId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSaveFilterRelation_UserSaveFilterId",
                table: "UserSaveFilterRelation",
                column: "UserSaveFilterId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSaveFilter_ProductCategory_CategoryId",
                table: "UserSaveFilter",
                column: "CategoryId",
                principalTable: "ProductCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSaveFilter_ProductCategory_CategoryId",
                table: "UserSaveFilter");

            migrationBuilder.DropTable(
                name: "UserSaveFilterRelation");

            migrationBuilder.DropIndex(
                name: "IX_UserSaveFilter_CategoryId",
                table: "UserSaveFilter");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "UserSaveFilter");

            migrationBuilder.DropColumn(
                name: "FromAmount",
                table: "UserSaveFilter");

            migrationBuilder.DropColumn(
                name: "Search",
                table: "UserSaveFilter");

            migrationBuilder.DropColumn(
                name: "ToAmount",
                table: "UserSaveFilter");

            migrationBuilder.AddColumn<string>(
                name: "FilterValueJson",
                table: "UserSaveFilter",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
