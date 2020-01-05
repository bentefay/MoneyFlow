import React from "react";
import { GeneralFailure } from "../../store/shared/models";
import { lowerFirst } from "lodash";
import { css } from "emotion";
import { colorInvalid1 } from "../styles/palette.style";
import Popper from "@material-ui/core/Popper";
import Button from "@material-ui/core/Button";
import Fade from "@material-ui/core/Fade";
import Paper from "@material-ui/core/Paper";
import { ObjectViewer } from "./objectViewer";

export const c = {
    container: css({
        boxShadow: "inset 0 1px 3px #ddd",
        borderRadius: "5px",
        background: "white",
        border: `1px solid ${colorInvalid1}`,
        padding: "20px",
        margin: "20px 0",
        fontSize: "80%"
    }),
    description: css({
        fontSize: "110%",
        color: colorInvalid1
    }),
    importantPart: css({
        fontWeight: "bold"
    }),
    reason: css({
        marginTop: "20px"
    }),
    solutions: {
        container: css({
            marginTop: "20px"
        }),
        title: css({
            fontWeight: "bold"
        }),
        list: css({
            padding: "0 0 0 15px"
        })
    },
    errorDetail: css({
        maxWidth: "90vw",
        padding: "20px",
        overflowX: "auto"
    })
};

export const GeneralFailureView = ({ value }: { value: GeneralFailure | undefined }) =>
    value != undefined ? (
        <div className={c.container}>
            <div className={c.description}>
                Oh no! Something bad happened while
                <span className={c.importantPart}> {lowerFirst(value.friendly.actionDescription)}</span>
            </div>

            <div className={c.reason}>{value.friendly.reason}</div>

            {value.possibleSolutions != null ? (
                <div className={c.solutions.container}>
                    <div className={c.solutions.title}>Possible Solutions</div>
                    <ul className={c.solutions.list}>
                        {value.possibleSolutions.map(solution => (
                            <li key={solution}>{solution}</li>
                        ))}
                    </ul>
                </div>
            ) : null}

            <DetailedError error={value.error} />
        </div>
    ) : null;

const DetailedError = ({ error }: { error: any }): JSX.Element => {
    const [state, setState] = React.useState<{ anchorElement: Element | null; open: boolean }>({
        anchorElement: null,
        open: false
    });

    const handleClick = (event: React.MouseEvent) => {
        const { currentTarget } = event;
        setState(state => ({
            anchorElement: currentTarget,
            open: !state.open
        }));
    };

    return (
        <>
            <Button onClick={handleClick}>More detail</Button>
            <Popper open={state.open} anchorEl={state.anchorElement} placement="top" transition>
                {({ TransitionProps }) => (
                    <Fade {...TransitionProps} timeout={350}>
                        <Paper className={c.errorDetail}>{<ObjectViewer object={error}></ObjectViewer>}</Paper>
                    </Fade>
                )}
            </Popper>
        </>
    );
};
