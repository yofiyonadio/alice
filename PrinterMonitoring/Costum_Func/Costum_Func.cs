
namespace PrinterMonitoring.Costum_Func
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using PrinterMonitoring.Models;


    public class Costum_Func
    {
        private InventoryDataContext db1 = new InventoryDataContext();

        public string generateProductId()
        {
            string newProductId;
            string prefix = "INV";
            string suffix = "00000";
            var getProductId = db1.inv_master_products.OrderByDescending(c => c.id_product).FirstOrDefault();
            if (getProductId == null)
            {
                newProductId = prefix + 1.ToString(suffix);
            }
            else
            {
                var oldProductId = getProductId.id_product;
                var skip = 3;
                var getNumber = oldProductId.Substring(skip, oldProductId.Length - skip);
                var newNumber = (Int32.Parse(getNumber) + 1).ToString(suffix);
                newProductId = prefix + newNumber;
            }
            return newProductId;
        }














    }
}