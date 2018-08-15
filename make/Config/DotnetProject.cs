namespace Make.Config
{
    public class DotnetProject
    {
        public DotnetProject(string name, string dir)
        {
            Name = name;
            Dir = dir;
        }

        public string Name { get; }
        public string Dir { get; }
    }
}