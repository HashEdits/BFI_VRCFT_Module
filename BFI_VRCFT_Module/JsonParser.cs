using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BFI_VRCFT_Module
{
    public class Expression
    {

        [JsonPropertyName("id")]
        public int Id { get; set; } = 0;

        [JsonPropertyName("weight")]
        public float ConfigWeight { get; set; } = 0;

        private float weight = 0;
        public float Weight { 
            get { return weight * ConfigWeight; }
            set { weight = value; } 
        }
    }

    public class Config
    {
        public Config()
        {
            this.ip = "127.0.0.1";
            port = 8999;
            timoutTime = 3;
        }

        [JsonPropertyName("ip")]
        public string ip { get; set; } = "127.0.0.1";
        [JsonPropertyName("port")]
        public int port { get; set; } = 8999;
        [JsonPropertyName("timouttime")]
        public double timoutTime { get; set; } = 3;
    }

    public class SupportedExpressions
    {
        [JsonPropertyName("supportedexpressions")]
        public Dictionary<string, Expression> Expressions { get; set; }
    }

    public class JsonParser
    {
        public JsonParser() {
            this.debugString = null;
        }
        public string debugString;//just to debug if file was not found
        private const string JsonFileName = "config.json";//expected location of json file relative to the dll

        public SupportedExpressions ParseExpressions()
        {
            string jsonFilePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), JsonFileName);
            if (!File.Exists(jsonFilePath))
            {
                //throw new FileNotFoundException($"The file {JsonFileName} was not found.");
                debugString = ($"The file {JsonFileName} was not found.");
            }

            string jsonString = File.ReadAllText(jsonFilePath);
            SupportedExpressions supportedExpressions = JsonSerializer.Deserialize<SupportedExpressions>(jsonString);//gotrough the json and deserialize it into the object
            return supportedExpressions;
        }

        public Config ParseConfig()
        {
            string jsonFilePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), JsonFileName);
            if (!File.Exists(jsonFilePath))
            {
                //throw new FileNotFoundException($"The file {JsonFileName} was not found.");
                debugString = ($"The file {JsonFileName} was not found.");
            }

            string jsonString = File.ReadAllText(jsonFilePath);
            Config config = JsonSerializer.Deserialize<Config>(jsonString);//gotrough the json and deserialize it into the object
            debugString = $"config: ip={config.ip}, port={config.port}, timout={config.timoutTime}";
            return config;
        }
    }
}