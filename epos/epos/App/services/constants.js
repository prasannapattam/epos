define([], function () {

    this.enum = new Object();

    this.enum.module = {
        patient: 'patient',
        user: 'user'
    };

    this.enum.notesType = {
        New: 1,
        Correct: 2,
        Saved: 3,
        Default: 4
    };

    this.enum.colourType = {
        Normal: 0,
        New: 1,
        Correct: 1
    };

    return this;
});