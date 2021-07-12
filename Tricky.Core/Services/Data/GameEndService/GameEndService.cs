using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Entities.DBEntities;

namespace Tricky.Core.Services.Data.GameEndService
{
    public class GameEndService: IgameEndService
    {
        private readonly IAmazonDynamoDB _client;        

        private static readonly string GameTableName = "game-over";        

        public GameEndService(IAmazonDynamoDB client)
        {
            _client = client;
        }

        public async Task CreateGameOver(GameOver gameOver )
        {
            try
            {
                var table = Table.LoadTable(_client, GameTableName);
                string annulmentReasonJson = JsonConvert.SerializeObject(gameOver, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                await table.PutItemAsync(Document.FromJson(annulmentReasonJson));
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred creating game over");
                throw;
            }
        }
    }
}
