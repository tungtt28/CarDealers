CREATE TABLE [Users] (
  [user_id] int PRIMARY KEY IDENTITY(1, 1),
  [user_role_id] int NOT NULL,
  [dob] date,
  [gender] bit NOT NULL,
  [delete_flag] bit NOT NULL,
  [status] int NOT NULL,
  [username] nvarchar(255) NOT NULL,
  [password] nvarchar(255) NOT NULL,
  [email] nvarchar(255) NOT NULL,
  [full_name] nvarchar(255) NOT NULL,
  [phone_number] nvarchar(20) NOT NULL,
  [address] ntext NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Customers] (
  [customer_id] int PRIMARY KEY IDENTITY(1, 1),
  [dob] date,
  [gender] bit,
  [status] int,
  [car_id] int,
  [username] nvarchar(255),
  [password] nvarchar(255),
  [email] nvarchar(255) NOT NULL,
  [full_name] nvarchar(255) NOT NULL,
  [phone_number] nvarchar(20) NOT NULL,
  [address] ntext,
  [kilometerage] int,
  [plate_number] nvarchar(20),
  [CustomerType] int NOT NULL,
  [delete_flag] bit NOT NULL,
  [image] nvarchar(255),
  [ads] bit,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime,
  [verify_code] nvarchar(5)
)
GO

CREATE TABLE [UserRoles] (
  [user_role_id] int PRIMARY KEY IDENTITY(1, 1),
  [role_name] nvarchar(255) NOT NULL,
  [delete_flag] bit NOT NULL
)
GO

