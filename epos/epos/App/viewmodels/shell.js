define(['plugins/router', 'services/navigation', 'services/session', 'services/utility', 'services/constants'], function (router, navigation, session, utility, constants) {
    return {
        activate: activate,
        router: router,
        navigation: navigation,
        utility: utility,
        logout: logout,
        attached: attached,
        compositionComplete: compositionComplete
    };


    function attached() {
        $(document).foundation();
    }
    function compositionComplete() {
        //alert('shell compositionComplete')
    }

    function activate() {
        var routes = [
                        { route: ['','home(/:module)(/:id)(/:criteria)'], moduleId: 'home', title: 'Home' },
                        { route: 'login', moduleId: 'login', title: 'Login' },
                        { route: 'queue', moduleId: 'queue', title: 'Print Queue' },
                        { route: 'default', moduleId: 'default', title: 'Defaults' },
                        { route: 'notes/:notestype(/:id)(/:examid)', moduleId: 'notes', title: 'Notes' },
                        { route: 'dropdown', moduleId: 'dropdown', title: 'Drop Downs' },
                        { route: 'autocorrect', moduleId: 'autocorrect', title: 'Auto Correct' },
                        { route: 'messagebox', moduleId: 'messagebox', title: 'messagebox' },
                        { route: 'patient/:id', moduleId: 'patient', title: 'Patient' }
        ];

        router.makeRelative({ moduleId: 'viewmodels' }) // router will look here for viewmodels by convention
            .map(routes)            // Map the routes
            .buildNavigationModel() // Finds all nav routes and readies them
            .mapUnknownRoutes('notfound', 'not-found')
            .activate();            // Activate the router

        //setting up the guardRoute
        router.guardRoute = function (instance, instruction) {
            if (session.profile.isAuthenticated() === false && instruction.config.route !== "login") {
                return 'login';
            }

            return true;
        };

        //global variables
        window.session = session;
        window.utility = utility;
        window.constants = constants;
        window.navigation = navigation;

        if (location.href.indexOf('#') !== -1)
            navigation.returnUrl = location.href.substring(location.href.indexOf('#'));

        //sync loading lookups
        return session.populateLookups();

    }

    function logout() {
        session.profile.logout();
        router.navigate('login');
    }

});
