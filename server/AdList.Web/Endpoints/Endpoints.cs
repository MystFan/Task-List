using System.Reflection;

namespace AdList.Web.Endpoints
{
    public static partial class Endpoints
    {
        private static string OpenApiTag
        {
            get => Assembly.GetExecutingAssembly().GetName().Name!;
        }

        public static void MapEndpoints(this IEndpointRouteBuilder app)
        {
            MapCommands(app);
            MapQueries(app);
        }
    }
}
