define([], function () {
    var userID = ko.observable();
    var firstName = ko.observable();
    var lastName = ko.observable();
    var userName = ko.observable();
    var isAutenticated = ko.observable(true);


    var vm = {
        userID: userID,
        firstName: firstName,
        lastName: lastName,
        userName: userName,
        isAutenticated: isAutenticated,
        activate: activate,
        populate: populate,
        logout: logout
    };

    return vm;

    function activate() {
        return true;
    }

    function populate(data) {
        this.userID(data.UserID);
        this.firstName(data.FirstName);
        this.lastName(data.LastName);
        this.userName(data.UserName);
        this.isAutenticated(true);
    };

    function logout() {
        this.isAutenticated(false);
    };
});

