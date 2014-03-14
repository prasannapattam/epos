define(['services/message'], function (message) {

    var title = ko.observable('Print Queue');
    var username = ko.observable();
    var doctors = ko.observableArray();
    var items = ko.observableArray();

    var queueItems = ko.computed(function () {
        var filter = username();
        if (filter === undefined) {
            return items();
        }
        else {
            var filterItems = ko.utils.arrayFilter(items(), function (item) {
                return item.UserName === filter;
            });

            return filterItems;
        }
    });

    // Animation callbacks for the planets list
    var showPlanetElement = function (elem) { if (elem.nodeType === 1) $(elem).hide().slideDown() }
    var hidePlanetElement = function (elem) { if (elem.nodeType === 1) $(elem).slideUp(function () { $(elem).remove(); }) }

    var vm = {
        title: title,
        doctors: doctors,
        items: items,
        queueItems: queueItems,
        username: username,
        noQueue: message.noQueue,
        activate: activate,
        remove: remove,
        showPlanetElement: showPlanetElement,
        hidePlanetElement: hidePlanetElement
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
                vm.doctors(data.Model.Doctors);
                vm.items(data.Model.Items);
                //vm.doctors(ko.viewmodel.fromModel(data.Model.Doctors)());
                //vm.items(ko.viewmodel.fromModel(data.Model.Items)());
                //ko.viewmodel.updateFromModel(vm.doctors, data.Model.Doctors);
                //ko.viewmodel.updateFromModel(vm.items, data.Model.Items);
            }
        });
    }

    function remove(item) {
        //alert(item.PrintQueueID);
    }
});