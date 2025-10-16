--create database ELibrary;
-- bang permission
CREATE TABLE Permission (
PermissionID INT PRIMARY KEY IDENTITY(1,1),
Link VARCHAR(255),
Description  NVARCHAR(500),
);
-- bang role
CREATE TABLE Role (
RoleID INT PRIMARY KEY IDENTITY(1,1),
RoleName VARCHAR(10),
);
-- bang role - permission
CREATE TABLE Role_Permission (
RoleID INT  ,
PermissionId INT ,
PRIMARY KEY(RoleID,PermissionID),
FOREIGN KEY (RoleID) REFERENCES Role(RoleID),
FOREIGN KEY (PermissionID) REFERENCES Permission(PermissionID)
);
-- bang employee
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),  
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DoB DATE,                                 
    Age INT,                                    
    Email VARCHAR(100) UNIQUE NOT NULL,       
    PhoneNumber VARCHAR(15),                    
    [Password] NVARCHAR(255) NOT NULL,        
    Avatar NVARCHAR(255),                      
    RoleID INT NOT NULL,                     
    StartDate DATE NOT NULL DEFAULT GETDATE(),  
    Status BIT DEFAULT 1,                      
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);
-- bang reader
CREATE TABLE Reader (
    ReaderID INT PRIMARY KEY IDENTITY(1,1),   
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DoB DATE,                                
    Age INT,                                 
    [Address] NVARCHAR(255),
    Email VARCHAR(100) UNIQUE NOT NULL,      
    PhoneNumber VARCHAR(15),
    [Password] NVARCHAR(255) NOT NULL,       
    Avatar NVARCHAR(255),                   
    Status BIT DEFAULT 1                  
);
-- bang library card
CREATE TABLE LibraryCard (
    CardID INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentDate DATE NOT NULL,
    CardExpiryDate DATE NOT NULL,
    ReaderID INT NOT NULL,
    Status BIT DEFAULT 1,
    FOREIGN KEY (ReaderID) REFERENCES Reader(ReaderID)
);
-- bang nha xuat ban
CREATE TABLE Publisher (
    PublisherID INT PRIMARY KEY IDENTITY(1,1),
    PublisherName NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(255)
);
-- bang Author
CREATE TABLE Author (
    AuthorID INT PRIMARY KEY IDENTITY(1,1),
    AuthorName NVARCHAR(100) NOT NULL
);
-- bang Category
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);




-- bang book
CREATE TABLE Book (
    BookID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    PublicationYear INT,
    PublisherID INT,
    CategoryID INT,
    AuthorID INT,
    Image NVARCHAR(255),
    [Description] NVARCHAR(MAX),
    PageCount INT,
    [Language] NVARCHAR(50),
    Quantity INT DEFAULT 1,
    Status BIT DEFAULT 1,
    FOREIGN KEY (PublisherID) REFERENCES Publisher(PublisherID),
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
    FOREIGN KEY (AuthorID) REFERENCES Author(AuthorID)
);

-- bang choeckout 
CREATE TABLE Checkout (
    CheckoutID INT PRIMARY KEY IDENTITY(1,1),
    BookID INT NOT NULL,
    ReaderID INT NOT NULL,
    BrowseDate DATE NOT NULL,       -- ngày mượn
    ReturnDate DATE,                -- ngày trả
    DueDate DATE,                   -- hạn trả
    Status BIT DEFAULT 1,           -- 1: đang mượn, 0: đã trả
    FOREIGN KEY (BookID) REFERENCES Book(BookID),
    FOREIGN KEY (ReaderID) REFERENCES Reader(ReaderID)
);
-- bang phat
CREATE TABLE Fine (
    FineID INT PRIMARY KEY IDENTITY(1,1),
    CheckoutID INT NOT NULL,
    ReaderID INT NOT NULL,
    FineAmount DECIMAL(10,2) NOT NULL,
    FineDate DATE NOT NULL DEFAULT GETDATE(),
    FineType NVARCHAR(100),         -- ví dụ: trễ hạn, hư hỏng
    Status BIT DEFAULT 1,           -- 1: chưa nộp, 0: đã nộp
    FOREIGN KEY (CheckoutID) REFERENCES Checkout(CheckoutID),
    FOREIGN KEY (ReaderID) REFERENCES Reader(ReaderID)
);
-- bnag yc muon sach
CREATE TABLE Reservation (
    ReservationID INT PRIMARY KEY IDENTITY(1,1),
    ReaderID INT NOT NULL,
    BookID INT NOT NULL,
    ReservationDate DATE NOT NULL DEFAULT GETDATE(),
    Status BIT DEFAULT 1,              -- 1: đang chờ, 0: hủy
    PickupDate DATE,
    FOREIGN KEY (ReaderID) REFERENCES Reader(ReaderID),
    FOREIGN KEY (BookID) REFERENCES Book(BookID)
);
-- bang ghi log he thong
CREATE TABLE AuditLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeID INT NOT NULL,
    [Timestamp] DATETIME DEFAULT GETDATE(),
    [Description] NVARCHAR(500),
    TableName NVARCHAR(100),
    RecordID INT,
    OldData NVARCHAR(MAX),
    NewData NVARCHAR(MAX),
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);




