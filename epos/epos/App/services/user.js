define(['services/message'], function (message) {

    var title = "Users";
    var addText = "Add User";
    var defaults = ko.observableArray();

    var model = {
        UserID: ko.observable(0),
        FirstName: ko.observable().extend({ required: { message: "required" } }),
        LastName: ko.observable().extend({ required: { message: "required" } }),
        UserName: ko.observable().extend({ required: { message: "required" } }),
        Password: ko.observable().extend({ required: { message: "required" } }),
        PhotoUrl: ko.observable(),
        ID: ko.observable()
    };

    model.Name = ko.computed(function () {
        return this.FirstName() + ' ' + this.LastName();
    }, model);

    var originalModel;
    var blankModel = ko.viewmodel.toModel(model);

    var vm = {
        title: title,
        addText: addText,
        model: model,
        originalModel: originalModel,
        blankModel: blankModel,
        noDefault: message.noDefault,
        getSearchResults: getSearchResults,
        defaults: defaults,
        navigateNotes: navigateNotes,
        getSelected: getSelected,
        cancelEdit: cancelEdit,
        save: save
    };

    return vm;


    function getSearchResults(criteria) {
        return utility.httpPost('api/usersearch', criteria);
    }

    function getSelected(userID) {
        var getdata = { "id": userID };
        return utility.httpGet('api/user', getdata).then(function (data) {
            if (data.Success === true) {
                setUser(data.Model);
                vm.defaults(data.Model.Defaults);
            }

            return data;
        });
    }

    function navigateNotes(history) {
        alert('This feature is not implemented yet');
    }


    function setUser(model) {
        ko.viewmodel.updateFromModel(vm.model, model);
        vm.originalModel = model;
    }

    function cancelEdit() {
        return utility.showMessage('Are you sure you want cancel and loose all changes?', 'Edit User').then(function (dialogResult) {
            if (dialogResult === 'Yes') {
                setUser(vm.originalModel);
            }

            return dialogResult;
        });
    }

    function save(currentRecord) {
        return utility.httpPost('api/user', vm.model).then(function (data) {
            if (data.Success === true) {
                vm.originalModel = ko.viewmodel.toModel(vm.model);
                if (currentRecord !== undefined) {
                    currentRecord.FullName(vm.model.Name());
                    currentRecord.UserName(vm.model.UserName());
                }
            }
            return data;
        });
    }

});
