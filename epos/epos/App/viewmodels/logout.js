define(['plugins/router'], function (router) {
    var vm = {
        canActivate: canActivate
    };

    return vm;

    function canActivate() {
        var self = this;
        session.profile.logout();
        router.navigate('login');
        return true;
    };

});