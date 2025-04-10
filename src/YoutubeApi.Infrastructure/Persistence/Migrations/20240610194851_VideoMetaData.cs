using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutubeDataApiClient.Migrations
{
    /// <inheritdoc />
    public partial class VideoMetaData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelId",
                table: "Videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChannelTitle",
                table: "Videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DislikeCount",
                table: "Videos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Videos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Videos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "Videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Videos",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ChannelTitle",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "DislikeCount",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Videos");
        }
    }
}
