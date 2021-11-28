import React  from 'react';
import { Row, Col } from 'react-bootstrap';
import CashFlowEntity  from './CashFlowEntity.js';

const CashFlow = function(props)  {

    const getReceiptHeaders = () => {
        return [
        'TransactionDate',
        'Parameter',
        'Subscription',
        'Sale',
        'Dividend',
        'Other'];
    };

    const getPaymentHeaders = () => {
        return [
        'TransactionDate',
        'Parameter',
        'Withdrawls',
        'Purchases',
        'Other' ];       
    };

    const getNonTotalValues = (values) => {
        return values
                .filter( i => i.IsTotal === false);
    };

    const getTotalValues = (values) => {
        return values
                .filter( i => i.IsTotal === true);
    };

    return(
              <Row className="cashFlowDisplay">                
                <Col>
                     <CashFlowEntity
                        title="Receipts"
                        valuationDate={props.valuationDate}
                        total={props.receiptsTotal}
                        headers={getReceiptHeaders()}
                        values={getNonTotalValues(props.receipts)}
                        totals={getTotalValues(props.receipts)}
                        addTransaction={() => props.addReceipt()}
                        deleteTransaction={(r) => props.deleteTransaction(r)}
                    />
                </Col>
                <Col>
                    <p>Payment</p>
                    <CashFlowEntity
                        title="Payments"
                        valuationDate={props.valuationDate}
                        total={props.paymentsTotal}
                        headers={getPaymentHeaders()}
                        values={getNonTotalValues(props.payments)}
                        totals={getTotalValues(props.payments)}
                        addTransaction={() => props.addPayment()}
                        deleteTransaction={(r) => props.deleteTransaction(r)}
                    />
                </Col>
            </Row>  
    );
};

export default CashFlow;