import React,  { useState, useRef } from 'react';
//import { render } from 'react-dom';
import {AgGridColumn, AgGridReact} from 'ag-grid-react';
import 'ag-grid-enterprise';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js";
import ButtonCellRenderer from "./ButtonCellRenderer.js";

const TestGrid = () => 
{
    const [rowData, setRowData] = useState([]);
    const gridRef = useRef(null);

    let reloadPortfolio = false;
    
    const portfolioLoaded = function( portfolio ){
        console.log('portfolio loaded: ' + JSON.stringify( portfolio ));
        setRowData( portfolio.Portfolio );
    }

    const loadPortfolio = function() {
        if (reloadPortfolio === true) {
            console.log("Loading portfolio!!");
            middlewareService.LoadPortfolio(portfolioLoaded);
            reloadPortfolio = false;
        }
    };

    const refreshPortfolio = function() {
        console.log("refreshPortfolio called");
        reloadPortfolio = true;
        loadPortfolio();
    };

    notifyService.RegisterPortfolioListener( loadPortfolio );

    notifyService.RegisterConnectionListener(refreshPortfolio);

    const onButtonClick = e => {
              const selectedNodes = gridRef.current.api.getSelectedNodes();
              if(selectedNodes.length === 1) {
                const selectedData = selectedNodes[0].data;
                alert('selected trade: ' + selectedData.Name);    
              }
          };

    const editTrade = function( field )  {
        alert(`${field} was clicked`);
    };
          
    const dateFormatter = function(val) {
        var dateobj = new Date(val.value);
        return dateobj.toLocaleDateString();
    };

        
    const columndefs = [
        { headerName: "Name", field: "Name", sortable: true, filter: true },
        { headerName: "Quantity", field: "Quantity", sortable: true, filter: true, type:'numericColumn', width:100 },
        { headerName: "Last Brought", field: "LastBrought", cellFormatter: dateFormatter, sortable: true, filter: true },
        { headerName: "Avg.Price Paid", field: "AveragePricePaid", sortable: true, filter: true,type:'numericColumn' },
        { headerName: "Total Cost", field: "TotalCost", sortable: true, filter: true, type:'numericColumn', width:100 },
        { headerName: "Current Price", field: "SharePrice", sortable: true, filter: true, type:'numericColumn' },
        { headerName: "Net Value", field: "NetSellingValue", sortable: true, filter: true, type:'numericColumn' },
        { headerName: "Profit/Loss", field: "ProfitLoss", sortable: true, filter: true, type:'numericColumn' },
        { headerName: "Month Change%", field: "MonthChangeRatio", sortable: true, filter: true, type:'numericColumn', width:100 },
        { headerName: "Options", field: "Name", cellRenderer: "btnCellRenderer", 
                                 cellRendererParams: {
                                    clicked : function( field ) {
                                        editTrade( field );            
                                    } ,
                                    label : 'Edit Trade'
                                 } 
        }
    ];

    const defaultColDefs = {
       maxWidth : 200
    };
    
    const frameworkComponents = {
        btnCellRenderer : ButtonCellRenderer
    };

    return (
        <div className="ag-theme-alpine mt-sm-3" >
             <button onClick={onButtonClick}>Get selected rows</button>
            <AgGridReact
                domLayout='autoHeight'
                ref={gridRef}
                rowData={rowData}
                rowSelection="single"
                columnDefs={columndefs}
                frameworkComponents={frameworkComponents}    
                defaultColDef={defaultColDefs}     
                >                        
            </AgGridReact>
        </div>
    );        

};

export default TestGrid;