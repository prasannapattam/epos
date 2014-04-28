define(['plugins/router', 'services/profile'], function (router, profile) {

    var model;

    var vm = {
        model: model,
        resetColour: resetColour,
        scrollToHeader: scrollToHeader,
        activate: activate,
        canReuseForRoute: canReuseForRoute,
        attached: attached,
        compositionComplete: compositionComplete,
        signOff: signOff,
        correct: correct,
        save: save,
        defaultSave: defaultSave,
        cancel: cancel,
        savePatient: savePatient
};

    return vm;

    function activate(notestype, id, examid) {
        session.isDirty(false);
        session.isNotesPatientDirty(false);
        session.trackDirty(false);

        if (parseInt(notestype) === constants.enum.notesType.Default) {
            var getdata = { "doctorUserID": id, "examDefaultID": examid };
            return utility.httpGet('api/examdefault', getdata).then(function (data) {
                if (data.Success === true) {
                    vm.model = ko.viewmodel.fromModel(data.Model);
                    addComputedProperties();
                }

                return data;
            });
        }
        else {
            var getdata = { "patientid": id, "examid": examid };
            return utility.httpGet('api/notes', getdata).then(function (data) {
                if (data.Success === true) {

                    data.Model.HxFromList = {
                        Name: 'HxFromList',
                        Value: data.Model.HxFrom.Value,
                        LookUpFieldName: data.Model.HxFrom.LookUpFieldName,
                        ColourType: data.Model.HxFrom.ColourType
                    };

                    data.Model.HxFromOther = {
                        Name: 'HxFromOther',
                        Value: data.Model.HxFrom.Value,
                        LookUpFieldName: data.Model.HxFrom.LookUpFieldName,
                        ColourType: data.Model.HxFrom.ColourType
                    }

                    vm.model = ko.viewmodel.fromModel(data.Model);
                    addComputedProperties();
                    setOverrides();
                }

                return data;
            });
        }

    };

    function canReuseForRoute() {
        return false;
    }


    function attached() {
//        alert('attached');
    }
    function compositionComplete() {
        session.trackDirty(true);
        var notesHeaderDefaultOffset = $("div.notes-header").offset().top;
        var infoOffset = $("#info-header").offset().top;
        var cchistoryOffset = $("#cc-history-header").offset().top;
        var acuityOffset = $("#acuity-header").offset().top;
        var ocularmotOffset = $("#ocular-mot-header").offset().top;
        var antsegOffset = $("#ant-seg-header").offset().top;
        var summaryOffset = $("#summary-header").offset().top;
        var currentOffset = infoOffset;
        $(window).scrollTop(0);

        $(window).scroll(function () {
            var winScroll = $(this).scrollTop(); // current scroll of window
            //if (winScroll > notesHeaderScrollTop)
            var notesHeaderoffset = notesHeaderDefaultOffset;
            if (winScroll > notesHeaderDefaultOffset) {
                notesHeaderoffset = winScroll
            }
            $("div.notes-header").offset({ top: notesHeaderoffset });

            //calculating the menu offset
            var newOffset = infoOffset;
            var notesHeight = $("div.notes-header").height();
            var winOffset = winScroll + notesHeight + 5;
            //checking for scroll end
            if ($(document).height() == $(window).scrollTop() + window.innerHeight) {
                winOffset = summaryOffset;
            }

            if (winOffset >= 0 && winOffset < cchistoryOffset && currentOffset != infoOffset) {
                $(".notes-menu").removeClass('notes-menu-selected');
                $("#default-info-menu").addClass('notes-menu-selected');
                $("#patient-info-menu").addClass('notes-menu-selected');
                currentOffset = infoOffset;
            }
            else if (winOffset >= cchistoryOffset && winOffset < acuityOffset && currentOffset != cchistoryOffset) {
                $(".notes-menu").removeClass('notes-menu-selected');
                $("#cc-history-menu").addClass('notes-menu-selected');
                currentOffset = cchistoryOffset;
            }
            else if (winOffset >= acuityOffset && winOffset < ocularmotOffset && currentOffset != acuityOffset) {
                $(".notes-menu").removeClass('notes-menu-selected');
                $("#acuity-menu").addClass('notes-menu-selected');
                currentOffset = acuityOffset;
            }
            else if (winOffset >= ocularmotOffset && winOffset < antsegOffset && currentOffset != ocularmotOffset) {
                $(".notes-menu").removeClass('notes-menu-selected');
                $("#ocular-mot-menu").addClass('notes-menu-selected');
                currentOffset = ocularmotOffset;
            }
            else if (winOffset >= antsegOffset && winOffset < summaryOffset && currentOffset != antsegOffset) {
                $(".notes-menu").removeClass('notes-menu-selected');
                $("#ant-seg-menu").addClass('notes-menu-selected');
                currentOffset = antsegOffset;
            }
            else if ((winOffset >= summaryOffset && currentOffset != summaryOffset)) {
                $(".notes-menu").removeClass('notes-menu-selected');
                $("#summary-menu").addClass('notes-menu-selected');
                currentOffset = summaryOffset;
            }
        });

        return true;
    }

    function scrollToHeader(tag) {
        //alert(tag);
        //document.getElementById(tag).scrollIntoView(true);
        var scrollPosition = $("#" + tag).offset().top;
        var notesHeight = $("div.notes-header").height();
        $(window).scrollTop(scrollPosition - notesHeight);
        alert(scrollPosition - notesHeight)
    }
    
    function addComputedProperties() {

        vm.model.HeaderText = ko.computed(function () {
            var notestype = ko.unwrap(vm.model.NotesType);

            var headerText;
            if (notestype === constants.enum.notesType.Default) {
                headerText = this.DoctorName.Value() + ' - Notes Default';
            }
            else {
                headerText = this.PatientName.Value();
                var ExamID = ko.unwrap(vm.model.hdnExamID);
                var ExamDate = ko.unwrap(vm.model.ExamDate);
                var ExamSaveDate = ko.unwrap(vm.model.ExamSaveDate);
                var ExamCorrectDate = ko.unwrap(vm.model.ExamCorrectDate);
                if (ExamSaveDate !== null) {
                    headerText += ' - Notes saved on ' + ExamSaveDate.Value()
                }
                else if (ExamCorrectDate !== null) {
                    headerText += ' - Notes taken on ' + ExamDate.Value() + ' (Corrected on ' + ExamCorrectDate.Value() + ')';
                }
                else if (ExamID !== null) {
                    headerText += ' -  Notes taken on ' + ExamDate.Value();
                }
                else {
                    headerText += ' - New notes'
                }
            }
            return headerText;
        }, vm.model);

        vm.model.AgeCalculation = ko.computed(function () {
            var examDateMoment = moment(vm.model.ExamDate.Value());
            var dobMoment = moment(vm.model.DOB.Value());

            if (examDateMoment.isValid() === true && dobMoment.isValid() === true) {
                var age = examDateMoment.diff(dobMoment);
                var duration = moment.duration(age);
                if (duration.asMonths() <= 6)
                    age = parseInt(duration.asWeeks()) + ' weeks';
                else if(duration.asMonths() < 12)
                    age = duration.months() + ' months';
                else if (duration.asYears() <= 10)
                    age = duration.years() + '.' + duration.months() + ' years';
                else
                    age = duration.years() + ' years';
                    
                vm.model.tbAge.Value(age);
            }

            return vm.model.ExamDate.Value() + '  ' + vm.model.DOB.Value(); //dummy return
        }, vm.model)

        vm.model.HxFromCalculation = ko.computed(function () {           
            if (vm.model.HxFromList.Value() === undefined) {
                vm.model.HxFrom.Value(vm.model.HxFromOther.Value());
            }
            else {
                vm.model.HxFromOther.Value('');
                vm.model.HxFrom.Value(vm.model.HxFromList.Value());
            }

            return vm.model.HxFromList.Value() + ' ' + vm.model.HxFromOther.Value();
        }, vm.model);

    }

    function setOverrides() {
        if (profile.userName() !== undefined) {
            vm.model.User.Value = profile.userName;
        }
    }

    function deleteComputedProperties() {
        delete vm.model.HeaderText;
        delete vm.model.AgeCalculation;
        delete vm.model.HxFromList;
        delete vm.model.HxFromOther;
        delete vm.model.HxFromCalculation;

    }

    function resetColour(item) {
        if (item.ColourType() === 1) {
            item.ColourType(2);
        }
        else {
            item.ColourType(1);

        }
    }

    function cancel() {
        if (session.isDirty()) {
            utility.showMessage('Are you sure you want cancel and loose all changes?', 'Notes').then(function (dialogResult) {
                if (dialogResult === 'Yes') {
                    router.navigateBack();
                }
            });
        }
        else {
            router.navigateBack();
        }
    }

    function signOff() {
        saveNotes(constants.enum.notesSaveType.SignOff);
    }

    function correct() {
        saveNotes(constants.enum.notesSaveType.Correct);
    }

    function save() {
        saveNotes(constants.enum.notesSaveType.Save);
    }

    function defaultSave() {
        alert('This feature is being implemented')
    }

    function saveNotes(saveType) {
        if (!session.isDirty())
            return;
        deleteComputedProperties();
        return utility.httpPost('api/notes?type=' + saveType.toString(), vm.model).then(function (data) {
            if (data.Success === true) {
                toastr.info(data.Message);
                router.navigateBack();
            }
            return data;
        });
    }

    function savePatient() {
        if (!session.isNotesPatientDirty())
            return;
        var patientData = {
            PatientID : vm.model.hdnPatientID.Value(),
            PatientNumber : vm.model.hdnPatientID.Value(),
            Greeting : vm.model.hdnPatientID.Value(),
            FirstName : vm.model.hdnPatientID.Value(),
            MiddleName : vm.model.hdnPatientID.Value(),
            LastName : vm.model.hdnPatientID.Value(),
            NickName : vm.model.hdnPatientID.Value(),
            DateOfBirth : vm.model.hdnPatientID.Value(),
            Sex : vm.model.hdnPatientID.Value(),
            Occupation : vm.model.hdnPatientID.Value(),
            HxFrom : vm.model.hdnPatientID.Value(),
            ReferredFrom : vm.model.hdnPatientID.Value(),
            ReferredDoctor : vm.model.hdnPatientID.Value(),
            Allergies : vm.model.hdnPatientID.Value(),
            Medications : vm.model.hdnPatientID.Value(),
            PrematureBirth : vm.model.hdnPatientID.Value()       
        }
    }
});
