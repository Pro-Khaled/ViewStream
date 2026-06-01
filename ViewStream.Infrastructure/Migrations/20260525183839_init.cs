using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViewStream.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Awards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Year = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Awards__3214EC07C9780B76", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ContentT__3214EC074F9C8EA7", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Continent = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Countrie__A25C5AA6B7190258", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Genres__3214EC07330D4828", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permissi__3214EC07E423F9A3", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Persons__3214EC07AA1ED38C", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountPercent = table.Column<short>(type: "smallint", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: true),
                    MaxUses = table.Column<int>(type: "int", nullable: true),
                    UsedCount = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    AppliesToPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PromoCod__3214EC073F066A55", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__3214EC07A79792D4", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseYear = table.Column<short>(type: "smallint", nullable: true),
                    MaturityRating = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RuntimeMinutes = table.Column<short>(type: "smallint", nullable: true),
                    PosterUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BackdropUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrailerUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImdbRating = table.Column<decimal>(type: "decimal(3,1)", nullable: true),
                    RottenTomatoesScore = table.Column<short>(type: "smallint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shows__3214EC07209A9206", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CountryCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BlockedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BlockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC077A0132A2", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonAwards",
                columns: table => new
                {
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    AwardId = table.Column<int>(type: "int", nullable: false),
                    Won = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PersonAw__512768BA576C8F43", x => new { x.PersonId, x.AwardId });
                    table.ForeignKey(
                        name: "FK__PersonAwa__Award__2CBDA3B5",
                        column: x => x.AwardId,
                        principalTable: "Awards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__PersonAwa__Perso__2BC97F7C",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RolePerm__6400A1A86994377D", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK__RolePermi__Permi__6383C8BA",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__RolePermi__RoleI__628FA481",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemVectors",
                columns: table => new
                {
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    EmbeddingJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ItemVect__6DE3E0B23338B579", x => x.ShowId);
                    table.ForeignKey(
                        name: "FK__ItemVecto__ShowI__51300E55",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonNumber = table.Column<short>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seasons__3214EC07B3E751F2", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Seasons__ShowId__0F624AF8",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShowAvailability",
                columns: table => new
                {
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    CountryCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    AvailableFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    AvailableUntil = table.Column<DateOnly>(type: "date", nullable: true),
                    LicensingNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ShowAvai__A83A5060A3C22196", x => new { x.ShowId, x.CountryCode });
                    table.ForeignKey(
                        name: "FK__ShowAvail__Count__3D2915A8",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK__ShowAvail__ShowI__3C34F16F",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShowAwards",
                columns: table => new
                {
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    AwardId = table.Column<int>(type: "int", nullable: false),
                    Won = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ShowAwar__96EB73EDF758669B", x => new { x.ShowId, x.AwardId });
                    table.ForeignKey(
                        name: "FK__ShowAward__Award__27F8EE98",
                        column: x => x.AwardId,
                        principalTable: "Awards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ShowAward__ShowI__2704CA5F",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShowGenres",
                columns: table => new
                {
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ShowGenr__9DDBB0E544383F30", x => new { x.ShowId, x.GenreId });
                    table.ForeignKey(
                        name: "FK__ShowGenre__Genre__0B91BA14",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ShowGenre__ShowI__0A9D95DB",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowTags",
                columns: table => new
                {
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ShowTags__BBB42F28EAFA6B28", x => new { x.ShowId, x.TagId });
                    table.ForeignKey(
                        name: "FK__ShowTags__ShowId__214BF109",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ShowTags__TagId__22401542",
                        column: x => x.TagId,
                        principalTable: "ContentTags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecordId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__3214EC07B9120EC7", x => x.Id);
                    table.ForeignKey(
                        name: "FK__AuditLogs__Chang__53D770D6",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DataDeletionRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DataDele__3214EC07DE48CE49", x => x.Id);
                    table.ForeignKey(
                        name: "FK__DataDelet__UserI__4D2A7347",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Platform = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LastActive = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    IsTrusted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Devices__3214EC0742FF21B1", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Devices__UserId__318258D2",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailPreferences",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    MarketingEmails = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    NewReleaseAlerts = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    RecommendationEmails = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    AccountUpdates = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmailPre__1788CC4CE42853B1", x => x.UserId);
                    table.ForeignKey(
                        name: "FK__EmailPref__UserI__69FBBC1F",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ErrorLog__3214EC07B6FEA7DA", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ErrorLogs__UserI__19AACF41",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    FriendId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Friendsh__3DA43A14362737CC", x => new { x.UserId, x.FriendId });
                    table.ForeignKey(
                        name: "FK__Friendshi__Frien__719CDDE7",
                        column: x => x.FriendId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Friendshi__UserI__70A8B9AE",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__3214EC07DB93714A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__5F7E2DAC",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProviderToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastFour = table.Column<string>(type: "char(4)", unicode: false, fixedLength: true, maxLength: 4, nullable: true),
                    CardType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ExpiryMonth = table.Column<short>(type: "smallint", nullable: true),
                    ExpiryYear = table.Column<short>(type: "smallint", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentM__3214EC07B8E7EE8B", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PaymentMe__UserI__6E01572D",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsKids = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    AvatarIcon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LanguagePref = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "en"),
                    MaturityLevel = table.Column<short>(type: "smallint", nullable: true, defaultValue: (short)18),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Profiles__3214EC07DF6515A8", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Profiles__UserId__4316F928",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PushTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PushToke__3214EC076D132F55", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PushToken__UserI__65370702",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PlanType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "active"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    PaymentMethodId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrip__3214EC070211634A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Subscript__UserI__66603565",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPromoUsage",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PromoCodeId = table.Column<int>(type: "int", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserProm__6FEF70141EA1457A", x => new { x.UserId, x.PromoCodeId });
                    table.ForeignKey(
                        name: "FK__UserPromo__Promo__00200768",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__UserPromo__UserI__7F2BE32F",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    EpisodeNumber = table.Column<short>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuntimeSeconds = table.Column<int>(type: "int", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Episodes__3214EC07465BA3F1", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Episodes__Season__151B244E",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoginSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<long>(type: "bigint", nullable: true),
                    SessionToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoginSes__3214EC074B8D7BC6", x => x.Id);
                    table.ForeignKey(
                        name: "FK__LoginSess__Devic__39237A9A",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__LoginSess__UserI__382F5661",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PersonalizedRows",
                columns: table => new
                {
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    RowName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShowIdsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Personal__B84DEF6EE6F7535A", x => new { x.ProfileId, x.RowName });
                    table.ForeignKey(
                        name: "FK__Personali__Profi__5BAD9CC8",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ratings__1FD2B6EFF6B59EB8", x => new { x.ProfileId, x.ShowId });
                    table.ForeignKey(
                        name: "FK__Ratings__Profile__2A164134",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Ratings__ShowId__2B0A656D",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SearchLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: true),
                    Query = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ResultsCount = table.Column<int>(type: "int", nullable: true),
                    ClickedShowId = table.Column<long>(type: "bigint", nullable: true),
                    SearchAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SearchLo__3214EC07D1C0389A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__SearchLog__Click__10216507",
                        column: x => x.ClickedShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__SearchLog__Profi__0F2D40CE",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SharedLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerProfileId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ShareCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SharedLi__3214EC07E1B7AD48", x => x.Id);
                    table.ForeignKey(
                        name: "FK__SharedLis__Owner__03BB8E22",
                        column: x => x.OwnerProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserInteractions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    InteractionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,3)", nullable: true, defaultValue: 1.0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserInte__3214EC07FAB3383B", x => x.Id);
                    table.ForeignKey(
                        name: "FK__UserInter__Profi__55009F39",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__UserInter__ShowI__55F4C372",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserLibrary",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: true),
                    SeasonId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EpisodesWatched = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    UserScore = table.Column<decimal>(type: "decimal(2,1)", nullable: true),
                    StartedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CompletedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserLibr__3214EC0740FEA656", x => x.Id);
                    table.ForeignKey(
                        name: "FK__UserLibra__Profi__30C33EC3",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__UserLibra__Seaso__32AB8735",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__UserLibra__ShowI__31B762FC",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserVectors",
                columns: table => new
                {
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    EmbeddingJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserVect__290C88E4B9482E78", x => x.ProfileId);
                    table.ForeignKey(
                        name: "FK__UserVecto__Profi__4D5F7D71",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    SubscriptionId = table.Column<long>(type: "bigint", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true, defaultValue: "USD"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending"),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoicePdfUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoices__3214EC07A0456B7C", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Invoices__Subscr__74AE54BC",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Invoices__UserId__73BA3083",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AudioTracks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    TrackType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "dub"),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AudioTra__3214EC0723BC2D93", x => x.Id);
                    table.ForeignKey(
                        name: "FK__AudioTrac__Episo__40F9A68C",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContentReports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: true),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending"),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ContentR__3214EC07C86474D5", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ContentRe__Episo__4589517F",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ContentRe__Profi__43A1090D",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ContentRe__ShowI__44952D46",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Credits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: true),
                    SeasonId = table.Column<long>(type: "bigint", nullable: true),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CharacterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Credits__3214EC0777517A30", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Credits__Episode__1EA48E88",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Credits__PersonI__1BC821DD",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Credits__SeasonI__1DB06A4F",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Credits__ShowId__1CBC4616",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EpisodeComments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ParentCommentId = table.Column<long>(type: "bigint", nullable: true),
                    CommentText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EpisodeC__3214EC0736497BE3", x => x.Id);
                    table.ForeignKey(
                        name: "FK__EpisodeCo__Episo__57A801BA",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__EpisodeCo__Paren__59904A2C",
                        column: x => x.ParentCommentId,
                        principalTable: "EpisodeComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__EpisodeCo__Profi__589C25F3",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OfflineDownloads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    DownloadQuality = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DownloadedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OfflineD__3214EC07DD5935AA", x => x.Id);
                    table.ForeignKey(
                        name: "FK__OfflineDo__Devic__3FD07829",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__OfflineDo__Episo__3EDC53F0",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__OfflineDo__Profi__3DE82FB7",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlaybackEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PositionSeconds = table.Column<int>(type: "int", nullable: true),
                    Quality = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BitrateKbps = table.Column<int>(type: "int", nullable: true),
                    BufferingCount = table.Column<int>(type: "int", nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Playback__3214EC0739FBD2F6", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PlaybackE__Episo__14E61A24",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__PlaybackE__Profi__13F1F5EB",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subtitles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    SubtitleUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsCc = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subtitle__3214EC07B002A42C", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Subtitles__Episo__489AC854",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WatchHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    WatchedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    ProgressSeconds = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Completed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WatchHis__3214EC0797BF91EB", x => x.Id);
                    table.ForeignKey(
                        name: "FK__WatchHist__Episo__245D67DE",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__WatchHist__Profi__236943A5",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WatchParties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostProfileId = table.Column<long>(type: "bigint", nullable: false),
                    EpisodeId = table.Column<long>(type: "bigint", nullable: false),
                    PartyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WatchPar__3214EC07164BE78C", x => x.Id);
                    table.ForeignKey(
                        name: "FK__WatchPart__Episo__793DFFAF",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__WatchPart__HostP__7849DB76",
                        column: x => x.HostProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SharedListItems",
                columns: table => new
                {
                    ListId = table.Column<long>(type: "bigint", nullable: false),
                    ShowId = table.Column<long>(type: "bigint", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    AddedByProfileId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SharedLi__D55D160EBAF96012", x => new { x.ListId, x.ShowId });
                    table.ForeignKey(
                        name: "FK__SharedLis__Added__0C50D423",
                        column: x => x.AddedByProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__SharedLis__ListI__09746778",
                        column: x => x.ListId,
                        principalTable: "SharedLists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__SharedLis__ShowI__0A688BB1",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                columns: table => new
                {
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    ReactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "like"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CommentL__91241744C80FEE26", x => new { x.CommentId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK__CommentLi__Comme__5F492382",
                        column: x => x.CommentId,
                        principalTable: "EpisodeComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__CommentLi__Profi__603D47BB",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommentReports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    ReportedByProfileId = table.Column<long>(type: "bigint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending"),
                    ReviewedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CommentR__3214EC07219A44C7", x => x.Id);
                    table.ForeignKey(
                        name: "FK__CommentRe__Comme__65F62111",
                        column: x => x.CommentId,
                        principalTable: "EpisodeComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__CommentRe__Repor__66EA454A",
                        column: x => x.ReportedByProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__CommentRe__Revie__6ABAD62E",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WatchPartyParticipants",
                columns: table => new
                {
                    PartyId = table.Column<long>(type: "bigint", nullable: false),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getutcdate())"),
                    LeftAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WatchPar__44D005BD3AF7CC60", x => new { x.PartyId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK__WatchPart__Party__7E02B4CC",
                        column: x => x.PartyId,
                        principalTable: "WatchParties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__WatchPart__Profi__7EF6D905",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "UQ_AudioTracks_EpisodeId_LanguageCode_TrackType",
                table: "AudioTracks",
                columns: new[] { "EpisodeId", "LanguageCode", "TrackType" },
                unique: true,
                filter: "[TrackType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedAt",
                table: "AuditLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedByUserId",
                table: "AuditLogs",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TableName_RecordId",
                table: "AuditLogs",
                columns: new[] { "TableName", "RecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_ProfileId",
                table: "CommentLikes",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_CommentId",
                table: "CommentReports",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReportedByProfileId",
                table: "CommentReports",
                column: "ReportedByProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReviewedByUserId",
                table: "CommentReports",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_Status",
                table: "CommentReports",
                column: "Status",
                filter: "([Status]='pending')");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_EpisodeId",
                table: "ContentReports",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ProfileId",
                table: "ContentReports",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ShowId",
                table: "ContentReports",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "UQ__ContentT__737584F6B9A5A42E",
                table: "ContentTags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Credits_EpisodeId",
                table: "Credits",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_PersonId",
                table: "Credits",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_SeasonId",
                table: "Credits",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_ShowId",
                table: "Credits",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_DataDeletionRequests_UserId",
                table: "DataDeletionRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_Devices_UserId_DeviceId",
                table: "Devices",
                columns: new[] { "UserId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeComments_EpisodeId",
                table: "EpisodeComments",
                column: "EpisodeId",
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeComments_ParentCommentId",
                table: "EpisodeComments",
                column: "ParentCommentId",
                filter: "([ParentCommentId] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_EpisodeComments_ProfileId",
                table: "EpisodeComments",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_IsDeleted",
                table: "Episodes",
                column: "IsDeleted",
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UQ_Episodes_SeasonId_EpisodeNumber",
                table: "Episodes",
                columns: new[] { "SeasonId", "EpisodeNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_UserId",
                table: "ErrorLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendId",
                table: "Friendships",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "UQ__Genres__737584F6BAE7C6A3",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SubscriptionId",
                table: "Invoices",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId",
                table: "Invoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginSessions_DeviceId",
                table: "LoginSessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginSessions_UserId",
                table: "LoginSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__LoginSes__46BDD124D09EBF84",
                table: "LoginSessions",
                column: "SessionToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OfflineDownloads_DeviceId",
                table: "OfflineDownloads",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_OfflineDownloads_EpisodeId",
                table: "OfflineDownloads",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "UQ_OfflineDownloads_ProfileId_EpisodeId_DeviceId",
                table: "OfflineDownloads",
                columns: new[] { "ProfileId", "EpisodeId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UserId",
                table: "PaymentMethods",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__Permissi__737584F69EDD8FB7",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonAwards_AwardId",
                table: "PersonAwards",
                column: "AwardId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackEvents_EpisodeId",
                table: "PlaybackEvents",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackEvents_ProfileId",
                table: "PlaybackEvents",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_IsDeleted",
                table: "Profiles",
                column: "IsDeleted",
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UQ_Profiles_UserId_Name",
                table: "Profiles",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__PromoCod__A25C5AA7AA56D5B1",
                table: "PromoCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_PushTokens_UserId_DeviceId",
                table: "PushTokens",
                columns: new[] { "UserId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ShowId",
                table: "Ratings",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NormalizedName",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__737584F6F8E85D53",
                table: "Roles",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_ClickedShowId",
                table: "SearchLogs",
                column: "ClickedShowId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_ProfileId",
                table: "SearchLogs",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_IsDeleted",
                table: "Seasons",
                column: "IsDeleted",
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "UQ_Seasons_ShowId_SeasonNumber",
                table: "Seasons",
                columns: new[] { "ShowId", "SeasonNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SharedListItems_AddedByProfileId",
                table: "SharedListItems",
                column: "AddedByProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedListItems_ShowId",
                table: "SharedListItems",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedLists_OwnerProfileId",
                table: "SharedLists",
                column: "OwnerProfileId");

            migrationBuilder.CreateIndex(
                name: "UQ__SharedLi__20877041F7CA4F8F",
                table: "SharedLists",
                column: "ShareCode",
                unique: true,
                filter: "[ShareCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShowAvailability_CountryCode",
                table: "ShowAvailability",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_ShowAwards_AwardId",
                table: "ShowAwards",
                column: "AwardId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenres_GenreId",
                table: "ShowGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_IsDeleted",
                table: "Shows",
                column: "IsDeleted",
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_ShowTags_TagId",
                table: "ShowTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_Subtitles_EpisodeId_LanguageCode",
                table: "Subtitles",
                columns: new[] { "EpisodeId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_ProfileId",
                table: "UserInteractions",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractions_ShowId",
                table: "UserInteractions",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibrary_ProfileId_Status",
                table: "UserLibrary",
                columns: new[] { "ProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLibrary_SeasonId",
                table: "UserLibrary",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibrary_ShowId",
                table: "UserLibrary",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserLibrary_ProfileId_Target",
                table: "UserLibrary",
                columns: new[] { "ProfileId", "ShowId", "SeasonId" },
                unique: true,
                filter: "[ShowId] IS NOT NULL AND [SeasonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPromoUsage_PromoCodeId",
                table: "UserPromoUsage",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive",
                filter: "([IsActive]=(1))");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted_Email",
                table: "Users",
                columns: new[] { "IsDeleted", "Email" },
                filter: "([IsDeleted]=(0))");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistory_EpisodeId",
                table: "WatchHistory",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistory_ProfileId",
                table: "WatchHistory",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchParties_EpisodeId",
                table: "WatchParties",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchParties_HostProfileId",
                table: "WatchParties",
                column: "HostProfileId");

            migrationBuilder.CreateIndex(
                name: "UQ__WatchPar__39A9713DF754EFCA",
                table: "WatchParties",
                column: "PartyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WatchPartyParticipants_ProfileId",
                table: "WatchPartyParticipants",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioTracks");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.DropTable(
                name: "CommentReports");

            migrationBuilder.DropTable(
                name: "ContentReports");

            migrationBuilder.DropTable(
                name: "Credits");

            migrationBuilder.DropTable(
                name: "DataDeletionRequests");

            migrationBuilder.DropTable(
                name: "EmailPreferences");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "ItemVectors");

            migrationBuilder.DropTable(
                name: "LoginSessions");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OfflineDownloads");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "PersonalizedRows");

            migrationBuilder.DropTable(
                name: "PersonAwards");

            migrationBuilder.DropTable(
                name: "PlaybackEvents");

            migrationBuilder.DropTable(
                name: "PushTokens");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SearchLogs");

            migrationBuilder.DropTable(
                name: "SharedListItems");

            migrationBuilder.DropTable(
                name: "ShowAvailability");

            migrationBuilder.DropTable(
                name: "ShowAwards");

            migrationBuilder.DropTable(
                name: "ShowGenres");

            migrationBuilder.DropTable(
                name: "ShowTags");

            migrationBuilder.DropTable(
                name: "Subtitles");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserInteractions");

            migrationBuilder.DropTable(
                name: "UserLibrary");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserPromoUsage");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "UserVectors");

            migrationBuilder.DropTable(
                name: "WatchHistory");

            migrationBuilder.DropTable(
                name: "WatchPartyParticipants");

            migrationBuilder.DropTable(
                name: "EpisodeComments");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "SharedLists");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Awards");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "ContentTags");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "WatchParties");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Shows");
        }
    }
}
