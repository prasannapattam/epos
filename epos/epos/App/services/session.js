define(['services/profile'], function (profile) {

    var lookups;
    var isDirty = ko.observable(false);
    var isNotesPatientDirty = ko.observable(false);
    var trackDirty = ko.observable(false);

    var vm = {
        profile: profile,
        lookups: lookups,
        isDirty: isDirty,
        isNotesPatientDirty: isNotesPatientDirty,
        trackDirty: trackDirty,
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
