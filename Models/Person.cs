using GiphyDotNet.Model.Results;
using Microsoft.Azure.Cosmos.Table;

namespace BirthdayMessenger {
    public class Person : TableEntity {

        public string name {get; set;}
        public string email {get; set;}
        public int month {get; set;}
        public int day {get; set;}
        public GiphyRandomResult gif {get; set;}

        public Person() {}

    }
}