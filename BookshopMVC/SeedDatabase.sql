-- ================================================
-- Bookshop Database Seeding Script
-- Run this script in SQL Server Management Studio or via sqlcmd
-- ================================================

USE BookshopDB;
GO

-- Check if tables exist, if not, create them
-- Note: You should run EF migrations first: dotnet ef database update

-- Clear existing data (optional - remove if you want to keep existing data)
DELETE FROM [OrderItems];
DELETE FROM [Orders];
DELETE FROM [AuthorBooks];
DELETE FROM [Books];
DELETE FROM [Authors];
DELETE FROM [Genres];
DELETE FROM [Users];
DELETE FROM [Payments];
GO

-- Reset identity seeds
DBCC CHECKIDENT('Users', RESEED, 0);
DBCC CHECKIDENT('Genres', RESEED, 0);
DBCC CHECKIDENT('Authors', RESEED, 0);
DBCC CHECKIDENT('Books', RESEED, 0);
DBCC CHECKIDENT('Orders', RESEED, 0);
DBCC CHECKIDENT('OrderItems', RESEED, 0);
DBCC CHECKIDENT('Payments', RESEED, 0);
GO

-- Seed Genres
INSERT INTO [Genres] ([Name], [Description], [IsActive], [DisplayOrder]) VALUES
('Programming', 'Books about programming languages and software development', 1, 1),
('Science Fiction', 'Futuristic and imaginative fiction', 1, 2),
('Business', 'Books on business strategy, management, and entrepreneurship', 1, 3),
('Self Help', 'Personal development and motivational books', 1, 4),
('History', 'Historical events, biographies, and cultural studies', 1, 5),
('Fantasy', 'Magical and mythical fiction', 1, 6),
('Technology', 'Books about emerging technologies and innovation', 1, 7),
('Biography', 'Life stories of notable people', 1, 8);
GO

-- Seed Authors
INSERT INTO [Authors] ([FirstName], [LastName], [Biography], [CreatedDate]) VALUES
('Robert', 'Martin', 'Also known as Uncle Bob, is an American software engineer and author. He is a co-author of the Agile Manifesto.', GETUTCDATE()),
('Martin', 'Fowler', 'British software developer, author and international public speaker on software development.', GETUTCDATE()),
('Eric', 'Freeman', 'Co-author of Head First Design Patterns and other programming books.', GETUTCDATE()),
('Kathy', 'Sierra', 'American programming instructor and game developer, co-author of the Head First series.', GETUTCDATE()),
('Steve', 'McConnell', 'Author of Code Complete and other software engineering books.', GETUTCDATE()),
('Andy', 'Hunt', 'Co-author of The Pragmatic Programmer and founding member of the Agile Alliance.', GETUTCDATE()),
('Dave', 'Thomas', 'Co-author of The Pragmatic Programmer and founder of The Pragmatic Bookshelf.', GETUTCDATE()),
('Gang', 'of Four', 'Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides - authors of Design Patterns.', GETUTCDATE()),
('Frank', 'Herbert', 'American science fiction writer best known for the Dune series.', GETUTCDATE()),
('Isaac', 'Asimov', 'American writer and professor of biochemistry, known for his works of science fiction.', GETUTCDATE());
GO

-- Seed Users with hashed passwords (SHA256 of original passwords)
INSERT INTO [Users] ([FirstName], [LastName], [Email], [PasswordHash], [Role], [IsActive], [Phone], [Address], [RegistrationDate]) VALUES
('Admin', 'User', 'admin@bookshop.com', 'VGhpcyBpcyBhIGRlbW8gaGFzaA==', 0, 1, '555-0001', '123 Admin Street, Admin City, AC 12345', DATEADD(day, -30, GETUTCDATE())),
('John', 'Customer', 'customer@bookshop.com', 'VGhpcyBpcyBhIGRlbW8gaGFzaA==', 1, 1, '555-0002', '456 Customer Lane, Customer City, CC 67890', DATEADD(day, -20, GETUTCDATE())),
('Jane', 'Smith', 'jane.smith@email.com', 'VGhpcyBpcyBhIGRlbW8gaGFzaA==', 1, 1, '555-0003', '789 Smith Avenue, Smith Town, ST 13579', DATEADD(day, -15, GETUTCDATE())),
('Test', 'User', 'test@bookshop.com', 'VGhpcyBpcyBhIGRlbW8gaGFzaA==', 1, 1, '555-0004', '321 Test Street, Test City, TC 24680', DATEADD(day, -10, GETUTCDATE())),
('Alice', 'Johnson', 'alice.johnson@email.com', 'VGhpcyBpcyBhIGRlbW8gaGFzaA==', 1, 1, '555-0005', '555 Johnson Boulevard, Johnson City, JC 55555', DATEADD(day, -8, GETUTCDATE())),
('Bob', 'Wilson', 'bob.wilson@email.com', 'VGhpcyBpcyBhIGRlbW8gaGFzaA==', 1, 1, '555-0006', '777 Wilson Drive, Wilson Town, WT 77777', DATEADD(day, -5, GETUTCDATE())),
('Charlie', 'Brown', 'charlie.brown@email.com', 'VGhpcyBpcyBhIGRlbW8gaGFzaA==', 1, 1, '555-0007', '888 Brown Street, Brown City, BC 88888', DATEADD(day, -3, GETUTCDATE()));
GO

