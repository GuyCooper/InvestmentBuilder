import 'bootstrap/dist/css/bootstrap.min.css';
import {Navbar, Nav, Form} from 'react-bootstrap';
import React,  { useRef, useState, useEffect } from 'react';
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js"

const HeaderBar = function() {

    const [accountNames, setAccountNames] = useState([{Name:null,Id:null}]);
    const [isTest, setIsTest] = useState( false );
   
    const accountSelect = useRef(null);

    const changeAccount = function(e) {
        console.log("selected account " + accountSelect.current.value);
        let selectedAccount = accountNames[accountSelect.current.selectedIndex];
        middlewareService.UpdateCurrentAccount(
                        selectedAccount, 
                         () => notifyService.InvokeAccountChange());            
    };

    const accountNamesLoaded = function(data)  {
        setAccountNames( data.AccountNames);
        setIsTest(data.IsTest);
    };

    const loadAccountNames = function() {
        middlewareService.GetAccountsForUser(accountNamesLoaded);
    };

    useEffect( () => {
        notifyService.RegisterConnectionListener(loadAccountNames);

        return function() {
            notifyService.UnRegisterConnectionListener(loadAccountNames);
        };
    });

    return(
        <Navbar bg="primary" variant="dark">
            <Navbar.Brand href="#home">Investment Builder</Navbar.Brand>
            <Nav className="mr-auto">
                <Nav.Link href="#home">Home</Nav.Link>
                {isTest && <h2>Test</h2>}
            </Nav >
            <Form inline>                
                <Form.Control as="select"
                              onChange={ (e) => changeAccount(e)}
                              custom
                              ref={accountSelect}>
                                  {
                                        accountNames.map((t,i) =>
                                            (<option key={i}>{t.Name}</option>)
                                        )
                                  }
                </Form.Control>    
            </Form>            
        </Navbar>

    );
};

export default HeaderBar;