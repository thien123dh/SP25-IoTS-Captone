using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class IoTTraddingSystemContext : DbContext
{
    public IoTTraddingSystemContext()
    {
    }

    public IoTTraddingSystemContext(DbContextOptions<IoTTraddingSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogAttachment> BlogAttachments { get; set; }

    public virtual DbSet<BlogFeedback> BlogFeedbacks { get; set; }

    public virtual DbSet<BlogsCategory> BlogsCategories { get; set; }

    public virtual DbSet<Lab> Labs { get; set; }

    public virtual DbSet<LabDetail> LabDetails { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialCategory> MaterialCategories { get; set; }

    public virtual DbSet<MaterialGroup> MaterialGroups { get; set; }

    public virtual DbSet<MaterialGroupCategory> MaterialGroupCategories { get; set; }

    public virtual DbSet<MaterialGroupItem> MaterialGroupItems { get; set; }

    public virtual DbSet<ProductRequest> ProductRequests { get; set; }

    public virtual DbSet<ProductRequestStatus> ProductRequestStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRequest> UserRequests { get; set; }

    public virtual DbSet<UserRequestAttachment> UserRequestAttachments { get; set; }

    public virtual DbSet<UserRequestStatus> UserRequestStatuses { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=iotsystem-db.c7yqwgmomb93.ap-southeast-2.rds.amazonaws.com;Uid=admin;Pwd=Iottradingsystem;Database=IoT_Tradding_System; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.ToTable("ActivityLog");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.ToTable("Lab");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<LabDetail>(entity =>
        {
            entity.ToTable("LabDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.LabContent)
                .HasMaxLength(1000)
                .HasColumnName("lab_content");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.MetaData)
                .HasMaxLength(500)
                .IsFixedLength()
                .HasColumnName("meta_data");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("video_url");

            entity.HasOne(d => d.Lab).WithMany(p => p.LabDetails)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK_LabDetail_Lab");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Material");

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
            entity.Property(e => e.LastWarrentyDate)
                .HasColumnType("datetime")
                .HasColumnName("last_warrenty_date");
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
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(200)
                .HasColumnName("serial_number");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Category).WithMany(p => p.Materials)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Material_MaterialCategory");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MaterialCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Material_Users");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MaterialUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Material_Users1");
        });

        modelBuilder.Entity<MaterialCategory>(entity =>
        {
            entity.ToTable("MaterialCategory");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Label)
                .HasMaxLength(200)
                .HasColumnName("label");
            entity.Property(e => e.Orders).HasColumnName("orders");
        });

        modelBuilder.Entity<MaterialGroup>(entity =>
        {
            entity.ToTable("MaterialGroup");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.Category).WithMany(p => p.MaterialGroups)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_MaterialGroup_MaterialGroupCategory");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MaterialGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_MaterialGroup_Users");

            entity.HasOne(d => d.Lab).WithMany(p => p.MaterialGroups)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK_MaterialGroup_Lab");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.MaterialGroupUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_MaterialGroup_Users1");
        });

        modelBuilder.Entity<MaterialGroupCategory>(entity =>
        {
            entity.ToTable("MaterialGroupCategory");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Label)
                .HasMaxLength(200)
                .HasColumnName("label");
            entity.Property(e => e.Orders).HasColumnName("orders");
        });

        modelBuilder.Entity<MaterialGroupItem>(entity =>
        {
            entity.ToTable("MaterialGroupItem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");

            entity.HasOne(d => d.Group).WithMany(p => p.MaterialGroupItems)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_MaterialGroupItem_MaterialGroup");

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialGroupItems)
                .HasForeignKey(d => d.MaterialId)
                .HasConstraintName("FK_MaterialGroupItem_Material");
        });

        modelBuilder.Entity<ProductRequest>(entity =>
        {
            entity.ToTable("ProductRequest");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionBy).HasColumnName("action_by");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductType).HasColumnName("product_type");
            entity.Property(e => e.Remark)
                .HasMaxLength(500)
                .HasColumnName("remark");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductRequestCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_ProductRequest_Users");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.ProductRequests)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK_ProductRequest_ProductRequestStatus");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ProductRequestUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_ProductRequest_Users1");
        });

        modelBuilder.Entity<ProductRequestStatus>(entity =>
        {
            entity.ToTable("ProductRequestStatus");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
                .HasMaxLength(10)
                .IsFixedLength()
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
