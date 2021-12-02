import React,  { useState, useEffect } from 'react';
import { Container } from 'react-bootstrap';
import notifyService from "./NotifyService.js";
import CashFlow  from './CashFlow.js';
import middlewareService from "./MiddlewareService.js";
import AddTransaction from "./AddTransaction.js"

const CashFlows = () => 
{    
    const [cashFlows, setCashFlows] = useState([]);
    const [receiptParamTypes, setreceiptParamTypes] = useState([]);
    const [paymentParamTypes, setpaymentParamTypes] = useState([]);
    const [reportingCurrency, setreportingCurrency] = useState(null);
    const [showAddReceipt, setShowAddReceipt] = useState(false);
    const [showAddPayment, setShowAddPayment] = useState(false);

    const onLoadContents = function(response) {

        if (response) {
            setCashFlows( response.CashFlows );
            setreceiptParamTypes( response.ReceiptParamTypes );
            setpaymentParamTypes( response.PaymentParamTypes );
            setreportingCurrency( response.ReportingCurrency );
        }

    };

    const loadCashflows = function() {
        console.log("loading cash flows...");
        middlewareService.GetCashFlowContents(null, onLoadContents);
  
    }

    useEffect( () => {
        notifyService.RegisterCashFlowListener( loadCashflows );

        return function() {
            notifyService.UnRegisterCashFlowListener( loadCashflows );
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
                />                
           )) 
        }
        <AddTransaction
            show={showAddReceipt}
            title="Add Receipt"
            transactionTypes={receiptParamTypes}
            onHide={() => setShowAddReceipt(false)} 
            onSubmit={(transaction) => addReceiptTransaction(transaction) }
        />
       <AddTransaction
            show={showAddPayment}
            title="Add Payment"
            transactionTypes={paymentParamTypes}
            onHide={() => setShowAddPayment(false)}             
            onSubmit={(transaction) => addPaymentTransaction(transaction) }
        />           

        </Container> 
    );
};

export default CashFlows;