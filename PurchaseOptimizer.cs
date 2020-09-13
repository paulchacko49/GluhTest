using Gluh.TechnicalTest.Models;
using Gluh.TechnicalTest.Result;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest
{
    public class PurchaseOptimizer
    {
        public List<SupplierResult> supplierResult { get; set; }
        public List<PurchaseRequirement> purchaseRequirementsPending { get; set; }

        /// <summary>
        /// Calculates the optimal set of supplier to purchase products from.
        /// The idea is to have few iternations
        /// 1) First iterate - bulk and in stock
        /// 2) Second iterate - bulk and in stock but no minimum quantity for Suppliers from 1st iterate
        /// 3) Third iterate -  bulk and in stock but with allstock of Supplier but not completely fullfiled
        /// 4) Fourth iterate - iterate individual product requirement
        /// ### Complete this method
        public void Optimize(List<PurchaseRequirement> purchaseRequirements)
        {
            supplierResult = new List<SupplierResult>();

            FirstIterate(purchaseRequirements);

            List<int> productIds = (from s in supplierResult
                                    from p in s.Products
                                    select p.ID).ToList();

            FilterAlreadyCompletlyFullFilled(ref purchaseRequirements, productIds);
            SecondIterate(purchaseRequirements);
            ThirdIterate(ref purchaseRequirements);
            FourthIterate(ref purchaseRequirements);
            purchaseRequirementsPending = purchaseRequirements.Where(p => p.Quantity > 0).ToList();
        }

        /// <summary>
        /// Filter purchases which have been fullfilled
        /// </summary>
        /// <param name="purchaseRequirements"></param>
        /// <param name="productIds"></param>
        private void FilterAlreadyCompletlyFullFilled(ref List<PurchaseRequirement> purchaseRequirements, List<int> productIds)
        {
            var fullFilled = purchaseRequirements.Where(p => productIds.Contains(p.Product.ID));
            purchaseRequirements = purchaseRequirements.Except(fullFilled).ToList();
        }

        /// <summary>
        ///  Check if any product can be be bought cheapest with shipping but within maxshippingcost
        /// </summary>
        private void FirstIterate(List<PurchaseRequirement> purchaseRequirements)
        {
            foreach (var purchase in purchaseRequirements)
            {
                var quant = purchase.Quantity;

                var selectedSupplier = (
                   from stock in purchase.Product.Stock
                   where stock.StockOnHand >= quant
                   where stock.Cost * quant >= stock.Supplier.ShippingCostMinOrderValue
                   where stock.Cost * quant <= stock.Supplier.ShippingCostMaxOrderValue
                   select new
                   {
                       ProductResult = new ProductResult
                       {
                           Cost = stock.Cost,
                           Quantity = quant,
                           ID = purchase.Product.ID
                       }, 
                       stock.Supplier,
                   })
                     .OrderBy(c => c.ProductResult.Cost + c.Supplier.ShippingCost)
                     .FirstOrDefault();

                if (selectedSupplier != null)
                {
                    //result.Add(selectedSupplier);
                    var returnData =  supplierResult.Where(p => p.Supplier.ID == selectedSupplier.Supplier.ID).FirstOrDefault();
                    if(returnData==null)
                    {

                        var list = new List<ProductResult>();
                        list.Add(selectedSupplier.ProductResult);
                        supplierResult.Add(new SupplierResult
                        {
                            Supplier = selectedSupplier.Supplier,
                            Products = list
                        });
                    }
                    else
                    {
                        var newTotal = returnData.GetTotal() + (selectedSupplier.ProductResult.Cost * selectedSupplier.ProductResult.Quantity);
                        if (newTotal <= returnData.Supplier.ShippingCostMaxOrderValue)
                        {
                            returnData.Products.Add(selectedSupplier.ProductResult);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Second iterate would check if we can fullfill purchases from already selected purchases without worrying about minimum shipping but within maxshippingcost
        /// </summary>
        /// <param name="purchaseRequirements"></param>
        /// <param name="result"></param>
        private void SecondIterate(List<PurchaseRequirement> purchaseRequirements)
        {
            foreach (var purchase in purchaseRequirements)
            {
                var quant = purchase.Quantity;

                var selectedSupplier = (
                   from stock in purchase.Product.Stock
                   where stock.StockOnHand >= quant
                   where stock.Cost * quant <= stock.Supplier.ShippingCostMaxOrderValue
                   select new
                   {
                       ProductResult = new ProductResult
                       {
                           Cost = stock.Cost,
                           Quantity = quant,
                           ID = purchase.Product.ID,
                           Name = purchase.Product.Name,
                       },
                       stock.Supplier,
                   })
                     .OrderBy(c => c.ProductResult.Cost+c.Supplier.ShippingCost)
                     .FirstOrDefault();

                if (selectedSupplier != null)
                {
                    //result.Add(selectedSupplier);
                    var returnData = supplierResult.Where(p => p.Supplier.ID == selectedSupplier.Supplier.ID).FirstOrDefault();
                    if (returnData == null)
                    {

                        var list = new List<ProductResult>();
                        list.Add(selectedSupplier.ProductResult);
                        supplierResult.Add(new SupplierResult
                        {
                            Supplier = selectedSupplier.Supplier,
                            Products = list
                        });
                    }
                    else
                    {
                        var newTotal = returnData.GetTotal() + (selectedSupplier.ProductResult.Cost * selectedSupplier.ProductResult.Quantity);
                        if (newTotal <= returnData.Supplier.ShippingCostMaxOrderValue)
                        {
                            returnData.Products.Add(selectedSupplier.ProductResult);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Third iterate will try to fullfill with whatever stock is available
        /// </summary>
        /// <param name="purchaseRequirements"></param>
        /// <param name="result"></param>
        private void ThirdIterate(ref List<PurchaseRequirement> purchaseRequirements)
        {
            foreach (var purchase in purchaseRequirements)
            {
                //var quant = purchase.Quantity;

                var selectedSupplier = (
                      from stock in purchase.Product.Stock
                     // where stock.StockOnHand >= quant
                      where stock.Cost * stock.StockOnHand >= stock.Supplier.ShippingCostMinOrderValue
                      where stock.Cost * stock.StockOnHand <= stock.Supplier.ShippingCostMaxOrderValue
                      where stock.StockOnHand >0
                      select new
                      {
                          ProductResult = new ProductResult
                          {
                              Cost = stock.Cost,
                              Quantity = stock.StockOnHand,
                              ID = purchase.Product.ID,
                              Name = purchase.Product.Name,
                          },
                          stock.Supplier,
                      })
                     .OrderBy(c => c.ProductResult.Cost + c.Supplier.ShippingCost)
                     .FirstOrDefault();



                if (selectedSupplier != null)
                {
                    //result.Add(selectedSupplier);
                    var returnData = supplierResult.Where(p => p.Supplier.ID == selectedSupplier.Supplier.ID).FirstOrDefault();
                    if (returnData == null)
                    {

                        var list = new List<ProductResult>();
                        list.Add(selectedSupplier.ProductResult);
                        supplierResult.Add(new SupplierResult
                        {
                            Supplier = selectedSupplier.Supplier,
                            Products = list
                        });
                        purchase.Quantity = purchase.Quantity - selectedSupplier.ProductResult.Quantity;
                    }
                    else
                    {
                        var newTotal = returnData.GetTotal() + (selectedSupplier.ProductResult.Cost * selectedSupplier.ProductResult.Quantity);
                        if (newTotal <= returnData.Supplier.ShippingCostMaxOrderValue)
                        {
                            returnData.Products.Add(selectedSupplier.ProductResult);
                            purchase.Quantity = purchase.Quantity - selectedSupplier.ProductResult.Quantity;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// The fourth iteration will loop one by one and try to retrieve data
        /// </summary>
        /// <param name="purchaseRequirements"></param>
        private void FourthIterate(ref List<PurchaseRequirement> purchaseRequirements)
        {
            int limitCheck = 10;
            foreach (var purchase in purchaseRequirements)
            {
                var quant = purchase.Quantity;
                var excludeSupplierList = new List<int>();
                int loopCount = 0;
                int purchaseLoopCount = quant + limitCheck;
                while (quant != 0 &&  loopCount< purchaseLoopCount)
                {
                    loopCount++;
                    var selectedSupplier = (
                       from stock in purchase.Product.Stock
                       where stock.StockOnHand >= 1
                       where stock.Cost >= stock.Supplier.ShippingCostMinOrderValue
                       where stock.Cost <= stock.Supplier.ShippingCostMaxOrderValue
                       where !excludeSupplierList.Contains(stock.Supplier.ID)
                       select new
                       {
                           ProductResult = new ProductResult
                           {
                               Cost = stock.Cost,
                               Quantity = 1,
                               ID = purchase.Product.ID,
                               Name = purchase.Product.Name,
                           },
                           stock.Supplier,
                       })
                         .OrderBy(c => c.ProductResult.Cost + c.Supplier.ShippingCost)
                         .FirstOrDefault();

                    if (selectedSupplier != null)
                    {
                        var returnData = supplierResult.Where(p => p.Supplier.ID == selectedSupplier.Supplier.ID).FirstOrDefault();
                        if (returnData == null)
                        {

                            var list = new List<ProductResult>();
                            list.Add(selectedSupplier.ProductResult);
                            supplierResult.Add(new SupplierResult
                            {
                                Supplier = selectedSupplier.Supplier,
                                Products = list
                            });
                            purchase.Quantity--;
                        }
                        else
                        {
                            var newTotal = returnData.GetTotal() + (selectedSupplier.ProductResult.Cost * selectedSupplier.ProductResult.Quantity);
                            if (newTotal <= returnData.Supplier.ShippingCostMaxOrderValue)
                            {
                                var returnProductData = returnData.Products.Where(p => p.ID == purchase.Product.ID).FirstOrDefault();
                                if (returnProductData == null)
                                {
                                    returnData.Products.Add(selectedSupplier.ProductResult);
                                }
                                else
                                {
                                    returnProductData.Quantity++;
                                }
                                purchase.Quantity--;
                            }
                            else
                            {
                                excludeSupplierList.Add(returnData.Supplier.ID);
                            }
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }
        }



    }
}
