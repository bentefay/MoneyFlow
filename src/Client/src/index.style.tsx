import { css, injectGlobal } from "emotion";
import { color0, color1, color7, color6, color4 } from './palette';

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
  
  export const background = css({
    position: "fixed",
    zIndex: -1,
    top: "0",
    left: "0",
    right: "0",
    bottom: "0",
    backgroundColor: color0
  });
  
  export const gray = css({
    fill: "#2e2e39"
  });
  
  export const blue = css({
    fill: "#2b445c"
  });
  
  export const orange = css({
    fill: "#8e4830"
  });
  
  export const brand = css({
    fontSize: '50px',
    marginBottom: '20px'
  });
  
  export const first = css({
    color: color1,
    fontSize: '65%',
    marginBottom: "-22px",
    marginLeft: "2px",
  });
  
  export const second = css({
    color: color7
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
    border: 'none',
    borderRadius: '2px',
    padding: '7px 15px',
    width: '100%',
    cursor: 'pointer',
    color: 'white'
  });