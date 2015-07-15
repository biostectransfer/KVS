


function RequestStart(sender, eventArgs) {

    var eventTarget = eventArgs.get_eventTarget();
    if (eventTarget.indexOf("LieferscheinDruckenButton") != -1) {
        eventArgs.set_enableAjax(false);
    }
}

function Closewindow() {
    var Result = confirm("Für ausgewählten Standort haben wir nicht geschlossene Auftraege gefunden. Wollen Sie trotzdem fortfahren?");
    if (Result == true) {
       // var UserValueConfirm = $find('MainContentPlaceHolder_LieferscheineuserControl_UserValueConfirm');
        //UserValueConfirm.value = Result;
        __doPostBack('UserValueConfirmLieferscheine', "CloseWindow");
        return true;
    }
    else {
       // $find('MainContentPlaceHolder_LieferscheineuserControl_UserValueConfirm').value = Result;
        __doPostBack('UserValueDontConfirmLieferscheine', "CloseWindow");
        return false;
    }
}
function CreatePacking() {

    __doPostBack('UserValueConfirmLieferscheine', "CreatePacking");
    return true;

}
function selectRows(sender) {
    var header = sender.parentNode.parentNode;
    var currentNode = header.nextSibling;
    var masterTable = $find('MainContentPlaceHolder_LieferscheineuserControl_RadGridLieferscheine').get_masterTableView();
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