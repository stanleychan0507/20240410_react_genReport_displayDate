namespace _20240314_1first.DTO
{
    public class ReportCategoryDTO : IReportDTO
    {
        public int CustomerId { get; set; }
        public string Mode { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalCountofCategories { get; set; }
        public decimal AvgPriceForCategories { get; set; }
        public List<CategoryDetail> CategoryDetails { get; set; }

        public string Tostring()
        {
            int one = 1;
            return "";
        }
    }

    public class CategoryDetail
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
