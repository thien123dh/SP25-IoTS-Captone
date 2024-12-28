using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Business
{
    public interface IBusinessResult
    {
        int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
