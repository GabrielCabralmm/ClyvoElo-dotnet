namespace ClyvoElo.Application.Common;

public abstract class RecursoHateoas
{
    public List<LinkDto> Links { get; set; } = new();
}
