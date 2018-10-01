import * as React from "react";
import { css } from "emotion";
import { color1, color7 } from '../styles/palette.style';

export const c = {
    brand: css({
        fontSize: '70px',
        marginBottom: '20px',
        "@media (max-width: 500px)": {
            marginBottom: '0',
            fontSize: '60px'
        }
    }),
    money: css({
        color: color1,
        fontSize: "75%",
        marginBottom: "-14px",
        marginLeft: "18px"
    }),
    flow: css({
        color: color7,
        fontFamily: "'Sacramento', serif"
    })
};

export const Brand = () => (
    <div className={c.brand}>
        <div className={c.money}>Money</div>
        <div className={c.flow}>Flow</div>
    </div>
);