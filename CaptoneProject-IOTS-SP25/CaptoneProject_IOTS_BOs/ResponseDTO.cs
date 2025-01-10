using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs
{
    public class ResponseDTO
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }

    public class GenericResponseDTO<T> : ResponseDTO
    {
        public T? Data { get; set; }
    }
}
