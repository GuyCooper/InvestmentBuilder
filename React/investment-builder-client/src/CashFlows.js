import React,  { useState  } from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import notifyService from "./NotifyService.js";
import CashFlow  from './CashFlow.js';
import middlewareService from "./MiddlewareService.js";

const CashFlows = () => 
{    
    const [cashFlows, setCashFlows] = useState([]);
    const [receiptParamTypes, setreceiptParamTypes] = useState([]);
    const [paymentParamTypes, setpaymentParamTypes] = useState([]);
    const [reportingCurrency, setreportingCurrency] = useState(null);
 
    const onLoadContents = function(response) {

        if (response) {
            setCashFlows( response.CashFlows );
            setreceiptParamTypes( response.ReceiptParamTypes );
            setpaymentParamTypes( response.PaymentParamTypes );
            setreportingCurrency( response.ReportingCUrrency );
        }

    };

    const loadCashflows = function() {
        console.log("loading cash flows...");
        middlewareService.GetCashFlowContents(null, onLoadContents);
  
    }

    notifyService.RegisterCashFlowListener( loadCashflows );

    let addReceipt = () => {
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
                />                
           )) 
        }
        </Container> 
    );
};

export default CashFlows;