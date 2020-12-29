import * as React from "react";
import { FormError, FormState, EventWithValue, OnChange } from "../../store/shared/models";
import { isString, isEmpty } from "lodash";
import { css } from "@emotion/css";
import { colorInvalid2, colorInvalid1 } from "../styles/palette.style";
import Popper from "@material-ui/core/Popper";
import Fade from "@material-ui/core/Fade";
import Paper from "@material-ui/core/Paper";
import { transparentize } from "polished";

export const c = {
    invalidControl: css({
        borderColor: `${colorInvalid2} !important`,
        color: colorInvalid1,
    }),
    invalidMessages: css({
        listStyle: "none",
        padding: "0",
        margin: "0",
        fontSize: "80%",
        color: colorInvalid1,
    }),
    invalidMessage: css({}),
};

export const renderFormField = <TState, TKey extends keyof FormState<TState>>(
    state: FormState<TState>,
    key: TKey,
    onChange: OnChange<TState, TKey>,
    render: (field: {
        onChange: (event: EventWithValue<TState[TKey]>) => void;
        onBlur: (event: EventWithValue<TState[TKey]>) => void;
        value: TState[TKey];
    }) => React.ReactElement<any>
) => {
    return (
        <FormErrors errors={state[key].errors}>
            {render({ onChange: onChange(key, "change"), onBlur: onChange(key, "blur"), value: state[key].value })}
        </FormErrors>
    );
};

export const FormErrors = ({ errors, children: child }: { errors: ReadonlyArray<FormError> | null; children: React.ReactElement<any> }) => {
    const childRef = React.useRef<HTMLElement>(null);
    const hasErrors = !isEmpty(errors);
    const props = hasErrors ? { className: `${child.props.className} ${c.invalidControl}`, ref: childRef } : { ref: childRef };
    return (
        <React.Fragment>
            {React.cloneElement(child, props)}
            <Popper open={hasErrors} anchorEl={() => childRef.current!} placement="top-end" transition>
                {({ TransitionProps }) => (
                    <Fade {...TransitionProps} timeout={{ enter: 350, exit: 0 }}>
                        <Paper elevation={0} style={{ maxWidth: "50vw", padding: "8px", background: transparentize(1, "white") }}>
                            {errors ? (
                                <ul className={c.invalidMessages}>
                                    {errors.map((error) => (
                                        <li key={isString(error) ? error : error.key} className={c.invalidMessage}>
                                            {error}
                                        </li>
                                    ))}
                                </ul>
                            ) : null}
                        </Paper>
                    </Fade>
                )}
            </Popper>
        </React.Fragment>
    );
};
