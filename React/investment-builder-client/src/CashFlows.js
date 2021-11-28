import React,  { useState  } from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import notifyService from "./NotifyService.js";
import CashFlow  from './CashFlow.js';
import middlewareService from "./MiddlewareService.js";
import AddTransaction from "./AddTransaction.js"
import YesNoChooser from "./YesNoChooser.js"

const CashFlows = () => 
{    
    const [cashFlows, setCashFlows] = useState([]);
    const [receiptParamTypes, setreceiptParamTypes] = useState([]);
    const [paymentParamTypes, setpaymentParamTypes] = useState([]);
    const [reportingCurrency, setreportingCurrency] = useState(null);
    const [showAddReceipt, setShowAddReceipt] = useState(false);
    const [showAddPayment, setShowAddPayment] = useState(false);
    const [showYesNo, setShowYesNo] = useState(false);

    let deleteTransactionID = null;

    const onLoadContents = function(response) {

        if (response) {
            setCashFlows( response.CashFlows );
            setreceiptParamTypes( response.ReceiptParamTypes );
            setpaymentParamTypes( response.PaymentParamTypes );
            setreportingCurrency( response.ReportingCUrrency );
        }

    };

    const loadCashflows = function() {
        deleteTransactionID = null;
        console.log("loading cash flows...");
        middlewareService.GetCashFlowContents(null, onLoadContents);
  
    }

    notifyService.RegisterCashFlowListener( loadCashflows );

    const addReceiptModal = function() {
        console.log("adding receipt: ");
        setShowAddReceipt(true);
    };

    const addPaymentModal= function() {
        console.log("adding payment: ");
        setShowAddPayment(true);
    };

    const deleteTransactionModal = function(transaction ) {
        deleteTransactionID = transaction.TransactionID; 
        setShowYesNo(true);
    };

    const addReceiptTransaction = function( transaction ) {
        middlewareService.AddReceiptTransaction( transaction, loadCashflows);
        setShowAddReceipt(false);
    };

    const addPaymentTransaction = function( transaction ) {
        middlewareService.AddPaymentTransaction( transaction, loadCashflows);
        setShowAddPayment(false);
    };

    const deleteTransaction = function() {
        if( deleteTransactionID !== null){
            middlewareService.RemoveTransaction({
                TransactionID: deleteTransactionID
            }, loadCashflows);    
        }

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
                    deleteTransaction={(r) => deleteTransactionModal(r)}
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

        <YesNoChooser
            show={showYesNo}
            title="Delete transaction"
            onYes={() => deleteTransaction()}
            onHide={() => setShowYesNo(false)}
        />
            

        </Container> 
    );
};

export default CashFlows;