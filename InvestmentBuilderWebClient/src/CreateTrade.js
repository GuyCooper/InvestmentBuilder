'use strict'

function CreateTrade(NotifyService, MiddlewareService) {
    var addTrade = this;
    addTrade.today = function () {
        addTrade.dt = new Date();
    };

    addTrade.dateOptions = {
        dateDisabled: false,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: null,
        startingDay: 1
    };

    addTrade.openDatePicker = function () {
        addTrade.datepicker.opened = true;
    };

    addTrade.format = 'dd-MMMM-yyyy';
    addTrade.altInputFormats = ['M!/d!/yyyy'];

    var onTradeSubmitted = function (response) {
        if (response.status == 'fail') {
            //trade submission failed. display error messages in error dialog

        }
    };

    addTrade.datepicker = {
        opened: false
    };

    addTrade.submitTrade = function () {
        MiddlewareService.UpdateTrade({
            TransactionDate: addTrade.dt,
            ItemName: addTrade.Name,
            Symbol: addTrade.Symbol,
            Quantity: addTrade.Quantity,
            ScalingFactor: addTrade.ScalingFactor,
            Currency: addTrade.currency,
            Exchange: addTrade.Exchange,
            TotalCost: addTrade.totalCost
        }, onTradeSubmitted);
    };

    addTrade.cancel = function () {

    };

    NotifyService.RegisterAddTradeListener = function () {
        addTrade.today();
    };
}