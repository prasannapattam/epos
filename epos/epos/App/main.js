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

    /*
    ------Keeping this code for future reference-----------------
    ko.bindingHandlers.color = {
    init:function (element, valueAccessor, allBindingsAccessor, data, context) {
        var value = valueAccessor();
        var newValueAccessor = function () {
            return {
                focus: function () {
                    if (value() === 1) {
                        value(0);
                    }
                }
            }
        };


        //call the real event binding's init function
        ko.bindingHandlers.event.init(element, newValueAccessor, allBindingsAccessor, data, context);
    },
    update: function (element, valueAccessor, allBindingsAccessor, data) {
            var value = valueAccessor();
            ko.bindingHandlers.css.update(element, function () {
                return { focusctrl: value() === 1, correctctrl: value() === 2 }
            });
        }
    };
    */

    ko.bindingHandlers.notesvalue = {
        init: function (element, valueAccessor, allBindingsAccessor, data) {
            var field = valueAccessor();

            AddNotesComputedFields(field);

            ko.applyBindingsToNode(element, {
                value: field.Value,
                css: { focusctrl: field.focusctrl, correctctrl: field.correctctrl },
                event: { 
                    focus: function() { 
                        if(field.ColourType() === 1){
                            field.ColourType(0);
                        }
                    } 
                }
            });
        }
    };

    ko.bindingHandlers.notesselect = {
        init: function (element, valueAccessor, allBindingsAccessor, data) {
            var field = valueAccessor();

            AddNotesComputedFields(field);

            var lookupFieldName = field.LookUpFieldName();
            if (lookupFieldName !== null && lookupFieldName !== undefined) {

                ko.applyBindingsToNode(element, {
                    options: session.lookups[field.LookUpFieldName()],
                    optionsValue: 'FieldDescription',
                    optionsText: 'FieldValue',
                    optionsCaption: '',
                    value: field.Value,
                    css: { focusctrl: field.focusctrl, correctctrl: field.correctctrl },
                    event: {
                        focus: function () {
                            if (field.ColourType() === 1) {
                                field.ColourType(0);
                            }
                        }
                    }
                });
            }
        }
    };

    ko.bindingHandlers.notesdatepicker = {
        init: function (element, valueAccessor, allBindingsAccessor, data) {
            var field = valueAccessor();
            if (field.Value === undefined)
                return;

            AddNotesComputedFields(field);

            ko.applyBindingsToNode(element, {
                datepicker: field.Value,
                css: { focusctrl: field.focusctrl, correctctrl: field.correctctrl },
                event: {
                    focus: function () {
                        if (field.ColourType() === 1) {
                            field.ColourType(0);
                        }
                    }
                }
            });
        }
    };

    var AddNotesComputedFields = function (field) {
        field.focusctrl = ko.computed(function () { return field.ColourType() === constants.enum.colourType.New });
        field.correctctrl = ko.computed(function () { return field.ColourType() === constants.enum.colourType.Correct });
        var fieldInit = ko.toJSON(field);
        field.isDirty = ko.computed(function () {
            var fieldJSON = ko.toJSON(field);
            if (session.trackDirty() && fieldInit !== fieldJSON){
                session.isDirty(true);
                if (field.FieldType !== undefined && field.FieldType() === constants.enum.fieldType.Patient) {
                    session.isNotesPatientDirty(true);
                }
            }
            else
                fieldInit = fieldJSON;
        });
    }

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

