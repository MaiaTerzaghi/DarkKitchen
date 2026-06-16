using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkKitchen.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceDeliveryTypeWithShippingType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear la tabla ShippingTypes
            migrationBuilder.CreateTable(
                name: "ShippingTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingTypes", x => x.Id);
                });

            // 2. Seedear los tipos de envío existentes
            migrationBuilder.InsertData(
                table: "ShippingTypes",
                columns: new[] { "Name", "Cost" },
                values: new object[,]
                {
                    { "Express", 50.0 },
                    { "Standard", 20.0 }
                });

            // 3. Agregar columna ShippingTypeId con default 0
            migrationBuilder.AddColumn<int>(
                name: "ShippingTypeId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 4. Migrar datos: asignar ShippingTypeId según el DeliveryType viejo
            migrationBuilder.Sql(
                """
                UPDATE Orders SET ShippingTypeId = (SELECT Id FROM ShippingTypes WHERE Name = Orders.DeliveryType)
                WHERE EXISTS (SELECT 1 FROM ShippingTypes WHERE Name = Orders.DeliveryType)
                """);

            // 5. Borrar la columna vieja
            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "Orders");

            // 6. Crear índice y FK
            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingTypeId",
                table: "Orders",
                column: "ShippingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingTypes_ShippingTypeId",
                table: "Orders",
                column: "ShippingTypeId",
                principalTable: "ShippingTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShippingTypes_ShippingTypeId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "ShippingTypes");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingTypeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingTypeId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryType",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
