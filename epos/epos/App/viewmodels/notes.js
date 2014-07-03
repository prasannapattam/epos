define(['plugins/router', 'services/profile'], function (router, profile) {

    var model
    var doctors;
    var notespatientid;
    var notesexamid;
    var pageloaded = false;

    var vm = {
        model: model,
        doctors: doctors,
        resetColour: resetColour,
        scrollToHeader: scrollToHeader,
        activate: activate,
        canDeactivate: canDeactivate,
        canReuseForRoute: canReuseForRoute,
        compositionComplete: compositionComplete,
        signOff: signOff,
        correct: correct,
        save: save,
        defaultSave: defaultSave,
        cancel: cancel,
        savePatient: savePatient,
        doctorChange: doctorChange,
        rfxHistory: rfxHistory,
        sumHistory: sumHistory,
        cchHistory: cchHistory,
        distHistory: distHistory,
        binoHistory: binoHistory,
        ocmHistory: ocmHistory
    };

    return vm;

    function activate(notestype, id, examid) {
        session.isDirty(false);
        session.isNotesPatientDirty(false);
        session.trackDirty(false);

        notespatientid = id;
        notesexamid = examid;
        pageloaded = false;
        vm.model = undefined;

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
            return getNotes(profile.userName());
        }

    };

    function getNotes(doctorUserName) {
        var getdata = { "userName": doctorUserName, "patientid": notespatientid, "examid": notesexamid };
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

                //removing the weeks (fix for old data)
                if (model.GA.Value === "weeks") {
                    model.GA.Value = "";
                    model.GA.ColourType = 0;
                }
                if (model.PCA.Value === "weeks") {
                    model.PCA.Value = "";
                    model.PCA.ColourType = 0;
                }

                if (vm.model !== undefined)
                    ko.viewmodel.updateFromModel(vm.model, model);
                else {
                    options = addViewModelExtenders();
                    vm.model = ko.viewmodel.fromModel(model, options);
                }
                vm.doctors = ko.viewmodel.fromModel(data.Model.Doctors);
                window.autoComplete = data.Model.AutoComplete;
                addComputedProperties();
                setOverrides();
                if (model.NotesType === constants.enum.notesType.Saved) {
                    session.isDirty(true);
                }
            }

            return data;
        });

    }

    function canReuseForRoute() {
        return false;
    }


    function compositionComplete() {
        session.trackDirty(true);
        pageloaded = true;
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

    function loadHistoryWindow(id, width, height, title, prefix) {
        var historyWindow = $("#" + id);
        if (!historyWindow.data("kendoWindow")) {
            historyWindow.kendoWindow({
                width: width,
                height: height,
                title: title,
                activate: function () {
                    var el = document.getElementById(prefix + notesexamid);
                    var winelement = document.getElementById(id);
                    if (el !== undefined && el !== null)
                        winelement.scrollTop = el.offsetTop;
                    else
                        winelement.scrollTop = 0;
                }
            });
        }

        historyWindow.data("kendoWindow").open();
    }

    function destroyWindows() {
        destroyWindow("rfxHistoryWindow");
        destroyWindow("cchHistoryWindow");
        destroyWindow("sumHistoryWindow");
        destroyWindow("distHistoryWindow");
        destroyWindow("binoHistoryWindow");
        destroyWindow("ocmHistoryWindow");
    }

    function destroyWindow(id) {
        var historyWindow = $("#" + id);
        if (historyWindow.data("kendoWindow"))
            historyWindow.data("kendoWindow").destroy();
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
                if (oldAge !== "" && summary.indexOf(oldAge) !== -1)
                    summary = summary.replace(oldAge, newAge);
                else
                    summary = summary.replace(/[0-9. ]+year[- ]+old/i, newAge);
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
            var discussed = "Discussed findings with " + vm.model.FirstName.Value();
            
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

        vm.model.PopulateRxGivenCalculation = ko.computed(function () {

            var populate = vm.model.PopulateRxGiven.Value();

            if (populate && pageloaded) {
                var manod = vm.model.ManRfxOD1.Value.peek();
                var cycod = vm.model.CycRfxOD.Value.peek();

                if (cycod != '')
                    vm.model.RxOD1.Value(cycod);
                else
                    vm.model.RxOD1.Value(manod);

                var manos = vm.model.ManRfxOS1.Value.peek();
                var cycos = vm.model.CycRfxOS.Value.peek();

                if (cycos != '')
                    vm.model.RXOS1.Value(cycos);
                else
                    vm.model.RXOS1.Value(manos);

                var manod2 = vm.model.ManRfxOD2.Value.peek();
                vm.model.RxOD2.Value(manod2);

                var manos2 = vm.model.ManRfxOS2.Value.peek();
                vm.model.RXOS2.Value(manos2);
            }
        }, vm.model)

        vm.model.PopulateCtlRxCalculation = ko.computed(function () {

            var populate = vm.model.PopulateCtlRx.Value();

            if (populate && pageloaded) {
                var manod = vm.model.ManRfxOD1.Value.peek();
                var cycod = vm.model.CycRfxOD.Value.peek();

                if (cycod != '')
                    vm.model.CTLRxOD1.Value(cycod);
                else
                    vm.model.CTLRxOD1.Value(manod);

                var manos = vm.model.ManRfxOS1.Value.peek();
                var cycos = vm.model.CycRfxOS.Value.peek();

                if (cycos != '')
                    vm.model.CTLRxOS1.Value(cycos);
                else
                    vm.model.CTLRxOS1.Value(manos);

                var manod2 = vm.model.ManRfxOD2.Value.peek();
                vm.model.CTLRxOD2.Value(manod2);

                var manos2 = vm.model.ManRfxOS2.Value.peek();
                vm.model.CTLRxOS2.Value(manos2);
            }
        }, vm.model)

    }

    function addViewModelExtenders() {
        return { 
            extend:{
                "{root}.History.Rfx[i].FieldValue": function (rfx) {
                    rfx.ManRfx = ko.observable(GetOdOsString(rfx.ManRfxOD1(), rfx.ManRfxOD2(), rfx.ManRfxOS1(), rfx.ManRfxOS2()));
                    rfx.ManVA = ko.observable(GetOdOsString(rfx.ManVAOD1(), rfx.ManVAOD2(), rfx.ManVSOS1(), rfx.ManVSOS2()));
                    rfx.CycRfx = ko.observable(GetOdOsString(rfx.CycRfxOD(), "", rfx.CycRfxOS(), ""));
                    rfx.CycVA = ko.observable(GetOdOsString(rfx.CycVAOD3(), rfx.CycVAOD4(), rfx.CycVSOS1(), rfx.CycVSOS2()));
                    rfx.HasHistory = ko.observable(rfx.ManRfx() !== "" || rfx.ManVA() !== "" || rfx.CycRfx() !== "" || rfx.CycVA() !== "");
                },
                "{root}.History.Cch[i].FieldValue": function (cch) {
                    cch.HasCcHistory = ko.observable(cch.Compliant() !== "" || cch.SubjectiveHistory() !== "");
                    cch.HasSumHistory = ko.observable(cch.Summary() !== "");
                },
                "{root}.History.Dist[i].FieldValue": function (dist) {
                    dist.VAsc = ko.observable(GetOdOsString(dist.VAscOD1(), dist.VAscOD2(), dist.DistOS1(), dist.DistOS2()));
                    dist.VAcc = ko.observable(GetOdOsString(dist.VAccOD1(), dist.VAccOD2(), dist.DistOS3(), dist.DistOS4()));
                    dist.VAnear = ko.observable(GetOdOsString(dist.VAOD1(), dist.VAOD2(), dist.NearOS1(), dist.NearOS2()));
                    dist.HasHistory = ko.observable(dist.VAsc() !== "" || dist.VAcc() !== "" || dist.VAnear() !== "");
                },
                "{root}.History.Bino[i].FieldValue": function (bino) {
                    bino.Binocularity = ko.observable(bino.Binocularity1() + " " + bino.Binocularity2() + " " + bino.Binocularity3() + " " + bino.Binocularity4());
                    bino.W4DNear = ko.observable(bino.W4DNear1() + " " + bino.W4DNear2());
                    bino.W4DDist = ko.observable(bino.W4DDistance1() + " " + bino.W4DDistance2());
                    bino.Stereo = ko.observable(bino.Stereo1() + " " +  bino.Stereo2());
                    bino.HasHistory = ko.observable(bino.Binocularity() !== "" || bino.W4DNear() !== "" || bino.W4DDist() !== "" || bino.Stereo() !== "");
                },
                "{root}.History.Ocm[i].FieldValue": function (ocm) {
                    ocm.HasHistory = ko.observable(true);
                },
            }
        };
    }

    function GetOdOsString(od1, od2, os1, os2) {
        od = od1;
        if (od2 !== "")
            od += " " + od2;
        os = os1;
        if (os2 !== "")
            os += " " + os2;

        ou = "";

        if (od == os)
            ou = od;

        od = od == "" ? od : od + " OD ";
        os = os == "" ? os : os + " OS";
        ou = ou == "" ? ou : ou + " OU";

        odosString = "";

        if (od != "" || os != "")
        {
            if (ou != "")
                odosString = ou;
            else
                odosString = od + os;
        }

        return odosString;
    }

    function setOverrides() {
        //if (profile.userName() !== undefined) {
        //    vm.model.User.Value = profile.userName;
        //}
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

    function rfxHistory() {
        //History windows
        loadHistoryWindow('rfxHistoryWindow', "600px", "200px", "Rfx History", "rfx");
        return false;
    }

    function cchHistory() {
        //History windows
        loadHistoryWindow('cchHistoryWindow', "600px", "200px", "CC History", "cch");
        return false;
    }

    function sumHistory() {
        //History windows
        loadHistoryWindow('sumHistoryWindow', "600px", "200px", "Summary History", "sum");
        return false;
    }

    function distHistory() {
        //History windows
        loadHistoryWindow('distHistoryWindow', "600px", "200px", "VA Dist & Near History", "dist");
        return false;
    }

    function binoHistory() {
        //History windows
        loadHistoryWindow('binoHistoryWindow', "600px", "200px", "Binocularity History", "bino");
        return false;
    }

    function ocmHistory() {
        //History windows
        loadHistoryWindow('ocmHistoryWindow', "600px", "200px", "Ocular Motility History", "ocm");
        return false;
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

        return false;
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
        
        if ((saveType === constants.enum.notesSaveType.SignOff || saveType === constants.enum.notesSaveType.Correct) && !validateNotes())
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

    function validateNotes() {
        var ret = CheckNoPref();

        if (ret) {
            if ((vm.model.SLE.Value() === true || vm.model.PenLight.Value() === true) && vm.model.Dilate3.Value() !== undefined)
                ret = true;
            else {
                toastr.error('SLE/Pen-light options and dilated options are required');
                ret = false;
            }
        }

        return ret;
    }

    function CheckNoPref() {
        var noprefchecked = vm.model.NoPref.Value();
        if (noprefchecked) {
            var od = vm.model.VAscOD1.Value() + ' ' + vm.model.VAscOD2.Value();
            var os = vm.model.DistOS1.Value() + ' ' + vm.model.DistOS2.Value();

            if (od != os) {
                toastr.error('VA sc Dist OD and OS should be equal when No Pref checkbox is checked');
                return false;
            }
        }

        return true;
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
                    destroyWindows();
                    return true;
                }
                else {
                    return false;
                }
            });
        }
        else {
            destroyWindows();
            return true;
        }
    }

    function doctorChange() {
        var doctorUserName = vm.model.User.Value();
        if (vm.model.DefaultInd() === true) {
            utility.showMessage('Do you want to loose your changes and load defaults for ' + doctorUserName + ' ?', 'Defaults').then(function (dialogResult) {
                if (dialogResult === 'Yes') {
                    session.isDirty(false);
                    session.isNotesPatientDirty(false);
                    getNotes(doctorUserName);
                }
                else {
                    getAutoComplete(doctorUserName)
                }
            });
        }
        else
        {
            getAutoComplete(doctorUserName)
        }
    }

    function getAutoComplete(doctorUserName) {
        var getdata = { "userName": doctorUserName };
        utility.httpGet('api/notesautocomplete', getdata).then(function (data) {
            if (data.Success === true) {
                window.autoComplete = data.Model;
            }
        });
    }
});
