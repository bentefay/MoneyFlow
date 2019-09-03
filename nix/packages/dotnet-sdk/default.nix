{ stdenv
, fetchurl
}:
stdenv.mkDerivation rec {
  version = "2.2.203";
  netCoreVersion = "2.2.4";
  name = "dotnet-sdk-${version}";

  src = if builtins.currentSystem == "x86_64-darwin" then
      fetchurl {
        url = "https://dotnetcli.azureedge.net/dotnet/Sdk/${version}/dotnet-sdk-${version}-osx-x64.tar.gz";
        sha512 = "ED3C0E954B317508E2980D3E96BBB0AF86C1C420C7C926A15ACBD3D48706EE2382E0A70C16CB3385B1CD2E6BD3CA2E68D9D2B6DC6746621A7E7E9DDF36D8EF11";
      }
    else
      fetchurl {
        url = "https://dotnetcli.azureedge.net/dotnet/Sdk/${version}/dotnet-sdk-${version}-linux-x64.tar.gz";
        sha512 = "8DA955FA0AEEBB6513A6E8C4C23472286ED78BD5533AF37D79A4F2C42060E736FDA5FD48B61BF5AEC10BBA96EB2610FACC0F8A458823D374E1D437B26BA61A5C";
      };

  unpackPhase = ''
    mkdir src
    cd src
    tar xvzf $src
  '';

  installPhase = ''
    runHook preInstall
    mkdir -p $out/bin
    cp -r ./ $out
    ln -s $out/dotnet $out/bin/dotnet
    runHook postInstall
  '';

  meta = with stdenv.lib; {
    homepage = https://dotnet.github.io/;
    description = ".NET Core SDK ${version} with .NET Core ${netCoreVersion}";
    license = licenses.mit;
  };
}
