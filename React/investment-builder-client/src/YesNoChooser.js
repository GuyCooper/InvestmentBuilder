import { Button, Modal } from 'react-bootstrap';

const YesNoChooser = function(props) {

    const onYes = function() {
        props.onHide();
        props.onYes();
    };

    return(
        <Modal
            size="sm"
            show={props.show}
            centered
            onhide={ props.onHide()}>
            <Modal.Title>
                <h2>{props.title}</h2>
            </Modal.Title>
            <Modal.Body>
                <Button variant="primary" onClick={onYes}>
                        Yes
                </Button>                    
                <Button variant="primary" onClick={props.onHide}>
                        No
                </Button>                    

            </Modal.Body>
        </Modal>
    );
};

export default YesNoChooser;    