define([], function () {

    var title = ko.observable('Print Queue');
    var username = ko.observable();
    var doctors;
    var items;

    var queueItems = ko.computed(function () {
        var filter = username();
        if (filter === undefined) {
            return items;
        }
        else {
            var filterItems = ko.utils.arrayFilter(items(), function (item) {
                return ko.utils.stringStartsWith(item.Value(), filter);
            });

            return filterItems;
        }
    });
    //queueItems: queueItems,

    var vm = {
        title: title,
        doctors: doctors,
        items: items,
        queueItems: queueItems,
        username: username,
        activate: activate
    };

    return vm;

    function activate() {
        navigation.clear();
        navigation.setHomeTab(vm.title, '#queue', true);

        return getQueue();
    };

    function getQueue() {
        return utility.httpGet('api/printqueue').then(function (data) {
            if (data.Success === true) {
                vm.doctors = ko.viewmodel.fromModel(data.Model.Doctors);
                vm.items = ko.viewmodel.fromModel(data.Model.Items);
            }
        });

    }
});