using CaptoneProject_IOTS_BOs;
using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.ResponseService
{
    public static class ResponseService<T>
    {
        public static GenericResponseDTO<T> CastTypeErrorResponse(ResponseDTO source)
        {
            return new GenericResponseDTO<T>
            {
                IsSuccess = source.IsSuccess,
                Message = source.Message,
                StatusCode = source.StatusCode,
            };
        }
        public static GenericResponseDTO<T> OK(T? data) => new GenericResponseDTO<T>
        {
            IsSuccess = true,
            Message = "Success",
            StatusCode = System.Net.HttpStatusCode.OK,
            Data = data
        };

        public static GenericResponseDTO<T> NotFound(string message) => new GenericResponseDTO<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = System.Net.HttpStatusCode.NotFound,
        };

        public static GenericResponseDTO<T> BadRequest(string message) => new GenericResponseDTO<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = System.Net.HttpStatusCode.BadRequest,
        };

        public static GenericResponseDTO<T> UnAuthorize(string message) => new GenericResponseDTO<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = System.Net.HttpStatusCode.Unauthorized,
        };
    }
}
