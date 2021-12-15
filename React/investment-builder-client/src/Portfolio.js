import React,  { useState, useRef, useEffect } from 'react';
import { AgGridReact} from 'ag-grid-react';
import 'ag-grid-enterprise';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js";
import ButtonCellRenderer from "./ButtonCellRenderer.js";
import EditTrade from './EditTrade'

const Portfolio = () => 
{
    const [rowData, setRowData] = useState([]);
    const [showEditTrade, setShowEditTrade] = useState(false);
    const [selectedTrade, setSelectedTrade] = useState('');

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

    useEffect( () => {

        console.log('Register Portfolio handlers');
        notifyService.RegisterPortfolioListener( loadPortfolio );
        notifyService.RegisterConnectionListener(refreshPortfolio);
        notifyService.RegisterAccountListener(refreshPortfolio);
    
        return function() {
            console.log('UnRegister CashFlows handlers');
            notifyService.UnRegisterPortfolioListener( loadPortfolio );
            notifyService.UnRegisterConnectionListener(refreshPortfolio);
            notifyService.UnRegisterAccountListener(refreshPortfolio);        
        };
    });
    
    const editTrade = function( field )  {
       console.log( field + ' was clicked');
       setSelectedTrade( field );
       setShowEditTrade( true )

    };
          
    const dateFormatter = function(val) {
        var dateobj = new Date(val.value);
        return dateobj.toLocaleDateString();
    };

    
    const numberFormatter = function( val ){
        return val.value.toFixed( 2 );
    };

    const columndefs = [
        { headerName: "Name", field: "Name", sortable: true, filter: true },
        { headerName: "Quantity", field: "Quantity", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn', width:100 },
        { headerName: "Last Brought", field: "LastBrought", valueFormatter: dateFormatter, sortable: true, filter: true },
        { headerName: "Avg.Price Paid", field: "AveragePricePaid", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn' },
        { headerName: "Total Cost", field: "TotalCost", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn', width:100 },
        { headerName: "Current Price", field: "SharePrice", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn' },
        { headerName: "Net Value", field: "NetSellingValue", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn' },
        { headerName: "Profit/Loss", field: "ProfitLoss", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn' },
        { headerName: "Month Change%", field: "MonthChangeRatio", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn', width:100 },
        { headerName: "Edit Trade", field: "Name", cellRenderer: "btnCellRenderer", 
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
        <>
            <div className="ag-theme-alpine mt-sm-3" >

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

            <EditTrade
                    name={selectedTrade}
                    show={showEditTrade}
                    onHide={() => setShowEditTrade(false)} />

        </>
    );        

};

export default Portfolio;