define(['services/message'], function (message) {

    var title = "Patients";
    var addText = "Add Patient";
    var notesText = "New Notes";
    var patientHistory = ko.observableArray();

    var model = {
        PatientID: ko.observable(0),
        PatientNumber: ko.observable().extend({ required: { message: "required" } }),
        PatientName: ko.observable(),
        Greeting: ko.observable(),
        FirstName: ko.observable().extend({ required: { message: "required" } }),
        MiddleName: ko.observable(),
        LastName: ko.observable().extend({ required: { message: "required" } }),
        NickName: ko.observable(),
        DateOfBirth: ko.observable(new Date()).extend({ required: { message: "required" } }),
        Sex: ko.observable(),
        Occupation: ko.observable(),
        HxFrom: ko.observable(),
        HxFromList: ko.observable(),
        HxFromOther: ko.observable(),
        ReferredFrom: ko.observable(),
        ReferredDoctor: ko.observable(),
        Allergies: ko.observable(),
        Medications: ko.observable(),
        PrematureBirth: ko.observable(),
        PcpInd: ko.observable(false)
    };

    model.Name = ko.computed(function () {
        return this.FirstName() + ' ' + this.LastName();
    }, model);

    model.ID = ko.computed(function () {
        return this.PatientID();
    }, model);

    model.PrematureBirthString = ko.computed({
        read: function () { return (this.PrematureBirth() ? "Yes" : "No"); },
        write: function (value) { this.PrematureBirth(value == "Yes" ? true : false); }
    }, model);

    var originalModel;
    var blankModel = ko.viewmodel.toModel(model);

    var vm = {
        title: title,
        addText: addText,
        notesText:notesText,
        model: model,
        originalModel: originalModel,
        blankModel: blankModel,
        noHistory: message.noHistory,
        getSearchResults: getSearchResults,
        patientHistory: patientHistory,
        navigateNotes: navigateNotes,
        getSelected: getSelected,
        setPCP: setPCP,
        cancelEdit: cancelEdit,
        save: save,
    };

    return vm;

    function getSearchResults(criteria) {
        return utility.httpPost('api/patientsearch', criteria);
    }

    function getSelected(patientID) {
        var getdata = { "id": patientID };
        return utility.httpGet('api/patient', getdata).then(function (data) {
            if (data.Success === true) {
                setPatient(data.Model);
                vm.patientHistory(data.Model.History);
            }

            return data;
        });
    }

    function navigateNotes(history) {
        alert('This feature is not implemented yet');
    }


    function setPatient(model) {
        ko.viewmodel.updateFromModel(vm.model, model);
        vm.originalModel = model;
        //setting the HxOther
        var match = ko.utils.arrayFirst(session.lookups.HxFrom(), function (item) {
            return vm.model.HxFrom() === item.FieldValue();
        });
        if (match !== null) {
            vm.model.HxFromList(vm.model.HxFrom());
            vm.model.HxFromOther('');
        }
        else {
            vm.model.HxFromList('');
            vm.model.HxFromOther(vm.model.HxFrom())
        }
    }

    function setPCP() {
        if (vm.model.PcpInd() === true) {
            vm.model.ReferredDoctor(vm.model.ReferredFrom());
        }
        return true;
    }

    function cancelEdit() {
        return utility.showMessage('Are you sure you want cancel and loose all changes?', 'Edit Patient').then(function (dialogResult) {
            if (dialogResult === 'Yes') {
                setPatient(vm.originalModel);
            }

            return dialogResult;
        });
    }

    function save(currentRecord) {
        if (vm.model.HxFromList() === undefined) {
            vm.model.HxFrom(vm.model.HxFromOther());
        }
        else {
            vm.model.HxFrom(vm.model.HxFromList());
        }
        return utility.httpPost('api/patient', vm.model).then(function (data) {
            if (data.Success === true) {
                vm.originalModel = ko.viewmodel.toModel(vm.model);
                if (currentRecord !== undefined) {
                    currentRecord.PatientName(vm.model.Name());
                    currentRecord.DateOfBirth(vm.model.DateOfBirth());
                }
            }
            return data;
        });
    }

});
