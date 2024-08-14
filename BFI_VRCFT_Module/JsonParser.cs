using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BFI_VRCFT_Module
{
    public class Expression
    {

        [JsonPropertyName("id")]
        public int Id { get; set; } = 0;

        [JsonPropertyName("supportedexpressions")]
        public float ConfigWeight { get; set; } = 0;

        public float Weight { get; set; } = 0;
    }

    public class SupportedExpressions
    {
        [JsonPropertyName("supportedexpressions")]
        public Dictionary<string, Expression> Expressions { get; set; }
    }

    public class JsonParser
    {
        public JsonParser() {
            this.data = null;
        }
        public string data;//just to debug if file was not found
        private const string JsonFileName = "expressions.json";//expected location of json file relative to the dll

        public SupportedExpressions ParseJson()
        {
            string jsonFilePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), JsonFileName);
            if (!File.Exists(jsonFilePath))
            {
                //throw new FileNotFoundException($"The file {JsonFileName} was not found.");
                data = ($"The file {JsonFileName} was not found.");
            }

            string jsonString = File.ReadAllText(jsonFilePath);
            SupportedExpressions supportedExpressions = JsonSerializer.Deserialize<SupportedExpressions>(jsonString);
            return supportedExpressions;
        }
    }
}