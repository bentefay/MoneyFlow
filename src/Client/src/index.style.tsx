import { css, injectGlobal } from "emotion";
import { color1, color7, color6, color4 } from './palette.style';

injectGlobal({
    '*': {
      boxSizing: "border-box"
    },
    'body, html': {
      height: "100%",
      padding: '0',
      margin: '0',
      fontFamily: '-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif,"Apple Color Emoji","Segoe UI Emoji","Segoe UI Symbol"'
    }
  });
  
  export const account = css({
    height: "100%",
    display: "flex",
    flexFlow: "nowrap column",
    alignItems: "center",
    justifyContent: "center",
    fontSize: "1.2em"
  });
    
  export const brand = css({
    fontSize: '70px',
    marginBottom: '20px'
  });
  
  export const first = css({
    color: color1,
    fontSize: "75%",
    marginBottom: "-14px",
    marginLeft: "18px"
  });
  
  export const second = css({
    color: color7,
    fontFamily: "'Sacramento', serif"
  });
  
  export const form = css({
    border: `1px solid ${color6}`,
    backgroundColor: color7,
    borderRadius: '5px',
    boxShadow: '5px 5px 20px #0004',
    padding: '40px',
    minWidth: '350px'
  });
  
  export const label = css({
    display: 'block',
    fontWeight: 600
  });
  
  export const input = css({
    fontSize: '18px',
    display: 'block',
    marginTop: '10px',
    marginBottom: '30px',
    border: `1px solid ${color6}`,
    borderRadius: '2px',
    padding: '5px 10px',
    width: '100%'
  });
  
  export const button = css({
    fontSize: '18px',
    marginTop: '10px',
    backgroundColor: color4,
    color: 'white',
    width: '100%'
  });