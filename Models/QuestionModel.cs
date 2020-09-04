using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNet.GraphQL.CosmosDB.Models
{
    public class QuestionModel
    {
        public string Id { get; set; }
        public string Question { get; set; }
        [JsonProperty("correct_answer")]
        public string CorrectAnswer { get; set; }
        [JsonProperty("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
    }
}
