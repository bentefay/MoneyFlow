import React from "react";
import { css } from "emotion";
import _ from "lodash";

export const c = {
    object: css({
        marginBottom: "-20px"
    }),
    field: css({
        marginBottom: "20px"
    }),
    fieldKey: css({
        fontWeight: "bold",
        display: "block",
        whiteSpace: "pre-wrap",
        marginBottom: "4px"
    }),
    fieldValue: css({
        marginLeft: "20px"
    }),
    array: css({}),
    arrayItem: css({}),
    leafValue: css({
        display: "block",
        whiteSpace: "pre-wrap"
    })
};

export const ObjectViewer = ({ object }: { object: any }): JSX.Element => {
    if (_.isString(object) || _.isNumber(object)) {
        return <code className={c.leafValue}>{object}</code>;
    } else if (_.isArrayLike(object)) {
        return (
            <ul className={c.array}>
                {_.map(object, item => (
                    <li className={c.arrayItem}>
                        <ObjectViewer object={item}></ObjectViewer>
                    </li>
                ))}
            </ul>
        );
    } else {
        return (
            <div className={c.object}>
                {_.keys(object).map(key => (
                    <div className={c.field}>
                        <code className={c.fieldKey}>{_.startCase(key)}</code>
                        <div className={c.fieldValue}>
                            <ObjectViewer object={object[key]}></ObjectViewer>
                        </div>
                    </div>
                ))}
            </div>
        );
    }
};
