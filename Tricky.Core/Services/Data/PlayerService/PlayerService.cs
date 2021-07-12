using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tricky.Core.Business.Interfaces;
using Tricky.Core.Entities.DBEntities;
using Amazon.DynamoDBv2.Model;

namespace Tricky.Core.Services.Data.PlayerService
{
    public class PlayerService : IplayerService
    {

        private readonly IAmazonDynamoDB _client;
        //private readonly DynamoDBContext _dataBaseContext;

        private static readonly string PlayerTableName = "player-tricky";//Environment.GetEnvironmentVariable("ANNULMENT_CONFIG_TABLE_NAME");
        private static readonly string PlayerIndexName = "namePlayer-index";// Environment.GetEnvironmentVariable("ANNULMENT_CONFIG_INDEX_NAME");

        public PlayerService(IAmazonDynamoDB client)
        {
            _client = client;
        }

        public async Task CreatePlayer(Player player)
        {
            try
            {
                var table = Table.LoadTable(_client, PlayerTableName);
                string annulmentReasonJson = JsonConvert.SerializeObject(player, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                await table.PutItemAsync(Document.FromJson(annulmentReasonJson));
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred creating the player in the database");
                throw;
            }
        }     

        public async Task<Player> ReadPlayer(string namePlayer)
        {
            try
            {
                QueryRequest queryRequest = new QueryRequest();
                queryRequest.TableName = PlayerTableName;
                queryRequest.IndexName = PlayerIndexName;
                queryRequest.KeyConditionExpression = "namePlayer = :v_namePlayer";
                Dictionary<string, AttributeValue> expressionAttributesValues = new Dictionary<string, AttributeValue>();
                expressionAttributesValues.Add(":v_namePlayer", new AttributeValue { S = namePlayer });
                //{
                //    TableName = PlayerTableName,
                //    IndexName = PlayerIndexName,
                //    KeyConditionExpression = "namePlayer  = :v_namePlayer",
                //    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                //        {":v_namePlayer", new AttributeValue { S =  namePlayer}}
                //    },
                //    ScanIndexForward = false
                //};
                queryRequest.ExpressionAttributeValues = expressionAttributesValues;
                queryRequest.ScanIndexForward = true;
                var result = await _client.QueryAsync(queryRequest);
                if (result.Items.Count > 0)
                {
                    var playerDocument = Document.FromAttributeMap(result.Items[0]);
                    string playerJson = playerDocument.ToJson();
                    Player player = JsonConvert.DeserializeObject<Player>(playerJson);

                    return player;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error while getting the player from the data base");
                throw;
            }
        }
    }    
}
