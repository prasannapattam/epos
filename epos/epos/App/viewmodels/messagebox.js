define(['plugins/dialog'], function (dialog) {
    var root = this;
    var title = ko.observable();
    var message = ko.observable();
    var vm = {
        title: title,
        message: message,
        set: set,
        click: click
    };

    return vm;

    function set(message, title) {
        vm.message(message);
        vm.title(title);
    }

    function click(dialogResult) {
        dialog.close(this, dialogResult);
    }

});
