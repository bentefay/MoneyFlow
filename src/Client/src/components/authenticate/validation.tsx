import * as React from "react";
import { ValidationError } from '../../store/shared/models';
import { isString, isEmpty } from 'lodash';
import { css } from 'emotion';
import { colorInvalid2, colorInvalid1 } from '../styles/palette.style';
import Popper from '@material-ui/core/Popper';
import Fade from '@material-ui/core/Fade';
import Paper from '@material-ui/core/Paper';
import { lighten, transparentize } from 'polished';

export const c = {
    invalidControl: css({
        borderColor: `${colorInvalid2} !important`,
        color: colorInvalid1
    }),
    invalidMessages: css({
        listStyle: "none",
        padding: "0",
        margin: "0",
        fontSize: "80%",
        color: lighten(0.4, colorInvalid1)
    }),
    invalidMessage: css({})
};

export const Validation = ({ errors, children: child }: { errors: ReadonlyArray<ValidationError> | undefined, children: React.ReactElement<any> }) => {
    const childRef = React.createRef<HTMLElement>();
    const hasErrors = !isEmpty(errors);
    const props = hasErrors ?
        { className: `${child.props.className} ${c.invalidControl}`, ref: childRef } :
        { ref: childRef }
    return (
        <React.Fragment>
            {React.cloneElement(child, props)}
            <Popper open={hasErrors} anchorEl={() => childRef.current!} placement="top-end" transition>
                {({ TransitionProps }) => (
                    <Fade {...TransitionProps} timeout={{ enter: 350, exit: 0 }}>
                        <Paper elevation={4} style={{ maxWidth: "50vw", padding: "8px", background: transparentize(0.05, "#616161") }}>
                            {
                                errors ?
                                    <ul className={c.invalidMessages}>
                                        {errors.map(error =>
                                            <li key={isString(error) ? error : error.key} className={c.invalidMessage}>{error}</li>)}
                                    </ul> :
                                    null
                            }
                        </Paper>
                    </Fade>
                )}
            </Popper>
        </React.Fragment>
    );
}