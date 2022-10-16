using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.ViewModel
{
    public class APIResponseModel<T>
    {
        public bool RequestSuccessful { get; set; }
        public T ResponseData { get; set; }
        public string Message { get; set; }
        public string ExtraMessage { get; set; }
        public string ResponseCode { get; set; }
    }

    public class WebApiResponses<T> where T : class
    {
        public static APIResponseModel<T> InvalidModel = new APIResponseModel<T>
        {
            RequestSuccessful = false,
            ResponseCode = "XX",
            Message = "Invalid Model",
            ExtraMessage = "An error occured",
            ResponseData = null
        };

        public static APIResponseModel<T> ErrorOccured(string errorMsg)
        {
            return new APIResponseModel<T>
            {
                RequestSuccessful = false,
                ResponseCode = "99",
                Message = errorMsg,
                ResponseData = null
            };
        }

        public static APIResponseModel<T> Successful(T model)
        {
            return new APIResponseModel<T>
            {
                RequestSuccessful = true,
                ResponseCode = "00",
                Message = "Successful",
                ResponseData = model
            };
        }
    }


    public class DxResponseModel
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Exception { get; set; }
        public Object Data { get; set; }

        public DxResponseModel(string responseCode, string responseMsg, string exception, object data)
        {
            ResponseCode = responseCode;
            ResponseMessage = responseMsg;
            Exception = exception;
            Data = data;
        }
    }

    public class DxResponse
    {
        public static DxResponseModel Success(object data) =>
            new DxResponseModel("00", "Successful", "", data);

        public static DxResponseModel Failed(object data, string ex) =>
            new DxResponseModel("01", "Failed", ex, data);

        public static DxResponseModel SystemError(string ex, object data) =>
            new DxResponseModel("91", "An error occured", ex, data);
    }
}
