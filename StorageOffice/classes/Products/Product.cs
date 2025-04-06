using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageOffice.classes.Products
{
    enum ProductCategory
    {

    }
    internal class Product : IProduct
    {
        private string _name;
        private string _manufacturer;
        private DateTime _expirationDate;
        private double _price;
        private double _weight;
        public string Name
        {
            get { return _name; }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException("Bład nazwy produktu: Podana nazwa produktu nie może być pusta!");
                }
            }
        }

        public string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException("Bład nazwy producenta: Podana nazwa producenta nie może być pusta!");
                }
            }
        }

        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
            set
            {
                if(value > DateTime.Now)
                {
                    _expirationDate = value;
                }
                else
                {
                    throw new ArgumentException("Błąd daty ważności produktu: Produkt musi mieć póżniejszą datę ważności niż dzisiejszy dzień!");
                }
            }
        }

        public double Price
        {
            get { return _price; }
            set
            {
                if(value >= 0)
                {
                    _price = value;
                }
                else
                {
                    throw new ArgumentException("Bład ceny produktu: Cena nie może być ujemna!");
                }
            }
        }

        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value > 0)
                {
                    _weight = value;
                }
                else
                {
                    throw new ArgumentException("Bład wagi produktu: Waga musi być większa od 0 kg!");
                }
            }
        }

        public string GetDescription()
        {
            return $"";
        }
    }
}
