$(document).ready(function () {

    const bookmarkIcons = document.querySelectorAll('i.fa.fa-bookmark');

    bookmarkIcons.forEach(icon => {
        icon.addEventListener('click', function () {
            let userId = this.getAttribute('data-userId');
            let articleId = this.getAttribute('data-articleId');
            const iconElement = this;
            console.log(`Bookmark button clicked for article ${articleId} by user ${userId}!`);
            let urlUpdateDisplayName = `https://localhost:8080/api/SaveArticles/RemoveOrAddSaveArticle?userId=${userId}&articleId=${articleId}`
            $.ajax({
                url: urlUpdateDisplayName,
                type: 'POST',
                contentType: "application/json",
                success: function (result, status, xhr) {

                    if (result === true) {
                        showSuccess("Đã lưu thành công bài báo");
                        iconElement.classList.add('text-primary'); 
                    } else {
                        showSuccess("Đã xóa thành công bài báo");
                        iconElement.classList.remove('text-primary');
                    }

                    console.log("Success:", result);
                },
                error: function (xhr, status, error) {
                    var text = xhr.responseText;
                    showError(text);
                    console.error("Error: " + text);
                }
            });

        });
    });

    function showSuccess(message) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        toastr["success"](message);
    }

    function showError(message) {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        toastr["error"](message);
    }




});