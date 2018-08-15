﻿using LanguageExt;

namespace Make.Utility.Commands
{
    public static class Parcel
    {
        public static EitherAsync<Error, Unit> RunDev(string projectDirectory, string outputDirectory, string cacheDirectory, int logLevel)
        {
            return Executable.RunToEither($"{Exe(projectDirectory)} {projectDirectory}/index.html --out-dir {outputDirectory} --cache-dir {cacheDirectory} --log-level {logLevel} --open");
        }

        public static EitherAsync<Error, Unit> BuildProd(string projectDirectory, string outputDirectory, string cacheDirectory, int logLevel)
        {
            return Executable.RunToEither($"{Exe(projectDirectory)} build {projectDirectory}/index.html --out-dir {outputDirectory} --cache-dir {cacheDirectory} --log-level {logLevel}");
        }

        private static string Exe(string projectDirectory)
        {
            return $"{projectDirectory}/node_modules/.bin/parcel";
        }
    }
}