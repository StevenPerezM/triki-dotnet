using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Business.Services;
using Tricky.Core.Entities.DBEntities;
using Tricky.Core.Entities.Response;
using Tricky.Core.Services.Data.PlayerService;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Tricky.Players
{
    public class Function : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        private ServiceCollection _serviceCollection;
        private readonly ServiceProvider _serviceProvider;
        private readonly RegionEndpoint regionEndpoint = RegionEndpoint.USEast1;
        public const string POST = "POST";
        public const string GET = "GET";
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>();
        }

    
    public Function()
        {
            ConfigureServices();
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            try
            {
                switch (input.HttpMethod)
                {
                    case POST:
                        Player player = JsonConvert.DeserializeObject<Player>(input.Body);
                        ResponseGeneral response = await _serviceProvider.GetService<PlayerServiceCore>().CreatePlayer(player);
                        string body = JsonConvert.SerializeObject(response);
                        return ReturnReponse(body, 200);
                    case GET:
                        string idPlayer = null;
                        if (input.QueryStringParameters.ContainsKey("nameplayer"))
                        {
                            idPlayer = input.QueryStringParameters["nameplayer"];
                        }
                        Player playerById = await _serviceProvider.GetService<PlayerServiceCore>().Readplayer(idPlayer);
                        string playerByIdString = JsonConvert.SerializeObject(playerById);
                        return ReturnReponse(playerByIdString, 200);

                    default:
                        return ReturnReponse("Unrecognized operation type", 200);
                }
                return ReturnReponse(null, 200);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new APIGatewayProxyResponse()
                {
                    Body = e.Message.ToString(),
                    StatusCode = 500
                };
            }
        }
        private void ConfigureServices()
        {
            _serviceCollection = new ServiceCollection();           
            _serviceCollection.AddScoped<PlayerServiceCore>();
            _serviceCollection.AddScoped<IplayerService, PlayerService>();
            _serviceCollection.AddScoped<IAmazonDynamoDB>(x => new AmazonDynamoDBClient(regionEndpoint)); 
        }

        private APIGatewayProxyResponse ReturnReponse(string body, int httpStatusCode)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Access-Control-Allow-Origin", "*");
            return new APIGatewayProxyResponse()
            {
                Body = body,
                StatusCode = httpStatusCode,
                Headers = dic
            };
        }
    }
}
