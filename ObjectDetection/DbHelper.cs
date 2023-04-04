using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ObjectDetection
{
    public static class DbHelper
    {
        public readonly static AmazonDynamoDBClient dbClient;
        public static DynamoDBContext dbContext;
       
        static DbHelper()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            // This client will access the US East 1 region.
            clientConfig.RegionEndpoint = RegionEndpoint.USEast1;
            dbClient = new AmazonDynamoDBClient(clientConfig);
            dbContext = new DynamoDBContext(dbClient);
        }
           
      
        public async static Task<bool> insertItem(ImageDetails imageDetails)
        {
            bool isSucc = false;
            try
            {
               await dbContext.SaveAsync(imageDetails);                
                isSucc = true;
            }
            catch (Exception ex)
            {
                isSucc = false;
            }
            return isSucc;
        }

    }
}
