import 'tslib';
import * as React from 'react';
import { render } from 'react-dom';

const Root = () => (
  <div>
    Hello, world!
  </div>
);

render(<Root />, document.getElementById('root'));
