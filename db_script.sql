USE [master]
GO
/****** Object:  Database [UnkCompanyDB]    Script Date: 16.02.2024 14:02:14 ******/
CREATE DATABASE [UnkCompanyDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'UnkCompanyDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\UnkCompanyDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'UnkCompanyDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\UnkCompanyDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [UnkCompanyDB] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [UnkCompanyDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [UnkCompanyDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [UnkCompanyDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [UnkCompanyDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [UnkCompanyDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [UnkCompanyDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET RECOVERY FULL 
GO
ALTER DATABASE [UnkCompanyDB] SET  MULTI_USER 
GO
ALTER DATABASE [UnkCompanyDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [UnkCompanyDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [UnkCompanyDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [UnkCompanyDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [UnkCompanyDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [UnkCompanyDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'UnkCompanyDB', N'ON'
GO
ALTER DATABASE [UnkCompanyDB] SET QUERY_STORE = OFF
GO
USE [UnkCompanyDB]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 16.02.2024 14:02:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[idDepartment] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[idParentDepartment] [int] NULL,
	[idDirector] [int] NULL,
	[Phone] [nvarchar](25) NOT NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[idDepartment] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 16.02.2024 14:02:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[idEmployee] [int] IDENTITY(1,1) NOT NULL,
	[idDepartment] [int] NULL,
	[Fullname] [nvarchar](200) NOT NULL,
	[Login] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[idPosition] [int] NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[idEmployee] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Positions]    Script Date: 16.02.2024 14:02:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Positions](
	[idPosition] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Positions] PRIMARY KEY CLUSTERED 
(
	[idPosition] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Departments]  WITH CHECK ADD  CONSTRAINT [FK_Departments_Departments] FOREIGN KEY([idParentDepartment])
REFERENCES [dbo].[Departments] ([idDepartment])
GO
ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_Departments_Departments]
GO
ALTER TABLE [dbo].[Departments]  WITH CHECK ADD  CONSTRAINT [FK_Departments_Employees] FOREIGN KEY([idDirector])
REFERENCES [dbo].[Employees] ([idEmployee])
GO
ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_Departments_Employees]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Departments] FOREIGN KEY([idDepartment])
REFERENCES [dbo].[Departments] ([idDepartment])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Departments]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Positions] FOREIGN KEY([idPosition])
REFERENCES [dbo].[Positions] ([idPosition])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Positions]
GO
USE [master]
GO
ALTER DATABASE [UnkCompanyDB] SET  READ_WRITE 
GO