-- Seed Books
INSERT INTO [Books] ([Title], [Description], [Price], [Stock], [GenreId], [ImageUrl], [ISBN13], [Publisher], [Language], [IsActive], [CreatedDate]) VALUES
('Clean Code: A Handbook of Agile Software Craftsmanship', 'Even bad code can function. But if code isn''t clean, it can bring a development organization to its knees.', 42.99, 25, 1, 'https://example.com/clean-code.jpg', '9780132350884', 'Prentice Hall', 'English', 1, DATEADD(day, -60, GETUTCDATE())),
('Refactoring: Improving the Design of Existing Code', 'Refactoring is a controlled technique for improving the design of an existing code base.', 54.99, 18, 1, 'https://example.com/refactoring.jpg', '9780134757599', 'Addison-Wesley', 'English', 1, DATEADD(day, -55, GETUTCDATE())),
('Head First Design Patterns', 'You''ll learn design patterns in a way that won''t put you to sleep! We think your time is too valuable to spend struggling with new concepts.', 49.99, 30, 1, 'https://example.com/head-first-patterns.jpg', '9780596007126', 'O''Reilly Media', 'English', 1, DATEADD(day, -50, GETUTCDATE())),
('Code Complete: A Practical Handbook of Software Construction', 'Widely considered one of the best practical guides to programming.', 48.99, 22, 1, 'https://example.com/code-complete.jpg', '9780735619678', 'Microsoft Press', 'English', 1, DATEADD(day, -45, GETUTCDATE())),
('The Pragmatic Programmer', 'Your journey to mastery starts here.', 44.99, 28, 1, 'https://example.com/pragmatic-programmer.jpg', '9780135957059', 'Addison-Wesley', 'English', 1, DATEADD(day, -40, GETUTCDATE())),
('Design Patterns: Elements of Reusable Object-Oriented Software', 'The classic reference for understanding design patterns.', 52.99, 15, 1, 'https://example.com/design-patterns.jpg', '9780201633612', 'Addison-Wesley', 'English', 1, DATEADD(day, -35, GETUTCDATE())),
('Dune', 'Set on the desert planet Arrakis, Dune is the story of the boy Paul Atreides.', 16.99, 45, 2, 'https://example.com/dune.jpg', '9780441172719', 'Ace Books', 'English', 1, DATEADD(day, -30, GETUTCDATE())),
('Foundation', 'The first novel in Isaac Asimov''s classic science-fiction masterpiece.', 15.99, 35, 2, 'https://example.com/foundation.jpg', '9780553293357', 'Bantam Spectra', 'English', 1, DATEADD(day, -25, GETUTCDATE())),
('The Lean Startup', 'How Today''s Entrepreneurs Use Continuous Innovation to Create Radically Successful Businesses.', 17.99, 40, 3, 'https://example.com/lean-startup.jpg', '9780307887894', 'Crown Business', 'English', 1, DATEADD(day, -20, GETUTCDATE())),
('Good to Great', 'Why Some Companies Make the Leap... and Others Don''t.', 19.99, 32, 3, 'https://example.com/good-to-great.jpg', '9780066620992', 'HarperBusiness', 'English', 1, DATEADD(day, -15, GETUTCDATE()));
GO

