/// <reference path="InvestmentAccount.js" />

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
    $("#memberEntry").each(() => {
        var level = this.authLevel.options[this.authLevel.selectedIndex].value;
        if (members != "[")
            members += ",";
        members += "{\"MemberID\":\"" + this.memberID.value + "\", \"AuthorisationType\":\"" + level + "\"}";
    });
    members += "]";

    accountForm.SerialisedMembers.value = members;
    //accountForm.submit();
    //add memebers to hidden serialisedMembers element and submit form

}

function addMemberSlot() {
    alert("addMemberSlot");
    $("#memberList").append($("#memberEntry").first().clone());
}