using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using System.Threading.Tasks;

namespace BirthdayMessenger
{
    public static class AddGIF
    {
        [FunctionName("AddGIF")]
        [return: ServiceBus("people", Connection = "PeopleServiceBusConnection")]
        public static async Task<String> Run(
            [ServiceBusTrigger("birthdayalert", Connection = "PeopleServiceBusConnection")] string myQueueItem,
            ILogger log)
        {
            if (myQueueItem is "null")
            {
                return null;
            }
            // log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            string api_key = Environment.GetEnvironmentVariable("GiphyAPIKey");

            try
            {
                var giphy = new Giphy(api_key);

                var parameters = new RandomParameter()
                {
                    Tag = "happybirthday",
                    Rating = Rating.G
                };

                var gifResult = await giphy.RandomGif(parameters);

                Person person  = JsonSerializer.Deserialize<Person>(myQueueItem);

                person.gif = gifResult;

                // return person;
                string personJsonString = JsonSerializer.Serialize(person);
                return personJsonString;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);

                return null;
            }
        }
    }
}
