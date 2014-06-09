﻿define([], function () {
    var title = ko.observable('Old EMR Compatibility');
    var displayType = ko.observable(1);
    var serverError = ko.observable(false);
    var serverMessage = ko.observable('');
    var patientCount = ko.observable();
    var vm = {
        activate: activate,
        title: title,
        updateLastDate: updateLastDate,
        updateLastDatePopup: updateLastDatePopup,
        updateHistory: updateHistory,
        updateHistoryPopup: updateHistoryPopup,
        displayType: displayType,
        serverMessage: serverMessage,
        serverError: serverError,
        patientCount: patientCount
    };

    return vm;

    function activate() {
        navigation.clear();
        navigation.setHomeTab(vm.title, '#compat', true);
        return true;
    }

    function windowClose() {
        vm.displayType(1);
        vm.serverError(false);
        vm.serverMessage('');
    }

    function updateLastDate() {
        var lastDateWindow = $("#lastDateWindow");
        if (!lastDateWindow.data("kendoWindow")) {
            lastDateWindow.kendoWindow({
                modal: true,
                width: "350px",
                title: "Update Patient's Last Visit Date",
                close: windowClose
            });
        }
        lastDateWindow.data("kendoWindow").open().center();

        return false;
    }


    function updateLastDatePopup() {
        vm.displayType(2);
        return utility.httpPost('api/compatlastvisit').then(function (data) {
            if (data.Success === true) {
                toastr.info(data.Message);
            }
            else {
                vm.serverError(true);
            }
            vm.displayType(3);
            vm.serverMessage(data.Message);
        });
        return false;
    }

    function updateHistory() {
        var patientHistoryWindow = $("#patientHistoryWindow");
        if (!patientHistoryWindow.data("kendoWindow")) {
            patientHistoryWindow.kendoWindow({
                modal: true,
                width: "350px",
                title: "Update Patient's History",
                close: windowClose
            });
        }
        patientHistoryWindow.data("kendoWindow").open().center();

        return false;
    }


    function updateHistoryPopup() {
        vm.displayType(2);
        return utility.httpPost('api/compatpatientids').then(function (data) {
            if (data.Success === true) {
                //toastr.info(data.Message);
                vm.displayType(3);
                updateHistoryForAllPatients(data.Model)
            }
            else {
                vm.serverError(true);
                vm.displayType(4);
                vm.serverMessage(data.Message);
            }
        });
        return false;
    }

    function updateHistoryForAllPatients(patients) {
        var patient;
        var currentIndex;
        var totalPatients = patients.length;
        patientCount(totalPatients);

        //setting the progress bar
        var historyProgressBar = $("#historyProgressBar").data("kendoProgressBar");
        if (!historyProgressBar) {
            $("#historyProgressBar").kendoProgressBar({
                max: totalPatients,
                value: 0,
                animation: {
                    duration: 50  
                }            
            });
            historyProgressBar = $("#historyProgressBar").data("kendoProgressBar");
        }
        historyProgressBar.value(0);

        for (var index = 0; index < patients.length; index++) {
            var patientID = patients[index];
            currentIndex = index + 1;
            updateHistoryForeachPatient(patientID, currentIndex, historyProgressBar)
        }

        //alert('done');

    }

    function updateHistoryForeachPatient(patientID, currentIndex, historyProgressBar) {
        utility.httpPost('api/compatupdatehistory', patientID).then(function (data) {
            if (currentIndex > historyProgressBar.value())
                historyProgressBar.value(currentIndex);
        });
    }

    //compatupdatehistory
});