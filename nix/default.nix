nixpkgsSource:
let
  nixpkgs = import nixpkgsSource { overlays = import ./overlays; };
in
with (nixpkgs); stdenv.mkDerivation rec {
  name = "env";

  env = buildEnv {
    name = name;
    paths = buildInputs;
  };

  buildInputs = [
    curl
    dotnet
    fd
    gnupg
    gron
    jq
    niv
    nixpkgs-fmt
    ripgrep
    terraform_0_12
    yarn
    zip
  ] ++ lib.optionals stdenv.isLinux [
    glibcLocales
  ];
}
