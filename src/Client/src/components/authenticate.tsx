import * as React from "react";
import { Background } from "./background";
import { Brand } from './brand';
import { css } from "emotion";
import { Form } from './form';

export const c = {
  account: css({
    height: "100%",
    display: "flex",
    flexFlow: "nowrap column",
    alignItems: "center",
    justifyContent: "center",
    fontSize: "1.2em"
  })
}

export const Authenticate = () => (
  <div className={c.account}>
    <Background />
    <Brand />
    <Form />
  </div>
);