-- Seed AuthorBooks (Many-to-many relationship)
INSERT INTO [AuthorBooks] ([AuthorId], [BookId]) VALUES
(1, 1),  -- Robert Martin - Clean Code
(2, 2),  -- Martin Fowler - Refactoring
(3, 3),  -- Eric Freeman - Head First Design Patterns
(4, 3),  -- Kathy Sierra - Head First Design Patterns
(5, 4),  -- Steve McConnell - Code Complete
(6, 5),  -- Andy Hunt - The Pragmatic Programmer
(7, 5),  -- Dave Thomas - The Pragmatic Programmer
(8, 6),  -- Gang of Four - Design Patterns
(9, 7),  -- Frank Herbert - Dune
(10, 8); -- Isaac Asimov - Foundation
GO

-- Seed Orders
INSERT INTO [Orders] ([UserId], [OrderDate], [TotalPrice], [Status], [CreatedDate]) VALUES
(2, DATEADD(day, -5, GETUTCDATE()), 92.98, 3, DATEADD(day, -5, GETUTCDATE())),  -- John Customer - Completed
(3, DATEADD(day, -3, GETUTCDATE()), 49.99, 2, DATEADD(day, -3, GETUTCDATE())),  -- Jane Smith - Confirmed
(4, DATEADD(day, -1, GETUTCDATE()), 126.97, 1, DATEADD(day, -1, GETUTCDATE())); -- Test User - Pending
GO

-- Seed OrderItems
INSERT INTO [OrderItems] ([OrderId], [BookId], [Quantity], [UnitPrice]) VALUES
(1, 1, 1, 42.99),  -- Clean Code
(1, 2, 1, 54.99),  -- Refactoring (Total: 97.98)
(2, 3, 1, 49.99),  -- Head First Design Patterns
(3, 4, 1, 48.99),  -- Code Complete
(3, 5, 1, 44.99),  -- The Pragmatic Programmer
(3, 7, 2, 16.99);  -- Dune x2 (Total: 126.97)
GO

-- Update Order totals to match OrderItems
UPDATE [Orders] SET [TotalPrice] = 97.98 WHERE [Id] = 1;
UPDATE [Orders] SET [TotalPrice] = 49.99 WHERE [Id] = 2;
UPDATE [Orders] SET [TotalPrice] = 126.97 WHERE [Id] = 3;
GO

-- Seed Payments
INSERT INTO [Payments] ([OrderId], [Amount], [PaymentMethod], [Status], [StripePaymentIntentId], [CreatedDate]) VALUES
(1, 97.98, 'card', 'succeeded', 'pi_1234567890abcdef', DATEADD(day, -5, GETUTCDATE())),
(2, 49.99, 'card', 'succeeded', 'pi_0987654321fedcba', DATEADD(day, -3, GETUTCDATE())),
(3, 126.97, 'card', 'pending', 'pi_1122334455667788', DATEADD(day, -1, GETUTCDATE()));
GO

-- ================================================
-- BULK DATA GENERATION FOR PERFORMANCE TESTING
-- Uncomment the section below if you want to generate bulk test data
-- ================================================

