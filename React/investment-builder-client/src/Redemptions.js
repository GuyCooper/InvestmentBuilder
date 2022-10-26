import React,  { useState, useRef, useEffect } from 'react';
import { Button } from 'react-bootstrap';
import { AgGridReact} from 'ag-grid-react';
import 'ag-grid-enterprise';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';
import notifyService from "./NotifyService.js";
import middlewareService from "./MiddlewareService.js";
import AddRedemption from './AddRedemption.js';
import ButtonCellRenderer from "./ButtonCellRenderer.js";

const Redemptions = () => 
{
    const [rowData, setRowData] = useState([]);
    const [showAddRedemption, setShowAddRedemption] = useState(false);
    const [members, setMembers] = useState([]);
    const gridRef = useRef(null);

    let reloadRedemptions = false;
    
    const accountMembersLoaded = function( members ) {
        setMembers(members.Members);
    };

    const redemptionsLoaded = function( redemptions ){
        console.log('redemptions loaded: ' + JSON.stringify( redemptions ));
        setRowData( redemptions.Redemptions );

        middlewareService.GetAccountMembers(accountMembersLoaded);
    };

    const loadRedemptions = function() {
        if (reloadRedemptions === true) {
            console.log("Loading redemptions!!");
            middlewareService.GetRedemptions(redemptionsLoaded);
            reloadRedemptions = false;
        }
    };

    const refreshRedemptions = function() {
        console.log("refreshRedemptions called");
        reloadRedemptions = true;
        loadRedemptions();
    };


    const addRedemptionModal = function() {
        setShowAddRedemption(true);
    }

    const addRedemption = function(redemption) {
        middlewareService.RequestRedemption( redemption, refreshRedemptions);
        setShowAddRedemption(false);
    };

    useEffect( () => {
        console.log('Register Redemption handlers');
        notifyService.RegisterRedemptionListener( loadRedemptions );
        notifyService.RegisterConnectionListener(refreshRedemptions);
        notifyService.RegisterAccountListener(refreshRedemptions);
    
        return function() {
            console.log('UnRegister Redemption handlers');
            notifyService.UnRegisterRedemptionListener( loadRedemptions );
            notifyService.UnRegisterConnectionListener(refreshRedemptions);
            notifyService.UnRegisterAccountListener(refreshRedemptions);        
        };        
    });
    
    const dateFormatter = function(val) {
        var dateobj = new Date(val.value);
        return dateobj.toLocaleDateString();
    };

    
    const numberFormatter = function( val ){
        return val.value.toFixed( 2 );
    };

    const removeRedemptionResponse = function( response ) {
        console.log('removeRedemptionResponse: ' + response);
        if( response.IsError === true) {
            alert( "remove redemption failed" + response.Error );
        }
        else {
            refreshRedemptions();
        }
    };

    const removeRedemption = function( redemption ) {
        let dto = {
            RedemptionId : redemption
        };

        middlewareService.RemoveRedemption( dto , removeRedemptionResponse);
    };

    const columndefs = [
        { headerName: "User", field: "User", sortable: true, filter: true },
        { headerName: "Amount", field: "Amount", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn', width:120 },
        { headerName: "Transaction Date", field: "TransactionDate", valueFormatter: dateFormatter, sortable: true, filter: true },
        { headerName: "Redeemed Units", field: "RedeemedUnits", sortable: true, filter: true, valueFormatter: numberFormatter, type:'numericColumn' },
        { headerName: "Status", field: "Status", sortable: true, filter: true },
        { headerName: "Remove", field: "Id", cellRenderer: "btnCellRenderer", 
                                 cellRendererParams: { 
                                     clicked : function( field ) {
                                        removeRedemption( field );            
                                     } ,
                                    label : 'Remove Redemption'
                                 }
        } 

    ];    

    const defaultColDefs = {
        maxWidth : 180
     };

     const frameworkComponents = {
        btnCellRenderer : ButtonCellRenderer
    };

     return (
        <>
            <Button onClick={ () => addRedemptionModal()} className='reportSection'>Add Redemption</Button>
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

            <AddRedemption
                show={showAddRedemption}
                users={members}
                onHide={() => setShowAddRedemption(false)} 
                onSubmit={(redemption) => addRedemption(redemption) }
            />
        </>
    );             
};

export default Redemptions;
