//require configuration for durandal
require.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions'
    }
});

//defining global libraries
define('jquery', function () { return jQuery; });
define('knockout', ko);


//starting the app
define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'durandal/composition'], function (system, app, viewLocator, composition) {

    app.title = 'Hello World';

    //specify which plugins to install and their configuration
    app.configurePlugins({
        router: true,
    });


    app.start().then(function () {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application.
        app.setRoot('viewmodels/shell', 'entrance');
    });


});

