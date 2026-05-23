using AdList.Domain.Exceptions;

namespace AdList.Application
{
    public class RequestValidationException : DomainException
    {
        public RequestValidationException(IDictionary<string, string> errors) : base(ExceptionReasonCode.InvalidRequest, "Request data is invalid.")
        {
            AdditionalData.ValidationErrors = errors;
        }
    }
}
