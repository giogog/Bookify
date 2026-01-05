using Application.CQRS.Commands;
using Application.Extensions;
using Application.MediatR.Commands;
using Application.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Endpoints.Book;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/Book/{page}", async (IMediator mediator, int page) =>
        {
            var books = await mediator.Send(new GetBooksQuery(page));

            var apiResponse = new PaginatedApiResponse(
                "Paged Books",
                true,
                books,
                Convert.ToInt32(HttpStatusCode.OK),
                books.SelectedPage,
                books.TotalPages,
                books.PageSize,
                books.ItemCount);

            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        app.MapGet("api/Book/Category/{categoryId}/{page}", async (IMediator mediator, int categoryId, int page) =>
        {
            var books = await mediator.Send(new GetBooksByCategoryQuery(categoryId, page));

            var apiResponse = new PaginatedApiResponse(
                "Books with category",
                true,
                books,
                Convert.ToInt32(HttpStatusCode.OK),
                books.SelectedPage,
                books.TotalPages,
                books.PageSize,
                books.ItemCount);

            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        app.MapGet("api/Book/Name/{name}/{page}", async (IMediator mediator, string name, int page) =>
        {
            var books = await mediator.Send(new GetBooksByNameQuery(name, page));

            var apiResponse = new PaginatedApiResponse(
                "Books with Name search",
                true,
                books,
                Convert.ToInt32(HttpStatusCode.OK),
                books.SelectedPage,
                books.TotalPages,
                books.PageSize,
                books.ItemCount);

            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        var adminRoutes = app.MapGroup("api/Admin").RequireAuthorization("Admin_Policy");

        adminRoutes.MapPost("/Book", async (IMediator mediator, AddBookCommand command) =>
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
}
