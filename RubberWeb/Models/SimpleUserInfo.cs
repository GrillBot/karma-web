using System.Text.Json.Serialization;

namespace RubberWeb.Models
{
    public class SimpleUserInfo
    {
        [JsonIgnore]
        public ulong ID { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public string AvatarUrl { get; set; }

        public static SimpleUserInfo DefaultUser => new()
        {
            AvatarUrl = "https://cdn.discordapp.com/embed/avatars/0.png",
            Discriminator = "0000",
            Name = "Neznámý uživatel"
        };
    }
}
