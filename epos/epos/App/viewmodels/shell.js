define(['plugins/router'], function (router) {
    return {
        activate: activate,
        router: router
    };

    function activate() {
        var routes = [
                        { route: ['', 'home'], moduleId: 'home', title: 'Home' },
        ];

        router.makeRelative({ moduleId: 'viewmodels' }) // router will look here for viewmodels by convention
            .map(routes)            // Map the routes
            .buildNavigationModel() // Finds all nav routes and readies them
            .activate();            // Activate the router

    }
});
