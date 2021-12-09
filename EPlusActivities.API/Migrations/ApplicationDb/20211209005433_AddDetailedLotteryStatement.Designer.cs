// <auto-generated />
using System;
using EPlusActivities.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EPlusActivities.API.Migrations.ApplicationDb
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20211209005433_AddDetailedLotteryStatement")]
    partial class AddDetailedLotteryStatement
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EPlusActivities.API.Entities.Activity", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ActivityCode")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("ActivityType")
                        .HasColumnType("int");

                    b.Property<string>("AvailableChannels")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Color")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("DailyDrawLimit")
                        .HasColumnType("int");

                    b.Property<int?>("DailyRedemptionLimit")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("Limit")
                        .HasColumnType("int");

                    b.Property<int>("LotteryDisplay")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("PrizeItemCount")
                        .HasColumnType("int");

                    b.Property<int?>("RequiredCreditForRedeeming")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartTime")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("ActivityCode")
                        .IsUnique();

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.ActivityUser", b =>
                {
                    b.Property<Guid?>("ActivityId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<int?>("AttendanceDays")
                        .HasColumnType("int");

                    b.Property<int>("Channel")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastAttendanceDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("RemainingDraws")
                        .HasColumnType("int");

                    b.Property<int?>("SequentialAttendanceDays")
                        .HasColumnType("int");

                    b.Property<int>("TodayUsedDraws")
                        .HasColumnType("int");

                    b.Property<int>("TodayUsedRedempion")
                        .HasColumnType("int");

                    b.Property<int>("UsedDraws")
                        .HasColumnType("int");

                    b.HasKey("ActivityId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ActivityUserLinks");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Address", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("City")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("DetailedAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Postcode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Province")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Recipient")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("RecipientPhoneNumber")
                        .HasColumnType("varchar(11) CHARACTER SET utf8mb4")
                        .HasMaxLength(11);

                    b.Property<string>("Region")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("UserId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Credit")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsMember")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("MemberId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.ApplicationUserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("RoleId1")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("UserId1")
                        .HasColumnType("char(36)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("RoleId1");

                    b.HasIndex("UserId1");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Attendance", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ActivityId")
                        .HasColumnType("char(36)");

                    b.Property<int>("ChannelCode")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Date")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<int>("EarnedCredits")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("UserId");

                    b.ToTable("AttendanceRecord");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Brand", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Category", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Coupon", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ActivityId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Code")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("PrizeItemId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Used")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("PrizeItemId");

                    b.HasIndex("UserId");

                    b.ToTable("Coupons");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Credit", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("MemberId")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("NewPoints")
                        .HasColumnType("int");

                    b.Property<int>("OldPoints")
                        .HasColumnType("int");

                    b.Property<int>("Points")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("RecordId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SheetId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("UpdateType")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasAlternateKey("SheetId");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.GeneralLotteryRecords", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ActivityId")
                        .IsRequired()
                        .HasColumnType("char(36)");

                    b.Property<int>("Channel")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Draws")
                        .HasColumnType("int");

                    b.Property<int>("Redemption")
                        .HasColumnType("int");

                    b.Property<int>("Winners")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasAlternateKey("ActivityId", "Channel", "DateTime");

                    b.ToTable("GeneralLotteryRecords");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Lottery", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ActivityId")
                        .HasColumnType("char(36)");

                    b.Property<int>("ChannelCode")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateTime")
                        .IsRequired()
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Delivered")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsLucky")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("LotteryDisplay")
                        .HasColumnType("int");

                    b.Property<bool>("PickedUp")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("PickedUpTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("PrizeItemId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("PrizeTierId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("PrizeItemId");

                    b.HasIndex("PrizeTierId");

                    b.HasIndex("UserId");

                    b.ToTable("LotteryResults");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.PrizeItem", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("BrandId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("CategoryId")
                        .HasColumnType("char(36)");

                    b.Property<string>("CouponActiveCode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("Credit")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("PrizeType")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("Stock")
                        .HasColumnType("int");

                    b.Property<decimal?>("UnitPrice")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.ToTable("PrizeItems");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.PrizeTier", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ActivityId")
                        .HasColumnType("char(36)");

                    b.Property<int?>("DailyLimit")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<double>("Percentage")
                        .HasColumnType("double");

                    b.Property<int>("RequiredCredit")
                        .HasColumnType("int");

                    b.Property<int>("TodayWinnerCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.ToTable("PrizeTiers");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.PrizeTierPrizeItem", b =>
                {
                    b.Property<Guid?>("PrizeTierId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("PrizeItemId")
                        .HasColumnType("char(36)");

                    b.HasKey("PrizeTierId", "PrizeItemId");

                    b.HasIndex("PrizeItemId");

                    b.ToTable("PrizeTierPrizeItems");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.ActivityUser", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.Activity", "Activity")
                        .WithMany("ActivityUserLinks")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", "User")
                        .WithMany("ActivityUserLinks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Address", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.ApplicationUserRole", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EPlusActivities.API.Entities.ApplicationRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId1");

                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Attendance", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId");

                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", "User")
                        .WithMany("AttendanceRecord")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Coupon", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.Activity", null)
                        .WithMany("Coupons")
                        .HasForeignKey("ActivityId");

                    b.HasOne("EPlusActivities.API.Entities.PrizeItem", "PrizeItem")
                        .WithMany("Coupons")
                        .HasForeignKey("PrizeItemId");

                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", "User")
                        .WithMany("Coupons")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.GeneralLotteryRecords", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.Lottery", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.Activity", "Activity")
                        .WithMany("LotteryResults")
                        .HasForeignKey("ActivityId");

                    b.HasOne("EPlusActivities.API.Entities.PrizeItem", "PrizeItem")
                        .WithMany("LotteryResults")
                        .HasForeignKey("PrizeItemId");

                    b.HasOne("EPlusActivities.API.Entities.PrizeTier", "PrizeTier")
                        .WithMany("LotteryResults")
                        .HasForeignKey("PrizeTierId");

                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", "User")
                        .WithMany("LotteryResults")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.PrizeItem", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.Brand", "Brand")
                        .WithMany("PrizeItems")
                        .HasForeignKey("BrandId");

                    b.HasOne("EPlusActivities.API.Entities.Category", "Category")
                        .WithMany("PrizeItems")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.PrizeTier", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.Activity", "Activity")
                        .WithMany("PrizeTiers")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EPlusActivities.API.Entities.PrizeTierPrizeItem", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.PrizeItem", "PrizeItem")
                        .WithMany("PrizeTierPrizeItems")
                        .HasForeignKey("PrizeItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EPlusActivities.API.Entities.PrizeTier", "PrizeTier")
                        .WithMany("PrizeTierPrizeItems")
                        .HasForeignKey("PrizeTierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("EPlusActivities.API.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
