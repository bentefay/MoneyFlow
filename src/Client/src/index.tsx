import "tslib";
import * as React from "react";
import { render } from "react-dom";
import { Authenticate } from './authenticate';

const Root = () => (
  <Authenticate />
);

render(<Root />, document.getElementById("root"));
