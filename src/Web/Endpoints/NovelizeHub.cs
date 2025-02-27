namespace Galliard.Web.Endpoints;

public class NovelizeHub : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapHub<Galliard.Infrastructure.Novelize.NovelizeHub>("/NovelizeHub");
    }
}
