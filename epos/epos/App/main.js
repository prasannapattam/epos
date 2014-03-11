//require configuration for durandal
require.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions'
}
});

//defining global libraries
define('jquery', function () { return jQuery; });
define('knockout', ko);


//starting the app
define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'durandal/composition'], function (system, app, viewLocator, composition) {

    app.title = 'Pediatric Ophthalmology';

    //specify which plugins to install and their configuration
    app.configurePlugins({
        router: true,
        http: true,
        dialog: true
    });

    ko.validation.configure({
        registerExtenders: true,
        messagesOnModified: true,
        insertMessages: true,
        parseInputAttributes: true,
        messageTemplate: null
    });


    composition.addBindingHandler('datepicker', {
        init: function (element, valueAccessor) {
            $(element).fdatepicker({ format: 'mm/dd/yyyy' });
            var value = valueAccessor();
            var mmt = moment(value());
            if (mmt.isValid()) {
                value(mmt.format('L'));
            }

            ko.utils.registerEventHandler(element, "change", function () {
                value(element.value);
            });
        },

        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor();
            var mmt = moment(value());
            if (mmt.isValid() === false){
                value("");
                element.value = "";
            }
            else {
                element.value = mmt.format('L');
            }
        }
    });

    //date format extenders
    ko.extenders.dateformat = function (target, format) {
        var result = ko.computed({
            read: target,
            write: function (newValue) {
                target(moment(newValue).format(format));
            }
        }).extend({ notify: 'always' });

        //initializing with the current value to make sure it is formatted correctly
        result(target());

        //return the new computed observable
        return result;
    };

    app.start().then(function () {
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.options.backgroundpositionClass = 'toast-bottom-right';

        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application.
        app.setRoot('viewmodels/shell', 'entrance');
    });


});

