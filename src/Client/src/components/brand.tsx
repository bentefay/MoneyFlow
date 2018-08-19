import * as React from "react";
import { css } from "emotion";
import { color1, color7 } from '../palette.style';

export const c = {
    brand: css({
        fontSize: '70px',
        marginBottom: '20px'
    }),
    first: css({
        color: color1,
        fontSize: "75%",
        marginBottom: "-14px",
        marginLeft: "18px"
    }),
    second: css({
        color: color7,
        fontFamily: "'Sacramento', serif"
    })
};

export const Brand = () => (
    <div className={c.brand}>
        <div className={c.first}>Money</div>
        <div className={c.second}>Flow</div>
    </div>
);