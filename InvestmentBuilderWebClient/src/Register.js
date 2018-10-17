"use strict"

function RegisterUser($scope, $http) {
    $scope.EMailAddress = "";
    $scope.Password = "";
    $scope.ConfirmPassword = "";
    $scope.IsError = false;
    $scope.ErrorList = [];

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
            $http.post(servername + "/RegisterUser", userDetails).then(function (response) {
                console.log("success");
            },
            function (response) {
                console.log("failed");
            }
            );
        }
    };

    $http.get("/config.json").then(function (data) {
        servername = data.data.registerurl;
    });
}

var module = angular.module("InvestmentBuilderRegister", []);
module.controller("RegisterUser", RegisterUser);