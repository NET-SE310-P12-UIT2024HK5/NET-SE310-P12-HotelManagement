

-- B?ng Roles
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

-- Chèn các vai trò
INSERT INTO Roles (RoleName) VALUES 
('Admin'), 
('Reception');

-- B?ng Users (Thay th? b?ng Reception)
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

-- B?ng Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) CHECK (PhoneNumber LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]%' OR PhoneNumber LIKE '0[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    CCCD NVARCHAR(50),
    Email NVARCHAR(100),
    DateOfBirth DATE
);

-- B?ng Rooms
CREATE TABLE Rooms (
    RoomID INT PRIMARY KEY IDENTITY(1,1),
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    RoomType NVARCHAR(50),
    Price DECIMAL(10, 2) NOT NULL,
    MaxOccupancy INT NOT NULL DEFAULT 2,
    Status NVARCHAR(50) DEFAULT 'Available',
    Description NVARCHAR(MAX)
);

-- B?ng Booking
CREATE TABLE Booking (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT,
    UserID INT,
    RoomID INT,
    CheckInDate DATE,
    CheckOutDate DATE,
    Status NVARCHAR(50) DEFAULT 'Pending',
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID)
);

-- B?ng FoodAndBeverageServices
CREATE TABLE FoodAndBeverageServices (
    ServiceID INT PRIMARY KEY IDENTITY(1,1),
    ItemName NVARCHAR(100) NOT NULL,
    ItemPrice DECIMAL(10, 2) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    ItemImage VARBINARY(MAX),
    IsAvailable BIT DEFAULT 1
);

-- B?ng BookingFoodServices
CREATE TABLE BookingFoodServices (
    BookingFoodServiceID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT,
    ServiceID INT,
    Quantity INT NOT NULL DEFAULT 1,
    TotalPrice DECIMAL(10, 2),
    OrderTime DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    FOREIGN KEY (ServiceID) REFERENCES FoodAndBeverageServices(ServiceID)
);

-- Bảng Invoice
CREATE TABLE Invoice (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1),
    TotalAmount DECIMAL(10, 2),
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending',
    PaymentDate DATE
);

-- Bảng InvoiceBookings
CREATE TABLE InvoiceBookings (
    InvoiceID INT,
    BookingID INT,
    FOREIGN KEY (InvoiceID) REFERENCES Invoice(InvoiceID),
    FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
    PRIMARY KEY (InvoiceID, BookingID)
);