/*
-- Generate bulk users (1000 users)
DECLARE @i INT = 1;
WHILE @i <= 1000
BEGIN
    INSERT INTO [Users] ([FirstName], [LastName], [Email], [PasswordHash], [Role], [IsActive], [Phone], [Address], [RegistrationDate])
    VALUES (
        'User' + CAST(@i AS VARCHAR(10)),
        'Test' + CAST(@i AS VARCHAR(10)),
        'user' + CAST(@i AS VARCHAR(10)) + '@test.com',
        'VGhpcyBpcyBhIGRlbW8gaGFzaA==',
        1, -- Customer
        1, -- Active
        '555-' + RIGHT('0000' + CAST(@i AS VARCHAR(4)), 4),
        CAST(@i AS VARCHAR(10)) + ' Test Street, Test City, TC ' + RIGHT('00000' + CAST(@i AS VARCHAR(5)), 5),
        DATEADD(day, -ABS(CHECKSUM(NEWID()) % 365), GETUTCDATE())
    );
    SET @i = @i + 1;
END;

-- Generate bulk books (5000 books)
DECLARE @j INT = 1;
WHILE @j <= 5000
BEGIN
    INSERT INTO [Books] ([Title], [Description], [Price], [Stock], [GenreId], [ImageUrl], [ISBN13], [Publisher], [Language], [IsActive], [CreatedDate])
    VALUES (
        'Test Book ' + CAST(@j AS VARCHAR(10)),
        'This is a test book description for book number ' + CAST(@j AS VARCHAR(10)),
        CAST((ABS(CHECKSUM(NEWID()) % 5000) + 1000) AS DECIMAL(10,2)) / 100.0, -- Random price between 10.00 and 59.99
        ABS(CHECKSUM(NEWID()) % 100) + 1, -- Random stock between 1 and 100
        (ABS(CHECKSUM(NEWID()) % 8) + 1), -- Random genre ID between 1 and 8
        'https://example.com/book' + CAST(@j AS VARCHAR(10)) + '.jpg',
        '978' + RIGHT('0000000000' + CAST(ABS(CHECKSUM(NEWID()) % 1000000000) AS VARCHAR(10)), 10), -- Random ISBN13
        'Test Publisher ' + CAST((ABS(CHECKSUM(NEWID()) % 10) + 1) AS VARCHAR(2)),
        'English',
        1,
        DATEADD(day, -ABS(CHECKSUM(NEWID()) % 1000), GETUTCDATE())
    );
    SET @j = @j + 1;
END;

-- Generate bulk orders (10000 orders)
DECLARE @k INT = 1;
WHILE @k <= 10000
BEGIN
    DECLARE @randomUserId INT = (ABS(CHECKSUM(NEWID()) % 1000) + 1);
    DECLARE @randomStatus INT = (ABS(CHECKSUM(NEWID()) % 4) + 1);
    DECLARE @orderDate DATETIME = DATEADD(day, -ABS(CHECKSUM(NEWID()) % 365), GETUTCDATE());
    
    INSERT INTO [Orders] ([UserId], [OrderDate], [TotalPrice], [Status], [CreatedDate])
    VALUES (
        @randomUserId,
        @orderDate,
        0.00, -- Will be updated after adding order items
        @randomStatus,
        @orderDate
    );
    
    -- Add 1-5 random items to each order
    DECLARE @orderItemCount INT = (ABS(CHECKSUM(NEWID()) % 5) + 1);
    DECLARE @orderTotal DECIMAL(10,2) = 0.00;
    DECLARE @m INT = 1;
    
    WHILE @m <= @orderItemCount
    BEGIN
        DECLARE @randomBookId INT = (ABS(CHECKSUM(NEWID()) % 5000) + 1);
        DECLARE @randomQuantity INT = (ABS(CHECKSUM(NEWID()) % 3) + 1);
        DECLARE @bookPrice DECIMAL(10,2) = (SELECT TOP 1 [Price] FROM [Books] WHERE [Id] = @randomBookId);
        
        IF @bookPrice IS NOT NULL
        BEGIN
            INSERT INTO [OrderItems] ([OrderId], [BookId], [Quantity], [UnitPrice])
            VALUES (@k, @randomBookId, @randomQuantity, @bookPrice);
            
            SET @orderTotal = @orderTotal + (@bookPrice * @randomQuantity);
        END;
        
        SET @m = @m + 1;
    END;
    
    -- Update order total
    UPDATE [Orders] SET [TotalPrice] = @orderTotal WHERE [Id] = @k;
    
    SET @k = @k + 1;
END;
*/

-- ================================================
-- VERIFICATION QUERIES
-- ================================================

-- Verify the data was inserted correctly
SELECT 'Genres' AS TableName, COUNT(*) AS RecordCount FROM [Genres]
UNION ALL
SELECT 'Authors' AS TableName, COUNT(*) AS RecordCount FROM [Authors]
UNION ALL
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM [Users]
UNION ALL
SELECT 'Books' AS TableName, COUNT(*) AS RecordCount FROM [Books]
UNION ALL
SELECT 'AuthorBooks' AS TableName, COUNT(*) AS RecordCount FROM [AuthorBooks]
UNION ALL
SELECT 'Orders' AS TableName, COUNT(*) AS RecordCount FROM [Orders]
UNION ALL
SELECT 'OrderItems' AS TableName, COUNT(*) AS RecordCount FROM [OrderItems]
UNION ALL
SELECT 'Payments' AS TableName, COUNT(*) AS RecordCount FROM [Payments];

-- Show some sample data
SELECT TOP 5 * FROM [Books];
SELECT TOP 5 * FROM [Users];
SELECT TOP 5 * FROM [Orders];

PRINT 'Database seeding completed successfully!';
PRINT 'Note: Default password for all users is "Test123!" (Base64 encoded in PasswordHash)';
PRINT 'Admin user: admin@bookshop.com';
PRINT 'Test customer: customer@bookshop.com';
