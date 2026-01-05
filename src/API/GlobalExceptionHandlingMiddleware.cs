using API;
using Application.Exceptions;
using System.Net;


public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception exception)
    {
        ApiResponse apiResponse = new();

        switch (exception)
        {
            case BookAlreadyExistsException bookAlreadyExistsException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.Conflict);
                apiResponse.IsSuccess = false;
                apiResponse.Message = bookAlreadyExistsException.Message;
                apiResponse.Data = null;
                break;
            case MailNotSend mailNotSend:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                apiResponse.IsSuccess = false;
                apiResponse.Message = mailNotSend.Message;
                apiResponse.Data = null;
                break;
            case MailNotConfirmedException mailNotConfirmedException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                apiResponse.IsSuccess = false;
                apiResponse.Message = mailNotConfirmedException.Message;
                apiResponse.Data = null;
                break;
            case NotFoundException notFoundException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.NotFound);
                apiResponse.IsSuccess = false;
                apiResponse.Message = notFoundException.Message;
                apiResponse.Data = null;
                break;
            case
                ArgumentException argumentException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                apiResponse.IsSuccess = false;
                apiResponse.Message = argumentException.Message;
                apiResponse.Data = null;
                break;
            case
                UnauthorizedAccessException unauthorizedAccessException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden);
                apiResponse.IsSuccess = false;
                apiResponse.Message = unauthorizedAccessException.Message;
                apiResponse.Data = null;
                break;
            case
                Exception ex:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.Data = null;
                break;
            default:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);
                apiResponse.IsSuccess = false;
                apiResponse.Message = "An unexpected error occurred.";
                apiResponse.Data = null;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = apiResponse.StatusCode;

        return context.Response.WriteAsJsonAsync(apiResponse);
    }
}
