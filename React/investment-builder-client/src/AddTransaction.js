import React,  { useState, useRef } from 'react';
import { Button, Form, Modal } from 'react-bootstrap';
import DatePicker from "react-datepicker";
import 'react-datepicker/dist/react-datepicker.css'
import middlewareService from "./MiddlewareService.js";

const AddTransaction = function(props) {

    const [selectedDate, setSelectedDate] = useState( null);
    const [transactionParameters, setTransactionParameters] = useState([]);
    const [amount, setAmount] = useState(null);

    const transactionTypeSelect = useRef(null);
    const parametersSelect = useRef(null);
    const selectedCurrency = useRef(null);

    const onloadTransactionParameters = function( response ) {
        setTransactionParameters( response.Parameters );
    };

    const loadTransactionParameters = function( transactionType )  {
        middlewareService.GetTransactionParameters(
                            {ParameterType: transactionType}, 
                            onloadTransactionParameters);
    };

    const showModal = function() {
        setSelectedDate( new Date());
        setAmount(0);
        if( props.transactionTypes.length > 0) {
            loadTransactionParameters(props.transactionTypes[0]);
        }
    };
    
    const changeSelectedTransaction = function() {
        let transactionType = transactionTypeSelect.current.value;
        console.log("selected transaction: " + transactionType);
        loadTransactionParameters(transactionType);
    };

    const handleSubmit = function() {   
        console.log("selected transaction type: " + transactionTypeSelect.current.value);
        console.log("selected parameter: " + parametersSelect.current.value);
        console.log("selected currency: " + selectedCurrency.current.value);

        let paramList = [parametersSelect.current.value];        
        if( parametersSelect.current.value === "ALL") {
            paramList = transactionParameters.filter( p => p !== "ALL");        
        }

        props.onSubmit({
            TransactionDate: selectedDate,
            ParamType: transactionTypeSelect.current.value,
            Parameter: paramList,
            Amount: amount,
            Currency : selectedCurrency.current.value
        });        
        
    };

    const closeModal = function() {
        //setSelectedDate( new Date());        
        props.onHide();
    };

    return(
        <Modal
            size="lg"
            show={props.show}
            aria-labelledby="contained-modal-title-vcenter"
            centered
            onShow={showModal}
            onHide={closeModal}>
            <Modal.Header closeButton>
                <Modal.Title
                    id="contained-modal-title-vcenter">
                        {props.title}
                </Modal.Title>                
            </Modal.Header>
            <Modal.Body>
                <Form>
                <Form.Group >
                        <Form.Label>Transaction Date</Form.Label>
                        <DatePicker
                            selected={selectedDate}
                            onChange={ (date) => setSelectedDate( date )}                            
                            className="form-control"
                        />
                    </Form.Group>   
                    <Form.Group>
                        <Form.Label>Transaction Type</Form.Label>
                        <Form.Control as="select"  
                                      onChange={(e) => changeSelectedTransaction()}
                                      custom
                                      ref={transactionTypeSelect}>
                            {
                                props.transactionTypes.map((t,i) =>
                                    (<option key={i}>{t}</option>)
                                )
                            }
                        </Form.Control>
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Parameter</Form.Label>
                        <Form.Control as="select" 
                                      custom
                                      ref={parametersSelect}>
                            {
                                transactionParameters.map((p,i) =>
                                    (<option key={i}>{p}</option>)
                                )
                            }
                        </Form.Control>
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Amount</Form.Label>
                        <Form.Control 
                                type="number"                                
                                value={amount} 
                                onChange={(e) => setAmount(e.currentTarget.value) } 
                                placeholder="Amount" />
                    </Form.Group>    
                    <Form.Group>
                        <Form.Label>Currency</Form.Label>
                        <Form.Control 
                                as="select"                                
                                custom
                                ref={selectedCurrency}>
                            {
                                props.currencies.map((t,i) =>
                                    (<option key={i}>{t}</option>)
                                )
                            }

                        </Form.Control>            
                    </Form.Group>    

                    <br/>
                    <Button variant="primary" onClick={handleSubmit}>
                        Submit
                    </Button>                    
                 </Form>
            </Modal.Body>
        </Modal>
    );
};

export default AddTransaction;