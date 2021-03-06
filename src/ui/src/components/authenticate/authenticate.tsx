import * as React from "react";
import { Background } from "./background";
import { Logo as Logo } from "./logo";
import { css } from "@emotion/css";
import { Form } from "./form";

export const c = {
    account: css({
        height: "100%",
        display: "flex",
        flexFlow: "nowrap column",
        alignItems: "center",
        justifyContent: "center",
        fontSize: "1.2em",
    }),
    content: css({
        display: "flex",
        flexFlow: "nowrap column",
        alignItems: "center",
        justifyContent: "center",
    }),
};

export const Authenticate = () => (
    <div className={c.account}>
        <Background />
        <div className={c.content}>
            <Logo />
            <Form />
        </div>
    </div>
);
