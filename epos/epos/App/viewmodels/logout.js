define(['plugins/router'], function (router) {
    var vm = {
        activate: activate
    };

    return vm;

    function activate() {
        var self = this;
        session.profile.logout();
        router.navigate('login');
        return true;
    };

});