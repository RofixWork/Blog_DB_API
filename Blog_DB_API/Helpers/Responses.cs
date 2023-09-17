namespace Blog_DB_API.Helpers
{
    public class Responses
    {
        public static ResponseResult BadRequestResponse(object message) => new(StatusCodes.Status400BadRequest, message);

        public static ResponseResult NotFoundResponse(object message) => new(StatusCodes.Status404NotFound, message);

        public static ResponseResult OkResponse(object message) => new(StatusCodes.Status200OK, message);
        public static ResponseResult InternalServerResponse(object message) => new(StatusCodes.Status500InternalServerError, message);
    }
}
