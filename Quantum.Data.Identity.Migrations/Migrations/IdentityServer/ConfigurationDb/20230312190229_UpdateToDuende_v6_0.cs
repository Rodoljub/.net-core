﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantum.Data.Identity.Migrations.Migrations.IdentityServer.ConfigurationDb
{
    /// <inheritdoc />
    public partial class UpdateToDuende_v6_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                table: "IdentityResourceProperties");

            migrationBuilder.DropIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId",
                table: "IdentityResourceClaims");

            migrationBuilder.DropIndex(
                name: "IX_ClientScopes_ClientId",
                table: "ClientScopes");

            migrationBuilder.DropIndex(
                name: "IX_ClientRedirectUris_ClientId",
                table: "ClientRedirectUris");

            migrationBuilder.DropIndex(
                name: "IX_ClientProperties_ClientId",
                table: "ClientProperties");

            migrationBuilder.DropIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId",
                table: "ClientPostLogoutRedirectUris");

            migrationBuilder.DropIndex(
                name: "IX_ClientIdPRestrictions_ClientId",
                table: "ClientIdPRestrictions");

            migrationBuilder.DropIndex(
                name: "IX_ClientGrantTypes_ClientId",
                table: "ClientGrantTypes");

            migrationBuilder.DropIndex(
                name: "IX_ClientCorsOrigins_ClientId",
                table: "ClientCorsOrigins");

            migrationBuilder.DropIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaims");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeProperties_ScopeId",
                table: "ApiScopeProperties");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceScopes_ApiResourceId",
                table: "ApiResourceScopes");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                table: "ApiResourceProperties");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceClaims_ApiResourceId",
                table: "ApiResourceClaims");

            migrationBuilder.AddColumn<int>(
                name: "CibaLifetime",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CoordinateLifetimeWithUserSession",
                table: "Clients",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PollingInterval",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RedirectUri",
                table: "ClientRedirectUris",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ApiScopes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                table: "ApiScopes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "ApiScopes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "ApiScopes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireResourceIndicator",
                table: "ApiResources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "IdentityProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scheme = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NonEditable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProviders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId_Key",
                table: "IdentityResourceProperties",
                columns: new[] { "IdentityResourceId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId_Type",
                table: "IdentityResourceClaims",
                columns: new[] { "IdentityResourceId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientScopes_ClientId_Scope",
                table: "ClientScopes",
                columns: new[] { "ClientId", "Scope" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientRedirectUris_ClientId_RedirectUri",
                table: "ClientRedirectUris",
                columns: new[] { "ClientId", "RedirectUri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientProperties_ClientId_Key",
                table: "ClientProperties",
                columns: new[] { "ClientId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris",
                columns: new[] { "ClientId", "PostLogoutRedirectUri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientIdPRestrictions_ClientId_Provider",
                table: "ClientIdPRestrictions",
                columns: new[] { "ClientId", "Provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId_GrantType",
                table: "ClientGrantTypes",
                columns: new[] { "ClientId", "GrantType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientCorsOrigins_ClientId_Origin",
                table: "ClientCorsOrigins",
                columns: new[] { "ClientId", "Origin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientClaims_ClientId_Type_Value",
                table: "ClientClaims",
                columns: new[] { "ClientId", "Type", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperties_ScopeId_Key",
                table: "ApiScopeProperties",
                columns: new[] { "ScopeId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId_Type",
                table: "ApiScopeClaims",
                columns: new[] { "ScopeId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ApiResourceId_Scope",
                table: "ApiResourceScopes",
                columns: new[] { "ApiResourceId", "Scope" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceProperties_ApiResourceId_Key",
                table: "ApiResourceProperties",
                columns: new[] { "ApiResourceId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceClaims_ApiResourceId_Type",
                table: "ApiResourceClaims",
                columns: new[] { "ApiResourceId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProviders_Scheme",
                table: "IdentityProviders",
                column: "Scheme",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityProviders");

            migrationBuilder.DropIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId_Key",
                table: "IdentityResourceProperties");

            migrationBuilder.DropIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId_Type",
                table: "IdentityResourceClaims");

            migrationBuilder.DropIndex(
                name: "IX_ClientScopes_ClientId_Scope",
                table: "ClientScopes");

            migrationBuilder.DropIndex(
                name: "IX_ClientRedirectUris_ClientId_RedirectUri",
                table: "ClientRedirectUris");

            migrationBuilder.DropIndex(
                name: "IX_ClientProperties_ClientId_Key",
                table: "ClientProperties");

            migrationBuilder.DropIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris");

            migrationBuilder.DropIndex(
                name: "IX_ClientIdPRestrictions_ClientId_Provider",
                table: "ClientIdPRestrictions");

            migrationBuilder.DropIndex(
                name: "IX_ClientGrantTypes_ClientId_GrantType",
                table: "ClientGrantTypes");

            migrationBuilder.DropIndex(
                name: "IX_ClientCorsOrigins_ClientId_Origin",
                table: "ClientCorsOrigins");

            migrationBuilder.DropIndex(
                name: "IX_ClientClaims_ClientId_Type_Value",
                table: "ClientClaims");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeProperties_ScopeId_Key",
                table: "ApiScopeProperties");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId_Type",
                table: "ApiScopeClaims");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceScopes_ApiResourceId_Scope",
                table: "ApiResourceScopes");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceProperties_ApiResourceId_Key",
                table: "ApiResourceProperties");

            migrationBuilder.DropIndex(
                name: "IX_ApiResourceClaims_ApiResourceId_Type",
                table: "ApiResourceClaims");

            migrationBuilder.DropColumn(
                name: "CibaLifetime",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CoordinateLifetimeWithUserSession",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PollingInterval",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "RequireResourceIndicator",
                table: "ApiResources");

            migrationBuilder.AlterColumn<string>(
                name: "RedirectUri",
                table: "ClientRedirectUris",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<string>(
                name: "PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                table: "IdentityResourceProperties",
                column: "IdentityResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId",
                table: "IdentityResourceClaims",
                column: "IdentityResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientScopes_ClientId",
                table: "ClientScopes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRedirectUris_ClientId",
                table: "ClientRedirectUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProperties_ClientId",
                table: "ClientProperties",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId",
                table: "ClientPostLogoutRedirectUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientIdPRestrictions_ClientId",
                table: "ClientIdPRestrictions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId",
                table: "ClientGrantTypes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCorsOrigins_ClientId",
                table: "ClientCorsOrigins",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaims",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperties_ScopeId",
                table: "ApiScopeProperties",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ApiResourceId",
                table: "ApiResourceScopes",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                table: "ApiResourceProperties",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceClaims_ApiResourceId",
                table: "ApiResourceClaims",
                column: "ApiResourceId");
        }
    }
}
