-- ================================================
-- Bookshop Database Seeding Script (CORRECTED VERSION)
-- Run this script in SQL Server Management Studio or via sqlcmd
-- ================================================

USE BookshopDB;
GO

-- Clear existing data (optional - remove if you want to keep existing data)
DELETE FROM [OrderItems];
DELETE FROM [Orders];
DELETE FROM [AuthorBooks];
DELETE FROM [Books];
DELETE FROM [Authors];
DELETE FROM [Genres];
DELETE FROM [Users];
DELETE FROM [Payments];
DELETE FROM [CartItems];
GO

-- Reset identity seeds
DBCC CHECKIDENT('Users', RESEED, 0);
DBCC CHECKIDENT('Genres', RESEED, 0);
DBCC CHECKIDENT('Authors', RESEED, 0);
DBCC CHECKIDENT('Books', RESEED, 0);
DBCC CHECKIDENT('Orders', RESEED, 0);
DBCC CHECKIDENT('OrderItems', RESEED, 0);
DBCC CHECKIDENT('Payments', RESEED, 0);
DBCC CHECKIDENT('CartItems', RESEED, 0);
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

-- Seed Users with hashed passwords (placeholder Base64 encoded password)
-- Note: In production, use proper password hashing like bcrypt or Argon2
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
('Good to Great', 'Why Some Companies Make the Leap... and Others Don''t.', 19.99, 32, 3, 'https://example.com/good-to-great.jpg', '9780066620992', 'HarperBusiness', 'English', 1, DATEADD(day, -15, GETUTCDATE())),
('The Lord of the Rings', 'The epic fantasy adventure that started it all.', 29.99, 50, 6, 'https://example.com/lotr.jpg', '9780544003415', 'Houghton Mifflin', 'English', 1, DATEADD(day, -10, GETUTCDATE())),
('1984', 'George Orwell''s dystopian masterpiece.', 13.99, 60, 2, 'https://example.com/1984.jpg', '9780452284234', 'Plume', 'English', 1, DATEADD(day, -5, GETUTCDATE()));
GO

-- Seed AuthorBooks (Many-to-many relationship) - Include AuthorOrder
INSERT INTO [AuthorBooks] ([AuthorId], [BookId], [AuthorOrder]) VALUES
(1, 1, 1),   -- Robert Martin - Clean Code
(2, 2, 1),   -- Martin Fowler - Refactoring
(3, 3, 1),   -- Eric Freeman - Head First Design Patterns
(4, 3, 2),   -- Kathy Sierra - Head First Design Patterns (co-author)
(5, 4, 1),   -- Steve McConnell - Code Complete
(6, 5, 1),   -- Andy Hunt - The Pragmatic Programmer
(7, 5, 2),   -- Dave Thomas - The Pragmatic Programmer (co-author)
(8, 6, 1),   -- Gang of Four - Design Patterns
(9, 7, 1),   -- Frank Herbert - Dune
(10, 8, 1);  -- Isaac Asimov - Foundation
GO

-- Seed Orders
INSERT INTO [Orders] ([UserId], [OrderDate], [TotalPrice], [Status], [CreatedDate]) VALUES
(2, DATEADD(day, -5, GETUTCDATE()), 97.98, 3, DATEADD(day, -5, GETUTCDATE())),  -- John Customer - Completed
(3, DATEADD(day, -3, GETUTCDATE()), 49.99, 2, DATEADD(day, -3, GETUTCDATE())),  -- Jane Smith - Confirmed
(4, DATEADD(day, -1, GETUTCDATE()), 126.97, 1, DATEADD(day, -1, GETUTCDATE())), -- Test User - Pending
(5, DATEADD(day, -2, GETUTCDATE()), 73.97, 3, DATEADD(day, -2, GETUTCDATE())),  -- Alice Johnson - Completed
(6, DATEADD(day, -4, GETUTCDATE()), 29.99, 2, DATEADD(day, -4, GETUTCDATE()));  -- Bob Wilson - Confirmed
GO

