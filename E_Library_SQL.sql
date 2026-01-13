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

-- ==========================================
-- 🔹 INSERT DATA FOR eLibrary DATABASE
-- ==========================================

-- 1️⃣ Permission
INSERT INTO Permission (Link, Description) VALUES
('/dashboard', N'Truy cập trang tổng quan'),
('/books', N'Xem danh sách sách'),
('/books/add', N'Thêm sách mới'),
('/books/edit', N'Chỉnh sửa thông tin sách'),
('/books/delete', N'Xóa sách'),
('/readers', N'Xem danh sách độc giả'),
('/employees', N'Quản lý nhân viên'),
('/checkout', N'Quản lý mượn/trả sách'),
('/fines', N'Quản lý tiền phạt'),
('/logs', N'Xem nhật ký hệ thống');

-- 2️⃣ Role
INSERT INTO Role (RoleName) VALUES
('Admin'),
('Librarian'),
('Staff'),
('Guest');





-- 7️⃣ Publisher
INSERT INTO Publisher (PublisherName, [Address]) VALUES
(N'NXB Trẻ', N'HCM'),
(N'NXB Kim Đồng', N'Hà Nội'),
(N'NXB Lao Động', N'Hà Nội'),
(N'NXB Văn Học', N'HCM'),
(N'NXB Giáo Dục', N'Hà Nội'),
(N'NXB Khoa Học', N'HCM'),
(N'NXB Thanh Niên', N'Hà Nội'),
(N'NXB Hội Nhà Văn', N'HCM'),
(N'NXB Tổng Hợp', N'HCM'),
(N'NXB Văn Lang', N'Hà Nội');

-- 8️⃣ Author
INSERT INTO Author (AuthorName) VALUES
(N'Nguyễn Nhật Ánh'),
(N'Tô Hoài'),
(N'Nam Cao'),
(N'Vũ Trọng Phụng'),
(N'Arthur Conan Doyle'),
(N'J.K. Rowling'),
(N'George Orwell'),
(N'Harper Lee'),
(N'Dan Brown'),
(N'Ernest Hemingway');

-- 9️⃣ Category
INSERT INTO Category (CategoryName) VALUES
(N'Truyện thiếu nhi'),
(N'Truyện ngắn'),
(N'Tiểu thuyết'),
(N'Truyện trinh thám'),
(N'Khoa học'),
(N'Giáo dục'),
(N'Lịch sử'),
(N'Tâm lý'),
(N'Chính trị'),
(N'Văn học nước ngoài');

-- 🔟 Book
INSERT INTO Book (Title, PublicationYear, PublisherID, CategoryID, AuthorID, Image, [Description], PageCount, [Language], Quantity)
VALUES
(N'Mắt Biếc', 1990, 1, 3, 1, 'matbiec.jpg', N'Tiểu thuyết tuổi học trò', 250, N'Tiếng Việt', 10),
(N'Dế Mèn Phiêu Lưu Ký', 1941, 2, 1, 2, 'demen.jpg', N'Truyện thiếu nhi nổi tiếng', 200, N'Tiếng Việt', 8),
(N'Chí Phèo', 1941, 3, 2, 3, 'chipheo.jpg', N'Truyện hiện thực phê phán', 180, N'Tiếng Việt', 6),
(N'Số Đỏ', 1936, 4, 2, 4, 'sodo.jpg', N'Truyện châm biếm nổi tiếng', 220, N'Tiếng Việt', 5),
(N'Sherlock Holmes', 1892, 5, 4, 5, 'holmes.jpg', N'Truyện trinh thám kinh điển', 320, N'English', 7),
(N'Harry Potter', 1997, 5, 3, 6, 'harry.jpg', N'Tiểu thuyết kỳ ảo', 400, N'English', 10),
(N'1984', 1949, 6, 10, 7, '1984.jpg', N'Tiểu thuyết phản địa đàng', 300, N'English', 6),
(N'To Kill a Mockingbird', 1960, 7, 10, 8, 'mockingbird.jpg', N'Tác phẩm nổi tiếng về nhân quyền', 280, N'English', 5),
(N'Da Vinci Code', 2003, 8, 4, 9, 'davinci.jpg', N'Truyện trinh thám nổi tiếng', 350, N'English', 9),
(N'Old Man and The Sea', 1952, 9, 10, 10, 'oldman.jpg', N'Tiểu thuyết kinh điển', 200, N'English', 8);



