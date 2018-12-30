import "tslib";
import * as React from "react";
import { render } from "react-dom";
import { Provider } from 'react-redux';
import { store } from './store/store';
import { Authenticate } from './components/authenticate/authenticate';
import "./components/styles/icons";

const Root = () => (
    <Authenticate />
);

render(
    <Provider store={store}>
        <Root />
    </Provider>,
    document.getElementById("root"));
