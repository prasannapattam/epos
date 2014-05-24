define(['services/profile', 'plugins/router'], function (profile, router) {

    var title = ko.observable('Login');

    var model = {
         UserName:  ko.observable().extend({ required: { message: "Username is required" }}),
         UserPassword: ko.observable().extend({ required: { message: "Password is required" } })
    };


    var vm = {
        title: title,
        model: model,
        activate: activate,
        compositionComplete: compositionComplete,
        validate: validate,
        userNameFocus: ko.observable(true),
        passwordFocus: ko.observable(true)
};

    vm.model.errors = ko.validation.group(vm.model);

    return vm;


    function activate() {
        session.profile.isAuthenticated(false);
        navigation.clear();
        navigation.setHomeTab(vm.title, '#login', true);
        vm.model.UserName('');
        vm.model.UserPassword('');
        vm.model.errors.showAllMessages(false);
        session.profile.photoUrl(undefined);
        return true;
    }
    function compositionComplete() {
        vm.userNameFocus(true);
        return true;
    }
    function validate() {
        if (vm.model.isValid()) {
            utility.httpPost('api/login', vm.model).then(function (data) {
                if (data.Success === true){
                    session.profile.populate(data.Model);
                    router.navigate('home');
                }
            });
        }
        else {
            toastr.error('Username and Password is required');
            vm.model.errors.showAllMessages();
            if (!vm.model.UserName.isValid()) {
                vm.userNameFocus(true);
            }
            if (!vm.model.UserPassword.isValid()) {
                vm.passwordFocus(true);
            }
        }
    }
});

