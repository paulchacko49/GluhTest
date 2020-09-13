using System.Text.Json;
using System;
using System.Linq;

namespace Gluh.TechnicalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var purchaseRequirements = new TestData().Create();
            var purchaseOptimizer = new PurchaseOptimizer();

            purchaseOptimizer.Optimize(purchaseRequirements);

            var supplierResult = purchaseOptimizer.supplierResult;
            var purchaseRequirementsPending = purchaseOptimizer.purchaseRequirementsPending;

            Console.WriteLine("Retrieved mapped data");
            var jsonMapped = JsonSerializer.Serialize(supplierResult);
            Console.WriteLine(jsonMapped);


            Console.WriteLine("Unmapped data");
            foreach (var i in purchaseRequirementsPending)
            {
                Console.WriteLine($"ProductName:{i.Product.Name}, QuantityLeft:{i.Quantity}");
            }
   


        }
    }
}