-- Seed OrderItems
INSERT INTO [OrderItems] ([OrderId], [BookId], [Quantity], [UnitPrice]) VALUES
(1, 1, 1, 42.99),  -- Clean Code
(1, 2, 1, 54.99),  -- Refactoring
(2, 3, 1, 49.99),  -- Head First Design Patterns
(3, 4, 1, 48.99),  -- Code Complete
(3, 5, 1, 44.99),  -- The Pragmatic Programmer
(3, 7, 2, 16.99),  -- Dune x2
(4, 8, 1, 15.99),  -- Foundation
(4, 9, 1, 17.99),  -- The Lean Startup
(4, 10, 2, 19.99), -- Good to Great x2
(5, 11, 1, 29.99); -- The Lord of the Rings
GO

-- Seed Payments with correct enum values
-- PaymentMethod: 0 = Card, 1 = BankTransfer, 2 = PayPal, 3 = Crypto
-- Status: 0 = Pending, 1 = Succeeded, 2 = Failed, 3 = Cancelled
INSERT INTO [Payments] ([OrderId], [Amount], [PaymentMethod], [Status], [StripePaymentIntentId], [StripeClientSecret], [CreatedDate]) VALUES
(1, 97.98, 0, 1, 'pi_1234567890abcdef', 'pi_1234567890abcdef_secret_abc123', DATEADD(day, -5, GETUTCDATE())),
(2, 49.99, 0, 1, 'pi_0987654321fedcba', 'pi_0987654321fedcba_secret_def456', DATEADD(day, -3, GETUTCDATE())),
(3, 126.97, 0, 0, 'pi_1122334455667788', 'pi_1122334455667788_secret_ghi789', DATEADD(day, -1, GETUTCDATE())),
(4, 73.97, 0, 1, 'pi_aabbccddeeff1122', 'pi_aabbccddeeff1122_secret_jkl012', DATEADD(day, -2, GETUTCDATE())),
(5, 29.99, 0, 1, 'pi_3344556677889900', 'pi_3344556677889900_secret_mno345', DATEADD(day, -4, GETUTCDATE()));
GO

-- Seed some CartItems for testing
INSERT INTO [CartItems] ([UserId], [BookId], [Quantity]) VALUES
(2, 12, 1),  -- John has 1984 in cart
(3, 11, 2),  -- Jane has 2 Lord of the Rings in cart
(4, 1, 1),   -- Test user has Clean Code in cart
(5, 6, 1);   -- Alice has Design Patterns in cart
GO

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
SELECT 'Payments' AS TableName, COUNT(*) AS RecordCount FROM [Payments]
UNION ALL
SELECT 'CartItems' AS TableName, COUNT(*) AS RecordCount FROM [CartItems];

-- Show sample data with relationships
SELECT 
    b.Title, 
    a.FirstName + ' ' + a.LastName AS Author,
    g.Name AS Genre,
    b.Price,
    b.Stock
FROM Books b
JOIN AuthorBooks ab ON b.Id = ab.BookId
JOIN Authors a ON ab.AuthorId = a.Id
JOIN Genres g ON b.GenreId = g.Id
ORDER BY b.Title;

-- Show order summary
SELECT 
    u.FirstName + ' ' + u.LastName AS Customer,
    o.OrderDate,
    o.TotalPrice,
    CASE o.Status 
        WHEN 1 THEN 'Pending'
        WHEN 2 THEN 'Confirmed'
        WHEN 3 THEN 'Completed'
        WHEN 4 THEN 'Cancelled'
        ELSE 'Unknown'
    END AS Status
FROM Orders o
JOIN Users u ON o.UserId = u.Id
ORDER BY o.OrderDate DESC;

PRINT 'Database seeding completed successfully!';
PRINT 'Records inserted:';
PRINT '- 8 Genres';
PRINT '- 10 Authors';
PRINT '- 7 Users';
PRINT '- 12 Books';
PRINT '- 10 Author-Book relationships';
PRINT '- 5 Orders';
PRINT '- 10 Order Items';
PRINT '- 5 Payments';
PRINT '- 4 Cart Items';
PRINT '';
PRINT 'Test login credentials:';
PRINT 'Admin: admin@bookshop.com / Test123!';
PRINT 'Customer: customer@bookshop.com / Test123!';
PRINT '';
PRINT 'Note: Passwords are placeholder Base64 encoded. In production, use proper password hashing!';
