let
  nixpkgs = import ../sources/nixpkgs.nix;
  config = import ../config.nix;
  overlays = import ../overlays;
in
import nixpkgs { inherit config overlays; }
