import React from 'react';
import { Button, Card } from 'react-bootstrap';


const CashFlowEntity = function(props) {

    const removePlural = function(str) {
        if(str.endsWith("s") || str.endsWith("S")) {
            return str.slice( 0, str.length - 1);
        }
        return str;
    }

    
    const getEntityValues = function(entity) {
        return props.headers.map( h => {
            let val = entity[h];
            return val;
        });
    };

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
                <Button size="sm">Add {removePlural(props.title)}</Button>
            </div>
            </Card.Header>
            <Card.Body>
                <table striped bordered hover className="table table-condensed">
                    <thead>
                        <tr>
                        {
                            props.headers.map( (h, i) =>
                                (<th key={i}>{h}</th>)
                            )
                        }
                        </tr>
                    </thead>     
                    <tbody>
                    {
                        props.values.map( (r, i) =>(
                            <tr key={i}>
                                {
                                    getEntityValues(r).map( (f, x) => (
                                        <td key={x}>
                                            <p>{f}</p>
                                        </td>        
                                    ))
                                }
                                <td>
                                    <Button size="sm">Delete</Button>
                                </td>    
                            </tr>                           
                        ))
                    }                     
                    {
                        props.totals.map( (r, i) => (
                            <tr key={i} className="cashFlowTotalDisplay">
                                {
                                    getEntityValues(r).map( (f, x) => (
                                        <td key={x}>
                                            <p>{f}</p>
                                        </td>        
                                    ))
                                }
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