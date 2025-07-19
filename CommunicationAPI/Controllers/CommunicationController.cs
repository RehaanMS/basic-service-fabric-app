using Communication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CommunicationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunicationController : ControllerBase
    {
        [HttpGet()]
        [Route("stateless")]
        public async Task<string> StatelessGet()
        {
            var statelessProxy = ServiceProxy.Create<IStatelessInterface>(
                new Uri("fabric:/Jumpstore/CustomerAnalytics"));

            var serviceName = await statelessProxy.GetServiceDetails();

            return serviceName;
        }

        [HttpGet()]
        [Route("stateful")]
        public async Task<string> StatefulGet(
            [FromQuery] string region)
        {
            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri($"fabric:/Jumpstore/ProductCatalog"),
                new ServicePartitionKey(region.ToUpperInvariant()));

            var serviceName = await statefulProxy.GetServiceDetails();

            return serviceName;
        }

        [HttpPost()]
        [Route("addproduct")]
        public async Task AddProduct(
            [FromBody] Product product)
        {
            var partitionId = product.Id % 3;

            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri($"fabric:/Jumpstore/ProductCatalog"),
                new ServicePartitionKey(partitionId));

            await statefulProxy.AddProduct(product);
        }

        [HttpGet()]
        [Route("getproduct")]
        public async Task<Product> GetProduct(
            [FromQuery] int productId)
        {
            var partitionId = productId % 3;

            var statefulProxy = ServiceProxy.Create<IStatefulInterface>(
                new Uri($"fabric:/Jumpstore/ProductCatalog"),
                new ServicePartitionKey(partitionId));
            
            var product = await statefulProxy.GetProductById(productId);
            
            return product;
        }
    }
}
