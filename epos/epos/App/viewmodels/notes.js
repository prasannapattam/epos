define(['plugins/router'], function (router) {

    var model;

    var vm = {
        model: model,
        activate: activate,
        resetColour: resetColour
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