using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public ErrorResponse(string code, string message, string details = "")
        {
            Result = "error";
            ErrorCode = code;
            ErrorMessage = message;
            ErrorDetails = details;
        }
        public string ErrorCode;
        public string ErrorMessage;
        public string ErrorDetails;

    }

}
