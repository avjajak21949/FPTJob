using Microsoft.EntityFrameworkCore.Migrations;

namespace FPTJob.Data.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CV",
                columns: table => new
                {
                    CVID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    file = table.Column<string>(nullable: true),
                    ApplicantId = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CV", x => x.CVID);
                    table.ForeignKey(
                        name: "FK_CV_AspNetUsers_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Salary = table.Column<int>(nullable: false),
                    Place = table.Column<string>(nullable: false),
                    Time = table.Column<string>(nullable: false),
                    CategoryID = table.Column<int>(nullable: false),
                    EmployerId = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Job_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Job_AspNetUsers_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobCV",
                columns: table => new
                {
                    JobCVID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(nullable: false),
                    CVID = table.Column<int>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCV", x => x.JobCVID);
                    table.ForeignKey(
                        name: "FK_JobCV_CV_CVID",
                        column: x => x.CVID,
                        principalTable: "CV",
                        principalColumn: "CVID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobCV_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "role0", "9735cf3d-5609-4020-a607-b986d2b5b587", "Adminstator", "ADMINSTRATOR" },
                    { "role1", "44619ae4-2c14-4750-8ec6-1d07e146e370", "Jobseeker", "JOBSEEKER" },
                    { "role2", "e6b69631-879b-4127-aa7d-8b3b050885bb", "Employer", "EMPLOYER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "user0", 0, "e1e1b64f-c025-4017-a5cc-d5057988ad92", "admin@gmail.com", true, false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAEAACcQAAAAEEvwqPu/BKOpmZyypLCT3kys1/roFdEQCXdQLtpYFiQojLL1tZKtO1avez8RdCtRfw==", null, false, "ca24bcb2-7130-4efc-8529-9b8dce22f70b", false, "admin@gmail.com" },
                    { "user1", 0, "b0829e2c-0498-4916-9e8e-5b0579b118a5", "jobseeker@gmail.com", true, false, null, "JOBSEEKER@GMAIL.COM", "JOBSEEKER@GMAIL.COM", "AQAAAAEAACcQAAAAEPGYeeLp6UMYIXwEaTMp8gLOdmUPhKUaW0LxuSTx2AYfxtxFJoH1ORkeQn9rziucGQ==", null, false, "c236dc3b-0034-459f-b219-c5c3e2ee4ff4", false, "jobseeker@gmail.com" },
                    { "user2", 0, "33dca589-e5a6-415b-89c4-7032c9c79bda", "employer@gmail.com", true, false, null, "EMPLOYER@GMAIL.COM", "EMPLOYER@GMAIL.COM", "AQAAAAEAACcQAAAAENOAtWWvbb79Wm9wEzWcFfupW56n20RfAZ+tf0XE88ItJgo1DbGVFpqFXWBQQlfzyA==", null, false, "c1f1e627-ff6e-4e5c-bf10-b705e8758153", false, "employer@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "user0", "role0" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "user1", "role1" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "user2", "role2" });

            migrationBuilder.CreateIndex(
                name: "IX_CV_ApplicantId",
                table: "CV",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Job_CategoryID",
                table: "Job",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Job_EmployerId",
                table: "Job",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCV_CVID",
                table: "JobCV",
                column: "CVID");

            migrationBuilder.CreateIndex(
                name: "IX_JobCV_JobId",
                table: "JobCV",
                column: "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobCV");

            migrationBuilder.DropTable(
                name: "CV");

            migrationBuilder.DropTable(
                name: "Job");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "user0", "role0" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "user1", "role1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "user2", "role2" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user0");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user2");
        }
    }
}
