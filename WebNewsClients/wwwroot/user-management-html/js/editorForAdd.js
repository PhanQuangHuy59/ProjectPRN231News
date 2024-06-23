var Ckeditor = function () {
    // Private functions
    var demos = function () {
        ClassicEditor
            .create(document.querySelector('.editorImport'))
            .then(editor => {
                console.log(editor);
            })
            .catch(error => {
                console.error(error);
            });
    }

    return {
        // public functions
        init: function () {
            demos();
        }
    };
}();

// Initialization
jQuery(document).ready(function () {
    Ckeditor.init();
});