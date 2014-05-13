define(['services/utility'], function (utility) {
    var userID = ko.observable(1);
    var firstName = ko.observable();
    var lastName = ko.observable();
    var userName = ko.observable('koty');
    var photoUrl = ko.observable('/pos/data/prasanna.jpg');
    var isAuthenticated = ko.observable(utility.isAuthenticated);


    var vm = {
        userID: userID,
        firstName: firstName,
        lastName: lastName,
        userName: userName,
        photoUrl: photoUrl,
        isAuthenticated: isAuthenticated,
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
            this.photoUrl(utility.virtualDirectory + '/Data/NoPhoto.jpg');
        }
        else {
            this.photoUrl(utility.virtualDirectory + '/Data/' + data.PhotoUrl + '.jpg');
        }
        this.isAuthenticated(true); 
    };

    function logout() {
        this.photoUrl(undefined);
        this.isAuthenticated(false);
    };
});

