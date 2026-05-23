using AdList.Application.Abstract.Command;

namespace AdList.Application.Abstract.Implementation;

public class EmptyCommandResponse : IApplicationCommandResponse
{
    public static EmptyCommandResponse Instance { get; } = new();

    private EmptyCommandResponse()
    {
        //
    }
}