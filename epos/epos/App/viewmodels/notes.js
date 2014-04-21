define(['plugins/router'], function (router) {

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
        cancel: cancel
};

    return vm;

    function activate(notestype, patientid, examid) {
        var getdata = { "type": notestype, "patientid": patientid, "examid": examid };
        return utility.httpGet('api/notes', getdata).then(function (data) {
            if (data.Success === true) {
                vm.model = ko.viewmodel.fromModel(data.Model);
                addComputedProperties();
            }

            return data;
        });
    };

    function canReuseForRoute() {
        return false;
    }


    function attached() {
//        alert('attached');
    }
    function compositionComplete() {
        var notesHeaderDefaultOffset = $("div.notes-header").offset().top;
        var patinetinfoOffset = $("#patinet-info-header").offset().top;
        var cchistoryOffset = $("#cc-history-header").offset().top;
        var acuityOffset = $("#acuity-header").offset().top;
        var ocularmotOffset = $("#ocular-mot-header").offset().top;
        var antsegOffset = $("#ant-seg-header").offset().top;
        var summaryOffset = $("#summary-header").offset().top;
        var currentOffset = patinetinfoOffset;

        $(window).scroll(function () {
            var winScroll = $(this).scrollTop(); // current scroll of window
            //if (winScroll > notesHeaderScrollTop)
            var notesHeaderoffset = notesHeaderDefaultOffset;
            if (winScroll > notesHeaderDefaultOffset) {
                notesHeaderoffset = winScroll
            }
            $("div.notes-header").offset({ top: notesHeaderoffset });

            //calculating the menu offset
            var newOffset = patinetinfoOffset;
            var notesHeight = $("div.notes-header").height();
            var winOffset = winScroll + notesHeight + 1;
            if (winOffset >= 0 && winOffset < cchistoryOffset && currentOffset != patinetinfoOffset) {
                $(".notes-menu").removeClass('notes-menu-selected');
                $("#patinet-info-menu").addClass('notes-menu-selected');
                currentOffset = patinetinfoOffset;
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
            else if (winOffset >= summaryOffset && currentOffset != summaryOffset) {
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
    }
    
    function addComputedProperties() {
        vm.model.HeaderText = ko.computed(function () {
            var headerText = this.PatientName.Value();
            var ExamID = ko.unwrap(vm.model.hdnExamID);
            var ExamDate = ko.unwrap(vm.model.ExamDate);
            var ExamSaveDate = ko.unwrap(vm.model.ExamSaveDate);
            var ExamCorrectDate = ko.unwrap(vm.model.ExamCorrectDate);
            if (ExamSaveDate !== null) {
                headerText += ' - Notes saved on ' + ExamSaveDate.Value()
            } 
            else if(ExamCorrectDate !== null){
                headerText += ' - Notes taken on ' + ExamDate.Value() + ' (Corrected on ' + ExamCorrectDate.Value() + ')';
            } 
            else if (ExamID !== null) {
                headerText += ' -  Notes taken on ' + ExamDate.Value();
            }
            else {
                headerText += ' - New notes'
            }
            return headerText;
        }, vm.model);

    }

    function deleteComputedProperties() {
        delete vm.model.HeaderText;
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
        router.navigateBack();
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

    function saveNotes(saveType) {
        deleteComputedProperties();
        return utility.httpPost('api/notes?type=' + saveType.toString(), vm.model).then(function (data) {
            if (data.Success === true) {
                toastr.info(data.Message);
                router.navigateBack();
            }
            return data;
        });
    }
});


//for (var key in obj) {
//    if(ko.isComputed(obj[key]))
//    {
//        delete obj[key];
//    }
//}