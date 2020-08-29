with (import ./nix/packages); stdenv.mkDerivation rec {
  name = "env";

  env = buildEnv {
    name = name;
    paths = buildInputs;
  };

  buildInputs = [
    dotnet
    nodejs-14_x
    yarn
  ];
}
