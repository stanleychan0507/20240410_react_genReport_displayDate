using _20240314_1first.DTO;
using _20240314_1first.Model;
using Riok.Mapperly.Abstractions;

namespace _20240314_1first
{
    [Mapper]
    public partial class ProductDtoMapper
    {
        public partial Product ToProduct(CreateProductDTO product);
    }
}