CREATE TABLE [Cars] (
  [car_id] int PRIMARY KEY IDENTITY(1, 1),
  [brand_id] int NOT NULL,
  [car_type_id] int NOT NULL,
  [quantity] int NOT NULL,
  [model] nvarchar(255) NOT NULL,
  [year] int,
  [import_price] DECIMAL(18,2),
  [export_price] DECIMAL(18,2),
  [deposit_price] DECIMAL(18,2),
  [Tax] DECIMAL(18,2),
  [mileage] int,
  [engine_type_id] int NOT NULL,
  [fuel_type_id] int NOT NULL,
  [transmission] nvarchar(50),
  [description] nvarchar(255),
  [image] nvarchar(255) NOT NULL,
  [status] bit NOT NULL,
  [delete_flag] bit NOT NULL,
  [content] ntext,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [ColorCarRefer] (
  [Color_id] int,
  [car_id] int,
  [image] nvarchar(255),
  PRIMARY KEY ([Color_id], [car_id])
)
GO

CREATE TABLE [Color] (
  [Color_id] int PRIMARY KEY IDENTITY(1, 1),
  [Color_name] nvarchar(255) NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [FuelTypes] (
  [fuel_type_id] int PRIMARY KEY IDENTITY(1, 1),
  [fuel_type_name] nvarchar(255) NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [EngineTypes] (
  [engine_type_id] int PRIMARY KEY IDENTITY(1, 1),
  [engine_type_name] nvarchar(255) NOT NULL,
  [description] nvarchar(255),
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [CarTypes] (
  [car_type_id] int PRIMARY KEY IDENTITY(1, 1),
  [type_name] nvarchar(255) NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Brands] (
  [brand_id] int PRIMARY KEY IDENTITY(1, 1),
  [brand_name] nvarchar(255) NOT NULL,
  [delete_flag] bit NOT NULL,
  [logo_image] nvarchar(255) NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [News] (
  [news_id] int PRIMARY KEY IDENTITY(1, 1),
  [author_id] int NOT NULL,
  [news_type_id] int NOT NULL,
  [order] int,
  [parent_id] int,
  [title] nvarchar(255),
  [content] ntext,
  [publish_date] datetime,
  [image] nvarchar(255),
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [NewsTypes] (
  [news_type_id] int PRIMARY KEY IDENTITY(1, 1),
  [news_type_name] nvarchar(255) NOT NULL,
  [description] nvarchar(255),
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Orders] (
  [order_id] int PRIMARY KEY IDENTITY(1, 1),
  [customer_id] int NOT NULL,
  [order_date] datetime,
  [status] int,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime,
  [delete_flag] bit NOT NULL
)
GO

CREATE TABLE [Order_Details] (
  [order_detail_id] int PRIMARY KEY IDENTITY(1, 1),
  [order_id] int NOT NULL,
  [car_id] int NOT NULL,
  [color_id] int NOT NULL,
  [coupon_id] int,
  [seller_id] int,
  [quantity] int NOT NULL,
  [total_price] DECIMAL(18,2),
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Reviews] (
  [review_id] int PRIMARY KEY IDENTITY(1, 1),
  [news_id] int NOT NULL,
  [customer_id] int NOT NULL,
  [rating] int,
  [comment] ntext,
  [publish_date] datetime NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Message] (
  [message_id] int PRIMARY KEY IDENTITY(1, 1),
  [message_content] ntext NOT NULL,
  [employee_id] int NOT NULL,
  [customer_id] int NOT NULL,
  [send-time] datetime NOT NULL
)
GO

CREATE TABLE [TrialDriving] (
  [trial_id] int PRIMARY KEY IDENTITY(1, 1),
  [email] nvarchar(255) NOT NULL,
  [full_name] nvarchar(255) NOT NULL,
  [phone_number] nvarchar(20) NOT NULL,
  [car_trail_id] int NOT NULL,
  [date_booking] datetime NOT NULL,
  [driver_license] nvarchar(12) NOT NULL,
  [request] nvarchar(255),
  [status] int NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [CarTrial] (
  [car_trial_id] int PRIMARY KEY IDENTITY(1, 1),
  [car_id] int NOT NULL,
  [plate_number] nvarchar(20) NOT NULL,
  [status] int NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Coupons] (
  [coupon_id] int PRIMARY KEY IDENTITY(1, 1),
  [name] nvarchar(128) NOT NULL,
  [code] nvarchar(10) NOT NULL,
  [description] nvarchar(255),
  [date_start] date NOT NULL DEFAULT '0000-00-00',
  [date_end] date NOT NULL DEFAULT '0000-00-00',
  [uses_total] int NOT NULL,
  [date_added] datetime NOT NULL,
  [percent_discount] FLOAT NOT NULL,
  [status] bit NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Image_cars] (
  [image_id] int PRIMARY KEY IDENTITY(1, 1),
  [car_id] int NOT NULL,
  [image] nvarchar(255) NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [AttachFiles] (
  [file_id] int PRIMARY KEY IDENTITY(1, 1),
  [order_detail_id] int NOT NULL,
  [file_name] nvarchar(255) NOT NULL,
  [Path] ntext NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [ServiceTypes] (
  [service_type_id] int PRIMARY KEY IDENTITY(1, 1),
  [service_type_name] nvarchar(255) NOT NULL,
  [description] nvarchar(255),
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [BookingServices] (
  [booking_id] int PRIMARY KEY IDENTITY(1, 1),
  [customer_id] int,
  [Customer_parent_id] int,
  [date_booking] datetime NOT NULL,
  [note] ntext,
  [status] int NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [BookingRefers] (
  [booking_id] int,
  [ServiceType_id] int,
  PRIMARY KEY ([booking_id], [ServiceType_id])
)
GO

CREATE TABLE [ AutoAccessories] (
  [accessory_id] int PRIMARY KEY IDENTITY(1, 1),
  [quantity] int NOT NULL,
  [accessory_name] nvarchar(255) NOT NULL,
  [import_price] DECIMAL(18,2),
  [export_price] DECIMAL(18,2),
  [description] ntext,
  [image] nvarchar(255) NOT NULL,
  [status] bit NOT NULL,
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

CREATE TABLE [Order_Accessory_Details] (
  [order_accessory_id] int PRIMARY KEY IDENTITY(1, 1),
  [order_id] int NOT NULL,
  [accessory_id] int NOT NULL,
  [coupon_id] int,
  [seller_id] int,
  [quantity] int NOT NULL,
  [total_price] DECIMAL(18,2),
  [delete_flag] bit NOT NULL,
  [created_by] int,
  [modified_by] int,
  [created_on] datetime,
  [modified_on] datetime
)
GO

ALTER TABLE [Order_Details] ADD FOREIGN KEY ([order_id]) REFERENCES [Orders] ([order_id])
GO

ALTER TABLE [Cars] ADD FOREIGN KEY ([brand_id]) REFERENCES [Brands] ([brand_id])
GO

ALTER TABLE [Cars] ADD FOREIGN KEY ([car_type_id]) REFERENCES [CarTypes] ([car_type_id])
GO

ALTER TABLE [Cars] ADD FOREIGN KEY ([engine_type_id]) REFERENCES [EngineTypes] ([engine_type_id])
GO

ALTER TABLE [Cars] ADD FOREIGN KEY ([fuel_type_id]) REFERENCES [FuelTypes] ([fuel_type_id])
GO

ALTER TABLE [ColorCarRefer] ADD FOREIGN KEY ([Color_id]) REFERENCES [Color] ([Color_id])
GO

ALTER TABLE [Order_Details] ADD FOREIGN KEY ([coupon_id]) REFERENCES [Coupons] ([coupon_id])
GO

ALTER TABLE [Image_cars] ADD FOREIGN KEY ([car_id]) REFERENCES [Cars] ([car_id])
GO

ALTER TABLE [Message] ADD FOREIGN KEY ([employee_id]) REFERENCES [Users] ([user_id])
GO

ALTER TABLE [News] ADD FOREIGN KEY ([author_id]) REFERENCES [Users] ([user_id])
GO

ALTER TABLE [ColorCarRefer] ADD FOREIGN KEY ([car_id]) REFERENCES [Cars] ([car_id])
GO

ALTER TABLE [Order_Details] ADD FOREIGN KEY ([car_id]) REFERENCES [Cars] ([car_id])
GO

ALTER TABLE [Order_Details] ADD FOREIGN KEY ([color_id]) REFERENCES [Color] ([Color_id])
GO

ALTER TABLE [Users] ADD FOREIGN KEY ([user_role_id]) REFERENCES [UserRoles] ([user_role_id])
GO

ALTER TABLE [Reviews] ADD FOREIGN KEY ([customer_id]) REFERENCES [Customers] ([customer_id])
GO

ALTER TABLE [Orders] ADD FOREIGN KEY ([customer_id]) REFERENCES [Customers] ([customer_id])
GO

ALTER TABLE [Order_Details] ADD FOREIGN KEY ([seller_id]) REFERENCES [Users] ([user_id])
GO

ALTER TABLE [Message] ADD FOREIGN KEY ([customer_id]) REFERENCES [Customers] ([customer_id])
GO

ALTER TABLE [Reviews] ADD FOREIGN KEY ([news_id]) REFERENCES [News] ([news_id])
GO

ALTER TABLE [News] ADD FOREIGN KEY ([news_type_id]) REFERENCES [NewsTypes] ([news_type_id])
GO

ALTER TABLE [BookingServices] ADD FOREIGN KEY ([customer_id]) REFERENCES [Customers] ([customer_id])
GO

ALTER TABLE [BookingRefers] ADD FOREIGN KEY ([booking_id]) REFERENCES [BookingServices] ([booking_id])
GO

ALTER TABLE [BookingRefers] ADD FOREIGN KEY ([ServiceType_id]) REFERENCES [ServiceTypes] ([service_type_id])
GO

ALTER TABLE [Order_Accessory_Details] ADD FOREIGN KEY ([accessory_id]) REFERENCES [ AutoAccessories] ([accessory_id])
GO

ALTER TABLE [Order_Accessory_Details] ADD FOREIGN KEY ([order_id]) REFERENCES [Orders] ([order_id])
GO

ALTER TABLE [AttachFiles] ADD FOREIGN KEY ([order_detail_id]) REFERENCES [Order_Details] ([order_detail_id])
GO

ALTER TABLE [Customers] ADD FOREIGN KEY ([car_id]) REFERENCES [Cars] ([car_id])
GO

ALTER TABLE [Order_Accessory_Details] ADD FOREIGN KEY ([seller_id]) REFERENCES [Users] ([user_id])
GO

ALTER TABLE [Order_Accessory_Details] ADD FOREIGN KEY ([coupon_id]) REFERENCES [Coupons] ([coupon_id])
GO

ALTER TABLE [TrialDriving] ADD FOREIGN KEY ([car_trail_id]) REFERENCES [CarTrial] ([car_trial_id])
GO

ALTER TABLE [CarTrial] ADD FOREIGN KEY ([car_id]) REFERENCES [Cars] ([car_id])
GO
