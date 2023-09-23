namespace demerge_blog_API.Helpers
{
    public class Responses
    {
        public static ResponseResult BadRequestResponse(object message) => new(StatusCodes.Status400BadRequest, message);

        public static ResponseResult NotFoundResponse(object message) => new(StatusCodes.Status404NotFound, message);

        public static ResponseResult OKtResponse(object message) => new(StatusCodes.Status200OK, message);
    }
}
