CREATE DATABASE HotelManagement

USE HotelManagement

-- Roles Table
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

-- Users Table
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    PhoneNumber NVARCHAR(20) CHECK (PhoneNumber LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%' OR PhoneNumber LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- Customer Table
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) CHECK (PhoneNumber LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%' OR PhoneNumber LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    CCCD NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100)
);

-- Rooms Table
CREATE TABLE Rooms (
    RoomID INT PRIMARY KEY IDENTITY(1,1),
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    RoomType NVARCHAR(50) NOT NULL,
    Price INT NOT NULL,
    MaxOccupancy INT NOT NULL DEFAULT 2,
    Status NVARCHAR(50) DEFAULT 'Available',
    Description NVARCHAR(MAX)
);

-- Booking Table
CREATE TABLE Booking (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT,
    UserID INT,
    RoomID INT,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID)
);

-- FoodAndBeverageServices Table
CREATE TABLE FoodAndBeverageServices (
    ServiceID INT PRIMARY KEY IDENTITY(1,1),
    ItemName NVARCHAR(100) NOT NULL,
    ItemPrice INT NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    ItemImage VARBINARY(MAX),
    IsAvailable BIT DEFAULT 1
);

-- BookingFoodServices Table
CREATE TABLE BookingFoodServices (
    BookingFoodServiceID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT,
    ServiceID INT,
    Quantity INT DEFAULT 1 NOT NULL,
    TotalPrice INT,
    OrderTime DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    FOREIGN KEY (ServiceID) REFERENCES FoodAndBeverageServices(ServiceID)
);

-- Invoice Table
CREATE TABLE Invoice (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT NOT NULL,
	Duration INT NOT NULL,
    TotalAmount INT NOT NULL,
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending',
    PaymentDate DATE NOT NULL,
    FOREIGN KEY (BookingID) REFERENCES Booking(BookingID)
);


-- Insert Roles
INSERT INTO Roles (RoleName) VALUES 
('Admin'), 
('Reception');

-- Insert sample data into Roles table
INSERT INTO Roles (RoleName) VALUES 
('Admin'), 
('Receptionist');

-- Insert sample data into Users table
INSERT INTO Users (Username, Password, FullName, Email, PhoneNumber, RoleID) VALUES
('admin01', 'hashed_password_123', 'John Doe', 'admin@hotel.com', '0123456789', 1),
('reception01', 'hashed_password_456', 'Jane Smith', 'reception@hotel.com', '0987654321', 2);

-- Insert sample data into Customer table
INSERT INTO Customer (FullName, PhoneNumber, CCCD, Email) VALUES
('Nguyen Van A', '0912345678', '123456789123', 'nguyenvana@example.com'),
('Tran Thi B', '0923456789', '987654321987', 'tranthib@example.com');

-- Insert sample data into Rooms table
INSERT INTO Rooms (RoomNumber, RoomType, Price, MaxOccupancy, Status, Description) VALUES
('101', 'Single', 500000, 1, 'Available', 'Cozy single room with a queen-size bed.'),
('102', 'Double', 800000, 2, 'Available', 'Spacious room with two single beds.'),
('201', 'Suite', 1500000, 4, 'Occupied', 'Luxurious suite with premium amenities.');

-- Insert sample data into Booking table
INSERT INTO Booking (CustomerID, UserID, RoomID, CheckInDate, CheckOutDate, Status) VALUES
(1, 2, 1, '2024-12-01', '2024-12-03', 'Confirmed'),
(2, 2, 2, '2024-12-05', '2024-12-08', 'Pending');

-- Insert sample data into FoodAndBeverageServices table
INSERT INTO FoodAndBeverageServices (ItemName, ItemPrice, Category, Description, IsAvailable) VALUES
('Coffee', 50000, 'Beverage', 'Freshly brewed coffee.', 1),
('Green Tea', 40000, 'Beverage', 'Hot and soothing green tea.', 1),
('Club Sandwich', 70000, 'Food', 'Classic sandwich with chicken and vegetables.', 1);

-- Insert sample data into BookingFoodServices table
-- INSERT INTO BookingFoodServices (BookingID, ServiceID, Quantity, TotalPrice, OrderTime) VALUES
-- (1, 1, 2, 100000, GETDATE()),
-- (1, 3, 1, 70000, GETDATE()),
-- (2, 2, 3, 120000, GETDATE());

-- Insert sample data into Invoice table
INSERT INTO Invoice (BookingID, Duration, TotalAmount, PaymentStatus, PaymentDate)
VALUES 
(1, 1, 500000, 'Paid', '2024-06-01'),
(2, 2 ,700000, 'Pending', '2024-06-03');
