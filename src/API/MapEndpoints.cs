using Application.CQRS.Commands;
using Application.Extensions;
using Application.MediatR.Commands;
using Application.MediatR.Queries;
using Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API
{
    public static class MapEndpoints
    {
        public static void MapGetEnpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("api/Book/{page}", async (IMediator mediator, int page) =>
            {
                var books = await mediator.Send(new GetBooksQuery(page));

                var apiResponse = new PaginatedApiResponse("Paged Books", true, books, Convert.ToInt32(HttpStatusCode.OK), books.SelectedPage, books.TotalPages, books.PageSize, books.ItemCount);
                return Results.Json(apiResponse,statusCode: apiResponse.StatusCode);
            });

            app.MapGet("api/Book/Category/{categoryId}/{page}", async (IMediator mediator, int categoryId, int page) =>
            {
                var books = await mediator.Send(new GetBooksByCategoryQuery(categoryId, page));

                var apiResponse = new PaginatedApiResponse("Books with category", true, books, Convert.ToInt32(HttpStatusCode.OK), books.SelectedPage, books.TotalPages, books.PageSize, books.ItemCount);
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            app.MapGet("api/Book/Name/{name}/{page}", async (IMediator mediator, string name, int page) =>
            {
                var books = await mediator.Send(new GetBooksByNameQuery(name, page));

                var apiResponse = new PaginatedApiResponse("Books with Name search", true, books, Convert.ToInt32(HttpStatusCode.OK), books.SelectedPage, books.TotalPages, books.PageSize, books.ItemCount);
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            app.MapGet("api/Categories", async (IMediator mediator) =>
            {
                var categories = await mediator.Send(new GetCategoriesQuery());

                var apiResponse = new ApiResponse("Categories", true, categories, Convert.ToInt32(HttpStatusCode.OK));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });



        }
        public static void MapAdminEnpoints(this IEndpointRouteBuilder app)
        {
            var adminRoutes = app.MapGroup("api/Admin").RequireAuthorization("Admin_Policy");

            adminRoutes.MapPost("/Book", async (IMediator mediator,AddBookCommand command) =>
            {
                
                await mediator.Send(command);

                var apiResponse = new ApiResponse("Book added Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            adminRoutes.MapPut("/Book/Update", async (IMediator mediator, UpdateBookCommand command) =>
            {
                await mediator.Send(command);

                var apiResponse = new ApiResponse("Book Updated Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            adminRoutes.MapDelete("/Book/Delete/{bid}", async (IMediator mediator, string bid) =>
            {
                await mediator.Send(new DeleteBookCommand(bid));

                var apiResponse = new ApiResponse("Book Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            adminRoutes.MapPut("/Book/Sale", async (IMediator mediator, AddBookSaleCommand command) =>
            {
                await mediator.Send(command);

                var apiResponse = new ApiResponse("Sale on Book Updated Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            adminRoutes.MapPost("/Book/Photo/{bid}", async (IMediator mediator, string bid, [FromForm] IFormFile file) =>
            {

                await mediator.Send(new AddPhotoToBookCommand(bid.GetGuid(), file));

                var apiResponse = new ApiResponse("Photo added Successfully", true, null, Convert.ToInt32(HttpStatusCode.Created));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            }).DisableAntiforgery();

            adminRoutes.MapDelete("/Book/Photo/{bid}", async (IMediator mediator, string bid) =>
            {
                await mediator.Send(new RemovePhotoFromBookCommand(bid));

                var apiResponse = new ApiResponse("Photo Deleted Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });
        }
        public static void MapUserEnpoints(this IEndpointRouteBuilder app)
        {
            var userRoutes = app.MapGroup("api/User").RequireAuthorization("User_Policy");

            userRoutes.MapPost("/Book/Rating", async (IMediator mediator, AddRatingCommand command) =>
            {
                await mediator.Send(command);

                var apiResponse = new ApiResponse("User rating on book is set", true, null, Convert.ToInt32(HttpStatusCode.Created));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            userRoutes.MapPost("/Wishlist/Add", async (string userId, string bookId, IServiceManager _serviceManager) =>
            {
                await _serviceManager.WishlistService.AddBookToWishlist(userId, bookId);

                var apiResponse = new ApiResponse("Book successfully added to wishlish", true, null, Convert.ToInt32(HttpStatusCode.Created));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            userRoutes.MapDelete("/Wishlist/Remove", async (string userId, string bookId, IServiceManager _serviceManager) =>
            {
                await _serviceManager.WishlistService.RemoveBookFromWishlist(userId, bookId);

                var apiResponse = new ApiResponse("Book successfully removed from wishlish", true, null, Convert.ToInt32(HttpStatusCode.OK));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

            userRoutes.MapGet("/Wishlist/{userId}", async (string userId, IServiceManager _serviceManager) =>
            {
                var wishlist = await _serviceManager.WishlistService.GetWishlistForUser(userId);

                var apiResponse = new ApiResponse("User Wishlist", true, wishlist, Convert.ToInt32(HttpStatusCode.OK));
                return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
            });

        }
    }
}
