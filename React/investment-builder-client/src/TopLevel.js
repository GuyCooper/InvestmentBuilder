import 'bootstrap/dist/css/bootstrap.min.css';
import React,  { useEffect, useState  } from 'react';
import {Container, Row, Col, Spinner } from 'react-bootstrap';
import Summary from './Summary.js';
import HeaderBar from './HeaderBar.js'
import TabOptions from './TabOptions.js'
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js"
import ReportCompletion from "./ReportCompletion.js"

const TopLevel = () =>
{
    const [isBusy, setIsBusy] = useState( false );
    const [showReportComplete, setShowReportComplete] = useState(false);
    const [buildSuccess, setBuildSuccess] = useState(false);
    const [buildErrors, setBuildErrors] = useState([]);
    const [completedReport, setCompletedReport] = useState(null);

    const onConnnectionSuccess = function( payload, username ) {

        console.log( 'Connection success! ' +  payload);
        let connectionObj = JSON.parse( payload );
        notifyService.OnConnected( connectionObj.ConnectionId, username );        
    };

    const onConnectionFailed = function( error) {
        console.log('Connection failed:( ' + error);
        alert( "connectionfailed " + error );
    };

    const onConfigLoaded = function (data) {
        console.log( 'middleware connection: ' + data.url);
        let username = 'guy@guycooper.plus.com';
        let password = 'N@omi13James12';
        middlewareService.Connect( data.url, username, password )        
        .then(  payload =>  onConnnectionSuccess( payload, username ) )
        .catch( error => onConnectionFailed( error) );
    };

    const onBusyStateChanged = function(busy) {
        setIsBusy( busy );
    };

    const onReportFinished = function (errors, completedReport) {
        setBuildSuccess(errors === null || errors.length === 0);
        setBuildErrors(errors);    
        setCompletedReport( completedReport + ";session=" + notifyService.GetSessionID());             
        setShowReportComplete(true);
    };

    useEffect(() =>
     {
        console.log("Registering TopLevel");
        if(middlewareService.ConnectionClosed()) {         
            let config = window.location.href + 'Config.json';
            console.log('loading config...' + config);
            fetch(config) // 'http://localhost:3000/Config.json')
            .then(result => result.json())
            .then( data => onConfigLoaded(data));
        }

        notifyService.RegisterBusyStateChangedListener(onBusyStateChanged);
        notifyService.RegisterReportCompleteListener(onReportFinished);
        
        return function() {
            console.log("Un Registering TopLevel");
            notifyService.UnRegisterBusyStateChangedListener();        
            notifyService.UnRegisterReportCompleteListener();    
        };

    });

    return(
        <Container fluid>
            <Row>
                <Col>
                    <HeaderBar/>
                </Col>
            </Row>
            {isBusy &&  <div className="center">
                            <Spinner animation="border" variant='primary' />
                        </div>}                       
            {!isBusy && <div><Row>
                <Col>
                    <Summary/>
                </Col>
            </Row> 
            <Row>
                <Col>
                    <TabOptions/>
                </Col>
            </Row></div>}
            <ReportCompletion
                show={showReportComplete}
                onHide={() => setShowReportComplete(false)} 
                success={buildSuccess}
                errors={buildErrors}
                completedReport={completedReport}/>                
        </Container> 
    );
}

export default TopLevel;
