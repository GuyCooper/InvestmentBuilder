import React,  { useState, useRef } from 'react';
import { Modal } from 'react-bootstrap';

const EditTrade = function(props)  {

    let onOk = function() {
        console.log('user clicked ok!');
        props.onHide();
    };

    return (
        <Modal 
            size="lg"
            show={props.show}
            aria-labelledby="contained-modal-title-vcenter"
            centered>
            <Modal.Header closeButton>
                <Modal.Title
                    id="contained-modal-title-vcenter">
                        {props.name}
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


export default EditTrade;