namespace AdList.Domain.Exceptions
{
    public class DomainException : ApplicationException
    {
        public AdditionalData AdditionalData { get; } = new();

        public ExceptionReasonCode ReasonCode { get; }

        public DomainException(ExceptionReasonCode reasonCode, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            ReasonCode = reasonCode;
        }
    }
}
