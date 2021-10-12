import 'bootstrap/dist/css/bootstrap.min.css';
import {Tab, Tabs} from 'react-bootstrap';
import React,  { useState } from 'react';
import Portfolio from './Portfolio.js'
import notifyService from "./NotifyService.js";

const TabOptions = () =>
{
    const portfolio = 'portfolio';
    const addinvestment = 'addinvestment';
    const cashflow = 'cashflow';

    const [key, setKey] = useState(portfolio);

    let tabActions = [];
    tabActions[portfolio] = () => notifyService.InvokePortfolio();
    tabActions[addinvestment] = () => notifyService.InvokeAddTrade();
    tabActions[cashflow] = () => notifyService.InvokeCashFlow();
    
  
    let onTabChanged = function(tab) {
        console.log("tab changed to " + tab);
        setKey( tab );
        tabActions[tab]();
    };

    return(
        <Tabs
        id="controlled-tab-example"
        className="mt-sm-3"
        activeKey={key}
        onSelect={(k) => onTabChanged(k)}        
        >
            <Tab eventKey={portfolio} title="Portfolio">
                <Portfolio/>
            </Tab>
            <Tab eventKey={addinvestment} title="Add Investment">
                Add a new investment
            </Tab>
            <Tab eventKey={cashflow} title="Cash Flow">
                cashflows
            </Tab>
        </Tabs>
    );
}

export default TabOptions;