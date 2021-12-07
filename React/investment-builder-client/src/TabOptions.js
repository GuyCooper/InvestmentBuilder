import 'bootstrap/dist/css/bootstrap.min.css';
import {Tab, Tabs} from 'react-bootstrap';
import React,  { useState } from 'react';
import Portfolio from './Portfolio.js'
import notifyService from "./NotifyService.js";
import CashFlows from './CashFlows.js';
import AddInvestment from './AddInvestment.js';
import Reports from './Reports.js'

const TabOptions = () =>
{
    const portfolio = 'portfolio';
    const addinvestment = 'addinvestment';
    const cashflow = 'cashflow';
    const reports='reports';

    const [key, setKey] = useState(portfolio);

    let tabActions = [];
    tabActions[portfolio] = () => notifyService.InvokePortfolio();
    tabActions[addinvestment] = () => notifyService.InvokeAddTrade();
    tabActions[cashflow] = () => notifyService.InvokeCashFlow();
    tabActions[reports] = () => notifyService.InvokeReports();
    
  
    let onTabChanged = function(tab) {
        console.log("tab changed to " + tab);
        setKey( tab );
        tabActions[tab]();
    };

    return(
        <Tabs
        id="controlled-tab-example"
        className="mt-sm-2"
        activeKey={key}
        onSelect={(k) => onTabChanged(k)}        
        >
            <Tab eventKey={portfolio} title="Portfolio">
                <Portfolio/>
            </Tab>
            <Tab eventKey={addinvestment} title="Add Investment">
                <AddInvestment/>
            </Tab>
            <Tab eventKey={cashflow} title="Cash Flow">
                <CashFlows/>
            </Tab>
            <Tab eventKey={reports} title="Reports">
                <Reports/>
            </Tab>

        </Tabs>
    );
}

export default TabOptions;