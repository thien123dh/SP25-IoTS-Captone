using System;
using System.Collections.Generic;
using CaptoneProject_IOTS_BOs.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class IoTTraddingSystemContext : DbContext
{
    private readonly string connectionString;
    public IoTTraddingSystemContext()
    {

    }

    public IoTTraddingSystemContext(DbContextOptions<IoTTraddingSystemContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Orders> Orders { set; get; }
    public virtual DbSet<OrderItem> OrderItems { set; get; }
    public virtual DbSet<CartItem> CartItems { set; get; }
    public virtual DbSet<Lab> Labs { set; get; }
    public virtual DbSet<Combo> Combos { set; get; }
    public virtual DbSet<IotsDevicesCombo> IotsDevicesCombos { set; get; }
    public virtual DbSet<AccountMembershipPackage> AccountMembershipPackages { set; get; }
    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
    public virtual DbSet<Attachment> Attachments { set; get; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogAttachment> BlogAttachments { get; set; }

    public virtual DbSet<BlogFeedback> BlogFeedbacks { get; set; }

    public virtual DbSet<BlogsCategory> BlogsCategories { get; set; }

    public virtual DbSet<IotsDevice> IotDevices { get; set; }

    public virtual DbSet<MaterialCategory> MaterialCategories { get; set; }
    public virtual DbSet<MembershipPackage> MembershipPackages { set; get; }

    public virtual DbSet<Notifications> Notifications { set; get; }

    public virtual DbSet<BusinessLicenses> BusinessLicenses { set; get; }
    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Store> Stores { get; set; }
    public virtual DbSet<TrainerBusinessLicense> TrainerBusinessLicenses { set; get; }
    public virtual DbSet<Transaction> Transactions { set; get; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRequest> UserRequests { get; set; }

    public virtual DbSet<UserRequestAttachment> UserRequestAttachments { get; set; }

    public virtual DbSet<UserRequestStatus> UserRequestStatuses { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Wallet> Wallets { set; get; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=iotsystem-db.c7yqwgmomb93.ap-southeast-2.rds.amazonaws.com;Uid=admin;Pwd=Iottradingsystem;Database=IoT_Tradding_System; TrustServerCertificate=True");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=iot-trading-system-db.database.windows.net;Uid=iot_trading_system_admin;Pwd=asdqwe@123;Database=IoT_Tradding_System;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.ToTable("ActivityLog");

            entity.Property(e => e.Id)
                //.ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Contents)
                .HasMaxLength(500)
                .HasColumnName("contents");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType).HasColumnName("entity_type");
            entity.Property(e => e.Metadata)
                .HasMaxLength(500)
                .HasColumnName("metadata");
            entity.Property(e => e.Title)
                .HasMaxLength(300)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.ToTable("Blog");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BlogContent)
                .HasMaxLength(1000)
                .HasColumnName("blog_content");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MetaData)
                .HasMaxLength(500)
                .HasColumnName("meta_data");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Category).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Blog_BlogsCategory");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Blog_Users");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BlogUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Blog_Users1");
        });

        modelBuilder.Entity<BlogAttachment>(entity =>
        {
            entity.ToTable("BlogAttachment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BlogAttachmentContent)
                .HasMaxLength(1000)
                .HasColumnName("blog_attachment_content");
            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.MetaData)
                .HasMaxLength(500)
                .HasColumnName("meta_data");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("video_url");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogAttachments)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_BlogAttachment_Blog");
        });

        modelBuilder.Entity<BlogFeedback>(entity =>
        {
            entity.ToTable("BlogFeedback");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(500)
                .HasColumnName("comment");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("rating");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogFeedbacks)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK_BlogFeedback_Blog");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BlogFeedbacks)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_BlogFeedback_Users");
        });

        modelBuilder.Entity<BlogsCategory>(entity =>
        {
            entity.ToTable("BlogsCategory");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Label)
                .HasMaxLength(200)
                .HasColumnName("label");
            entity.Property(e => e.Orders).HasColumnName("orders");
        });

        modelBuilder.Entity<IotsDevice>(entity =>
        {
            entity.ToTable("IotsDevices");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Connectivity)
                .HasMaxLength(200)
                .HasColumnName("connectivity");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.FirmwareVersion).HasColumnName("firmware_version");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(500)
                .HasColumnName("manufacturer");
            entity.Property(e => e.Model)
                .HasMaxLength(200)
                .HasColumnName("model");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .HasColumnName("name");
            entity.Property(e => e.PowerSource)
                .HasMaxLength(200)
                .HasColumnName("power_source");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(200)
                .HasColumnName("serial_number");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Category).WithMany(p => p.IotDevices)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_IotsDevices_MaterialCategory");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MaterialCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_IotsDevices_Users");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MaterialUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_IotsDevices_Users1");
        });

        modelBuilder.Entity<MaterialCategory>(entity =>
        {
            entity.ToTable("MaterialCategory");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Label)
                .HasMaxLength(200)
                .HasColumnName("label");
            entity.Property(e => e.Orders).HasColumnName("orders");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Label)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("label");
            entity.Property(e => e.Orders).HasColumnName("orders");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("Store");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Owner).WithMany(p => p.Stores)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Store_Users");
        });


        modelBuilder.Entity<StoreAttachment>()
            .HasOne(sa => sa.StoreNavigation)
            .WithMany(s => s.StoreAttachmentsNavigation)
            .HasForeignKey(sa => sa.StoreId);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(1000)
                .HasColumnName("address");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(300)
                .HasColumnName("fullname");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserRequest>(entity =>
        {
            entity.ToTable("UserRequest");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionBy).HasColumnName("action_by");
            entity.Property(e => e.ActionDate)
                .HasColumnType("datetime")
                .HasColumnName("action_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.ExpiredDate)
                .HasColumnType("datetime")
                .HasColumnName("expired_date");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("otp_code");
            entity.Property(e => e.Remark)
                .HasMaxLength(1000)
                .HasColumnName("remark");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRequests)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRequest_Role");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.UserRequests)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK_UserRequest_UserRequestStatus");
        });

        modelBuilder.Entity<UserRequestAttachment>(entity =>
        {
            entity.ToTable("UserRequestAttachment");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Contents)
                .HasMaxLength(1000)
                .HasColumnName("contents");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.MetaData)
                .HasMaxLength(1000)
                .HasColumnName("meta_data");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");
            entity.Property(e => e.UserRequestId).HasColumnName("user_request_id");

            entity.HasOne(d => d.UserRequest).WithMany(p => p.UserRequestAttachments)
                .HasForeignKey(d => d.UserRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRequestAttachment_UserRequest");
        });

        modelBuilder.Entity<UserRequestStatus>(entity =>
        {
            entity.ToTable("UserRequestStatus");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Label)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("label");
            entity.Property(e => e.Orders).HasColumnName("orders");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRole");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
