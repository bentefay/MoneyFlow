# Inspired by https://github.com/NixOS/nixpkgs/blob/master/pkgs/development/compilers/dotnet
{ stdenv, buildEnv, fetchurl }:
let
  downloadUrl = version: os: "https://dotnetcli.azureedge.net/dotnet/Sdk/${version}/dotnet-sdk-${version}-${os}-x64.tar.gz";

  sha = version: os: {
    "5.0.100" = {
      osx = "69ccc7c686ac06f6c658d118f59cf1a0e7284b4570375dd88d3e3043098e311745922301f2650d159624d09c4d39a1f3cbdd5daee0e408eef915de839e3bce8f";
      linux = "bec37bfb327c45cc01fd843ef93b22b556f753b04724bba501622df124e7e144c303a4d7e931b5dbadbd4f7b39e5adb8f601cb6293e317ad46d8fe7d52aa9a09";
    };
    "3.1.401" = {
      osx = "5e18bb49ce41856d599e089c881950a3a9397a9866227a24bf9ab1a858fc38545b5b3fd5fa53f245acf5dc79af94d64a477a23b5b11cf87953e9e115501d5c8c";
      linux = "5498add9ef83da44d8f7806ca1ce335ad4193c0d3181a5abda4b65e116c7331aac37a229817ff148e4487e9734ad2438f102a0eef0049e26773a185ceb78aac4";
    };
    "2.1.807" = {
      osx = "d20bb5f986e9f568f553d3ce387a8f108e059297239247996f6144345761e834857b6df534ee0cadf8f030d41da7a98701219cbb3d48b7790bca4e2d0f95029e";
      linux = "85bfe356d1b6ac19ae5abe9f34f4cc4437f65c87ac8dff90613f447da619724ddcda5cbd1a403cd2ab96db8456d964fa60b83c468f7803d3caadbee4e8d134ec";
    };
  }."${version}"."${os}";

  platformSetForVersion = version: {
    x86_64-linux =
      let
        os = "linux";
      in
      {
        src = fetchurl {
          url = downloadUrl version os;
          sha512 = sha version os;
        };
      };
    x86_64-darwin =
      let
        os = "osx";
      in
      {
        src = fetchurl {
          url = downloadUrl version os;
          sha512 = sha version os;
        };
      };
  };

  mkDerivation = version:
    let
      platformsForVersion = platformSetForVersion version;
    in
    stdenv.mkDerivation (
      platformsForVersion."${builtins.currentSystem}" // {
        name = "dotnet-sdk-${version}";
        version = version;

        sourceRoot = ".";

        dontPatchELF = true;

        noDumpEnvVars = true;

        installPhase =
          ''
            runHook preInstall
            mkdir -p $out/bin
            cp -r ./ $out
            ln -s $out/dotnet $out/bin/dotnet
            runHook postInstall
          '';

        meta = with stdenv.lib; {
          homepage = "https://dotnet.github.io/";
          description = ".NET SDK";
          platforms = builtins.attrNames platformsForVersion;
          license = licenses.mit;
        };
      }
    );

  combineVersions = dotnetDerivations:
    let
      main = builtins.head dotnetDerivations;
    in
    buildEnv {
      name = "dotnet-core";
      paths = dotnetDerivations;
      pathsToLink = [ "/host" "/packs" "/sdk" "/shared" "/templates" ];
      ignoreCollisions = true;
      postBuild = ''
        cp ${main}/dotnet $out/dotnet
        mkdir $out/bin
        ln -s $out/dotnet $out/bin/
      '';
    };
in
combineVersions (builtins.map mkDerivation [ "5.0.100" "3.1.401" "2.1.807" ])
