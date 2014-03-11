define(['services/profile', 'plugins/router'], function (profile, router) {

    var title = ko.observable('login');

    var model = {
         UserName:  ko.observable().extend({ required: { message: "Username is required" }}),
         UserPassword: ko.observable().extend({ required: { message: "Password is required" } })
    };


    var vm = {
        title: title,
        model: model,
        activate: activate,
        validate: validate
    };

    vm.model.errors = ko.validation.group(vm.model);

    return vm;


    function activate() {
        session.profile.isAutenticated(false);
        return true;
    }

    function validate() {
        var self = this;
        if (self.model.isValid()) {
            utility.httpPost('api/login', self.model).then(function (data) {
                if (data.Success === true){
                    session.profile.populate(data.Model);
                    router.navigate('home');
                }
            });
        }
        else {
            toastr.error('Username and Password is required');
            self.model.errors.showAllMessages();
        }
    }


});

