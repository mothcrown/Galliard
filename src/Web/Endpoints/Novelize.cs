using Galliard.Application.Novelize.Commands.Novelize;
using Galliard.Infrastructure.Novelize;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Galliard.Web.Endpoints;

public class Novelize : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(NovelizeProcess);
    }

    public async Task<Ok<string>> NovelizeProcess(ISender sender, NovelizeCommand command)
    {
        return TypedResults.Ok(await sender.Send(command));
    }
}
