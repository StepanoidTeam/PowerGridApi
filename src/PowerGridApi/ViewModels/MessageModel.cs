using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PowerGridEngine
{
    public class MessageModel
    {
        public string Message { get; set;}
        
        public bool IsSuccess { get; set; }
        
        public object Data { get; set; }

        public MessageModel()
        {
            Data = null;
            Message = string.Empty;
            IsSuccess = false;
        }

        public MessageModel(string msg, bool isSuccess)
        {
            IsSuccess = IsSuccess;
            Message = msg;
        }

        public MessageModel(object data)
        {
            IsSuccess = true;
            Data = data;
        }

    }
}
