namespace _20240314_1first.DTO
{
    public class ReportProductDTO : IReportDTO
    {
        public int CustomerId { get; set; }
        public string Mode { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalCountofProducts { get; set; }
        public decimal AvgPriceForProducts { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
    }

    public class ProductDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public short ModelYear { get; set; }
        public decimal ListPrice { get; set; }
        public int Quantity { get; set; }
    }
}
