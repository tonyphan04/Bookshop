# Bookstore Database Models - Based on Professional Schema

This document outlines the database models implemented for the BookshopMVC application, based on the professional bookstore schema from dbmodeller.net.

## Core Book-Related Models

### 1. Book
- **Primary Fields**: Title, ISBN13, ISBN10, Description, PublicationDate, PageCount, EditionNumber
- **Pricing**: Price, AverageRating, ReviewCount
- **Inventory**: Stock, IsActive
- **Relationships**: Genre, Publisher, Language, Authors (many-to-many), Reviews, Stock Movements, Suppliers

### 2. Author
- **Fields**: FirstName, LastName, Biography, BirthDate, DeathDate, Nationality, Website
- **Relationships**: Books (many-to-many through AuthorBook)

### 3. AuthorBook (Junction Table)
- **Purpose**: Many-to-many relationship between Authors and Books
- **Fields**: AuthorOrder, Role (Author, Editor, Translator, etc.)

### 4. Genre
- **Fields**: Name, Description, IsActive, DisplayOrder
- **Purpose**: Categorize books by literary genre (Fiction, Non-Fiction, Technology, etc.)

### 5. Publisher
- **Fields**: Name, Description, Website, Email, Phone, Address, City, Country
- **Purpose**: Track book publishers and their information

### 6. Language
- **Fields**: Name, Code (ISO 639-1), CultureCode (e.g., en-US)
- **Purpose**: Track the language of books

## Customer & Order Models

### 7. Customer
- **Personal Info**: FirstName, LastName, Email, Phone, BirthDate, Gender
- **Address**: Address, City, PostalCode, Country
- **Account**: IsActive, RegistrationDate, LastLoginDate, NewsletterSubscription
- **Relationships**: Orders, Reviews

### 8. Order
- **Financial**: SubTotal, Tax, ShippingCost, TotalPrice
- **Status**: OrderStatus (Pending, Confirmed, Processing, Shipped, Delivered, Cancelled, Returned)
- **Shipping**: ShippingAddress, ShippingCity, ShippingPostalCode, ShippingCountry
- **Tracking**: ShippedDate, DeliveredDate, TrackingNumber
- **Relationships**: Customer, OrderItems

### 9. OrderItem
- **Fields**: Amount (quantity), LinePrice (historical price), TotalPrice (calculated)
- **Purpose**: Individual items within an order with preserved pricing

## Review & Rating System

### 10. Review
- **Content**: Rating (1-5), Title, Comment
- **Metadata**: IsVerifiedPurchase, IsApproved, HelpfulCount
- **Relationships**: Book, Customer

## Inventory Management

### 11. StockMovement
- **Purpose**: Track all stock changes (purchases, sales, returns, adjustments)
- **Fields**: MovementType, Quantity, StockAfterMovement, Notes, UnitCost
- **Relationships**: Book, Order (if applicable)

### 12. Supplier
- **Contact Info**: Name, ContactPerson, Email, Phone, Address
- **Relationships**: Books (many-to-many through SupplierBook)

### 13. SupplierBook (Junction Table)
- **Purpose**: Track which suppliers provide which books
- **Fields**: SupplierPrice, SupplierProductCode, MinOrderQuantity, LeadTimeDays
- **Business Logic**: IsPreferredSupplier flag

## General Category System

### 14. Category
- **Purpose**: General product categories (can expand beyond books)
- **Features**: Hierarchical structure with ParentCategory
- **Fields**: Name, Description, IsActive, DisplayOrder

## Key Features Implemented

1. **Professional ISBN Handling**: Both ISBN-13 (modern) and ISBN-10 (legacy)
2. **Comprehensive Author Management**: Multiple authors per book with roles
3. **Advanced Order System**: Multiple statuses, shipping tracking, tax calculation
4. **Review System**: Customer reviews with verification and moderation
5. **Inventory Tracking**: Complete stock movement history
6. **Supplier Management**: Multiple suppliers per book with pricing
7. **Hierarchical Categories**: Flexible category system for future expansion

## Database Relationships Summary

- **Books ↔ Authors**: Many-to-Many (through AuthorBook)
- **Books ↔ Suppliers**: Many-to-Many (through SupplierBook)  
- **Books → Genre**: Many-to-One
- **Books → Publisher**: Many-to-One
- **Books → Language**: Many-to-One
- **Customers ↔ Orders**: One-to-Many
- **Orders ↔ OrderItems**: One-to-Many
- **Books ↔ Reviews**: One-to-Many
- **Customers ↔ Reviews**: One-to-Many
- **Books ↔ StockMovements**: One-to-Many

This schema provides a robust foundation for a professional bookstore application with room for future expansion and comprehensive business feature support.
