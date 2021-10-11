import React,  { useState, useRef } from 'react';
import { Modal } from 'react-bootstrap';

const EditTradeModal = (props) => {

    let onOk = function() {
        console.log('user clicked ok!');
        props.onHide();
    };

    return (
        <Modal 
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            centered>
            <Modal.Header closeButton>
                <Modal.Title
                    id="contained-modal-title-vcenter">
                        {props.Name}
                </Modal.Title>                
            </Modal.Header>
            <Modal.Body>
                <p>add some stuff here...</p>
            </Modal.Body>
            <Modal.Footer>
                <button onClick={onOk} >OK</button>
                <button onClick={props.onHide}   >Cancel</button>
            </Modal.Footer>
        </Modal>
    );

};

const EditTrade = function(props) {

    const [modalShow, setModalShow] = useState(false);

    return (
        <div>
            <button onClick={() => setModalShow(true) }>Edit Trade</button>            
            <EditTradeModal
                name={props.value} 
                show={modalShow}
                onHide={() => setModalShow(false)} />
        </div>        
    );

};

export default EditTrade;