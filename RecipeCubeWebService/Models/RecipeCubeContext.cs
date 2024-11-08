﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RecipeCubeWebService.Models;

public partial class RecipeCubeContext : DbContext
{
    public RecipeCubeContext(DbContextOptions<RecipeCubeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<ExclusiveIngredient> ExclusiveIngredients { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PantryManagement> PantryManagements { get; set; }

    public virtual DbSet<PreferedIngredient> PreferedIngredients { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductEvaluate> ProductEvaluates { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserCoupon> UserCoupons { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<UserIdMapping> UserIdMappings { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.CouponName)
                .HasMaxLength(50)
                .HasColumnName("coupon_name");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(50)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount_value");
            entity.Property(e => e.MinSpend).HasColumnName("minSpend");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<ExclusiveIngredient>(entity =>
        {
            entity.ToTable("Exclusive_Ingredients");

            entity.Property(e => e.ExclusiveIngredientId).HasColumnName("exclusive_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.ExpireDay).HasColumnName("expire_day");
            entity.Property(e => e.Gram)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("gram");
            entity.Property(e => e.IngredientName)
                .HasMaxLength(255)
                .HasColumnName("ingredient_name");
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasColumnName("photo");
            entity.Property(e => e.Synonym)
                .HasMaxLength(255)
                .HasColumnName("synonym");
            entity.Property(e => e.Unit)
                .HasMaxLength(10)
                .HasColumnName("unit");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("user_id");
            entity.Property(e => e.Visibility).HasColumnName("visibility");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.OrderAddress)
                .HasMaxLength(50)
                .HasColumnName("order_address");
            entity.Property(e => e.OrderEmail)
                .HasMaxLength(50)
                .HasColumnName("order_email");
            entity.Property(e => e.OrderName)
                .HasMaxLength(50)
                .HasColumnName("order_name");
            entity.Property(e => e.OrderPhone)
                .HasMaxLength(50)
                .HasColumnName("order_phone");
            entity.Property(e => e.OrderRemark).HasColumnName("order_remark");
            entity.Property(e => e.OrderTime).HasColumnName("order_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("Order_Items");

            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<PantryManagement>(entity =>
        {
            entity.HasKey(e => e.PantryId);

            entity.ToTable("Pantry_Management");

            entity.Property(e => e.PantryId).HasColumnName("pantry_id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.OwnerId)
                .HasMaxLength(450)
                .HasColumnName("ownerId");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<PreferedIngredient>(entity =>
        {
            entity.HasKey(e => e.PreferIngredientId);

            entity.ToTable("Prefered_Ingredients");

            entity.Property(e => e.PreferIngredientId).HasColumnName("prefer_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasColumnName("photo");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.UnitQuantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_quantity");
        });

        modelBuilder.Entity<ProductEvaluate>(entity =>
        {
            entity.HasKey(e => e.EvaluateId);

            entity.ToTable("Product_evaluate");

            entity.Property(e => e.EvaluateId).HasColumnName("evaluate_id");
            entity.Property(e => e.CommentMessage).HasColumnName("comment_message");
            entity.Property(e => e.CommentStars).HasColumnName("comment_stars");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.Category)
                .HasMaxLength(10)
                .HasColumnName("category");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DetailedCategory)
                .HasMaxLength(255)
                .HasColumnName("detailed_category");
            entity.Property(e => e.IsCustom).HasColumnName("is_custom");
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasColumnName("photo");
            entity.Property(e => e.RecipeName)
                .HasMaxLength(255)
                .HasColumnName("recipe_name");
            entity.Property(e => e.Restriction).HasColumnName("restriction");
            entity.Property(e => e.Seasoning)
                .HasMaxLength(255)
                .HasColumnName("seasoning");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Steps).HasColumnName("steps");
            entity.Property(e => e.Time)
                .HasMaxLength(255)
                .HasColumnName("time");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("user_id");
            entity.Property(e => e.Visibility).HasColumnName("visibility");
            entity.Property(e => e.WestEast).HasColumnName("west_east");
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.ToTable("Recipe_Ingredients");

            entity.Property(e => e.RecipeIngredientId).HasColumnName("recipe_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_RoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.DietaryRestrictions).HasColumnName("dietary_restrictions");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.ExclusiveChecked).HasColumnName("exclusive_checked");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PreferredChecked).HasColumnName("preferred_checked");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.UserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserCoupon>(entity =>
        {
            entity.HasKey(e => e.UserConponId);

            entity.ToTable("User_Coupons");

            entity.Property(e => e.UserConponId).HasColumnName("user_conponId");
            entity.Property(e => e.AcquireDate).HasColumnName("acquire_date");
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId);

            entity.ToTable("User_Groups");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupAdmin)
                .HasMaxLength(450)
                .HasColumnName("group_admin");
            entity.Property(e => e.GroupInvite).HasColumnName("group_invite");
            entity.Property(e => e.GroupName)
                .HasMaxLength(255)
                .HasColumnName("group_name");
        });

        modelBuilder.Entity<UserIdMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserIdMapping");

            entity.Property(e => e.NewUserId)
                .HasMaxLength(450)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_UserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasNoKey();
        });
        modelBuilder.HasSequence<int>("InventoryIDSeq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
