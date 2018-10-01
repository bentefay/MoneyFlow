import * as React from "react";
import { GeneralFailure } from '../../store/shared/models';
import { lowerFirst } from 'lodash';
import { css } from 'emotion';
import { colorInvalid1, colorInvalid2 } from '../styles/palette.style';

export const c = {
    container: css({
        border: `1px solid ${colorInvalid2}`,
        padding: "10px",
        margin: "20px 0",
        fontSize: "80%",
    }),
    description: css({
        fontSize: "110%",
        color: colorInvalid1
    }),
    importantPart: css({
        fontWeight: "bold"
    }),
    reason: css({
        marginTop: "10px"
    }),
    solutions: {
        container: css({
            marginTop: "10px"
        }),
        title: css({
            fontWeight: "bold"
        }),
        list: css({
            padding: "0 0 0 15px"
        })
    },
    error: css({
        marginTop: "10px",
        display: "block",
        whiteSpace: "pre-wrap"
    })
};

export const GeneralFailureView = ({ value }: { value: GeneralFailure | undefined }) => 
    value != undefined ?
        <div className={c.container}>
            <div className={c.description}>
                Oh no! Something bad happened while
                <span className={c.importantPart}> {lowerFirst(value.friendly.actionDescription)}</span>
            </div>
            <div className={c.reason}>{value.friendly.reason}</div>
            {value.possibleSolutions != null ? 
                <div className={c.solutions.container}>
                    <div className={c.solutions.title}>Possible Solutions</div>
                    <ul className={c.solutions.list}>
                        {value.possibleSolutions.map(solution => <li>{solution}</li>)}
                    </ul>
                </div>
                : null}
            <code className={c.error}>{JSON.stringify(value.error, undefined, 4)}</code>
        </div> : 
        null;