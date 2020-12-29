import * as React from "react";
import { css } from "@emotion/css";

export const c = {
    background: css({
        position: "fixed",
        zIndex: -1,
        top: "0",
        left: "0",
        right: "0",
        bottom: "0",
    }),
};

export const Background = () => (
    <svg className={c.background} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100" width="100%" height="100%" preserveAspectRatio="none">
        <defs>
            <linearGradient id="blue" x1="0%" y1="0%" x2="0%" y2="100%">
                <stop offset="0%" style={{ stopColor: "#fff", stopOpacity: 0.1 }} />
                <stop offset="100%" style={{ stopColor: "#fff", stopOpacity: 0 }} />
            </linearGradient>

            <radialGradient id="background" cx="-25%" cy="0%" r="150%" fx="-10%" fy="-10%">
                <stop offset="0%" style={{ stopColor: "#00abe3", stopOpacity: 1 }} />
                <stop offset="40%" style={{ stopColor: "#013274", stopOpacity: 1 }} />
                <stop offset="70%" style={{ stopColor: "#013274", stopOpacity: 1 }} />
                <stop offset="150%" style={{ stopColor: "#e35760", stopOpacity: 1 }} />
            </radialGradient>
        </defs>

        <rect width="100%" height="100%" fill="url(#background)" />

        <path d={path(200, 0, 60, 0)} fill="url(#blue)" />
        <path d={path(200, 0, 60, 5)} fill="url(#blue)" />
        <path d={path(200, 0, 60, 50)} fill="url(#blue)" />
        <path d={path(200, 0, 60, 60)} fill="url(#blue)" />
    </svg>
);

function path(period: number, phase: number, amplitude: number, shift: number) {
    return (
        `M${phase} 50 ` +
        `Q ${phase + period / 4} ${80 - amplitude}, ${phase + period / 2} 50 ` +
        `L${phase + period / 2} ${50 - amplitude} ` +
        `L${phase} ${50 + amplitude + shift} `
    );
}
