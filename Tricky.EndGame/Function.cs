using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Business.Services;
using Tricky.Core.Entities.DBEntities;
using Tricky.Core.Services.Data.GameEndService;
using Tricky.Core.Services.SQSService;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Tricky.EndGame
{
    public class Function
    {
        private ServiceCollection _serviceCollection;
        private readonly ServiceProvider _serviceProvider;
        private static RegionEndpoint regionEndpoint = RegionEndpoint.USEast1;
        public Function()
        {
            ConfigureServices();
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            try
            {                
                CurrentGame currentGame = JsonConvert.DeserializeObject<CurrentGame>(sqsEvent.Records[0].Body);
                await _serviceProvider.GetService<GameOverServiceCore>().EndGame(currentGame);
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error while saving End Game");
                throw;
            }
        }

        private void ConfigureServices()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddScoped<GameOverServiceCore>();
            _serviceCollection.AddScoped<IAmazonSQS>(x => new AmazonSQSClient(regionEndpoint));
            _serviceCollection.AddScoped<IQueueService, SqsQueueService>();
            _serviceCollection.AddScoped<IAmazonDynamoDB>(x => new AmazonDynamoDBClient(regionEndpoint));
            _serviceCollection.AddScoped<IgameEndService, GameEndService>();
        }
    }
}
