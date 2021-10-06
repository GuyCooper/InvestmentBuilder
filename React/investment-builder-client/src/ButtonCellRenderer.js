import { Component } from "react";

class ButtonCellRenderer extends Component {
    constructor( props ) {
        super(props);
        this.btnClickHandler = this.btnClickHandler.bind( this );
    }

    btnClickHandler() {
        this.props.clicked( this.props.value );
    };

    render() {
        return (
                <button onClick={this.btnClickHandler}>{this.props.label}</button>
        )
    };
};

export default ButtonCellRenderer; 