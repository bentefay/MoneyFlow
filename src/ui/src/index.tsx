import "tslib";
import * as React from "react";
import { render } from "react-dom";
import { Provider } from 'react-redux';
import { store } from './store/store';
import { Authenticate } from './components/authenticate/authenticate';
import { hot } from 'react-hot-loader';
import "./components/styles/icons";

const Root = () => (
    <Authenticate />
);

const EnrichedRoot = hot(module)(Root);

render(
    <Provider store={store}>
        <EnrichedRoot />
    </Provider>,
    document.getElementById("root"));
