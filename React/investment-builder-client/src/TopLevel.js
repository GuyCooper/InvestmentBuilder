import 'bootstrap/dist/css/bootstrap.min.css';
import React,  { useEffect, useState  } from 'react';
import {Container, Row, Col, Spinner } from 'react-bootstrap';
import Summary from './Summary.js';
import HeaderBar from './HeaderBar.js'
import TabOptions from './TabOptions.js'
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js"

const TopLevel = () =>
{
    const [isBusy, setIsBusy] = useState( false );

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

    useEffect(() =>
     {
        console.log('loading config...');
        let config = window.location.href + 'Config.json';
        fetch(config) // 'http://localhost:3000/Config.json')
        .then(result => result.json())
        .then( data => onConfigLoaded(data));
        
        notifyService.RegisterBusyStateChangedListener(onBusyStateChanged);
        
        return function() {
            notifyService.UnRegisterBusyStateChangedListener();            
        };

    });

    return(
        <Container fluid>
            <Row>
                <Col>
                    <HeaderBar/>
                </Col>
            </Row>
            {isBusy && <Spinner animation="border" variant="primary" />}
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
        </Container> 
    );
}

export default TopLevel;
