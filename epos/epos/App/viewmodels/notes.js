define(['plugins/router'], function (router) {

    var model;

    var vm = {
        model: model,
        resetColour: resetColour,
        activate: activate,
        attached: attached,
        compositionComplete: compositionComplete
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

    function attached() {
//        alert('attached');
    }
    function compositionComplete() {

        //var firstLocation = $("div.firstscroll").offset().top;
        //var secondLocation = $("div.secondscroll").offset().top;
        //var thirdLocation = $("div.thirdscroll").offset().top;

        //$(window).scroll(function () {
        //    var winScroll = $(this).scrollTop(); // current scroll of window

        //    //console.log(winScroll + ' - ' + firstLocation + ' - ' + secondLocation + ' - ' + thirdLocation  )

        //});
        return true;
    }


    function addComputedProperties() {
        vm.model.PrematureBirthString = ko.computed({
            read: function () { return (this.Premature.Value() ? "Yes" : "No") },
            write: function (value) { this.Premature.Value(value == "Yes" ? true : false); }
        }, vm.model);

    }

    function resetColour(item) {
        if (item.ColourType() === 1) {
            item.ColourType(2);
        }
        else {
            item.ColourType(1);

        }
    }

});