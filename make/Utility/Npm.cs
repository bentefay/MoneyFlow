using LanguageExt;

namespace Make.Utility
{
    public static class Npm
    {
        public static EitherAsync<Error, Unit> Install(string projectDirectory)
        {
            return CommandLine.RunToOption($"npm install {projectDirectory}");
        }
        
        public static EitherAsync<Error, Unit> Update(string projectDirectory)
        {
            return CommandLine.RunToOption($"npm update {projectDirectory}");
        }    
    }
}