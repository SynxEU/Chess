using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Internal;
using Newtonsoft.Json;

namespace ChessAPI.Models
{
    public class ChessPlayer
    {
        [Key]
        public int ChessId { get; set; }
        public string? avatar { get; set; }
        public int player_id { get; set; }

        [JsonProperty("@id")]
        public string? apiURL { get; set; }
        public string? url { get; set; }
        public string? name { get; set; }
        public string? username { get; set; }
        public int followers { get; set; }
        public string? country { get; set; }
        public string? location { get; set; }
        public int last_online { get; set; }
        public int joined { get; set; }
        public string? status { get; set; }
        public bool is_streamer { get; set; }
        public bool verified { get; set; }
        public string? league { get; set; }
        public List<StreamingPlatform>? streaming_platforms { get; set; }
        public DateOnly FetchedAtDate { get; set; }
        public TimeSpan FetchedAtTime { get; set; }

        public List<Stats>? Stats { get; set; }
        
        public int Weight { get; set; }
        public DateOnly UpdatedAtDate { get; set; }
        public TimeSpan UpdatedAtTime { get; set; }
    }
}