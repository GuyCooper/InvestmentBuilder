"use strict"

function RegisterUser($scope, $http) {
    $scope.EMailAddress = "";
    $scope.Password = "";
    $scope.ConfirmPassword = "";
    $scope.IsError = false;
    $scope.ErrorList = [];
    $scope.IsBusy = false;
    $scope.Complete = false;
    $scope.Success = false;

    var validated = false;
    var servername = "";

    var validateEmailAddress = function () {
        var validated = $scope.EMailAddress.match(/.+@.+/) != null;
        if (validated === false) {
            $scope.IsError = true;
            $scope.ErrorList.push("Invalid email address");
        }
    };

    var validatePassword = function () {

        var validated = $scope.Password.length >= 8;
        validated &= ($scope.Password.match(/[A-Z]/) != null);
        validated &= ($scope.Password.match(/[a-z]/) != null);
        validated &= ($scope.Password.match(/[0-9]/) != null);
        if (validated == false) {
            $scope.IsError = true;
            $scope.ErrorList.push("password must contain at least 8 characters, include at least one uppercase character, one lowercase character and one digit");
            return;
        }

        if ($scope.Password !== $scope.ConfirmPassword) {
            $scope.ErrorList.push("passwords do not match");
            $scope.IsError = true;
            return;
        }
        
        $scope.IsError = false;
    };


    var onsuccess = function(response) {
        console.log("success");
        $scope.Complete = true;
        $scope.IsBusy = false;
        $scope.Success = true;
    };

    var onfail = function(response) {
        console.log("failed");
        $scope.Complete = true;
        $scope.IsBusy = false;
        $scope.Success = false;
        $scope.ErrorList.push(response);
    };

    var getToken = function () {
        var index = window.location.search.indexOf("=");
        if(index > 1) {
            return window.location.search.slice(index+1);
        }
        return "";
    }

    $scope.registerUser = function () {
        $scope.ErrorList = [];
        validateEmailAddress();
        validatePassword();
        if ($scope.IsError === false) {            
            var userDetails = {
                UserName: "",
                EMailAddress : $scope.EMailAddress, 
                Password: $scope.Password,
                ConfirmPassword: $scope.ConfirmPassword,
                PhoneNumber : ""
            };

            $scope.IsBusy = true;
            $http.post(servername + "/RegisterUser", userDetails).then(onsuccess, onfail);
        }
    };

    $scope.forgottenPassword = function () {
        $scope.ErrorList = [];
        validateEmailAddress();
        if ($scope.IsError === false) {
            var config = {
                params: { "EMailAddress": $scope.EMailAddress }
            };

            $scope.IsBusy = true;
            $http.get(servername + "/ForgottonPassword", config).then(onsuccess, onfail);
        }
    };

    $scope.changePassword = function () {
        var token = getToken();
        $scope.ErrorList = [];
        validateEmailAddress();
        validatePassword();
        if ($scope.IsError === false) {
            var userDetails = {
                "Token": token,
                "EMail": $scope.EMailAddress,
                "Password": $scope.Password
            };

            $scope.IsBusy = true;
            $http.post(servername + "/ChangePassword", userDetails).then((result) => {
                if (result.data.Result !== 1) {
                    onfail(result.data.ResultMessage);
                } else {
                    onsuccess();
                }
            }
            , onfail);
        }
    };

    $http.get("/config.json").then(function (data) {
        servername = data.data.registerurl;
    });
}

var module = angular.module("InvestmentBuilderRegister", []);
module.controller("RegisterUser", RegisterUser);