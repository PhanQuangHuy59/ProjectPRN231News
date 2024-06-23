document.addEventListener('DOMContentLoaded', function () {
    // Lấy tất cả các thẻ a có class 'comment-reply-link'
    const replyLinks = document.querySelectorAll('.comment-reply-link');
    $('#articleId').val('');
    $('#userId').val('');
    $('#replyFor').val('');
    $('#cancel-comment-reply-link').hide();
    console.log("Chay vao Khoi Dong");

    // Thêm event listener cho mỗi thẻ a
    replyLinks.forEach(function (link) {
        link.addEventListener('click', function (event) {
            event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ a nếu cần
            // hiển thị nut cancle
            $('#cancel-comment-reply-link').show();
            // Lấy các dữ liệu từ thuộc tính data-*
            const commentId = link.dataset.commentid;
            const articleid = link.dataset.articleid;
            const userid = link.dataset.userid;

            document.getElementById('articleId').value = articleid;
            document.getElementById('userId').value = userid; // Thay thế 'currentUserId' bằng ID người dùng hiện tại nếu có
            document.getElementById('replyFor').value = commentId;

            // In các dữ liệu ra console hoặc xử lý chúng theo nhu cầu của bạn
            console.log('Comment ID:', commentId);
            console.log('Article ID:', articleid);
            console.log('User Id:', userid);


            // Bạn có thể thực hiện các hành động khác tại đây
        });
    });
    main_information_comment
    $("#cancel-comment-reply-link").on('click', function (event) {
        $('#articleId').val('');
        $('#userId').val('');
        $('#replyFor').val
        $('#cancel-comment-reply-link').hide();
        console.log("Chay vao Cancel");

    });

    $('#commentform').on('submit', function (event) {
        event.preventDefault(); // Ngăn chặn hành vi mặc định của form

        // Tạo đối tượng FormData từ form

        var inforMainComment = document.getElementById('main_information_comment');
        if ($('#articleId').val() === ''
            & $('#userId').val() === '') {
            $('#articleId').val("" + inforMainComment.dataset.articleid);
            $('#userId').val("" + inforMainComment.dataset.userid);
            $('#replyFor').val('-1');

        }

        var formData = new FormData(this);
        // Lấy dữ liệu từ các input ẩn và thêm vào FormData nếu chưa có
        if (!formData.has('ArticleId')) {
            formData.append('ArticleId', $('#articleId').val());
        }
        if (!formData.has('UserId')) {
            formData.append('UserId', $('#userId').val());
        }
        if (!formData.has('ReplyFor')) {
            formData.append('ReplyFor', $('#replyFor').val());
        }
        const data = {};

        formData.forEach((value, key) => {
            if (key != "comment_post_ID" & key != "comment_parent"
                & key != "__RequestVerificationToken"
                & key != "wantispam_q" & key != "wantispam_e_email_url_website") {
                data[key] = value;
            }
            if (key == "__RequestVerificationToken") {
                requestToken = value;
            }

        });
        // In ra console để kiểm tra các giá trị
        console.log(JSON.stringify(data));
        var urlComment = "";
        //Tạo và gửi request bằng AJAX

        if (data["UserId"] === "nologin") {
            ShowError("Bạn không thể bình luận khi bạn chưa đăng nhập");
        } else {
            $.ajax({
                url: `https://localhost:7251/api/Comments`,
                type: 'POST',
                data: JSON.stringify(data), // Chuyển đổi dữ liệu thành chuỗi JSON
                contentType: "application/json",
                success: function (result, status, xhr) {
                    //location.reload
                    var message = 'Comment thanh cong hãy reload lại trang';
                    ShowSuccess(message);
                    $("#comment").val('');
                    console.log('Success:', result);
                },
                error: function (xhr, status, error) {
                    var text = xhr.responseText;
                    ShowError(text);
                }
            });
        }
    });

    const timerId = setTimeout(function () {
        addToViewsOfUser();
    }, 5000);
   
   
    function addToViewsOfUser() {
        var inforMainComment = document.getElementById('main_information_comment');

        var articleId = inforMainComment.dataset.articleid;
        var userId = inforMainComment.dataset.userid;
        console.log(userId + "\n" + articleId);
        if (userId !== "nologin") {
            var urlAddViwe = `https://localhost:7251/api/Users/AddArticleToViewUser?userId=${userId}&articleId=${articleId}`;
            $.ajax({
                url: urlAddViwe,
                type: 'POST',
                contentType: "application/json",
                success: function (result, status, xhr) {
                    //location.reload
                    var message = 'Thêm vào Danh sách bài báo người dùng xem thành công';
                    ShowSuccess(message);
                    console.log('Success:', result);
                },
                error: function (xhr, status, error) {
                    var text = xhr.responseText;
                    ShowError(text);
                }
            });
        }
    }
    // to cancel the timer
    


    function ShowSuccess(message) {
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
        toastr["success"](message)
    }

    function ShowError(message) {
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
        toastr["error"](message)
    }
});


    







