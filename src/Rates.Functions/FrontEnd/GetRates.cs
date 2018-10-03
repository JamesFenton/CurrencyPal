using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Rates.Functions.FrontEnd
{
    public static class GetRates
    {
        [FunctionName("GetRates")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
            ILogger log)
        {
            var mediator = ContainerFactory.Container.Resolve<IMediator>();
            
            var query = new Domain.ReadModel.GetRates.Query();
            var response = await mediator.Send(query);
            
            return req.CreateResponse(HttpStatusCode.OK, response, Lookups.JsonFormatter);
        }
    }
}
