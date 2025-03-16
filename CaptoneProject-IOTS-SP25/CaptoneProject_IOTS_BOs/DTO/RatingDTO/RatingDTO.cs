namespace CaptoneProject_IOTS_BOs.DTO.RatingDTO
{
    public class RatingRequestDTO
    {
        public int OrderItemId { set; get; }

        public decimal Rating { set; get; }

        public string Content { set; get; } = "";
    }
}
