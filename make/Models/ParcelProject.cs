namespace Make.Models
{
    public class ParcelProject
    {
        public ParcelProject(string name, string projectDirectory)
        {
            Name = name;
            ProjectDirectory = projectDirectory;
        }

        public string Name { get; }
        public string ProjectDirectory { get; }
    }
}