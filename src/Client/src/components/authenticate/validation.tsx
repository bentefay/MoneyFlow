import * as React from "react";
import { ValidationError } from '../../store/shared/models';
import { isString } from 'lodash';
import { css } from 'emotion';
import { colorInvalid2, colorInvalid1 } from '../styles/palette.style';

export const c = {
    invalidControl: css({
        borderColor: `${colorInvalid2} !important`,
        color: colorInvalid1
    }),
    invalidMessages: css({ 
        listStyle: "none",
        padding: "0",
        margin: "-20px 0 30px 0",
        fontSize: "80%",
        color: colorInvalid1
    }),
    invalidMessage: css({})
};

export const Validation = ({ errors, children }: { errors: ReadonlyArray<ValidationError> | undefined, children: React.ReactElement<any> }) =>
    <React.Fragment>
      {React.cloneElement(children, errors && errors.length > 0 ? { className: `${children.props.className} ${c.invalidControl}` } : {})}
      {
        errors && errors.length > 0 ? 
          <ul className={c.invalidMessages}>
            {errors.map(error => 
              <li key={isString(error) ? error : error.key} className={c.invalidMessage}>{error}</li>)}
          </ul> :
          null
      }
    </React.Fragment>;