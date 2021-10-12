import 'bootstrap/dist/css/bootstrap.min.css';
import React,  { useEffect  } from 'react';
import {Container, Row, Col } from 'react-bootstrap';
import Summary from './Summary.js';
import HeaderBar from './HeaderBar.js'
import TabOptions from './TabOptions.js'
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js"

const TopLevel = () =>
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
                    <HeaderBar/>
                </Col>
            </Row>
            <Row>
                <Col>
                    <Summary/>
                </Col>
            </Row> 
            <Row>
                <Col>
                    <TabOptions/>
                </Col>
            </Row>
        </Container> 
    );
}

export default TopLevel;
