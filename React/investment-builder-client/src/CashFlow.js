import React,  { useState  } from 'react';
import { Container, Row, Col } from 'react-bootstrap';
import notifyService from "./NotifyService.js";
import CashFlowEntity  from './CashFlowEntity.js';

const CashFlow = () => 
{
    const [valuationDate, setValuationDate] = useState(null);
    const [receiptsTotal, setReceiptsTotal] = useState(null);
    const [receiptHeaders, setReceiptHeaders] = useState([]);
    const [receiptValues, setReceiptValues] = useState([]);
    const [paymentsTotal, setPaymentsTotal] = useState(null);
    const [paymentHeaders, setPaymentHeaders] = useState([]);
    const [paymentValues, setPaymentValues] = useState([]);  

    const loadCashflow = function() {
        console.log("loading cash flow...");

        setValuationDate( "25 Oct 2021");
        setReceiptsTotal( 459.25 );
        setPaymentsTotal( 364.98);
        setReceiptHeaders([
                'Transaction Date',
                'Parameter',
                'Subscription',
                'Sale',
                'Dividend',
                'Other Receipts'
                 ]);

        setPaymentHeaders([
            'Transaction Date',
            'Parameter',
            'Withdrawls',
            'Purchases',
            'Other'        
        ]);

        setReceiptValues(
            [
                {
                    values : [
                        "21-Oct-2021",
                        "bob",
                        100,
                        0,
                        0,
                        0
                    ]
                },
                {
                    values : [
                        "20-Oct-2021",
                        "acmes plc",
                        0,
                        0,
                        34.65,
                        0    
                    ]
                } ,
                {
                    values : [
                        "10-Oct-2021",
                        "interest",
                        0,
                        0,
                        0,
                        34.54    
                    ]
                }                                       
            ]
        );

        setPaymentValues(
            [
                {
                    values : [
                        "17-Oct-2021",
                        "tesla",
                        0,
                        937.82,
                        0                        
                    ]
                },
                {
                    values : [
                        "22-Oct-2021",
                        "anna",
                        354.76,
                        0,
                        0    
                    ]
                }                
            ]
        );

    }

    notifyService.RegisterCashFlowListener( loadCashflow );

    let addReceipt = () => {
    };

    return(
        <Container fluid>
            <Row>
                <Col>
                <CashFlowEntity
                    title="Receipts"
                    valuationDate={valuationDate}
                    total={receiptsTotal}
                    headers={receiptHeaders}
                    values={receiptValues}
                />
                </Col>
                <Col>
                <CashFlowEntity
                    title="Payments"
                    valuationDate={valuationDate}
                    total={paymentsTotal}
                    headers={paymentHeaders}
                    values={paymentValues}
                />
                </Col>
            </Row>
        </Container> 
    );
};

export default CashFlow;