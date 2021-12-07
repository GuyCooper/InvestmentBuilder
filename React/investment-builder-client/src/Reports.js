import React,  { useState, useEffect } from 'react';
import { Container, ListGroup, Row, Button } from 'react-bootstrap';
import middlewareService from "./MiddlewareService.js";
import notifyService from "./NotifyService.js";

const Reports =  function() {

    const[fromDate, setFromDate] = useState(new Date());
    const[recentReports, setRecentReports] = useState([]);
    const[sessionId, setSessionId] = useState(null);

    const onLoadContents = function (response) {
        setRecentReports( response.RecentReports);        
    };

    const loadReportsFromDate = function (date) { 

        setFromDate( date );
        var request = {
            DateFrom: date
        };

        middlewareService.LoadRecentReports(request, onLoadContents);
    };

    // Call server to load the reports...
    const loadReports = function () { 
       
        loadReportsFromDate( fromDate );
    };

    const loadRecentreports = function() {
        loadReportsFromDate( new Date());
    };

    const loadPreviousReports = function() {
        let dt = recentReports[recentReports.length - 1].ValuationDate;
        loadReportsFromDate( dt );
    };

    useEffect( () => {
        
        setSessionId( notifyService.GetSessionID());
        notifyService.RegisterReportsListener( loadReports );
        return function() {
            notifyService.UnRegisterReportsListener( loadReports );
        };
    });

    return(
        <Container fluid>
            <ListGroup>
                {
                    recentReports.map( (r,x) => (
                        <ListGroup.Item key={x}>
                            <a 
                                href={r.Link + ";session=" + sessionId} 
                                target="_blank" 
                                rel="noopener">
                                    {r.ValuationDate}
                            </a>
                        </ListGroup.Item>    
                    ))
                }
            </ListGroup>    

            <ListGroup horizontal>
                <ListGroup.Item>
                    <Button 
                        size="sm" 
                        variant="info"
                        onClick={loadRecentreports}>
                            Recent
                    </Button>
                </ListGroup.Item>
                <ListGroup.Item>
                    <Button 
                        size="sm" 
                        variant="info"
                        onClick={loadPreviousReports}>
                            Previous
                    </Button>                
                </ListGroup.Item>                                        
            </ListGroup>
        </Container>
    );
};

export default Reports;