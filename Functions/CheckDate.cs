using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using System.Text.Json;
using System.Threading.Tasks;

namespace BirthdayMessenger
{
    public static class CheckDate
    {
        [FunctionName("CheckDate")]
        public static async void Run(
            [TimerTrigger("%Timer%")]TimerInfo myTimer,             // variable Timer set at local.settings.json
            [Table("people", Connection = "PeopleTable")] CloudTable cloudTable,
            [ServiceBus("birthdayalert", Connection = "PeopleServiceBusConnection")] IAsyncCollector<Person> messages,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                var rangeQuery = new TableQuery<Person>()
                    .Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForInt("month", QueryComparisons.Equal,
                            DateTime.Now.Month),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForInt("day", QueryComparisons.Equal,
                            DateTime.Now.Day)));

                // Execute the query and loop through the results
                foreach (var entity in await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null))
                {
                    // log.LogInformation($"{entity.Name}\t{entity.BirthMonth}\t{entity.BirthDay}");
                    // string entityJsonString = JsonSerializer.Serialize(entity);
                    await messages.AddAsync(entity);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}
