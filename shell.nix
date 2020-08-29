let
  pkgs = import ./nix/packages;
  default = import ./default.nix;
in
pkgs.mkShell {
  inputsFrom = [
    default
  ];
}
