using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.StoreDTO
{
    public class BusinessLicensesDTO
    {
        public int StoreId { set; get; }
        public string FrontIdentification { set; get; }
        public string BackIdentification { set; get; }
        public string BusinessLicences { set; get; }
        public string LiscenseNumber { set; get; }
        public DateTime IssueDate { set; get; }
        public DateTime ExpiredDate { set; get; }
        public string IssueBy { set; get; }
    }
}
