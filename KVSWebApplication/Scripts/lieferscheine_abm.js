function Closewindow() {
    var Result = confirm("Für ausgewählten Standort haben wir nicht geschlossene Auftraege gefunden. Wollen Sie trotzdem fortfahren?");
    if (Result == true) {
//        var UserValueConfirm = $('<%= UserValueConfirm.ClientID %>');
//        UserValueConfirm.value = Result;
        __doPostBack('UserValueConfirmLieferscheine', "CloseWindow");
        return true;
    }
    else {

//        var hidField = $('<%= UserValueConfirm.ClientID %>');
//        hidField.value = Result;
        __doPostBack('UserValueDontConfirmLieferscheine', "CloseWindow");
        return false;
    }
}

function selectRows(sender) {
    var header = sender.parentNode.parentNode;
    var currentNode = header.nextSibling;
    var masterTable = $('<%= RadGridLieferscheine.ClientID %>').get_masterTableView();
    if (sender.checked) {
        while (currentNode.className != 'rgGroupHeader' && typeof currentNode.className != 'undefined') {
            masterTable.selectItem(currentNode);
            currentNode = currentNode.nextSibling;
        }
    }
    else {
        while (currentNode.className != 'rgGroupHeader' && typeof currentNode.className != 'undefined') {
            masterTable.deselectItem(currentNode);
            currentNode = currentNode.nextSibling;
        }
    }
}
function CreatePacking() {

    __doPostBack('UserValueConfirmLieferscheine', "CreatePacking");
    return true;

}
function RequestStart(sender, eventArgs) {

    var eventTarget = eventArgs.get_eventTarget();
    if (eventTarget.indexOf("LieferscheinDruckenButton") != -1) {
        eventArgs.set_enableAjax(false);
    }
}
