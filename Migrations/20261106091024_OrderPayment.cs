using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalShop.Migrations
{
    /// <inheritdoc />
    public partial class OrderPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentAuthority",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PaymentRefId",
                table: "Orders",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentAuthority",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentRefId",
                table: "Orders");
        }
    }
}
