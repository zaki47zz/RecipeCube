using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RecipeCube.Models;

public partial class RecipeCubeContext : DbContext
{
    public RecipeCubeContext() { }

    public RecipeCubeContext(DbContextOptions<RecipeCubeContext> options)
        : base(options) { }

    public virtual DbSet<ExclusiveIngredient> ExclusiveIngredients { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PantryManagement> PantryManagements { get; set; }

    public virtual DbSet<PreferedIngredient> PreferedIngredients { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RecipeCube;TrustServerCertificate=True;Integrated Security=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExclusiveIngredient>(entity =>
        {
            entity.ToTable("Exclusive_Ingredients");

            entity.Property(e => e.ExclusiveIngredientId).HasColumnName("exclusive_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Category).HasMaxLength(100).HasColumnName("category");
            entity.Property(e => e.ExpireDay).HasColumnName("expire_day");
            entity.Property(e => e.Gram).HasColumnType("decimal(10, 2)").HasColumnName("gram");
            entity
                .Property(e => e.IngredientName)
                .HasMaxLength(255)
                .HasColumnName("ingredient_name");
            entity.Property(e => e.Photo).HasMaxLength(255).HasColumnName("photo");
            entity.Property(e => e.Synonym).HasMaxLength(255).HasColumnName("synonym");
            entity.Property(e => e.Unit).HasMaxLength(10).HasColumnName("unit");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.IsExpiring).HasColumnName("is_expiring");
            entity
                .Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Visibility).HasColumnName("visibility");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderTime).HasColumnName("order_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");
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
            entity.Property(e => e.Action).HasMaxLength(50).HasColumnName("action");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.OutOfStock).HasColumnName("out_of_stock");
            entity
                .Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<PreferedIngredient>(entity =>
        {
            entity.HasKey(e => e.PerferIngredientId);

            entity.ToTable("Prefered_Ingredients");

            entity.Property(e => e.PerferIngredientId).HasColumnName("perfer_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Photo).HasMaxLength(255).HasColumnName("photo");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductName).HasMaxLength(255).HasColumnName("product_name");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.Category).HasMaxLength(10).HasColumnName("category");
            entity
                .Property(e => e.DetailedCategory)
                .HasMaxLength(255)
                .HasColumnName("detailed_category");
            entity.Property(e => e.IsCustom).HasColumnName("is_custom");
            entity.Property(e => e.Photo).HasMaxLength(255).HasColumnName("photo");
            entity.Property(e => e.RecipeName).HasMaxLength(255).HasColumnName("recipe_name");
            entity.Property(e => e.Restriction).HasColumnName("restriction");
            entity.Property(e => e.Seasoning).HasMaxLength(255).HasColumnName("seasoning");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Steps).HasColumnName("steps");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Visibility).HasColumnName("visibility");
            entity.Property(e => e.WestEast).HasColumnName("west_east");
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.ToTable("Recipe_Ingredients");

            entity.Property(e => e.RecipeIngredientId).HasColumnName("recipe_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity
                .Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DietaryRestrictions).HasColumnName("dietary_restrictions");
            entity.Property(e => e.Email).HasMaxLength(255).IsUnicode(false).HasColumnName("email");
            entity.Property(e => e.ExclusiveChecked).HasColumnName("exclusive_checked");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity
                .Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone).HasMaxLength(20).IsUnicode(false).HasColumnName("phone");
            entity.Property(e => e.PreferredChecked).HasColumnName("preferred_checked");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Username).HasMaxLength(255).HasColumnName("username");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId);

            entity.ToTable("User_Groups");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.GroupAdmin).HasColumnName("group_admin");
            entity.Property(e => e.GroupInvite).HasColumnName("group_invite");
            entity.Property(e => e.GroupName).HasMaxLength(255).HasColumnName("group_name");
        });
        modelBuilder.HasSequence<int>("InventoryIDSeq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
