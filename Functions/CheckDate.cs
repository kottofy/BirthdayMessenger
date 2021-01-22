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
        [return: ServiceBus("birthdayalert", Connection = "PeopleServiceBusConnection")]
        public static async Task<String> Run(
            [TimerTrigger("0 30 9 * * *")]TimerInfo myTimer, // once every day at 9:30 AM
            // [TimerTrigger("30 * * * * *")] TimerInfo myTimer, // once every 30 seconds - great for testing
            [Table("people", Connection = "PeopleTable")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                var rangeQuery = new TableQuery<Person>().Where(
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
                    // return entity;
                    string entityJsonString = JsonSerializer.Serialize(entity);
                    return entityJsonString;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);

                return null;
            }
        }
    }
}
