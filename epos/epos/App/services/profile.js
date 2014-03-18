define([], function () {
    var userID = ko.observable();
    var firstName = ko.observable();
    var lastName = ko.observable();
    var userName = ko.observable();
    var photoUrl = ko.observable();
    var isAutenticated = ko.observable(false);


    var vm = {
        userID: userID,
        firstName: firstName,
        lastName: lastName,
        userName: userName,
        photoUrl: photoUrl,
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
        if (data.PhotoUrl === null) {
            this.photoUrl(null);
        }
        else {
            this.photoUrl(utility.virtualDirectory + '/Data/' + data.PhotoUrl + '.jpg');
        }
        this.isAutenticated(true);
    };

    function logout() {
        this.isAutenticated(false);
    };
});

