using AdList.Application.Abstract.Command;

namespace AdList.Application.Abstract.Implementation;

public class EmptyResponse : ICommandResponse
{
    public static EmptyResponse Instance { get; } = new();

    private EmptyResponse()
    {
        //
    }
}