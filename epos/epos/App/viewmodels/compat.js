define([], function () {
    var title = ko.observable('Old EMR Compatibility');
    var vm = {
        activate: activate,
        title: title,
        updateLastDate: updateLastDate
    };

    return vm;

    function activate() {
        navigation.clear();
        navigation.setHomeTab(vm.title, '#compat', true);
        return true;
    }

    function updateLastDate() {
        var lastDateWindow = $("#lastDateWindow");
        if (!lastDateWindow.data("kendoWindow")) {
            lastDateWindow.kendoWindow({
                modal: true,
                width: "650px",
                title: "Update Patient's Last Visit Date",
            });
        }
        lastDateWindow.data("kendoWindow").open();

        return false;
    }

    function updateLastDatePopup() {
        return utility.httpPost('api/printqueue', item).then(function (data) {
            if (data.Success === true) {
                removeItem = true;
                vm.items.remove(item);
                toastr.info(data.Message);
            }
        });
        return false;
    }
});