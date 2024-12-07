using MongoDB.Driver;
using OMSAPI.Models.Store;
using OMSAPI.Services.ServicesInterfaces;


namespace OMSAPI.Services.StoreServices
{
    public class ProductServices: IProductServices
    {

        private readonly ILogger<ProductServices> _logger;
        private readonly IDatabaseServices _databaseServices;
        private readonly string productCollectionName = General.Constants.productsCollectionName;

        public ProductServices(ILogger<ProductServices> logger, IDatabaseServices databaseServices)
        {
            _logger = logger;
            _databaseServices = databaseServices;
        }

        public async Task<List<Product>?> GetAllProducts(string dbName)
        {
            var collection = _databaseServices.FindCollectionByDB<Product>(dbName, productCollectionName);
            if (collection != null)
            {
                var list = await collection.Find(product => true).ToListAsync();
                if (list != null)
                {
                    _logger.LogInformation($"GetAllProducts finished successfully for {dbName}");
                    return list;
                }
            }

            _logger.LogError($"The collection {productCollectionName} was not found in the database {dbName}");
            return null;
        }

        public async Task<Product?> GetProduct(string dbName, string productId)
        {

            var collection = _databaseServices.FindCollectionByDB<Product>(dbName, productCollectionName);

            if (collection != null)
            {
                var product = await collection.Find(product => product.Id == productId).FirstOrDefaultAsync();
                if (product != null)
                {
                    _logger.LogInformation($"GetProduct finished successfully for {dbName}");
                    return product;
                }
            }
            _logger.LogError($"The collection {productCollectionName} was not found in the database {dbName}");
            return null;
        }

        public async Task<Product?> CreateProduct(string dbName, Product product)
        {
            try
            {
                var productCollection = _databaseServices.FindCollectionByDB<Product>(dbName, productCollectionName);

                if (productCollection != null)
                {
                    await productCollection.InsertOneAsync(product);
                    _logger.LogInformation($"Product {product.ToString()} created for tenant {dbName}");
                    return product;
                }
                _logger.LogError($"The collection {productCollectionName} was not found in the database {dbName}");
                return null;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<Product?> UpdateProduct(string dbName,string productId, Product newProduct)
        {
            try
            {
                var productCollection = _databaseServices.FindCollectionByDB<Product>(dbName, productCollectionName);

                newProduct.Id = productId;
                if (productCollection != null)
                {
                    await productCollection.ReplaceOneAsync(p => p.Id == productId, newProduct);
                    _logger.LogInformation($"Product {newProduct.ToString()} updated for tenant {dbName}");
                    return newProduct;
                }
                _logger.LogError($"The collection {productCollectionName} was not found in the database {dbName}");
                return null;
            }
            catch
            {

                return null;
            }
        }

        public async Task<Product?> DeleteProduct(string dbName, Product product)
        {
            try
            {
                var productCollection = _databaseServices.FindCollectionByDB<Product>(dbName, productCollectionName);

                if (productCollection != null)
                {
                    await productCollection.DeleteOneAsync(p=> p.Id == product.Id);
                    _logger.LogInformation($"Product {product.ToString()} deleted for tenant {dbName}");
                    return product;
                }
                _logger.LogError($"The collection {productCollectionName} was not found in the database {dbName}");
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Substract quantity from product stack.
        /// </summary>
        public async Task<bool> SubstractQuantity(string tenantId, ShoppingCart cart)
        {
            // substract quantity from product stack
            foreach (var item in cart.CartItems)
            {
                // get product by id.
                var product = await GetProduct(tenantId, item.ProductId);
                if (product == null)
                {
                    return false;
                }
                product.Stock -= item.Quantity;

                // update product in database
                var updateProduct = await UpdateProduct(tenantId, product.Id, product);
                if (updateProduct == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// add quantity to product stack
        /// </summary>
        public async Task<bool> AddQuantity(string tenantId, Order order)
        {
            // substract quantity from product stack
            foreach (var item in order.OrderItems)
            {
                // get product by id.
                var product = await GetProduct(tenantId, item.ProductId);
                if (product == null)
                {
                    return false;
                }
                product.Stock += item.Quantity;
                // update product in database
                var updateProduct = await UpdateProduct(tenantId, product.Id, product);
                if (updateProduct == null)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
