using System;
using System.Collections.Generic;

namespace _20240314_1first.Model;

public partial class OrderItem
{
    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal ListPrice { get; set; }

    public decimal Discount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    //public static OrderItem[] Add(OrderItem o)
    //{
    //    return ;
    //}
}
