import React,  { useState } from 'react';
import { Button, Form, InputGroup, FormControl, Spinner} from 'react-bootstrap';
import DatePicker from "react-datepicker";
import 'react-datepicker/dist/react-datepicker.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCheck, faTimes } from '@fortawesome/free-solid-svg-icons'
import middlewareService from "./MiddlewareService.js";
import Confirmation from './Confirmation.js';
import notifyService from "./NotifyService.js";

const AddInvestment = function() {

    const NotValidated = 0;
    const Validating = 1;
    const ValidationPassed = 2;
    const ValidationFailed = 3;

    const [name, setName] = useState(null);
    const [selectedDate, setSelectedDate] = useState( new Date());
    const [quantity, setQuantity] = useState(null);
    const [symbol, setSymbol] = useState(null);
    const [validationStatus, setValidationStatus] = useState(NotValidated);
    const [currency, setCurrency] = useState(null);
    const [totalCost, setTotalCost] = useState(null);
    const [showConfirmation, setShowConfirmation] = useState(false);
    const [message, setMessage] = useState(null);

    const onTradeSubmitted = function (response) {
        if (response.status === 'fail') {
            setMessage("Add investment failed: " + response.error);
        }
        else {
            setMessage("Add investment succeded");
            notifyService.InvokeAccountChange();
        }
    }

    const handleSubmit = function() {  
        console.log("total: " + totalCost); 

        var trade = {
            TransactionDate: selectedDate,
            ItemName: name,
            Symbol: symbol,
            Quantity: quantity,
            Currency: currency,
            TotalCost: parseFloat(totalCost),
            Action: "Buy"
        };

        middlewareService.UpdateTrade(trade,onTradeSubmitted);        
        setShowConfirmation(true);
    };

    const onSymbolValidated = function(response) {

        if(response.IsError === false) {
            setValidationStatus(ValidationPassed);
        }
        else {
            setValidationStatus(ValidationFailed);
        }
    };

    const validateSymbol = function() {
    
        setValidationStatus(Validating);
        let payload = {
            Symbol : symbol
        };

        middlewareService.GetPrice(payload, onSymbolValidated);

    };

    return (
        <>
             <Form.Group>
                <Form.Label>Name</Form.Label>
                <Form.Control
                    type="text"
                    value={name}
                    onChange={(e) => setName(e.currentTarget.value) } 
                    placeholder="Name of Investment" />
            </Form.Group>
            <Form.Group >
                <Form.Label>Transaction Date</Form.Label>
                <DatePicker
                    selected={selectedDate}
                    onChange={ (date) => setSelectedDate( date )}                            
                    className="form-control"
                />
            </Form.Group>    
            <Form.Group>
                <Form.Label>Quantity</Form.Label>
                <Form.Control 
                        type="number"                                
                        value={quantity} 
                        onChange={(e) => setQuantity(e.currentTarget.value) } 
                        placeholder="Quantity" />
            </Form.Group>      
            <Form.Group>
                <Form.Label>Symbol</Form.Label>
                <InputGroup className="mb-3">
                    <FormControl
                    placeholder="symbol"
                    aria-label="symbol"
                    aria-describedby="basic-addon2"
                    value={symbol}
                    onChange={(e) => setSymbol(e.currentTarget.value) } 
                    />
                    <Button variant="outline-secondary" 
                            id="button-addon2"
                            onClick={validateSymbol}>
                        Validate
                    </Button>
                </InputGroup>                
            </Form.Group>                 
            <Form.Group>                
                {validationStatus === ValidationPassed && <FontAwesomeIcon color="LightGreen" icon={faCheck}/>}
                {validationStatus ===  ValidationFailed && <FontAwesomeIcon color="Red" icon={faTimes}/>}
                {validationStatus === Validating && <Spinner animation="border" variant="primary" />}
            </Form.Group>
            <Form.Group>
                <Form.Label>Investment Currency</Form.Label>
                <Form.Control
                    type="text" 
                    value={currency}
                    onChange={(e) => setCurrency(e.currentTarget.value) } 
                    placeholder="Currency"/>
            </Form.Group>
            <Form.Group>
                <Form.Label>Total Cost</Form.Label>
                <Form.Control
                    value={totalCost}
                    onChange={(e) => setTotalCost(e.currentTarget.value) } 
                    placeholder="Total Cost"/>
            </Form.Group>

            <Form.Group>
                <Button 
                    type="primary"
                    disabled={validationStatus !== ValidationPassed }
                    onClick={handleSubmit}>
                    Submit
                </Button>                    
            </Form.Group>    

        <Confirmation 
                show={showConfirmation}
                onHide={() => setShowConfirmation(false)}
                title="Add Investment"
                message={message}
        />

        </>
    );
};

export default AddInvestment;