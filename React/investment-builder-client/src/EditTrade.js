import React,  { useState } from 'react';
import { Button, Modal } from 'react-bootstrap';
import {Form} from 'react-bootstrap';
import DatePicker from "react-datepicker";
import 'react-datepicker/dist/react-datepicker.css'
import middlewareService from "./MiddlewareService.js";

const EditTrade = function(props)  {

    const BUY = 'Buy';
    const SELL = 'Sell';
    const OTHER = 'Change';

    const [selectedDate, setSelectedDate] = useState( new Date());
    const [selectedAction, setSelectedAction] = useState(BUY);
    const [quantity, setQuantity] = useState('');
    const [amount, setAmount] = useState('');
    const [sellAll, setSellAll] = useState(false);

    let handleSubmit = function() {    
        console.log( 'selected data ' + selectedDate);
        console.log( 'selected action ' + selectedAction);
        console.log( 'selected quantity ' + quantity);
        console.log( 'selected amount ' + amount);
        console.log( 'sell all ' + sellAll);

        if( selectedAction === SELL && sellAll === true) {
            middlewareService.SellTrade(props.name,
                                        () => props.onHide()); 
        }
        else {
            middlewareService.UpdateTrade( 
                {
                    ItemName: props.name,
                    TransactionDate: selectedDate,
                    Action: selectedAction,
                    Quantity: quantity,
                    TotalCost: amount        
                },
                () => props.onHide()
            );   
        }

    };

    let closeModal = function() {
        setSelectedDate( new Date());
        setSelectedAction(BUY);
        setQuantity('');
        setAmount('');
        setSellAll(false);
        props.onHide();
    };

    return (
        <Modal 
            size="lg"
            show={props.show}
            aria-labelledby="contained-modal-title-vcenter"
            centered
            onHide={closeModal}>
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
                    <Form.Group >
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
                        <Form.Check 
                            label="Sell All"
                            type="checkbox"
                            disabled={selectedAction !== SELL}
                            onChange={() => setSellAll(!sellAll)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Quantity</Form.Label>
                        <Form.Control 
                                type="number"
                                disabled={selectedAction === SELL && sellAll === true} 
                                value={quantity} 
                                onChange={(e) => setQuantity(e.currentTarget.value) } 
                                placeholder="Quantity" />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Amount</Form.Label>
                        <Form.Control 
                                type="number" 
                                disabled={selectedAction === SELL && sellAll === true} 
                                value={amount} 
                                onChange={(e) => setAmount(e.currentTarget.value) } 
                                placeholder="Amount" />
                    </Form.Group>    

                    <br/>
                    <Button variant="primary" type="submit">
                        Submit
                    </Button>
                </Form>
            </Modal.Body>

        </Modal>
    );

};


export default EditTrade;