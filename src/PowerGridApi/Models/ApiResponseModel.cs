using System;

namespace PowerGridEngine
{
    public enum ResponseType
    {
        Ok,
        UnexpectedError,
        Unauthorized,
        NotInRoom,
        NotInGame,
        NotYourTurn,
        NotFound,
        InvalidModel,
        /// <summary>
        /// (Means not unauthorized but by some another reason) and reason in message
        /// </summary>
        NotAllowed, 
        NotAllowActionInThisPhase
    }

    /// <summary>
    /// Generic response. Use it for format ANY response in ANY Api method
    /// </summary>
    public class ApiResponseModel
    {
        public string Message { get; set;}
        
        public ResponseType Status { get; set; }
        
        public object Data { get; set; }

        public ApiResponseModel()
        {
            Data = null;
            Message = string.Empty;
            Status = ResponseType.UnexpectedError;
        }

        public ApiResponseModel(string msg, ResponseType status)
        {
            Status = status;
            Message = msg;
        }

        public ApiResponseModel(object data)
        {
            Status = ResponseType.Ok;
            Data = data;
        }

    }
}
