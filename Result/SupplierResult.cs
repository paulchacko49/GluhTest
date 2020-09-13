using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Result;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Result
{
    public class SupplierResult
    {
        public Supplier Supplier { get; set; }

        public List<ProductResult> Products { get; set; }

        public decimal GetTotal()
        {
            var t = this.Products.Select(p => p.Cost * p.Quantity).ToList().Sum();
            return t;
        }

        //public List<int> GetProductIds()
        //{
        //    var t = this.Products.Select(p => p.ID).ToList();
        //    return t;
        //}
    }
}
