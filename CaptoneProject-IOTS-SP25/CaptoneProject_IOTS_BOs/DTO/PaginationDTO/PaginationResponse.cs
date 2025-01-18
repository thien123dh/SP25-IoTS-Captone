using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.PaginationDTO
{
    public class PaginationResponse<T>
    {
        public int PageIndex { set; get; }
        public int PageSize { set; get; }
        public int TotalCount { set; get; }
        public IEnumerable<T>? Data { set; get; }
    }

    public class PaginationRequest
    {
        [Required]
        public int PageIndex { set; get; }
        [Required]
        public int PageSize { set; get; }
        public string SearchKeyword { set; get; } = "";
        public DateTime? StartFilterDate { set; get; }
        public DateTime? EndFilterDate { set; get; }
        //public IEnumerable<string>? SortProperties { set; get; }
    }
}
