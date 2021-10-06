import 'bootstrap/dist/css/bootstrap.min.css';
import React,  { useEffect  } from 'react';
import {Container, Row, Col } from 'react-bootstrap';
import Testcard from './TestCard.js';
import TestNav from './TestNav.js'
import TestTabs from './TestTabs.js'
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js"

const Testbootstrap = () =>
{
    let onConnnectionSuccess = function( payload, username ) {

        console.log( 'Connection success! ' +  JSON.stringify(payload));
        notifyService.OnConnected( payload.ConnectionId, username );        
    };

    let onConnectionFailed = function( error) {
        console.log('Connection failed:( ' + error);
        alert( "connectionfailed " + error );
    };

    let onConfigLoaded = function (data) {
        console.log( 'middleware connection: ' + data.url);
        let username = 'guy@guycooper.plus.com';
        let password = 'N@omi13James12';
        middlewareService.Connect( data.url, username, password )        
        .then(  payload =>  onConnnectionSuccess( payload, username ) )
        .catch( error => onConnectionFailed( error) );
    };

    useEffect(() =>
     {
        console.log('loading config...');
        fetch('http://localhost:3000/Config.json')
        .then(result => result.json())
        .then( data => onConfigLoaded(data));
        
    }, []);

    return(
        <Container fluid>
            <Row>
                <Col>
                    <TestNav/>
                </Col>
            </Row>
            <Row>
                <Col>
                    <Testcard/>
                </Col>
            </Row> 
            <Row>
                <Col>
                    <TestTabs/>
                </Col>
            </Row>
        </Container> 
    );
}

export default Testbootstrap;
