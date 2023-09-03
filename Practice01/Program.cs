using System;
using System.Collections.Generic;
using Bogus;

namespace CaisseApp
{
    // Interface for all discount types
    public interface IDiscount
    {
        decimal ApplyDiscount(decimal originalPrice);
    }

    // Discount class for Percentage discount
    public class PercentageDiscount : IDiscount
    {
        private decimal discountPercentage;

        public PercentageDiscount(decimal discountPercentage)
        {
            this.discountPercentage = discountPercentage;
        }

        public decimal ApplyDiscount(decimal originalPrice)
        {
            return originalPrice - (originalPrice * (discountPercentage / 100));
        }
    }

    // Discount class for Buy One, Get One (BOGO) offer
    public class BOGODiscount : IDiscount
    {
        private int buyQuantity;
        private int freeQuantity;

        public BOGODiscount(int buyQuantity, int freeQuantity)
        {
            this.buyQuantity = buyQuantity;
            this.freeQuantity = freeQuantity;
        }

        public decimal ApplyDiscount(decimal originalPrice)
        {
            // Calculate the discounted price based on BOGO logic
            int totalQuantity = buyQuantity + freeQuantity;
            int numberOfDiscountedItems = totalQuantity <= 0 ? 0 : (int)Math.Floor(originalPrice / totalQuantity);
            decimal discountedPrice = numberOfDiscountedItems * buyQuantity * originalPrice / totalQuantity;
            return discountedPrice;
        }
    }

    // Discount class for Fixed Amount discount
    public class FixedAmountDiscount : IDiscount
    {
        private decimal discountAmount;

        public FixedAmountDiscount(decimal discountAmount)
        {
            this.discountAmount = discountAmount;
        }

        public decimal ApplyDiscount(decimal originalPrice)
        {
            return originalPrice - discountAmount;
        }
    }

    // Discount class for Free Shipping
    public class FreeShippingDiscount : IDiscount
    {
        public decimal ApplyDiscount(decimal originalPrice)
        {
            // Implement logic for Free Shipping discount here
            // For example, you can return 0 to indicate free shipping.
            return 0;
        }
    }

    // Discount class for Buy Two Get One Free (BTGOF) offer
    public class BTGOFDiscount : IDiscount
    {
        public decimal ApplyDiscount(decimal originalPrice)
        {
            // Implement logic for BTGOF discount here
            // For example, you can calculate the discounted price for BTGOF.
            return originalPrice - (originalPrice / 3);
        }
    }

    public class DiscountFactory
    {
        public IDiscount CreateDiscount(DiscountType discountType, params object[] parameters)
        {
            switch (discountType)
            {
                case DiscountType.Percentage:
                    decimal percentage = (decimal)parameters[0];
                    return new PercentageDiscount(percentage);

                case DiscountType.BOGO:
                    int buyQuantity = (int)parameters[0];
                    int freeQuantity = (int)parameters[1];
                    return new BOGODiscount(buyQuantity, freeQuantity);

                case DiscountType.FixedAmount:
                    decimal fixedAmount = (decimal)parameters[0];
                    return new FixedAmountDiscount(fixedAmount);

                case DiscountType.FreeShipping:
                    // Create an instance of the FreeShippingDiscount class
                    return new FreeShippingDiscount();

                case DiscountType.BTGOF:
                    // Create an instance of the BTGOFDiscount class
                    return new BTGOFDiscount();

                default:
                    throw new ArgumentException("Invalid discount type.");
            }
        }
    }

    public enum DiscountType
    {
        Percentage,
        BOGO,
        FixedAmount,
        FreeShipping,
        BTGOF,
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Générer des données fictives pour les produits
            var products = GenerateDummyProducts();

            // Générer des données fictives pour les clients
            var customers = GenerateDummyCustomers();

            // Create a discount factory
            var discountFactory = new DiscountFactory();

