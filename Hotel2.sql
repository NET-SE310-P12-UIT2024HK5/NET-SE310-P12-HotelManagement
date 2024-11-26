-- T?o c? s? d? li?u
CREATE DATABASE HotelManagement;
USE HotelManagement;

-- B?ng Roles
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY AUTO_INCREMENT,
    RoleName ENUM('Admin', 'Reception') NOT NULL UNIQUE
);

-- Chèn các vai trò
INSERT INTO Roles (RoleName) VALUES 
('Admin'), 
('Reception');

-- B?ng Users (Thay th? b?ng Reception)
CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    PhoneNumber VARCHAR(20) CHECK (PhoneNumber REGEXP '^(0[0-9]{9,10})$'),
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- B?ng Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY AUTO_INCREMENT,
    FullName VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20) CHECK (PhoneNumber REGEXP '^(0[0-9]{9,10})$'),
    CCCD VARCHAR(50),
    Email VARCHAR(100),
    DateOfBirth DATE
);

-- B?ng Rooms
CREATE TABLE Rooms (
    RoomID INT PRIMARY KEY AUTO_INCREMENT,
    RoomNumber VARCHAR(10) NOT NULL UNIQUE,
    RoomType VARCHAR(50),
    Price DECIMAL(10, 2) NOT NULL,
    MaxOccupancy INT NOT NULL DEFAULT 2,
    Status ENUM('Available', 'Occupied', 'UnderMaintenance') DEFAULT 'Available',
    Description TEXT
);

-- B?ng Booking
CREATE TABLE Booking (
    BookingID INT PRIMARY KEY AUTO_INCREMENT,
    CustomerID INT,
    UserID INT,
    RoomID INT,
    CheckInDate DATE,
    CheckOutDate DATE,
    TotalPrice DECIMAL(10, 2),
    Status ENUM('Pending', 'Confirmed', 'Cancelled') DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID)
);

-- B?ng FoodAndBeverageServices
CREATE TABLE FoodAndBeverageServices (
    ServiceID INT PRIMARY KEY AUTO_INCREMENT,
    ItemName VARCHAR(100) NOT NULL,
    ItemPrice DECIMAL(10, 2) NOT NULL,
    Category ENUM('Food', 'Beverage', 'Combo') NOT NULL,
    Description TEXT,
    ItemImage LONGBLOB,
    IsAvailable BOOLEAN DEFAULT TRUE
);

-- B?ng BookingFoodServices
CREATE TABLE BookingFoodServices (
    BookingFoodServiceID INT PRIMARY KEY AUTO_INCREMENT,
    BookingID INT,
    ServiceID INT,
    Quantity INT NOT NULL DEFAULT 1,
    TotalPrice DECIMAL(10, 2),
    OrderTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    FOREIGN KEY (ServiceID) REFERENCES FoodAndBeverageServices(ServiceID)
);

-- B?ng Invoices
CREATE TABLE Invoice (
    InvoiceID INT PRIMARY KEY AUTO_INCREMENT,
    TotalAmount DECIMAL(10, 2),
    PaymentStatus ENUM('Pending', 'Paid') DEFAULT 'Pending',
    PaymentDate DATE
);

CREATE TABLE InvoiceBookings (
    InvoiceID INT,
    BookingID INT,
    FOREIGN KEY (InvoiceID) REFERENCES Invoice(InvoiceID),
    FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    PRIMARY KEY (InvoiceID, BookingID)
);