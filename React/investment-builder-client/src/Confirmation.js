import { Button, Modal } from 'react-bootstrap';

const Confirmation = function(props) {

    return(
        <Modal
            size='sm'
            centered
            aria-labelledby="contained-modal-title-vcenter"
            show={props.show}
            onHide={props.onHide}>
            <Modal.Header closeButton>
                <Modal.Title
                    id="contained-modal-title-vcenter">
                        {props.title}
                </Modal.Title>                
            </Modal.Header>
            <Modal.Body>
                <h4>{props.message}</h4>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={props.onHide}>
                        close
                </Button>                      
            </Modal.Footer>
        </Modal>
    );
};

export default Confirmation;