            // Simuler des achats et calculer les montants des factures
            foreach (var customer in customers)
            {
                Console.WriteLine($"Client : {customer.Name}");

                var shoppingCart = new Dictionary<string, int>(); // Produits dans le panier

                // Simuler des achats aléatoires
                foreach (var product in products)
                {
                    var quantity = new Random().Next(1, 5); // Quantité aléatoire
                    shoppingCart.Add(product.Name, quantity);

                    Console.WriteLine($"Achat : {quantity} {product.Name}");
                }

                // Appliquer des réductions (ex. : produits supplémentaires)
                ApplyDiscounts(shoppingCart, products, discountFactory);

                // Calculer le montant total de la facture
                var totalAmount = CalculateTotalAmount(products, shoppingCart);
                Console.WriteLine($"Montant total : {totalAmount:C}");
                Console.WriteLine();
            }
        }

        // Générer des données fictives pour les produits
        static List<Product> GenerateDummyProducts()
        {
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 100));

            return productFaker.Generate(10);
        }

        // Générer des données fictives pour les clients
        static List<Customer> GenerateDummyCustomers()
        {
            var customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Person.FullName);

            return customerFaker.Generate(5);
        }

        // Inside the ApplyDiscounts function
        static void ApplyDiscounts(Dictionary<string, int> shoppingCart, List<Product> products, DiscountFactory discountFactory)
        {
            foreach (var item in shoppingCart)
            {
                var product = products.Find(p => p.Name == item.Key);
                if (product != null)
                {
                    // Determine the discount type for the product
                    DiscountType discountType = DetermineDiscountType(product.Name);

                    // Create the discount instance using the factory
                    var discount = CreateDiscountWithParameters(discountFactory, discountType);

                    // Apply the discount to the product's price
                    decimal discountedPrice = discount.ApplyDiscount(product.Price);

                    // Update the product price in the shopping cart
                    shoppingCart[item.Key] = (int)Math.Floor(item.Value * discountedPrice / product.Price);
                }
            }
        }

        // Helper method to create a discount instance with parameters
        static IDiscount CreateDiscountWithParameters(DiscountFactory discountFactory, DiscountType discountType)
        {
            switch (discountType)
            {
                case DiscountType.Percentage:
                    decimal percentage = new Faker().Random.Decimal(1, 50); // Generate a random percentage (adjust the range as needed)
                    return discountFactory.CreateDiscount(discountType, percentage);

                case DiscountType.BOGO:
                    int buyQuantity = new Faker().Random.Int(1, 5); // Generate a random buy quantity (adjust the range as needed)
                    int freeQuantity = new Faker().Random.Int(1, 5); // Generate a random free quantity (adjust the range as needed)
                    return discountFactory.CreateDiscount(discountType, buyQuantity, freeQuantity);

                case DiscountType.FixedAmount:
                    decimal fixedAmount = new Faker().Random.Decimal(1, 10); // Generate a random fixed amount (adjust the range as needed)
                    return discountFactory.CreateDiscount(discountType, fixedAmount);

                case DiscountType.FreeShipping:
                    // For Free Shipping, you don't need additional parameters, so just create the discount instance
                    return discountFactory.CreateDiscount(discountType);

                case DiscountType.BTGOF:
                    // For BTGOF, you don't need additional parameters, so just create the discount instance
                    return discountFactory.CreateDiscount(discountType);

                default:
                    throw new ArgumentException("Invalid discount type.");
            }
        }

        // Calculate le montant total de la facture
        static decimal CalculateTotalAmount(List<Product> products, Dictionary<string, int> shoppingCart)
        {
            decimal totalAmount = 0;

            foreach (var item in shoppingCart)
            {
                var product = products.Find(p => p.Name == item.Key);
                if (product != null)
                {
                    totalAmount += product.Price * item.Value;
                }
            }

            return totalAmount;
        }

        // Determine the discount type for a product (you can implement your logic here)
        static DiscountType DetermineDiscountType(string productName)
        {
            // Generate a random discount type using Faker
            var randomDiscountType = new Faker().PickRandom<DiscountType>();

            return randomDiscountType;
        }
    }

    // Modèles de données pour les produits et les clients
    class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    class Customer
    {
        public string Name { get; set; }
    }
}
