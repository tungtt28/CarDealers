using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarDealers.Entity;

public partial class CarDealersContext : DbContext
{
    public CarDealersContext()
    {
    }

    public CarDealersContext(DbContextOptions<CarDealersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AttachFile> AttachFiles { get; set; }

    public virtual DbSet<AutoAccessory> AutoAccessories { get; set; }

    public virtual DbSet<BookingService> BookingServices { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Car> Cars { get; set; }
    public virtual DbSet<CarTrial> CarTrials { get; set; }

    public virtual DbSet<CarType> CarTypes { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<ColorCarRefer> ColorCarRefers { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<EngineType> EngineTypes { get; set; }

    public virtual DbSet<FuelType> FuelTypes { get; set; }

    public virtual DbSet<ImageCar> ImageCars { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsType> NewsTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderAccessoryDetail> OrderAccessoryDetails { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<TrialDriving> TrialDrivings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<BookingRefer> BookingRefers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var builder = new ConfigurationBuilder()
				  .SetBasePath(Directory.GetCurrentDirectory())
				  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
		IConfigurationRoot configuration = builder.Build();
		optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultString"));
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<AttachFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__AttachFi__07D884C646D8911D");

            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.Path).HasColumnType("ntext");

            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.AttachFiles)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AttachFil__order__6E01572D");
        });


        modelBuilder.Entity<BookingRefer>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.ServiceTypeId });
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.ServiceTypeId).HasColumnName("ServiceType_id");
        });

        modelBuilder.Entity<AutoAccessory>(entity =>
        {
            entity.HasKey(e => e.AccessoryId).HasName("PK__ AutoAcc__2ED8EB45EAEFD97C");

            entity.ToTable(" AutoAccessories");

            entity.Property(e => e.AccessoryId).HasColumnName("accessory_id");
            entity.Property(e => e.AccessoryName)
                .HasMaxLength(255)
                .HasColumnName("accessory_name");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ExportPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("export_price");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.ImportPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("import_price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__BookingS__5DE3A5B1AAA6631F");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.CustomerParentId).HasColumnName("customer_parent_id");
            entity.Property(e => e.DateBooking)
                .HasColumnType("datetime")
                .HasColumnName("date_booking");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Note)
                .HasColumnType("ntext")
                .HasColumnName("note");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__BookingSe__custo__693CA210");

            entity.HasMany(d => d.ServiceTypes).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingRefer",
                    r => r.HasOne<ServiceType>().WithMany()
                        .HasForeignKey("ServiceTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookingRe__Servi__6B24EA82"),
                    l => l.HasOne<BookingService>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookingRe__booki__6A30C649"),
                    j =>
                    {
                        j.HasKey("BookingId", "ServiceTypeId").HasName("PK__BookingR__F2C8812A0F8B917B");
                        j.ToTable("BookingRefer");
                        j.IndexerProperty<int>("BookingId").HasColumnName("booking_id");
                        j.IndexerProperty<int>("ServiceTypeId").HasColumnName("ServiceType_id");
                    });
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__5E5A8E277A640BD1");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandName)
                .HasMaxLength(255)
                .HasColumnName("brand_name");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.LogoImage)
                .HasMaxLength(255)
                .HasColumnName("logo_image");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__4C9A0DB3AC7C99AB");

            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CarTypeId).HasColumnName("car_type_id");
            entity.Property(e => e.Content)
                .HasColumnType("ntext")
                .HasColumnName("content");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.DepositPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("deposit_price");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.EngineTypeId).HasColumnName("engine_type_id");
            entity.Property(e => e.ExportPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("export_price");
            entity.Property(e => e.FuelTypeId).HasColumnName("fuel_type_id");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.ImportPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("import_price");
            entity.Property(e => e.Mileage).HasColumnName("mileage");
            entity.Property(e => e.Model)
                .HasMaxLength(255)
                .HasColumnName("model");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modified_on");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Tax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Transmission)
                .HasMaxLength(50)
                .HasColumnName("transmission");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.Brand).WithMany(p => p.Cars)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__brand_id__571DF1D5");

            entity.HasOne(d => d.CarType).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__car_type_i__5812160E");

            entity.HasOne(d => d.EngineType).WithMany(p => p.Cars)
                .HasForeignKey(d => d.EngineTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__engine_typ__59063A47");

            entity.HasOne(d => d.FuelType).WithMany(p => p.Cars)
                .HasForeignKey(d => d.FuelTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__fuel_type___59FA5E80");
        });

        modelBuilder.Entity<CarTrial>(entity =>
        {
            entity.HasKey(e => e.CarTrialId).HasName("PK__CarTrial__CFC7B68613C5C247");

            entity.ToTable("CarTrial");

            entity.Property(e => e.CarTrialId).HasColumnName("car_trial_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modified_on");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(20)
                .HasColumnName("plate_number");
            entity.HasOne(d => d.Car).WithMany(p => p.CarTrials)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CarTrial__car_id__72C60C4A");
        });

        modelBuilder.Entity<CarType>(entity =>
        {
            entity.HasKey(e => e.CarTypeId).HasName("PK__CarTypes__019EDE8B5D12D64B");

            entity.Property(e => e.CarTypeId).HasColumnName("car_type_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.TypeName)
                .HasMaxLength(255)
                .HasColumnName("type_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__Color__795A112C517811CB");

            entity.ToTable("Color");

            entity.Property(e => e.ColorId).HasColumnName("Color_id");
            entity.Property(e => e.ColorName)
                .HasMaxLength(255)
                .HasColumnName("Color_name");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<ColorCarRefer>(entity =>
        {
            entity.HasKey(e => new { e.ColorId, e.CarId }).HasName("PK__ColorCar__5D93B1F749FACEC8");

            entity.ToTable("ColorCarRefer");

            entity.Property(e => e.ColorId).HasColumnName("Color_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");

            entity.HasOne(d => d.Car).WithMany(p => p.ColorCarRefers)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ColorCarR__car_i__5DCAEF64");

            entity.HasOne(d => d.Color).WithMany(p => p.ColorCarRefers)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ColorCarR__Color__59063A47");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PK__Coupons__58CF6389AF4A7AA3");

            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.DateAdded)
                .HasColumnType("datetime")
                .HasColumnName("date_added");
            entity.Property(e => e.DateEnd)
                .HasDefaultValueSql("('0000-00-00')")
                .HasColumnType("date")
                .HasColumnName("date_end");
            entity.Property(e => e.DateStart)
                .HasDefaultValueSql("('0000-00-00')")
                .HasColumnType("date")
                .HasColumnName("date_start");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
            entity.Property(e => e.PercentDiscount).HasColumnName("percent_discount");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UsesTotal).HasColumnName("uses_total");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB8587FCB39A");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasColumnType("ntext")
                .HasColumnName("address");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Kilometerage).HasColumnName("kilometerage");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(20)
                .HasColumnName("plate_number");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.VerifyCode)
                .HasMaxLength(5)
                .HasColumnName("verify_code");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Ads).HasColumnName("ads");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.Car).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__Customers__car_i__6EF57B66");
        });

        modelBuilder.Entity<EngineType>(entity =>
        {
            entity.HasKey(e => e.EngineTypeId).HasName("PK__EngineTy__5F2BEC0657BEB56A");

            entity.Property(e => e.EngineTypeId).HasColumnName("engine_type_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description).HasMaxLength(255).HasColumnName("description");

			entity.Property(e => e.EngineTypeName)
                .HasMaxLength(255)
                .HasColumnName("engine_type_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<FuelType>(entity =>
        {
            entity.HasKey(e => e.FuelTypeId).HasName("PK__FuelType__7FF67CA358FD385C");

            entity.Property(e => e.FuelTypeId).HasColumnName("fuel_type_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.FuelTypeName)
                .HasMaxLength(255)
                .HasColumnName("fuel_type_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<ImageCar>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Image_ca__DC9AC955211C4726");

            entity.ToTable("Image_cars");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.Car).WithMany(p => p.ImageCars)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Image_car__car_i__5AEE82B9");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__0BBF6EE679EDC33E");

            entity.ToTable("Message");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.MessageContent)
                .HasColumnType("ntext")
                .HasColumnName("message_content");
            entity.Property(e => e.SendTime)
                .HasColumnType("datetime")
                .HasColumnName("send-time");

            entity.HasOne(d => d.Customer).WithMany(p => p.Messages)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__custome__656C112C");

            entity.HasOne(d => d.Employee).WithMany(p => p.Messages)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__employe__5BE2A6F2");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__News__4C27CCD8A7AB8BE3");

            entity.Property(e => e.NewsId).HasColumnName("news_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content)
                .HasColumnType("ntext")
                .HasColumnName("content");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.NewsTypeId).HasColumnName("news_type_id");
            entity.Property(e => e.PublishDate)
                .HasColumnType("datetime")
                .HasColumnName("publish_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");

            entity.HasOne(d => d.Author).WithMany(p => p.News)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__News__author_id__5CD6CB2B");

            entity.HasOne(d => d.NewsType).WithMany(p => p.News)
                .HasForeignKey(d => d.NewsTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__News__news_type___68487DD7");
        });

        modelBuilder.Entity<NewsType>(entity =>
        {
            entity.HasKey(e => e.NewsTypeId).HasName("PK__NewsType__08F6B8E3763E0F70");

            entity.Property(e => e.NewsTypeId).HasColumnName("news_type_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.NewsTypeName)
                .HasMaxLength(255)
                .HasColumnName("news_type_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__46596229DDDAAECD");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status)
                .HasColumnName("status");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__customer__6383C8BA");
        });

        modelBuilder.Entity<OrderAccessoryDetail>(entity =>
        {
            entity.HasKey(e => e.OrderAccessoryId).HasName("PK__Order_Ac__A2F09073E9DEC15D");

            entity.ToTable("Order_Accessory_Details");

            entity.Property(e => e.OrderAccessoryId).HasColumnName("order_accessory_id");
            entity.Property(e => e.AccessoryId).HasColumnName("accessory_id");
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.Accessory).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.AccessoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Acc__acces__6C190EBB");

            entity.HasOne(d => d.Coupon).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.CouponId)
                .HasConstraintName("FK__Order_Acc__coupo__70DDC3D8");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Acc__order__6D0D32F4");

            entity.HasOne(d => d.Seller).WithMany(p => p.OrderAccessoryDetails)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Acc__selle__6FE99F9F");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__Order_De__3C5A40809D3E7452");

            entity.ToTable("Order_Details");

            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.ColorId).HasColumnName("color_id");
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.Car).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Det__car_i__5FB337D6");

            entity.HasOne(d => d.Color).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Det__color__60A75C0F");

            entity.HasOne(d => d.Coupon).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.CouponId)
                .HasConstraintName("FK__Order_Det__coupo__59FA5E80");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Det__order__5441852A");

            entity.HasOne(d => d.Seller).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Det__selle__6477ECF3");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__60883D90CC73D7C8");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .HasColumnType("ntext")
                .HasColumnName("comment");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.NewsId).HasColumnName("news_id");
            entity.Property(e => e.PublishDate)
                .HasColumnType("datetime")
                .HasColumnName("publish_date");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__custome__628FA481");

            entity.HasOne(d => d.News).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.NewsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__news_id__6754599E");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.ServiceTypeId).HasName("PK__ServiceT__288B52C6458FFE51");

            entity.Property(e => e.ServiceTypeId).HasColumnName("service_type_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");
            entity.Property(e => e.ServiceTypeName)
                .HasMaxLength(255)
                .HasColumnName("service_type_name");
        });

        modelBuilder.Entity<TrialDriving>(entity =>
        {
            entity.HasKey(e => e.TrialId).HasName("PK__TrialDri__B1C18AD5CCAF8B87");

            entity.ToTable("TrialDriving");

            entity.Property(e => e.TrialId).HasColumnName("trial_id");
            entity.Property(e => e.CarTrailId).HasColumnName("car_trail_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DateBooking)
                .HasColumnType("datetime")
                .HasColumnName("date_booking");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.DriverLicense)
                .HasMaxLength(12)
                .HasColumnName("driver_license");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modified_on");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Request)
                .HasMaxLength(255)
                .HasColumnName("request");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.CarTrail).WithMany(p => p.TrialDrivings)
                .HasForeignKey(d => d.CarTrailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TrialDriv__car_t__71D1E811");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F86F1F593");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasColumnType("ntext")
                .HasColumnName("address");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.CreatedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("created_on");
            entity.Property(e => e.ModifiedOn)
                  .HasColumnType("datetime")
                  .HasColumnName("modified_on");

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__user_role__619B8048");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__B8D9ABA2C754E09A");

            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
