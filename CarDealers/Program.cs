using CarDealers.Entity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CarDealersContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultString")));
builder.Services.AddMvc()
        .AddSessionStateTempDataProvider();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddSession(cfg => {                    // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "xuanthulab";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
});
var app = builder.Build();
// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CarDealersContext>();

    // Ensure the database is created (if it doesn't exist)
    context.Database.EnsureCreated();

    // Check if data exists, and if not, add seed data for UserRoles
    if (!context.UserRoles.Any())
    {
        using (var transaction = context.Database.BeginTransaction())
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.UserRoles ON");

            context.UserRoles.Add(new UserRole { UserRoleId = 1, RoleName = "admin", DeleteFlag = false });
            context.UserRoles.Add(new UserRole { UserRoleId = 2, RoleName = "employee", DeleteFlag = false });
            // Add more seed data for UserRoles as needed

            context.SaveChanges();

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.UserRoles OFF");
            transaction.Commit();
        }
    }

    // Check if data exists, and if not, add seed data for Users
    if (!context.Users.Any())
    {
        using (var transaction = context.Database.BeginTransaction())
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Users ON");

            // Make sure the UserRoleId corresponds to an existing UserRoleId in UserRoles
            context.Users.Add(new User
            {
                UserId = 1,
                UserRoleId = 1, // Ensure this corresponds to an existing UserRoleId in UserRoles
                Address = "carDealers",
                Email = "locvip@gmail.com",
                Username = "admin1",
                Password = "303ef249a551b530782ec04ef8fabdfb",
                Gender = false,
                FullName = "admin",
                Status = 1,
                PhoneNumber = "999999999",
                DeleteFlag = false
            });
            // Add more seed data for Users as needed

            context.SaveChanges();

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Users OFF");
            transaction.Commit();
        }
    }

    if (!context.Customers.Any())
    {
        using (var transaction = context.Database.BeginTransaction())
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Customers ON");

            // Make sure the UserRoleId corresponds to an existing UserRoleId in UserRoles
            context.Customers.Add(new Customer
            {
                CustomerId = 1,
                Dob = new DateTime(2002, 01, 01),
                Gender = true,
                Status = 1,
                Username = "customer1",
                Password = "303ef249a551b530782ec04ef8fabdfb",
                Email = "customer1@gmail.com",
                FullName = "Customer 1",
                PhoneNumber = "0123456789",
                Address = "Hanoi",
                CustomerType = 2,
                DeleteFlag = false,
                Ads = false,
                CreatedOn = new DateTime(2022, 01, 01)
            });
            // Add more seed data for Users as needed

            context.SaveChanges();

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Customers OFF");
            transaction.Commit();
        }
    }

    // Check if data exists, and if not, add seed data for Users
    if (!context.NewsTypes.Any())
    {
        using (var transaction = context.Database.BeginTransaction())
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.NewsTypes ON");

            // Make sure the UserRoleId corresponds to an existing UserRoleId in UserRoles
            context.NewsTypes.Add(new NewsType
            {
                NewsTypeId = 1,
                NewsTypeName = "footer",
                DeleteFlag = false
            });

            context.NewsTypes.Add(new NewsType
            {
                NewsTypeId = 2,
                NewsTypeName = "menu",
                DeleteFlag = false
            });
            // Add more seed data for Users as needed

            context.SaveChanges();

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.NewsTypes OFF");
            transaction.Commit();
        }
    }

    // Check if data exists, and if not, add seed data for Users
    if (!context.News.Any())
    {
        using (var transaction = context.Database.BeginTransaction())
        {
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.News ON");

            // Make sure the News corresponds to an existing UserRoleId in UserRoles
            context.News.Add(new News
            {
                NewsId = 1,
                NewsTypeId = 1,
                Order = 1,
                AuthorId = 1,
                Title = "CONTACT INFO",
                Content = @"<p>Showroom Name: Jomo </br>
                Address: 123 Main Street, Hanoi, Vietnam</br>
                Phone: (+84) 967-355-255</br>
                Email: <a href=""mailto:SwpG1@gmail.com"">SwpG1@gmail.com</a></br>
                Website:  <a href=""http://www.jomo-showroom.com"" target=""_blank"">www.jomo-showroom.com</a></p>",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });
            context.News.Add(new News
            {
                NewsId = 2,
                NewsTypeId = 1,
                Order = 2,
                AuthorId = 1,
                Title = "OPEN HOURS",
                Content = @"<p>From Monday to Saturday: 10:00 am to 8:00 pm</br>
                            Sunday: Off</p>",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now

            });

            context.News.Add(new News
            {
                NewsId = 3,
                NewsTypeId = 1,
                Order = 3,
                AuthorId = 1,
                Title = "USEFUL LINK",
                Content = "<ul><li><a href='/cars'>Cars</a></li><li><a href='/portfolio'>Portfolio</a></li><li><a href='/news'>News</a></li></ul>",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });

            context.News.Add(new News
            {
                NewsId = 4,
                NewsTypeId = 2,
                Order = 1,
                AuthorId = 1,
                Title = "Home",
                Content = "/Home/Index",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });

            context.News.Add(new News
            {
                NewsId = 5,
                NewsTypeId = 2,
                Order = 2,
                AuthorId = 1,
                Title = "About",
                Content = "/AboutPage/Index",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });


            context.News.Add(new News
            {
                NewsId = 6,
                NewsTypeId = 2,
                Order = 3,
                AuthorId = 1,
                Title = "Car",
                Content = "/CarPage/ListCar",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });

            context.News.Add(new News
            {
                NewsId = 7,
                NewsTypeId = 2,
                Order = 4,
                AuthorId = 1,
                Title = "Accessories",
                Content = "/AccessoriesPage/Accessories",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });

            context.News.Add(new News
            {
                NewsId = 8,
                NewsTypeId = 2,
                Order = 5,
                AuthorId = 1,
                Title = "News",
                Content = "/NewsPage/News",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });

            context.News.Add(new News
            {
                NewsId = 9,
                NewsTypeId = 2,
                Order = 6,
                AuthorId = 1,
                Title = "Trial Driving",
                Content = "/TrialDriving/Index",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });

            context.News.Add(new News
            {
                NewsId = 10,
                NewsTypeId = 2,
                Order = 7,
                AuthorId = 1,
                Title = "Service",
                Content = "/Service/ServicePage",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });

            context.News.Add(new News
            {
                NewsId = 11,
                NewsTypeId = 2,
                Order = 8,
                AuthorId = 1,
                Title = "Cart",
                Content = "/cart",
                DeleteFlag = false,
                Image = "",
                PublishDate = DateTime.Now
            });




            // Add more seed data for Users as needed

            context.SaveChanges();

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.News OFF");
            transaction.Commit();
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin_area",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
