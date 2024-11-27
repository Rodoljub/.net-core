using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantum.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidatedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CLRTypes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLRTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CLRTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CLRTypes_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CLRTypes_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfFails = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Events_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileDetails",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageAnalysis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FileDetails_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileDetails_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileDetails_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileTypes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FileTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileTypes_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileTypes_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Folders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Folders_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Folders_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaveSearchResults",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SearchText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaveSearchResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaveSearchResults_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaveSearchResults_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaveSearchResults_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ResourceKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tags_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportedContentReasons",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    ResourceKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportedContentTypeID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportedContentReasons", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportedContentReasons_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportedContentReasons_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportedContentReasons_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportedContentReasons_CLRTypes_ReportedContentTypeID",
                        column: x => x.ReportedContentTypeID,
                        principalTable: "CLRTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    File = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    TypeId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    FolderId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileDetailsId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Files_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Files_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Files_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Files_FileDetails_FileDetailsId",
                        column: x => x.FileDetailsId,
                        principalTable: "FileDetails",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Files_FileTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "FileTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Files_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SaveSearchTags",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TagId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    SaveSearchResultsId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaveSearchTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SaveSearchTags_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaveSearchTags_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaveSearchTags_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaveSearchTags_SaveSearchResults_SaveSearchResultsId",
                        column: x => x.SaveSearchResultsId,
                        principalTable: "SaveSearchResults",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SaveSearchTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReportedContets",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ReportedId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedTypeID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    ReporterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReportedContentReasonId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportedContets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportedContets_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportedContets_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportedContets_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportedContets_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportedContets_CLRTypes_ReportedTypeID",
                        column: x => x.ReportedTypeID,
                        principalTable: "CLRTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReportedContets_ReportedContentReasons_ReportedContentReasonId",
                        column: x => x.ReportedContentReasonId,
                        principalTable: "ReportedContentReasons",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    UrlSegment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ImageFileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    TermsAndConditions = table.Column<bool>(type: "bit", nullable: false),
                    UploadsCount = table.Column<int>(type: "int", nullable: false),
                    CommentsCount = table.Column<int>(type: "int", nullable: false),
                    LikesCount = table.Column<int>(type: "int", nullable: false),
                    FavouritesCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FileID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    UserProfileId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CommentsCount = table.Column<int>(type: "int", nullable: false),
                    LikesCount = table.Column<int>(type: "int", nullable: false),
                    FavouritesCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Items_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_Files_FileID",
                        column: x => x.FileID,
                        principalTable: "Files",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Items_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    ParentTypeID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    UserProfileId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    ChildCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_CLRTypes_ParentTypeID",
                        column: x => x.ParentTypeID,
                        principalTable: "CLRTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Comments_Items_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Favourites",
                columns: table => new
                {
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityTypeID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    ItemID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favourites", x => new { x.EntityId, x.CreatedById });
                    table.ForeignKey(
                        name: "FK_Favourites_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favourites_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favourites_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favourites_CLRTypes_EntityTypeID",
                        column: x => x.EntityTypeID,
                        principalTable: "CLRTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Favourites_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ItemTags",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ItemId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    TagID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    Display = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ItemTags_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemTags_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemTags_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemTags_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ItemTags_Tags_TagID",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Views",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ItemId = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Views", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Views_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Views_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Views_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Views_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityTypeID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    ItemID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CommentID = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.EntityId, x.CreatedById });
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_CLRTypes_EntityTypeID",
                        column: x => x.EntityTypeID,
                        principalTable: "CLRTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Likes_Comments_CommentID",
                        column: x => x.CommentID,
                        principalTable: "Comments",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Likes_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CLRTypes_CreatedById",
                table: "CLRTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CLRTypes_DeletedById",
                table: "CLRTypes",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_CLRTypes_LastModifiedById",
                table: "CLRTypes",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DeletedById",
                table: "Comments",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LastModifiedById",
                table: "Comments",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentTypeID",
                table: "Comments",
                column: "ParentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserProfileId",
                table: "Comments",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedById",
                table: "Events",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Events_DeletedById",
                table: "Events",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LastModifiedById",
                table: "Events",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_CreatedById",
                table: "Favourites",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_DeletedById",
                table: "Favourites",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_EntityTypeID",
                table: "Favourites",
                column: "EntityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_ItemID",
                table: "Favourites",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_LastModifiedById",
                table: "Favourites",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetails_CreatedById",
                table: "FileDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetails_DeletedById",
                table: "FileDetails",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetails_LastModifiedById",
                table: "FileDetails",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CreatedById",
                table: "Files",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Files_DeletedById",
                table: "Files",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileDetailsId",
                table: "Files",
                column: "FileDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FolderId",
                table: "Files",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_LastModifiedById",
                table: "Files",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Files_TypeId",
                table: "Files",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_CreatedById",
                table: "FileTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_DeletedById",
                table: "FileTypes",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileTypes_LastModifiedById",
                table: "FileTypes",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_CreatedById",
                table: "Folders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_DeletedById",
                table: "Folders",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_LastModifiedById",
                table: "Folders",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedById",
                table: "Items",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_DeletedById",
                table: "Items",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_FileID",
                table: "Items",
                column: "FileID",
                unique: true,
                filter: "[FileID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Items_LastModifiedById",
                table: "Items",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserProfileId",
                table: "Items",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTags_CreatedById",
                table: "ItemTags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTags_DeletedById",
                table: "ItemTags",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTags_ItemId",
                table: "ItemTags",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTags_LastModifiedById",
                table: "ItemTags",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTags_TagID",
                table: "ItemTags",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_CommentID",
                table: "Likes",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_CreatedById",
                table: "Likes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_DeletedById",
                table: "Likes",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_EntityId",
                table: "Likes",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_EntityTypeID",
                table: "Likes",
                column: "EntityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ItemID",
                table: "Likes",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LastModifiedById",
                table: "Likes",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContentReasons_CreatedById",
                table: "ReportedContentReasons",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContentReasons_DeletedById",
                table: "ReportedContentReasons",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContentReasons_LastModifiedById",
                table: "ReportedContentReasons",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContentReasons_ReportedContentTypeID",
                table: "ReportedContentReasons",
                column: "ReportedContentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContets_CreatedById",
                table: "ReportedContets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContets_DeletedById",
                table: "ReportedContets",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContets_LastModifiedById",
                table: "ReportedContets",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContets_ReportedContentReasonId",
                table: "ReportedContets",
                column: "ReportedContentReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContets_ReportedTypeID",
                table: "ReportedContets",
                column: "ReportedTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedContets_ReporterId",
                table: "ReportedContets",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchResults_CreatedById",
                table: "SaveSearchResults",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchResults_DeletedById",
                table: "SaveSearchResults",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchResults_LastModifiedById",
                table: "SaveSearchResults",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchTags_CreatedById",
                table: "SaveSearchTags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchTags_DeletedById",
                table: "SaveSearchTags",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchTags_LastModifiedById",
                table: "SaveSearchTags",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchTags_SaveSearchResultsId",
                table: "SaveSearchTags",
                column: "SaveSearchResultsId");

            migrationBuilder.CreateIndex(
                name: "IX_SaveSearchTags_TagId",
                table: "SaveSearchTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedById",
                table: "Tags",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_LastModifiedById",
                table: "Tags",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_CreatedById",
                table: "UserNotifications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_DeletedById",
                table: "UserNotifications",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_LastModifiedById",
                table: "UserNotifications",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CreatedById",
                table: "UserProfiles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_DeletedById",
                table: "UserProfiles",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_FileId",
                table: "UserProfiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_LastModifiedById",
                table: "UserProfiles",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Views_CreatedById",
                table: "Views",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Views_DeletedById",
                table: "Views",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Views_ItemId",
                table: "Views",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Views_LastModifiedById",
                table: "Views",
                column: "LastModifiedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Favourites");

            migrationBuilder.DropTable(
                name: "ItemTags");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "ReportedContets");

            migrationBuilder.DropTable(
                name: "SaveSearchTags");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "Views");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ReportedContentReasons");

            migrationBuilder.DropTable(
                name: "SaveSearchResults");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "CLRTypes");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "FileDetails");

            migrationBuilder.DropTable(
                name: "FileTypes");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
