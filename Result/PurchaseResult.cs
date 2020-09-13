using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Result
{
    class PurchaseResult
    {
        public Supplier Supplier { get; set; }
        public int ProductId { get; set; }

        public decimal Cost { get; set; }

        public int Quantity { get; set; }
    }
}
