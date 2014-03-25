define(['plugins/router', 'services/patient', 'services/user'], function (router, patient, user) {

    //patient default valules
    var patientSearchTemplate = 'patient-search-template';
    var patientHistoryTemplate = 'patient-history-template';
    var patientDetailTemplate = 'patient-detail-template';

    //Use default valules
    var userSearchTemplate = 'user-search-template';
    var userDefaultTemplate = 'user-default-template';
    var userDetailTemplate = 'user-detail-template';


    //common variables for both the modules
    var module;
    var moduleName = ko.observable();
    var searchTemplate = ko.observable();
    var summaryTemplate = ko.observable();
    var detailTemplate = ko.observable();


    //search results
    var title = ko.observable();
    var addText = ko.observable();
    var criteria = ko.observable();
    var lastCriteria = '';
    var viewActivate = false;
    var resultHeader = ko.computed(function () {
        if (criteria() == '')
            return 'Recent ' + title();
        else
            return 'Search Results';
    }, this);

    var results = ko.observableArray();

    var windowHeight = (window.innerHeight - 170) + 'px';

    var getSearchResults = ko.computed(function () {
        if (criteria() !== undefined) {
            vm.module.getSearchResults(criteria()).then(function (data) {
                if (data.Success === true) {
                    results(ko.viewmodel.fromModel(data.Model)());
                    vm.lastCriteria = criteria();
                    if (vm.viewActivate === true) {
                        vm.viewActivate = false;
                        getSelected();
                    }
                    else {
                        var el = document.getElementById('results');
                        el.scrollTop = 0;
                    }
                }
            });
        }
        return criteria();
    });



    //module details
    var currentRecord;
    var currentID = ko.observable();
    var model;
    var blankModel;
    var newModel;

    var moduleTitle = ko.observable();
    var moduleHideInd = ko.observable(true);
    var moduleAddInd = ko.observable(false);
    var detailEditInd = ko.observable(false);
    var detailHideInd = ko.computed(function () {
        return moduleHideInd() && !moduleAddInd();
    });
    var moduleEditInd = ko.computed(function () {
        if (moduleAddInd() === false || moduleHideInd() === false)
            return detailEditInd()
        else
            return true;
    });


    var vm = {
        //variables to hold module details
        module: module,
        moduleName: moduleName,
        moduleTitle: moduleTitle,
        moduleHideInd: moduleHideInd,
        moduleAddInd: moduleAddInd,

        //templates
        searchTemplate: searchTemplate,
        summaryTemplate: summaryTemplate,
        detailTemplate: detailTemplate,

        //common variables
        model: model,

        title: title,
        addText: addText,

        //search
        criteria: criteria,
        lastCriteria: lastCriteria,
        resultHeader: resultHeader,
        results: results,
        currentRecord: currentRecord,
        currentID: currentID,

        //others
        detailEditInd: detailEditInd,
        detailHideInd: detailHideInd,
        moduleEditInd: moduleEditInd,
        viewActivate: viewActivate,
        windowHeight: windowHeight,

        //events
        show: show,
        add: add,
        edit: edit,
        save: save,
        cancelEdit: cancelEdit,

        //durandal events
        activate: activate,
        canReuseForRoute: canReuseForRoute
};

    return vm;

    function canReuseForRoute() {
        return true;
    }

    function activate(searchModule, searchID, searchCriteria) {
        vm.viewActivate = true;

        if (searchModule === undefined || searchModule === null || searchModule === '')
            searchModule = 'patient';

        if (searchModule !== 'user')
            searchModule = 'patient';

        //setting the default values based on the module
        if (searchModule === 'patient') {
            vm.module = patient;
            vm.model = patient.model;
            vm.title(patient.title);
            vm.addText(patient.addText);
            vm.searchTemplate(patientSearchTemplate);
            vm.summaryTemplate(patientHistoryTemplate);
            vm.detailTemplate(patientDetailTemplate);
        }
        else {
            vm.module = user;
            vm.model = user.model;
            vm.title(user.title);
            vm.addText(user.addText);
            vm.searchTemplate(userSearchTemplate);
            vm.summaryTemplate(userDefaultTemplate);
            vm.detailTemplate(userDetailTemplate);
        }


        if (vm.moduleName() !== undefined && vm.moduleName() !== searchModule) {
            vm.criteria(undefined);
            vm.moduleAddInd(false);
            navigation.clear();
        }
        vm.moduleName(searchModule);
        vm.model.errors = ko.validation.group(vm.model);
        blankModel = vm.module.blankModel;

        vm.currentID(0);
        var homeTabActive = true;
        if ((searchID === undefined || searchID === null) && (searchCriteria === undefined || searchCriteria === null)) {
            setCriteria('');
            vm.moduleHideInd(true);
            if (vm.moduleAddInd() === true) {
                ko.viewmodel.updateFromModel(vm.model, newModel);
                vm.model.errors.showAllMessages(false)
            }
        }
        else {
            if (searchID !== null || searchID !== undefined) {
                vm.currentID(parseInt(searchID));
                homeTabActive = false;
            }

            if (searchCriteria !== null && searchCriteria !== undefined) {
                searchCriteria = decodeURI(searchCriteria);
                searchCriteria = searchCriteria.replace(/\//g, '');
                setCriteria(searchCriteria)
            }
            else {
                setCriteria('');
            }
        }

        //setting the Home tab
        navigation.setHomeTab(vm.title, '#home/' + searchModule, homeTabActive);
        $(document).foundation();
    }

    function setCriteria(searchCriteria) {
        if (searchCriteria === vm.criteria()) {
            getSelected();
        }
        else {
            vm.criteria(searchCriteria)
        }
    }

    function show(record) {
        //getting the data from server
        vm.currentRecord = record;
        vm.currentID(record.ID());
        getSelected();
    }

    function setCurrentRecord() {
        var searchResults = vm.results();
        var currentID = vm.currentID()
        for (index = 0; index < searchResults.length ; index++) {
            if (searchResults[index].ID() === currentID) {
                vm.currentRecord = searchResults[index];
                break;
            }
        }
    }

    function getSelected() {
        var currentID = vm.currentID();
        if (currentID === 0)
            return;
        vm.detailEditInd(false);
        if (vm.moduleAddInd() === true && vm.model.ID() === 0)
            newModel = ko.viewmodel.toModel(vm.model);
        vm.module.getSelected(currentID).then(function (data) {
            if (data.Success === true) {
                vm.moduleTitle(vm.model.Name());
                vm.moduleHideInd(false);
                //if (vm.currentRecord === undefined)
                //    setCurrentRecord();
                setCurrentRecord();

                //setting the tabs & url
                var hash = '#home/' + vm.moduleName() + '/' + vm.model.ID() + '/' + encodeURI(vm.lastCriteria);
                navigation.addTab(vm.model.Name(), hash);
                router.navigate(hash, false);

                var el = document.getElementById(vm.model.ID());
                if (el !== undefined && el !== null)
                    el.scrollIntoViewIfNeeded(false);
            }
        });
    }

    function add() {
        newModel = blankModel;
        ko.viewmodel.updateFromModel(vm.model, newModel);
        vm.moduleAddInd(true);
        vm.model.errors.showAllMessages(false)
        vm.currentRecord = undefined;
    }

    function edit() {
        vm.detailEditInd(true);
    }

    function cancelEdit() {

        vm.module.cancelEdit().then(function (dialogResult) {
            if (dialogResult === 'Yes') {
                vm.detailEditInd(false);
                vm.moduleAddInd(false);
                newModel = blankModel;
            }
        });
    }

    function save() {
        if (vm.model.isValid()) {

            vm.module.save(vm.currentRecord).then(function (data) {
                if (data.Success === true) {
                    toastr.info(data.Message);
                    vm.detailEditInd(false);
                    vm.moduleAddInd(false);

                    var moduleTitle = vm.model.Name();
                    vm.moduleTitle(moduleTitle);
                    navigation.setCurrentTitle(moduleTitle);
                }
            });

        }
        else {
            toastr.error('Please fix the validation errors');
            vm.model.errors.showAllMessages();
        }
    }
});