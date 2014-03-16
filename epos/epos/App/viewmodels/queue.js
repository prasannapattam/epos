define(['services/message'], function (message) {

    var removeItem = false;

    var hideElement = function (elem) {
        if (elem.nodeType === 1) {
            //$(elem).hide();
            //$(elem).slideUp(function () { $(elem).remove(); })
            //$(elem).fadeOut(function () { $(elem).remove(); })
            //$(elem).find('td').animate({ padding: '0px' }, { duration: 200 });

            if (removeItem === true) {
                $(elem).find('td')
                .wrapInner('<div style="display: block;" />')
                .parent()
                .find('td > div')
                .slideUp(300, function () {
                    $(this).parent().parent().remove();
                });
                removeItem = false;
            }
            else {
                $(elem).hide();
            }

        }

    }



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


    var vm = {
        hideElement: hideElement,

        title: title,
        doctors: doctors,
        items: items,
        queueItems: queueItems,
        username: username,
        noQueue: message.noQueue,
        activate: activate,
        remove: remove,
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
            }
        });
    }

    function remove(item) {
        return utility.httpPost('api/printqueue', item).then(function (data) {
            if (data.Success === true) {
                removeItem = true;
                vm.items.remove(item);
                toastr.info(data.Message);
            }
        });
    }
});