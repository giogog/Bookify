Online Book Store

This project is a back-end application for an online book store, developed using ASP.NET Core Web API. It leverages multiple technologies including Entity Framework, Dapper, and CQRS with MediatR, and follows a clean architecture approach.

Project Structure

The application is built with a clean architecture, featuring:
Entity Framework and Dapper for database interactions, allowing concurrent calls to the database.
A separate Contracts layer for abstractions.
CQRS (Command Query Responsibility Segregation) with MediatR for handling business logic.
Entities, along with the DbContext class for Entity Framework, and Dapper interface implementations, are configured in the Infrastructure layer.
API endpoints are implemented using Minimal APIs in extension classes within the API layer, except for the Account Controller.

Roles


There are two roles in the system:

Admin: Can add, update, and delete books, set sales on specific books, and manage photos of books (add or remove). Admins have full control over the book listings.

User: Can search for books, add them to a wishlist, and rate books. Users have more limited access but can engage with the store's catalog.

Functionality

All of the main functionalities are implemented using CQRS:
CQRS Handlers utilize Repository and Service Managers to perform operations.
The project also includes Notification Handlers for email confirmation and password reset.
Authentication and Authorization
The application uses Microsoft Identity with JWT tokens for secure authentication and authorization.
Services are configured in the ServiceConfiguration.cs file located in the API layer.

Additional Features

Image Management: Admins can manage book images using Cloudinary to save and remove photos.
Concurrent Database Calls: By utilizing both Entity Framework and Dapper, the application can efficiently handle database operations.
