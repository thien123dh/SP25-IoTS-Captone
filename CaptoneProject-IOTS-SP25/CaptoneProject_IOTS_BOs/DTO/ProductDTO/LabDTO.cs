using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CaptoneProject_IOTS_BOs.DTO.ProductDTO
{
    public class LabFilterRequestDTO
    {
        public int? UserId { set; get; }

        public int? StoreId { set; get; }

        public int? ComboId { set; get; }

        public int? LabStatus { set; get; }
    }

    public class LabDetailsInformationResponseDTO
    {
        public int Id { set; get; }

        public string? Title { set; get; }

        public string? Summary { set; get; }

        public int ComboId { set; get; }

        public string? ComboNavigationName { set; get; }

        public string? Description { set; get; }

        public string? Remark { set; get; }

        public string? SerialNumber { set; get; }

        public string? ApplicationSerialNumber { set; get; }

        public string? ImageUrl { set; get; }

        public string? PreviewVideoUrl { set; get; }

        public decimal Price { set; get; } = 0;

        public DateTime CreatedDate { set; get; } = DateTime.Now;

        public DateTime UpdatedDate { set; get; } = DateTime.Now;

        public int? CreatedBy { set; get; }

        public string? CreatedByNavigationEmail { set; get; }

        public int? UpdatedBy { set; get; }

        public decimal? Rating { set; get; } = 4;

        public int Status { set; get; }
        public bool HasAbilityToViewPlaylist { set; get; }
    }

    public class CreateUpdateLabInformationDTO
    {

        [MaxLength(150)]
        public string? Title { set; get; }

        [MaxLength(300)]
        public string Summary { set; get; }

        public int ComboId { set; get; }

        [MaxLength(1000)]
        public string Description { set; get; }

        //[MaxLength(300)]
        //public string? Remark { set; get; }

        [MaxLength(100)]
        public string? SerialNumber { set; get; }

        [MaxLength(1000)]
        public string ImageUrl { set; get; }

        [MaxLength(1000)]
        public string PreviewVideoUrl { set; get; }

        [Precision(18, 1)]
        public decimal Price { set; get; } = 0;

        //[Range(0, 10)]
        //public int LabStatus { set; get; } = (int)LabStatusEnum.DRAFT;
    }

    public class LabItemDTO
    {
        public int Id { set; get; }

        public string? Title { set; get; }

        public string? Summary { set; get; }

        public int ComboId { set; get; }
        public string? ComboNavigationName { set; get; }
        public int StoreId { set; get; }
        public string? StoreName { set; get; }
        public string? ApplicationSerialNumber { set; get; }

        public string? ImageUrl { set; get; }

        public decimal Price { set; get; } = 0;

        public bool HasBeenAddToCartAlready { set; get; } = false;

        public bool HasBeenBought { set; get; } = false;

        public DateTime? CreatedDate { set; get; } = DateTime.Now;

        public DateTime? UpdatedDate { set; get; } = DateTime.Now;

        public int? CreatedBy { set; get; }

        public int? UpdatedBy { set; get; }

        public decimal? Rating { set; get; } = 4;

        public int Status { set; get; } = (int)LabStatusEnum.DRAFT;
    }

    public class LabVideoResponseDTO
    {
        public int Id { set; get; }

        public int LabId { set; get; }

        public string? Title { set; get; }

        public string? Description { set; get; }

        public string? VideoUrl { set; get; }

        public int OrderIndex { set; get; }

        public DateTime CreatedDate { set; get; } = DateTime.Now;

        public DateTime UpdatedDate { set; get; } = DateTime.Now;
    }

    public class CreateUpdateLabVideo
    {
        public int Id { set; get; }
        public int LabId { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }

        public string VideoUrl { set; get; }
    }
}
