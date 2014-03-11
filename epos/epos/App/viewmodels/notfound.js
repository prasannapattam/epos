define([], function () {
    var title = 'Page Not Found';
    var vm = {
        activate: activate,
        title: title
    };

    return vm;

    function activate() {
        return true;
    }
});