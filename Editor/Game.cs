using System.Collections.Generic;
using Newtonsoft.Json;

public class Game
{
    [JsonProperty("user")]
    public User user { get; set; }

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("title")]
    public string title { get; set; }

    [JsonProperty("short_text")]
    public string short_text { get; set; }

    [JsonProperty("cover")]
    public string cover { get; set; }

    [JsonProperty("url")]
    public string url { get; set; }
}

public class JamGame
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("game")]
    public Game game { get; set; }

    [JsonProperty("created_at")]
    public string created_at { get; set; }

    [JsonProperty("rating_count")]
    public int rating_count { get; set; }

    [JsonProperty("coolness")]
    public int coolness { get; set; }

    [JsonProperty("url")]
    public string url { get; set; }
}

public class ItichJam
{
    [JsonProperty("jam_games")]
    public List<JamGame> jam_games { get; set; }

    [JsonProperty("generated_on")]
    public double generated_on { get; set; }
}

public class User
{
    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("id")]
    public double id { get; set; }

    [JsonProperty("url")]
    public string url { get; set; }
}