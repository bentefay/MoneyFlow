using LanguageExt;

namespace Make.Utility
{
    public static class Npm
    {
        public static EitherAsync<Error, Unit> Update(string projectDirectory)
        {
            return CommandLine.RunToOption($"npm update");
        }    
    }
}