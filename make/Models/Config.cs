using System;

namespace Make.Models
{
    public class Config
    {
        public string RootDir => Environment.CurrentDirectory;
        public string BuildDir => $"{RootDir}/artifacts";
        public string PublishDir => $"{BuildDir}/publish";
        public string PublishZipPath => $"{BuildDir}/publish.zip";
    }
}