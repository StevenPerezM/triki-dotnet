using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Business.Services;
using Tricky.Core.Entities.DBEntities;
using Tricky.Core.Entities.Response;
using Tricky.Core.Services.Data.CurrentGameService;
using Tricky.Core.Services.Data.GameEndService;
using Tricky.Core.Services.SQSService;
using Tricky.Core.Utilities;
using Tricky.Players;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Tricky.play
{
    public class Function : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        private ServiceCollection _serviceCollection;
        private readonly ServiceProvider _serviceProvider;
        private readonly RegionEndpoint regionEndpoint = RegionEndpoint.USEast1;
        public const string POST = "POST";
        public const string POST_CREATE = "create-game";
        public const string GET = "GET";
        private readonly string REDIS_HOST = Environment.GetEnvironmentVariable("REDIS_HOST");
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
                        if (input.Resource.Contains("add-movement"))
                        {
                            AddMovement addMovement = JsonConvert.DeserializeObject<AddMovement>(input.Body);
                            CurrentGame response = await _serviceProvider.GetService<CurrentGameServiceCore>().AddMovementsCurrentGame(addMovement);
                            string body = JsonConvert.SerializeObject(response);
                            return ReturnReponse(body, 200);
                        }
                        else 
                        {                            
                            CurrentGame currentGame = JsonConvert.DeserializeObject<CurrentGame>(input.Body);
                            CurrentGame response = await _serviceProvider.GetService<CurrentGameServiceCore>().CreateGame(currentGame);
                            string body = JsonConvert.SerializeObject(response);
                            return ReturnReponse(body, 200);
                        }
                       
                    case GET:
                        string idPlayer = null;
                        if (input.QueryStringParameters.ContainsKey("id"))
                        {
                            idPlayer = input.QueryStringParameters["id"];
                        }
                        CurrentGame playerById = await _serviceProvider.GetService<CurrentGameServiceCore>().GetCurrentGame(idPlayer);
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
            _serviceCollection.AddSingleton(new CurrentGameService(REDIS_HOST));
            _serviceCollection.AddScoped<CurrentGameServiceCore>();
            _serviceCollection.AddScoped<GameOverServiceCore>();
            _serviceCollection.AddScoped<IAmazonDynamoDB>(x => new AmazonDynamoDBClient(regionEndpoint));
            _serviceCollection.AddScoped<IgameEndService, GameEndService>();
            _serviceCollection.AddScoped<IAmazonSQS>(x => new AmazonSQSClient(regionEndpoint));
            _serviceCollection.AddScoped<IQueueService, SqsQueueService>();
            _serviceCollection.AddScoped<IAmazonDynamoDB>(x => new AmazonDynamoDBClient(regionEndpoint));
            _serviceCollection.AddScoped<IgameEndService, GameEndService>();
            _serviceCollection.AddCors();            
            
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
