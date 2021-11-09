import React from 'react';
import { Button, Card } from 'react-bootstrap';

const CashFlowEntity = function(props) {
    return(
        <Card bg="light" border="primary">
            <Card.Header className="cashFlowActionSummary">
            <h3>{props.title}</h3>
            <div className="cashSummaryDisplay">
                <div className="cashSummaryItemDisplay">
                    <div><strong>Valuation Date</strong></div>
                    <div>{props.valuationDate}</div>
                </div>
                <div className="cashSummaryItemDisplay">
                    <div><strong>Total</strong></div>
                    <div>{props.total}</div>
                </div>
                <Button size="sm">Add Receipt</Button>
            </div>
            </Card.Header>
            <Card.Body>
                <table striped bordered hover className="table table-condensed">
                    <thead>
                        {
                            props.headers.map( (h) =>
                                (<th>{h}</th>)
                            )
                        }
                    </thead>     
                    <tbody>
                    {
                        props.values.map( (r) =>(
                        <tr>
                            {
                                r.values.map( (i) => (
                                    <td>
                                        <p>{i}</p>
                                    </td>        
                                ))
                            }
                            <td>
                                <Button size="sm">Delete</Button>
                            </td>    
                        </tr>                           
                        ))
                    }                     
                    </tbody>               
                </table>                
            </Card.Body>
        </Card>
    );
};

export default CashFlowEntity;