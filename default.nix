with (import ./nix/packages); stdenv.mkDerivation rec {
  name = "env";

  env = buildEnv {
    name = name;
    paths = buildInputs;
  };

  buildInputs = if builtins.currentSystem == "x86_64-darwin"
    then [
        dotnet-sdk
        mono
        nodejs-10_x
    ]
    else [
        dotnet-sdk
        mono
        nodejs-10_x
    ];

  shellHook = ''
    export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
    export SOURCE_DATE_EPOCH=$(date +%s)
  '';
}
