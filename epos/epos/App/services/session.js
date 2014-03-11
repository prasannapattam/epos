define(['services/profile'], function (profile) {

    var lookups;

    var vm = {
        profile: profile,
        lookups: lookups,
        populateLookups: populateLookups
    };

    return vm;

    function populateLookups() {
        return utility.httpGet('api/lookup').then(function (data) {
            if (data.Success === true) {
                session.lookups = ko.viewmodel.fromModel(data.Model);
            }
        });
    }
});
