import * as React from "react";
import { css } from "emotion";
import { color1 } from "../styles/palette.style";
import LinearProgress from "@material-ui/core/LinearProgress";
import { lighten } from "polished";
import Fade from "@material-ui/core/Fade";

export const c = {
    loading: css({
        position: "absolute",
        borderRadius: "5px",
        background: "rgba(255, 255, 255, 0.5)",
        zIndex: 100,
        top: -1,
        left: -1,
        right: -1,
        bottom: -1
    }),
    foreground: css({
        backgroundColor: `${color1} !important`
    }),
    background: css({
        backgroundColor: `${lighten(0.5, color1)} !important`
    })
};

export const Loading = ({ isLoading }: { isLoading: boolean }) => (
    <Fade in={isLoading} style={{ pointerEvents: isLoading ? "auto" : "none" }}>
        <div className={c.loading}>
            <LinearProgress classes={{ colorPrimary: c.background, barColorPrimary: c.foreground }} />
        </div>
    </Fade>
);
