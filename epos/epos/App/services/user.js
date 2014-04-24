define(['plugins/router', 'services/message'], function (router, message) {

    var title = "Users";
    var addText = "Add User";
    var notesText = "New Default";
    var defaults = ko.observableArray();

    var model = {
        UserID: ko.observable(0),
        FirstName: ko.observable().extend({ required: { message: "required" } }),
        LastName: ko.observable().extend({ required: { message: "required" } }),
        UserName: ko.observable().extend({ required: { message: "required" } }),
        Password: ko.observable().extend({ required: { message: "required" } }),
        PhotoUrl: ko.observable(),
        Photo: ko.observable(),
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
        notesText: notesText,
        model: model,
        originalModel: originalModel,
        blankModel: blankModel,
        noDefault: message.noDefault,
        getSearchResults: getSearchResults,
        getPhotoUrl: getPhotoUrl,
        defaults: defaults,
        navigateNotes: navigateNotes,
        getSelected: getSelected,
        cancelEdit: cancelEdit,
        save: save,
        photoSelect: photoSelect,
        deleteDefault: deleteDefault
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

    function navigateNotes(item) {
        var hash = '#notes/4/' + vm.model.UserID();
        if (item.ExamDefaultID !== undefined) {
            hash += '/' + item.ExamDefaultID
        }
        router.navigate(hash);
    }

    function deleteDefault(item) {
        alert('This feature is being implemented');
    }

    function setUser(model) {
        ko.viewmodel.updateFromModel(vm.model, model);
        vm.originalModel = model;
        vm.model.Photo(getPhotoUrl(model.PhotoUrl));
        //vm.model.PhotoUrl(model.PhotoUrl);
    }

    function getPhotoUrl(photoUrl) {
        if (photoUrl === null) {
            return utility.virtualDirectory + '/Data/NoPhoto.jpg';
        }
        else {
            return utility.virtualDirectory + '/Data/' + photoUrl + '.jpg';
        }
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
        utility.loading(true);
        $("#user_form").submit();  // Submits the form on change event, you consider this code as the start point of your request (to show loader)
        var defer = $.Deferred();
        $("#uploader_iframe").unbind().load(function () {  // This block of code will execute when the response is sent from the server.
            var data = JSON.parse($(this).contents().text());
            if (data.Success === true) {
                if (vm.model.Photo() !== getPhotoUrl(vm.model.PhotoUrl())) {
                    vm.model.PhotoUrl(vm.model.UserName());
                    vm.model.Photo(getPhotoUrl(vm.model.PhotoUrl()));
                    $("#Photo").replaceWith($("#Photo").clone());

                }
                vm.originalModel = ko.viewmodel.toModel(vm.model);
                if (currentRecord !== undefined) {
                    currentRecord.FullName(vm.model.Name());
                    currentRecord.UserName(vm.model.UserName());
                }
            }
            else {
                toastr.error(data.Message);
            }
            utility.loading(false);
            defer.resolve(data);
        });

        return defer;

        return utility.httpPost('api/user', vm.model).then(function (data) {
            return data;
        });
    }

    function photoSelect(elemet, event) {
        var file = event.target.files[0];
        if (!file.type.match('image.*')) {
            return;
        }
        var reader = new FileReader();

        // Closure to capture the file information.
        reader.onload = (function (theFile) {
            return function (e) {
                //self.files.push(new FileModel(escape(theFile.name),e.target.result));
                //alert(e.target.result);
                vm.model.Photo(e.target.result);
            };
        })(file);
        // Read in the image file as a data URL.
        reader.readAsDataURL(file);
    }
});
