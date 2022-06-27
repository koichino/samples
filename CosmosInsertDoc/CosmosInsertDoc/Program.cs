using System;
using System.Security;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CosmosInsertDoc
{
    class Program
    {

        private string DatabaseName = "sampledb";
        private string ContainerName = "samplecontainer";
        private CosmosClient client;

        static void Main(string[] args)
        {
            try
            {
                Program p = new Program();
                p.InsertDocumentOne().Wait();
            }
            catch (CosmosException ce)
            {
                Exception baseException = ce.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", ce.StatusCode, ce.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of process, press any key to exit.");
                Console.ReadKey();
            }

        }

        public class SupportCase
        {
            public string id { get; set; }
            public string deviceId { get; set; }
            public string date { get; set; }
            public string message1 { get; set; }
            public string message2 { get; set; }
            public string message3 { get; set; }
            public string message4 { get; set; }
            public string message5 { get; set; }
            public string message6 { get; set; }
            public string message7 { get; set; }
            public string message8 { get; set; }
            public string message9 { get; set; }
            public string message10 { get; set; }

        }

        private async Task InsertDocumentOne()
        {
            string EndpointUri = ConfigurationManager.AppSettings["EndPointUrl"];
            string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

            CosmosClientOptions policy = new CosmosClientOptions()
            {
                ConsistencyLevel = ConsistencyLevel.Session
            };
            this.client = new CosmosClient(EndpointUri,PrimaryKey);

            Database database = await this.client.CreateDatabaseIfNotExistsAsync(
                id:DatabaseName
                ) ;
            Container container = await database.CreateContainerIfNotExistsAsync(
                id: ContainerName,
                partitionKeyPath: "/deviceId",
                throughput: 400
            );


            for (Int64 i = 0; i < 2000; i++)
            {
  //              char pad = 'A';
                string str = "abcdefg12345678";

                SupportCase supportCase = new SupportCase
                {
                    id = i.ToString(),
                    deviceId = "device1",
                    date = "Wed, 06 Mar 2019 05:55:38 GMT",
                    message1 = str,
                    message2 = str,
                    message3 = str,
                    message4 = str,
                    message5 = str,
                    message6 = str,
                    message7 = str,
                    message8 = str,
                    message9 = str,
                    message10 = str

                };

                await this.CreateSupportCaseDocumentIfNotExists(DatabaseName, Container
                    Name, container, supportCase);
            }
        }

        private async Task CreateSupportCaseDocumentIfNotExists(string databaseName, string containerName, Container container, SupportCase supportCase)
        {
            try
            {
               ItemRequestOptions iro = new ItemRequestOptions {};

               SupportCase createdItem = await container.CreateItemAsync<SupportCase>(
               item: supportCase,
               partitionKey: new PartitionKey(supportCase.deviceId),
               requestOptions : iro
               );

                Console.WriteLine("Created SupportCase {0}", supportCase.id);
                //Console.WriteLine("Consumed RU is {0}", response.RequestCharge);
            }
            catch (CosmosException ce)
            {
                Console.WriteLine(ce.ToString());
            }
        }
    }
}
