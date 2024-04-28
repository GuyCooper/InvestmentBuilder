import React,  { useState, useEffect } from 'react';
import { Container, Form } from 'react-bootstrap';
import DatePicker from "react-datepicker";
import 'react-datepicker/dist/react-datepicker.css'
import notifyService from "./NotifyService.js";
import CashFlow  from './CashFlow.js';
import middlewareService from "./MiddlewareService.js";
import AddTransaction from "./AddTransaction.js"

const CashFlows = () => 
{    
    const [cashFlows, setCashFlows] = useState([]);
    const [receiptParamTypes, setreceiptParamTypes] = useState([]);
    const [paymentParamTypes, setpaymentParamTypes] = useState([]);
    const [showAddReceipt, setShowAddReceipt] = useState(false);
    const [showAddPayment, setShowAddPayment] = useState(false);
    const [fromDate, setFromDate] = useState( new Date());
    const [allCurrencies, setCurrencies] = useState([]);

    const onLoadContents = function(response) {

        if (response) {
            setCashFlows( response.CashFlows );
            setreceiptParamTypes( response.ReceiptParamTypes );
            setpaymentParamTypes( response.PaymentParamTypes );

            if (response.CashFlows.length > 0) {
                notifyService.InvokeBuildStatusChange(response.CashFlows[0].CanBuild);
            }            
        }
    };

    const loadCashFlowsFromDate = function( date ) {
        console.log("loading cash flows from date " + date);
        setFromDate( date );
        middlewareService.GetCashFlowContents(date, onLoadContents);
    };

    const loadCashflows = function() {
        loadCashFlowsFromDate( fromDate );
    }

    const refreshAccount = function() {
       console.log('CashFlows refreshAccount called');
       //setFromDate( new Date());
    };

    useEffect( () => {
        console.log('Register CashFlows handlers');
        notifyService.RegisterCashFlowListener( loadCashflows );
        notifyService.RegisterAccountListener(refreshAccount);
        notifyService.RegisterConnectionListener(loadCurrencies);

        return function() {
            console.log('UnRegister CashFlows handlers');
            notifyService.UnRegisterCashFlowListener( loadCashflows );
            notifyService.UnRegisterAccountListener(refreshAccount);
        };
    });    

    const addReceiptModal = function() {
        console.log("adding receipt: ");
        setShowAddReceipt(true);
    };

    const addPaymentModal= function() {
        console.log("adding payment: ");
        setShowAddPayment(true);
    };

    const addReceiptTransaction = function( transaction ) {
        middlewareService.AddReceiptTransaction( transaction, loadCashflows);
        setShowAddReceipt(false);
    };

    const addPaymentTransaction = function( transaction ) {
        middlewareService.AddPaymentTransaction( transaction, loadCashflows);
        setShowAddPayment(false);
    };

    const deleteTransaction = function( transaction) {
        middlewareService.RemoveTransaction({
                TransactionID: transaction.TransactionID
            }, loadCashflows);    
    };

    const loadCurrencies = function() {
        middlewareService.GetCurrencies(onCurrenciesLoaded);
    }

    const onCurrenciesLoaded = function(payload) {
        setCurrencies(payload.Currencies);
    }

    return(
        <Container fluid>               
        {
           cashFlows.map( (c, i) => (
                <CashFlow
                    key={i}
                    valuationDate={c.ValuationDate}
                    receiptsTotal={c.ReceiptsTotal}
                    paymentsTotal={c.PaymentsTotal}
                    receipts={c.Receipts}
                    payments={c.Payments}
                    addReceipt={() => addReceiptModal()}
                    addPayment={() => addPaymentModal()}
                    deleteTransaction={(r) => deleteTransaction(r)}
                    editable={i === 0}
                />                
           )) 
        }

        <Form.Group>
            <Form.Label>Cash flow from</Form.Label>   
            <DatePicker
                selected={fromDate}
                onChange={ (date) => loadCashFlowsFromDate( date )}                            
                className="form-control"
            />

        </Form.Group>

        <AddTransaction
            show={showAddReceipt}
            title="Add Receipt"
            transactionTypes={receiptParamTypes}
            currencies={allCurrencies}
            onHide={() => setShowAddReceipt(false)} 
            onSubmit={(transaction) => addReceiptTransaction(transaction) }
        />
       <AddTransaction
            show={showAddPayment}
            title="Add Payment"
            transactionTypes={paymentParamTypes}
            currencies={allCurrencies}
            onHide={() => setShowAddPayment(false)}             
            onSubmit={(transaction) => addPaymentTransaction(transaction) }
        />           

        </Container> 
    );
};

export default CashFlows;