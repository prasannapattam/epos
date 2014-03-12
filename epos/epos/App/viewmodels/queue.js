define([], function () {

    var title = ko.observable('Print Queue');
    var doctors;
    var items;

    var vm = {
        title: title,
        doctors: doctors,
        activate: activate
    };

    return vm;

    function activate() {
        navigation.clear();
        navigation.setHomeTab(vm.title, '#queue', true);

        return true;
    };

    function getQueue() {
        return utility.httpGet('api/printqueue').then(function (data) {
            if (data.Success === true) {
                session.lookups = ko.viewmodel.fromModel(data.Model);
            }
        });

    }
});