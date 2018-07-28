using LanguageExt;

namespace Build.Utility
{
    public static class Parcel
    {
        public static OptionAsync<Unit> Build(string path, string outputDirectory)
        {
            return CommandLine.RunToOption($"parcel build {path}/index.html {outputDirectory}");
        }
    }
}