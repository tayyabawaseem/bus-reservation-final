
USE [Bus_Reservation_System]
GO
/****** Object:  User [TW]    Script Date: 2/24/2025 12:48:30 AM ******/
CREATE USER [TW] FOR LOGIN [TW] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [TW]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[Password] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[Booking_Id] [int] IDENTITY(1,1) NOT NULL,
	[Customer_Name] [varchar](100) NOT NULL,
	[Customer_Age] [int] NOT NULL,
	[Customer_Contact] [varchar](15) NOT NULL,
	[Customer_Email] [varchar](255) NULL,
	[Booking_Date] [datetime] NOT NULL,
	[Bus_Id] [int] NOT NULL,
	[Seat_Number] [int] NOT NULL,
	[Fare] [int] NOT NULL,
	[Branch_Id] [int] NULL,
	[Employee_Id] [int] NULL,
	[RefundAmount] [int] NULL,
	[Admin_Id] [int] NULL,
	[Status_Booking] [varchar](20) NOT NULL,
	[Created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Booking_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Branch]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Branch](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[branch_code] [varchar](255) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
	[city] [nvarchar](max) NOT NULL,
	[state] [nvarchar](max) NOT NULL,
	[zip_code] [varchar](255) NOT NULL,
	[contact] [varchar](255) NOT NULL,
	[date_created] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bus]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bus](
	[Bus_Id] [int] IDENTITY(1,1) NOT NULL,
	[Bus_Number] [varchar](10) NOT NULL,
	[Bus_Type_Id] [int] NOT NULL,
	[Route_Id] [int] NOT NULL,
	[Total_Seats] [int] NOT NULL,
	[Available_Seats] [int] NOT NULL,
	[Departure_Time] [datetime] NOT NULL,
	[Arrival_Time] [datetime] NOT NULL,
	[Bus_Image] [varchar](max) NULL,
	[Created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Bus_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bus_Type]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bus_Type](
	[Bus_Type_Id] [int] IDENTITY(1,1) NOT NULL,
	[Bus_Type_Name] [varchar](100) NOT NULL,
	[Created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Bus_Type_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[Id] [int] IDENTITY(100000,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Fathername] [varchar](255) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[Password] [varchar](100) NOT NULL,
	[Age] [int] NOT NULL,
	[dob] [date] NOT NULL,
	[martial_status] [varchar](100) NOT NULL,
	[Contact] [varchar](255) NOT NULL,
	[Qualification] [varchar](100) NOT NULL,
	[Address] [varchar](255) NOT NULL,
	[designation] [varchar](255) NOT NULL,
	[salary] [int] NOT NULL,
	[Branch_Id] [int] NOT NULL,
	[employee_image] [varchar](max) NOT NULL,
	[date_created] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Enquiries]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Enquiries](
	[enquiry_id] [int] IDENTITY(1,1) NOT NULL,
	[starting_place] [int] NULL,
	[destination_place] [int] NULL,
	[travel_date] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[enquiry_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Location]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Location](
	[Location_Id] [int] IDENTITY(1,1) NOT NULL,
	[Location_Name] [varchar](100) NOT NULL,
	[Created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Location_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Routes]    Script Date: 2/24/2025 12:48:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Routes](
	[RouteId] [int] IDENTITY(1,1) NOT NULL,
	[RouteName] [varchar](255) NOT NULL,
	[StartingPlace] [int] NOT NULL,
	[DestinationPlace] [int] NOT NULL,
	[distance] [int] NOT NULL,
	[Created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RouteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (getdate()) FOR [Booking_Date]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ('Confirmed') FOR [Status_Booking]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (getdate()) FOR [Created_at]
GO
ALTER TABLE [dbo].[Branch] ADD  DEFAULT (getdate()) FOR [date_created]
GO
ALTER TABLE [dbo].[Bus] ADD  DEFAULT (getdate()) FOR [Created_at]
GO
ALTER TABLE [dbo].[Bus_Type] ADD  DEFAULT (getdate()) FOR [Created_at]
GO
ALTER TABLE [dbo].[Employee] ADD  DEFAULT (getdate()) FOR [date_created]
GO
ALTER TABLE [dbo].[Location] ADD  DEFAULT (getdate()) FOR [Created_at]
GO
ALTER TABLE [dbo].[Routes] ADD  DEFAULT (getdate()) FOR [Created_at]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Booking_Admin] FOREIGN KEY([Admin_Id])
REFERENCES [dbo].[Admin] ([ID])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Ticket_Booking_Admin]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Booking_Branch] FOREIGN KEY([Branch_Id])
REFERENCES [dbo].[Branch] ([id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Ticket_Booking_Branch]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Booking_Bus] FOREIGN KEY([Bus_Id])
REFERENCES [dbo].[Bus] ([Bus_Id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Ticket_Booking_Bus]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Booking_Employee] FOREIGN KEY([Employee_Id])
REFERENCES [dbo].[Employee] ([Id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Ticket_Booking_Employee]
GO
ALTER TABLE [dbo].[Bus]  WITH CHECK ADD  CONSTRAINT [FK_Bus_ToBusType] FOREIGN KEY([Bus_Type_Id])
REFERENCES [dbo].[Bus_Type] ([Bus_Type_Id])
GO
ALTER TABLE [dbo].[Bus] CHECK CONSTRAINT [FK_Bus_ToBusType]
GO
ALTER TABLE [dbo].[Bus]  WITH CHECK ADD  CONSTRAINT [FK_Bus_ToRoute] FOREIGN KEY([Route_Id])
REFERENCES [dbo].[Routes] ([RouteId])
GO
ALTER TABLE [dbo].[Bus] CHECK CONSTRAINT [FK_Bus_ToRoute]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Branch] FOREIGN KEY([Branch_Id])
REFERENCES [dbo].[Branch] ([id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Branch]
GO
ALTER TABLE [dbo].[Enquiries]  WITH CHECK ADD  CONSTRAINT [FK_Routes_ToDestinationPlaceSearch] FOREIGN KEY([destination_place])
REFERENCES [dbo].[Location] ([Location_Id])
GO
ALTER TABLE [dbo].[Enquiries] CHECK CONSTRAINT [FK_Routes_ToDestinationPlaceSearch]
GO
ALTER TABLE [dbo].[Enquiries]  WITH CHECK ADD  CONSTRAINT [FK_Routes_ToStartingPlaceSearch] FOREIGN KEY([starting_place])
REFERENCES [dbo].[Location] ([Location_Id])
GO
ALTER TABLE [dbo].[Enquiries] CHECK CONSTRAINT [FK_Routes_ToStartingPlaceSearch]
GO
ALTER TABLE [dbo].[Routes]  WITH CHECK ADD  CONSTRAINT [FK_Routes_ToDestinationPlace] FOREIGN KEY([DestinationPlace])
REFERENCES [dbo].[Location] ([Location_Id])
GO
ALTER TABLE [dbo].[Routes] CHECK CONSTRAINT [FK_Routes_ToDestinationPlace]
GO
ALTER TABLE [dbo].[Routes]  WITH CHECK ADD  CONSTRAINT [FK_Routes_ToStartingPlace] FOREIGN KEY([StartingPlace])
REFERENCES [dbo].[Location] ([Location_Id])
GO
ALTER TABLE [dbo].[Routes] CHECK CONSTRAINT [FK_Routes_ToStartingPlace]
GO
