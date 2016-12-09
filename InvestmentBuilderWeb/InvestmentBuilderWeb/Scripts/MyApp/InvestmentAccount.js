/// <reference path="InvestmentAccount.js" />

$(document).ready(function () {
    $(".removeEntry").click(removeMemberSlot);
});

function submitInvestmentAccount() {

    //client side validation!!!
    var accountForm = document.forms.investmentAccountForm;
    var name = accountForm.AccountName.value;
    var description = accountForm.AccountDescription.value;

    var currencySelect = accountForm.ReportingCurrency;
    var currency = currencySelect.options[currencySelect.selectedIndex].value;

    var selectedType = null;
    var typeSelect = accountForm.AccountType;
    for (var i = 0; i < typeSelect.length; i++) {
        if (typeSelect[i].checked) selectedType = typeSelect[i].value;
    }

    //var serialisedMembers = accountForm.SerialisedMembers;

    var members = "[";
    for (i = 0; i < this.memberID.length; i++) {
        var level = this.authLevel[i].options[this.authLevel[i].selectedIndex].value;
        if (members != "[")
            members += ",";
        members += "{\"MemberID\":\"" + this.memberID[i].value + "\", \"AuthorisationType\":\"" + level + "\"}";
    }

    //$(".memberEntry").each(() => {
    //    var level = this.authLevel.options[this.authLevel.selectedIndex].value;
    //    if (members != "[")
    //        members += ",";
    //    members += "{\"MemberID\":\"" + this.memberID.value + "\", \"AuthorisationType\":\"" + level + "\"}";
    //});

    members += "]";

    accountForm.SerialisedMembers.value = members;
    //accountForm.submit();
    //add memebers to hidden serialisedMembers element and submit form

}

function addMemberSlot() {
    var members = $(".memberEntry"); 
    var entry = members.first().clone();
    entry.children(".removeEntry").click(removeMemberSlot);
    $("#memberList").append(entry);
}

function selectClubAccountType() {
    $("#addMemberSlot").show();
    $("#memberList").show();
}

function selectPersonalAccountType() {  
    $("#addMemberSlot").hide();
    $("#memberList").hide();
}

function removeMemberSlot() {

    //var selected = $(this).attr("name");
    var index = $(".memberEntry").index($(this).parent());
    alert("selected index: " + index);
    //var index = parseInt(selected);
    if (index > 0) {
        var elems = $("#memberList").children();
        if (elems.length > index) {
            elems.eq(index).remove();
        }
    }
}