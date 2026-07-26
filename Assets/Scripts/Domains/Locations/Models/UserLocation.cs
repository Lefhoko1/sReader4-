using SReader.Core.Common;

namespace SReader.Domains.Locations.Models
{
    public class UserLocation : Entity
    {
        public string UserId { get; set; }

        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
