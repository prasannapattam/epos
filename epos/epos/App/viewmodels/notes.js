define(['plugins/router', 'services/profile'], function (router, profile) {

    var model;
    var doctors;

    var vm = {
        model: model,
        doctors: doctors,
        resetColour: resetColour,
        scrollToHeader: scrollToHeader,
        activate: activate,
        canDeactivate: canDeactivate,
        canReuseForRoute: canReuseForRoute,
        attached: attached,
        compositionComplete: compositionComplete,
        signOff: signOff,
        correct: correct,
        save: save,
        defaultSave: defaultSave,
        cancel: cancel,
        savePatient: savePatient,
        doctorChange: doctorChange
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
                    vm.model = ko.viewmodel.fromModel(data.Model.Notes);
                    vm.doctors = ko.viewmodel.fromModel(data.Model.Doctors);
                    addComputedProperties();
                }

                return data;
            });
        }
        else {
            var getdata = { "userName": profile.userName(), "patientid": id, "examid": examid };
            return utility.httpGet('api/notes', getdata).then(function (data) {
                if (data.Success === true) {

                    var model = data.Model.Notes;
                    model.HxFromList = {
                        Name: 'HxFromList',
                        Value: model.HxFrom.Value,
                        LookUpFieldName: model.HxFrom.LookUpFieldName,
                        ColourType: model.HxFrom.ColourType
                    };

                    model.HxFromOther = {
                        Name: 'HxFromOther',
                        Value: model.HxFrom.Value,
                        LookUpFieldName: model.HxFrom.LookUpFieldName,
                        ColourType: model.HxFrom.ColourType
                    }

                    vm.model = ko.viewmodel.fromModel(model);
                    vm.doctors = ko.viewmodel.fromModel(data.Model.Doctors);
                    window.autoComplete = data.Model.AutoComplete;
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
        var scrollPosition = $("#" + tag).offset().top;
        var notesHeight = $("div.notes-header").height();
        $(window).scrollTop(scrollPosition - notesHeight);
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
                var totalDays = duration.asDays();
                var totalWeeks = parseInt(totalDays / 7);
                var totalMonths = parseInt(totalDays / 30);
                var totalYears = parseInt(totalMonths / 12);
                var months = totalMonths - (totalYears * 12);

                var age = '';
                if (totalMonths <= 6)
                    age = totalWeeks + " weeks";
                else if(totalWeeks < 12)
                    age = totalMonths + " month-old";
                else if (totalYears <= 10)
                    age = totalYears + '.' + months + " year-old";
                else
                    age = totalYears + " year-old";

                vm.model.tbAge.Value(age);
            }
        }, vm.model)

        vm.model.HxFromCalculation = ko.computed(function () {           
            if (vm.model.HxFromList.Value() === undefined) {
                vm.model.HxFrom.Value(vm.model.HxFromOther.Value());
            }
            else {
                vm.model.HxFromOther.Value('');
                vm.model.HxFrom.Value(vm.model.HxFromList.Value());
            }

        }, vm.model);

        vm.model.CopyToCalculation = ko.computed(function () {
            var refd = vm.model.Refd.Value();
            var refDoctor = vm.model.RefDoctor.Value();

            var copyTo = refd;

            if (refDoctor !== "" && refd !== refDoctor){
                if (refd === "")
                    copyTo = refDoctor;
                else
                    copyTo += ', ' + refDoctor;
            }

            vm.model.CopyTo.Value(copyTo);
        }, vm.model);

        vm.model.SummaryCalculation = ko.computed(function () {
            var summary = vm.model.Summary.Value();

            //replacing the age
            var oldAge = vm.model.Age.Value();
            var newAge = vm.model.tbAge.Value();
            if (oldAge !== newAge) {
                summary = summary.replace(oldAge, newAge);
                vm.model.Age.Value(newAge)
            }
               

            var GAtext = vm.model.GA.Value();
            var PCAtext = vm.model.PCA.Value();
            var BirthWttext = vm.model.BirthWt.Value();

            if (GAtext != "weeks")
                summary = summary.replace("[GA]", GAtext);
            if (PCAtext != "weeks")
                summary = summary.replace("[PCA]", PCAtext);
            if (BirthWttext != "")
                summary = summary.replace("[BW]", BirthWttext);

            if (vm.model.Summary.Value() !== summary)
                vm.model.Summary.Value(summary);
        }, vm.model);

        vm.model.DiscussedCalculation = ko.computed(function () {
            var discussed = "Discussed findings with " + vm.model.PatientName.Value();
            
            var hxFrom = vm.model.HxFrom.Value();
            if (hxFrom !== "" && hxFrom != "patient")
            {
                var displaySex = '';
                if (hxFrom.indexOf("patient and") >= 0) {
                    hxFrom = hxFrom.replace("patient and", "").trim();
                    if (vm.model.Sex.Value().toLowerCase() == 'female')
                        displaySex += "her";
                    else 
                        displaySex += "his";
                    discussed += " and " + displaySex + " " + hxFrom;
                }
                else {
                    discussed += "'s " + hxFrom;
                }

            }
            vm.model.Discussed.Value(discussed);
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
        delete vm.model.CopyToCalculation;
        delete vm.model.SummaryCalculation;
        delete vm.model.DiscussedCalculation;
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
                    session.isDirty(false);
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
                session.isDirty(false);
            }
            return data;
        });
    }

    function savePatient() {
        if (!session.isNotesPatientDirty())
            return;
        var patientData = {
            PatientID : vm.model.hdnPatientID.Value(),
            PatientNumber: vm.model.PatientNumber.Value(),
            Greeting: vm.model.Greeting.Value(),
            FirstName: vm.model.FirstName.Value(),
            MiddleName: vm.model.MiddleName.Value(),
            LastName: vm.model.LastName.Value(),
            NickName: vm.model.NickName.Value(),
            DateOfBirth: vm.model.DOB.Value(),
            Sex: vm.model.Sex.Value(),
            Occupation: vm.model.Occupation.Value(),
            HxFrom: vm.model.HxFrom.Value(),
            ReferredFrom: vm.model.Refd.Value(),
            ReferredDoctor: vm.model.RefDoctor.Value(),
            Allergies: vm.model.Allergies.Value(),
            Medications: vm.model.Medications.Value(),
            PrematureBirth: vm.model.PrematureBirth.Value()
        }

        utility.httpPost('api/patient', patientData).then(function (data) {
            if (data.Success === true) {
                toastr.info(data.Message);
                session.isNotesPatientDirty(false);
            }
        });
    }

    function canDeactivate() {
        if (session.isDirty()) {
            return utility.showMessage('There are unsaved changes. Do you want to loose those changes?', 'Notes').then(function (dialogResult) {
                if (dialogResult === 'Yes') {
                    return true;
                }
                else {
                    return false;
                }
            });
        }
        else {
            return true;
        }
    }

    function doctorChange(item) {
       
        //var getdata = { "userName": item.Value()};
        //return utility.httpGet('api/examdefault', getdata).then(function (data) {
        //    if (data.Success === true) {
        //        vm.model = ko.viewmodel.fromModel(data.Model.Notes);
        //        vm.doctors = ko.viewmodel.fromModel(data.Model.Doctors);
        //        addComputedProperties();
        //    }

        //    return data;
        //});
    }
});
