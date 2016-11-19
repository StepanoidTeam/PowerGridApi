using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class ActionResponse
    {
        public bool IsSuccess { get; private set; }

        public string ErrorMsg { get; private set; }

        protected ActionResponse()
        {
            IsSuccess = true;
        }

        /// <summary>
        /// Any class should inherit this constructor
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="errorMsg"></param>
        public ActionResponse(bool isSuccess, string errorMsg)
        {
            IsSuccess = isSuccess;
            ErrorMsg = errorMsg;
        }
    }
}
