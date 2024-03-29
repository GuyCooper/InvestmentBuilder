import React,  { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import {Card, Button} from 'react-bootstrap';
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js";

const Summary = () =>
{
    const [accountName, setAccountName] = useState(null);
    const [reportingCurrency, setReportingCurrency] = useState(null);
    const [valuePerUnit, setValuePerUnit] = useState(null);
    const [netAssets, setNetAssets] = useState(null);
    const [bankBalance, setBankBalance] = useState(null);
    const [monthlyPnL, setmonthlyPnL] = useState(null);
    const [valuationDate, setValuationDate] = useState(null);
    const [canBuild, setCanBuild] = useState(false);

    const onLoadAccountSummary = function (response) {

        console.log(  "account summary: " + JSON.stringify(response));

        setAccountName( response.AccountName.Name );
        setReportingCurrency( response.ReportingCurrency );
        setValuePerUnit( response.ValuePerUnit );
        setNetAssets( response.NetAssets );
        setBankBalance( response.BankBalance );
        setmonthlyPnL( response.MonthlyPnL );

        var dtValuation = new Date(response.ValuationDate);
        setValuationDate( dtValuation.toDateString() );
    };

    const loadAccountSummary = function () {
        console.log("loadAccountSummary called");
        middlewareService.GetInvestmentSummary(onLoadAccountSummary);
    }

    const onBuildProgress = function(response) {

        let buildStatus = response.Status;
        if(buildStatus !== undefined && buildStatus !== null) {
            console.log("IsBuilding: " + buildStatus.IsBuilding);
            notifyService.UpdateBusyState(buildStatus.IsBuilding);
            if (buildStatus.IsBuilding === false) {
                console.log("Build Complete. Report: " + buildStatus.CompletedReport );
                notifyService.InvokeAccountChange();
                notifyService.OnReportComplete(buildStatus.Errors, buildStatus.CompletedReport);
            }
        } 
    }

    const buildReport = function() {
        setCanBuild(false);
        middlewareService.BuildReport(onBuildProgress);
    };

    const onBuildStatusChanged = function(status) {
        setCanBuild(status);
    };

    useEffect( () => {
        console.log('Register Summary handlers');
        notifyService.RegisterConnectionListener(loadAccountSummary);
        notifyService.RegisterAccountListener(loadAccountSummary);
        notifyService.RegisterBuildStatusListener(onBuildStatusChanged);

        return function() {
            console.log('Un Register Summary handlers');
            notifyService.UnRegisterConnectionListener(loadAccountSummary);
            notifyService.UnRegisterAccountListener(loadAccountSummary);    
            notifyService.UnRegisterBuildStatusListener(onBuildStatusChanged);
        };
    });


    return(
        <Card bg="light" className="mt-sm-3 text-center">
            <Card.Header as="h3">{accountName}</Card.Header>
            <Card.Body>
                <div className="summaryContainer">
                    <div className="summaryChild">
                        <div className="summaryDisplay">
                            <div><strong>Reporting Currency</strong></div>
                            <div>{reportingCurrency}</div>
                        </div>
                        <div className="summaryDisplay">
                            <div><strong>Valuation Date</strong></div>
                            <div>{valuationDate}</div>
                        </div>
                        <div className="summaryDisplay">
                            <div><strong>Value Per Unit</strong></div>
                            <div>{valuePerUnit}</div>
                        </div>
                        <div className="summaryDisplay">
                            <div><strong>Net Assets</strong></div>
                            <div>{netAssets}</div>
                        </div>
                        <div className="summaryDisplay">
                            <div><strong>Bank Balance</strong></div>
                            <div>{bankBalance}</div>
                        </div>
                        <div className="summaryDisplay">
                            <div><strong>Month PnL</strong></div>
                            <div>{monthlyPnL}</div>
                        </div>
                    </div>
                </div>
                <Button onClick={buildReport}  variant="primary" disabled={canBuild === false}>Build Report</Button>
            </Card.Body>
        </Card>        
    );
}

export default Summary;