using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.Products
{
    internal interface IProduct
    {
        string Name { get; set; }
        string Manufacturer { get; set; }
        DateTime ExpirationDate { get; set; }
        double Price { get; set; }
        double Weight { get; set; }
        string GetDescription();
    }
}
