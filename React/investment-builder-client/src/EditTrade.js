import React,  { useState } from 'react';
import { Button, ButtonGroup, Modal, ToggleButton } from 'react-bootstrap';
import {Form} from 'react-bootstrap';
import DatePicker from "react-datepicker";
import 'react-datepicker/dist/react-datepicker.css'

const EditTrade = function(props)  {

    const BUY = '1';
    const SELL = '2';
    const OTHER = '3';

    const [selectedDate, setSelectedDate] = useState( new Date());
    const [selectedAction, setSelectedAction] = useState(BUY);
    const [quantity, setQuantity] = useState('');

    let handleSubmit = function() {    
        console.log( 'selected data ' + selectedDate);
        console.log( 'selected action ' + selectedAction);
        console.log( 'selected quantity ' + quantity);
        props.onHide();
    };

    return (
        <Modal 
            size="lg"
            show={props.show}
            aria-labelledby="contained-modal-title-vcenter"
            centered
            onHide={props.onHide}>
            <Modal.Header closeButton>
                <Modal.Title
                    id="contained-modal-title-vcenter">
                        Edit Trade
                </Modal.Title>                
            </Modal.Header>
            <Modal.Body>
                <h2>{props.name}</h2>
                <br/>
                <Form onSubmit={handleSubmit}>
                    <Form.Group>
                        <Form.Label>Transaction Date</Form.Label>
                        <DatePicker
                            selected={selectedDate}
                            onChange={ (date) => setSelectedDate( date )}                            
                            className="form-control"
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Action</Form.Label>
                        <div key={'inline-radio'}>
                            <Form.Check 
                                inline
                                label="Buy"
                                name="tradeAction"
                                type="radio"
                                defaultChecked={selectedAction === BUY }
                                onChange={(e) => setSelectedAction(BUY)}
                            />
                            <Form.Check 
                                inline
                                label="Sell"
                                name="tradeAction"
                                type="radio"
                                defaultChecked={selectedAction === SELL}                                
                                onChange={(e) => setSelectedAction(SELL)}
                            />
                            <Form.Check 
                                inline
                                label="Other"
                                name="tradeAction"
                                type="radio"
                                defaultChecked={selectedAction === OTHER}
                                onChange={(e) => setSelectedAction(OTHER)}
                            />                                                        
                        </div>
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Quantity</Form.Label>
                        <Form.Control 
                                type="number" 
                                value={quantity} 
                                onChange={(e) => setQuantity(e.currentTarget.value) } 
                                placeholder="Amount" />
                    </Form.Group>    
                    <br/>
                    <Button variant="primary" type="submit">
                        Submit
                    </Button>
                </Form>
            </Modal.Body>
            {/* <Modal.Footer>
                <button variant="primary" onClick={onOk} >OK</button>
                <button onClick={props.onHide}   >Cancel</button>
            </Modal.Footer> */}
        </Modal>
    );

};


export default EditTrade;