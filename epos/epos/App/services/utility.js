define(['plugins/http', 'plugins/dialog', 'viewmodels/messagebox'], function (http, dialog, messagebox) {
    var loading = ko.observable(false);
    var virtualDirectory = '';
    var vm =
        {
            loading: loading,
            virtualDirectory: virtualDirectory,
            httpPost: httpPost,
            httpGet: httpGet,
            showMessage: showMessage
        };

    return vm;

    function httpPost(url, data) {
        var self = this;
        self.loading(true);
        url = vm.virtualDirectory + '/' + url;
        return http.post(url, data).then(function (returndata) {
            self.loading(false);
            if (returndata.Success === false) {
                toastr.error(returndata.Message);
            }
            return returndata;
            }).fail(function (e) {
                self.loading(false);
                var message = '';
                if (e.responseJSON.ExceptionMessage !== undefined)
                    message = e.responseJSON.ExceptionMessage;
                else
                    message = e.responseJSON.Message;
                toastr.error(message);
            });
    }

    function httpGet(url, data) {
        var self = this;
        url = vm.virtualDirectory + '/' + url;
        self.loading(true);
        return http.get(url, data).then(function (returndata) {
            self.loading(false);
            if (returndata.Success === false) {
                toastr.error(returndata.Message);
            }
            return returndata;
        }).fail(function (e) {
            self.loading(false);
            var message = '';
            if (e.responseJSON.ExceptionMessage !== undefined)
                message = e.responseJSON.ExceptionMessage;
            else
                message = e.responseJSON.Message;
            toastr.error(message);
        });
    }

    function showMessage(message, title) {
        var app = require('durandal/app');
        messagebox.set(message, title);
        return dialog.show(messagebox);
    }

});