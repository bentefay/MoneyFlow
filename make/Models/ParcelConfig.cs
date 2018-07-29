using System.Linq;

namespace Make.Models
{
    public class ParcelConfig
    {
        public ParcelProject Project => new[] {"Web"}.Select(name => new ParcelProject(name, $"src/{name}/Client/index.html")).First();
    }
}