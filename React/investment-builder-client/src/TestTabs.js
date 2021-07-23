import 'bootstrap/dist/css/bootstrap.min.css';
import {Tab, Tabs} from 'react-bootstrap';
import React,  { useState } from 'react';
import TestGrid from './TestGrid.js'

const TestTabs = () =>
{
    const [key, setKey] = useState('portfolio');

    return(
        <Tabs
        id="controlled-tab-example"
        className="mt-sm-3"
        activeKey={key}
        onSelect={(k) => setKey(k)}        
        >
            <Tab eventKey="portfolio" title="Portfolio">
                <TestGrid/>
            </Tab>
            <Tab eventKey="addinvestment" title="Add Investment">
                Add a new investment
            </Tab>
            <Tab eventKey="cashflow" title="Cash Flow">
                cashflows
            </Tab>
        </Tabs>
    );
}

export default TestTabs;