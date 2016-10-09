using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace PowerGridEngine
{
    /// <summary>
    /// Generic response. Use it for format ANY response in ANY Api method
    /// </summary>
    public class ApiResponseModel
    {
        public string Message { get; set;}
        
        public bool IsSuccess { get; set; }
        
        public object Data { get; set; }

        public ApiResponseModel()
        {
            Data = null;
            Message = string.Empty;
            IsSuccess = false;
        }

        public ApiResponseModel(string msg, bool isSuccess)
        {
            IsSuccess = IsSuccess;
            Message = msg;
        }

        public ApiResponseModel(object data)
        {
            IsSuccess = true;
            Data = data;
        }

    }
}
