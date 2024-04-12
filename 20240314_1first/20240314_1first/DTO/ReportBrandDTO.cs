namespace _20240314_1first.DTO
{
    public class ReportBrandDTO : IReportDTO
    {
        public int CustomerId { get; set; }
        public string Mode { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalCountofBrands { get; set; }
        public decimal AvgPriceForBrands { get; set; }
        public List<BrandDetail> BrandDetails { get; set; }
    }

    public class BrandDetail
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
    }
}

