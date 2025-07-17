<div align="center">

# BookshopMVC 📚

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![API](https://img.shields.io/badge/API-REST-orange)

A modern, secure, and scalable book management system built with ASP.NET Core MVC. Complete backend API for managing books, authors, genres, customers, shopping carts, and orders with authentication and role-based authorization.

**Live API Demo!** ➡️ `https://localhost:7123/swagger`

</div>

## Features

- 📖 **Complete Book Management** - CRUD operations with detailed metadata and search
- 👥 **Author Profiles** - Manage author information and their published works
- 🏷️ **Genre Classification** - Organize books by categories and genres
- 🛒 **Shopping Cart System** - Add, update, remove items with persistent storage
- 📋 **Order Management** - Complete order processing and history tracking
- 🔐 **JWT Authentication** - Secure login system with role-based access control
- 📚 **Interactive API Docs** - Swagger/OpenAPI for testing and documentation
- 🌱 **Database Seeding** - Pre-populated test data for immediate development
- 📱 **RESTful Design** - Clean API endpoints following REST principles

## Built With

- [ASP.NET Core 9.0](https://dotnet.microsoft.com/apps/aspnet) - Web framework
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) - Object-relational mapping
- [SQL Server](https://www.microsoft.com/sql-server) - Database system
- [JWT Bearer](https://jwt.io/) - Authentication tokens
- [Swagger/OpenAPI](https://swagger.io/) - API documentation
- [AutoMapper](https://automapper.org/) - Object mapping
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net) - Password hashing

## Getting Started

1. **Clone the repository**

   ```bash
   git clone https://github.com/yourusername/BookshopMVC.git
   cd BookshopMVC
   ```

2. **Install dependencies**

   ```bash
   dotnet restore
   ```

3. **Update connection string** in `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookshopMVC;Trusted_Connection=true"
     }
   }
   ```

4. **Run database migrations**

   ```bash
   cd BookshopMVC
   dotnet ef database update
   ```

5. **Start the application**
   ```bash
   dotnet run
   ```

🎉 **That's it!** Visit `https://localhost:7123/swagger` for the interactive API documentation.

## Quick Start

**Default Test Accounts:**

- **Admin**: `admin@bookshop.com` / `Admin123!`
- **Customer**: `customer@bookshop.com` / `Customer123!`

**Key API Endpoints:**

- `POST /api/auth/login` - User authentication
- `GET /api/books` - Browse books (public)
- `POST /api/cart/add` - Add items to cart (auth required)
- `POST /api/orders` - Place orders (auth required)

## Project Structure

```
BookshopMVC/
├── Controllers/          # API endpoints
├── Models/              # Data models
├── Data/                # Database context & seeding
├── Migrations/          # EF Core migrations
└── wwwroot/            # Static files
```

## API Authentication

The API uses JWT tokens for authentication:

1. **Login** with test credentials to get a token
2. **Add Bearer token** to Authorization header
3. **Access protected endpoints** with authenticated requests

**Authorization Levels:**

- **Public** - Book browsing, user registration/login
- **Customer** - Cart management, order placement
- **Admin** - Full CRUD operations on all resources

## Database Schema

**Core Entities:**

- **Books** - Title, ISBN, Price, Publication info
- **Authors** - Name, biography, birth date
- **Genres** - Categories and descriptions
- **Users** - Authentication and profile data
- **Orders** - Purchase history and order items
- **Cart** - Shopping cart with item quantities

## Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add some amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

**Areas for Contribution:**

- 🎨 Frontend development (React, Angular, Blazor)
- 💳 Payment integration
- 📧 Email notifications
- 📊 Analytics dashboard
- 🔍 Advanced search features

## Bug / Feature Request

Found a bug or want to request a new feature? Tell me about it [here](https://github.com/yourusername/BookshopMVC/issues/new)!

## Acknowledgements

Built with ♥ using ASP.NET Core and modern development practices. This project is for educational and demonstration purposes.

---

⭐ **Star this repository** if you find it helpful!
