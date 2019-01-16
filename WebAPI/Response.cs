
namespace WebAPI
{
    public class Response
    {
        public string Result;
    }
    public class SuccessResponse : Response
    {
        public SuccessResponse(object data)
        {
            Result = "success";
            Data = data;
        }
        public object Data;
    }
    public class ErrorResponse : Response
    {
        public ErrorResponse(ErrorCode code, string message, string details = "")
        {
            Result = "error";
            ErrorCode = code.ToString();
            ErrorMessage = message;
            ErrorDetails = details;
        }
        public string ErrorCode;
        public string ErrorMessage;
        public string ErrorDetails;

    }


    public enum ErrorCode
    {
        INTERNAL_ERROR, // internal service error
        LIMIT_REACHED, // api limitations was reached
        BAD_DATA, // bad data passed to api
        NOT_SUPPORTED, // possible good data but unsupported in this version of api
        MISS_PARAM // missed parameter
    }
}
