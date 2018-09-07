"use strict"

// controller for add / edit account
function AddAccount($scope, $uibModalInstance, user, currencies, account, brokers) {

    //set the default values
    $scope.AccountName = "";
    $scope.AccountDescription;
    $scope.Currencies = currencies;
    $scope.Brokers = brokers;
    $scope.ReportingCurrency = currencies[0];
    $scope.SelectedBroker = brokers[0];
    $scope.AccountType = "Club";

    var members = [];

    if (account != null) {
        $scope.Title = "Edit Account";
        $scope.AccountName = account.AccountName;
        $scope.AccountDescription = account.Description;
        $scope.ReportingCurrency = account.ReportingCurrency;
        $scope.AccountType = account.AccountType;
        $scope.SelectedBroker = account.Broker;
        for(var index = 0; index < account.Members.length; index++)
        {
            members.push({ Name: account.Members[index].Name, Permission: account.Members[index].Permission });
        }
    }
    else {
        $scope.Title = "Add Account";
         members.push({ Name: user, Permission: "ADMINISTRATOR" });
    }
    
    $scope.EditAccount = account != null;

    $scope.gridOptions = {
        columnDefs: [
            { headerName: "Name", field: "Name", editable: true },
            {
                headerName: "Permission", field: "Permission", editable: true,
                cellEditor: 'select',
                cellEditorParams : { values : ["ADMINISTRATOR", "NONE", "READ", "UPDATE"] }
            }
        ],
        rowData: members,
        angularCompileRows: true,
        enableColResize: true,
        enableSorting: true,
        enableFilter: true,
        suppressRowClickSelection: true,
        singleClickEdit: true,
        rowSelection: 'multiple',
        defaultColDef: {
            width: 100,
            headerCheckboxSelection: isFirstColumn,
            checkboxSelection: isFirstColumn
        },
        //cellValueChanged : onCellValueChanged 
    };

    function isFirstColumn(params) {
        var displayedColumns = params.columnApi.getAllDisplayedColumns();
        var thisIsFirstColumn = displayedColumns[0] === params.column;
        return thisIsFirstColumn;
    };

    $scope.gridOptions.onCellValueChanged = function (data) {
        console.log(data);
    };

    $scope.onTypeChange = function () {
        $scope.IsClubTypeAccount = $scope.AccountType === "Club";
    };

    $scope.onTypeChange();

    $scope.accountNameChanged = function () {
        $scope.CanSubmit = $scope.AccountName != "";
    };

    $scope.accountNameChanged();

    $scope.addMember = function () {
        members.push({ Name: "", Permission: "READ"});
        $scope.gridOptions.api.setRowData(members);
    };

    $scope.removeMembers = function () {
        var selectedNodes = $scope.gridOptions.api.getSelectedNodes();
        console.log("removing nodes: " + selectedNodes.length);
        
        var updated = members.filter((member, index) => {
            return selectedNodes.find(node => node.rowIndex === index) === undefined;
        });
        members = updated;
        $scope.gridOptions.api.setRowData(members);
    };

    $scope.ok = function () {
        console.log("name: " + $scope.AccountName + ", AccountType: " + $scope.AccountType);
        $uibModalInstance.close({
            AccountName : $scope.AccountName,
            Description:  $scope.AccountDescription,
            ReportingCurrency: $scope.ReportingCurrency,
            Broker: $scope.SelectedBroker,
            AccountType: $scope.AccountType,
            Enabled : true,
            Members : members
        }, $scope.EditAccount);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};