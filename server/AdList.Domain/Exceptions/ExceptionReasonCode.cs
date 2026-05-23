namespace AdList.Domain.Exceptions
{
    public enum ExceptionReasonCode
    {
        InvalidRequest = 10000,
        UserNotFound,
        UserEmailIsRequired,
        TaskNotFound,
        TaskAlreadyCompleted
    }
}
