using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BirthdayMessenger
{
    public static class AddPerson
    {
        [FunctionName("AddPerson")]
        [return: Table("people", Connection = "PeopleTable")]
        public static Person Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                //todo check if not null
                string name = req.Query["name"];
                string email = req.Query["email"];
                string month = req.Query["month"];
                string day = req.Query["day"];

                //should do some variable verification

                Person person = new Person();
                person.name = name;
                person.email = email;
                person.month = Int32.Parse(month);
                person.day = Int32.Parse(day);
                person.PartitionKey = "1";
                person.RowKey = Guid.NewGuid().ToString();

                return person;
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
