using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnomalyDetection.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMLModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastTrainedAt",
                table: "MLModels",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "DecisionThreshold",
                table: "MLModels",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProduction",
                table: "MLModels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEvaluatedAt",
                table: "MLModels",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "MLModels",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfFeatures",
                table: "MLModels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingDataSize",
                table: "MLModels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainingParameters",
                table: "MLModels",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "MLModels",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                defaultValue: "1.0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecisionThreshold",
                table: "MLModels");

            migrationBuilder.DropColumn(
                name: "IsProduction",
                table: "MLModels");

            migrationBuilder.DropColumn(
                name: "LastEvaluatedAt",
                table: "MLModels");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "MLModels");

            migrationBuilder.DropColumn(
                name: "NumberOfFeatures",
                table: "MLModels");

            migrationBuilder.DropColumn(
                name: "TrainingDataSize",
                table: "MLModels");

            migrationBuilder.DropColumn(
                name: "TrainingParameters",
                table: "MLModels");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "MLModels");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastTrainedAt",
                table: "MLModels",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
