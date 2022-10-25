import React,  { useState, useRef } from 'react';
import { Button, Form, Modal} from 'react-bootstrap';
import DatePicker from "react-datepicker";
import 'react-datepicker/dist/react-datepicker.css'

const AddRedemption = function(props) {

    const [transactionDate, setTransactionDate] = useState( new Date());
    const [amount, setAmount] = useState(null);

    const usersSelect = useRef(null);

    const showModal = function() {
        setTransactionDate( new Date());
        setAmount(0);
    };

    const handleSubmit = function() {   
        console.log("add redemption handleSubmit called");
        console.log("selected user: " + usersSelect.current.value);

        props.onSubmit({
            TransactionDate: transactionDate,
            UserName: usersSelect.current.value,
            Amount: amount
        });        
        
    };

    const closeModal = function() {
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
                        Add Redemption
                </Modal.Title>                
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group >
                        <Form.Label>Transaction Date</Form.Label>
                        <DatePicker
                            selected={transactionDate}
                            onChange={ (date) => setTransactionDate( date )}                            
                            className="form-control"
                        />
                    </Form.Group>   
                    <Form.Group>
                        <Form.Label>User</Form.Label>
                        <Form.Control as="select"  
                                      custom
                                      ref={usersSelect}>
                            {
                                props.users.map((t,i) =>
                                    (<option key={i}>{t}</option>)
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
                    <br/>
                    <Button variant="primary" onClick={handleSubmit}>
                        Submit
                    </Button>                    
                 </Form>
            </Modal.Body>
        </Modal>
    );
};

export default AddRedemption;