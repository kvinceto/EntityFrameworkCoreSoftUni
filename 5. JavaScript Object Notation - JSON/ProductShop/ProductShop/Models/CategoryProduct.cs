namespace ProductShop.Models
{
    public class CategoryProduct
    {
        public CategoryProduct()
        {

        }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
