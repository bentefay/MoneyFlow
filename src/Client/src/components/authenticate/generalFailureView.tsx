import * as React from "react";
import { GeneralFailure } from '../../store/shared/models';
import { lowerFirst } from 'lodash';
import { css } from 'emotion';
import { colorInvalid1 } from '../styles/palette.style';
import Popper from '@material-ui/core/Popper';
import Button from '@material-ui/core/Button';
import Fade from '@material-ui/core/Fade';
import Paper from '@material-ui/core/Paper';

export const c = {
    container: css({
        boxShadow: "inset 0 1px 3px #ddd",
        borderRadius: "5px",
        background: "white",
        border: `1px solid ${colorInvalid1}`,
        padding: "20px",
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
        marginTop: "20px"
    }),
    solutions: {
        container: css({
            marginTop: "20px"
        }),
        title: css({
            fontWeight: "bold"
        }),
        list: css({
            padding: "0 0 0 15px"
        })
    },
    error: css({
        padding: "20px",
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

            <DetailedError error={value.error} />
        </div> :
        null;


interface DetailedErrorProps extends Readonly<{
    error: any
}> { }

interface DetailedErrorState extends Readonly<{
    anchorElement: Element | null,
    open: boolean
}> { }

class DetailedError extends React.Component<DetailedErrorProps, DetailedErrorState> {

    public state = {
        anchorElement: null,
        open: false,
    };

    handleClick = (event: React.MouseEvent) => {
        const { currentTarget } = event;
        this.setState({
            anchorElement: currentTarget,
            open: !this.state.open,
        });
    };

    render() {
        return (
            <React.Fragment>
                <Button onClick={this.handleClick}>
                    More detail
                </Button>
                <Popper open={this.state.open} anchorEl={this.state.anchorElement} placement="top" transition>
                    {({ TransitionProps }) => (
                        <Fade {...TransitionProps} timeout={350}>
                            <Paper style={{ maxWidth: "50%" }}>
                                <code className={c.error}>{JSON.stringify(this.props.error, undefined, 4)}</code>
                            </Paper>
                        </Fade>
                    )}
                </Popper>
            </React.Fragment>
        );
    }
}
