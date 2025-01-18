using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.FileDTO
{
    public static class FileDTO
    {
        public class FileResponseDTO
        {
            public string Id { get; set; }
            public string? FileName { set; get; }
            public int? FileSize { set; get; }
        }

        public class FileCreateDTO
        {
            public string? Id { get; set; }
            public string? FileName { set; get; }
            public BinaryData? FileContent { set; get; }
            public int? FileSize { set; get; }
        }
    }
}
