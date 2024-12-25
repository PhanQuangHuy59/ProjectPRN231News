document.addEventListener('DOMContentLoaded', function () {
    // Lấy tất cả các thẻ a có class 'comment-reply-link'
    var inforMainComment = document.getElementById('main_information_comment');
    var userLoginId = inforMainComment.dataset.userid;
    var articleIdSave = inforMainComment.dataset.articleid;
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
    function addEventClickForReply() {
        const replyLinks1 = document.querySelectorAll('.comment-reply-link');
        replyLinks1.forEach(function (link) {
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
    }
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
                url: `https://localhost:8080/api/Comments`,
                type: 'POST',
                data: JSON.stringify(data), // Chuyển đổi dữ liệu thành chuỗi JSON
                contentType: "application/json",
                success: function (result, status, xhr) {
                    //location.reload
                    var message = 'Comment thanh công hãy reload lại trang';
                    ShowSuccess(message);
                    $("#comment").val('');

                    var commentHtml = `
                <li style="margin-left: ${result.replyFor ? '60px' : '20px'}" class="list-group-item" id="li-comment-${result.commentId}">
                    <div class="comment-wrapper" id="comment-${result.commentId}">
                        <div class="comment-inner">
                            <div class="comment-avatar">
                                <img alt='' src='${result.userImage ? result.userImage : "../images/male_default.png"}' class='avatar avatar-46 photo' height='46' width='46' loading='lazy' decoding='async' />
                            </div>
                            <div class="commentmeta">
                                <p class="comment-meta-1">
                                    <cite class="fn">${result.userName}</cite>
                                </p>
                                <p class="comment-meta-2">
                                    ${new Date(result.createDate).toLocaleString()}
                                </p>
                            </div>
                            <div class="text">
                                <div class="c">
                                    <p>${result.content}</p>
                                </div>
                            </div>
                            <div class="clear"></div>
                            <div class="comment-reply"><span class="reply"><a rel='nofollow' class='comment-reply-link' href='index${result.commentId}.html?replytocom=${result.commentId}#respond' data-commentid="${result.commentId}" data-articleid="${result.articleId}" data-userid="${result.userId}" data-postid="${result.articleId}" data-belowelement="comment-${result.commentId}" data-respondelement="respond" data-replyto="Reply to ${result.commentId}" aria-label='Reply to ${result.commentId}'>Reply</a></span></div>
                        </div>
                    </div>
                </li>
            `;

                    if (result.replyFor) {
                        var parentComment = $(`#li-comment-${result.replyFor}`);
                        if (parentComment.find('ul.list-group').length === 0) {
                            parentComment.append('<ul class="list-group"></ul>');
                        }
                        parentComment.find('ul.list-group').append(commentHtml);
                    } else {
                        $("#mainCommentArticle").append(commentHtml);
                    }
                    addEventClickForReply();
                    console.log("Success:", result);
                    
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
    }, 2000);


    function addToViewsOfUser() {
        var inforMainComment = document.getElementById('main_information_comment');

        var articleId = inforMainComment.dataset.articleid;
        var userId = inforMainComment.dataset.userid;
        console.log(userId + "\n" + articleId);
        if (userId !== "nologin") {
            var urlAddViwe = `https://localhost:8080/api/Users/AddArticleToViewUser?userId=${userId}&articleId=${articleId}`;
            $.ajax({
                url: urlAddViwe,
                type: 'POST',
                contentType: "application/json",
                success: function (result, status, xhr) {
                    //location.reload
                    if (result === true) {
                        var message = 'Thêm vào Danh sách bài báo người dùng xem thành công';
                        ShowSuccess(message);
                        console.log('Success:', result);
                    }

                },
                error: function (xhr, status, error) {
                    var text = xhr.responseText;
                    ShowError(text);
                }
            });
        } else {
            var urlIncreateViewWithNoLogin = `https://localhost:8080/api/Articles/IncreaseViewArticle?articleId=${articleId}`;

            $.ajax({
                url: urlIncreateViewWithNoLogin,
                type: 'POST',
                contentType: "application/json",
                success: function (result, status, xhr) {
                },
                error: function (xhr, status, error) {
                    var text = xhr.responseText;
                    ShowError(text);
                }
            });
        }
    }
    // to cancel the timer
    // Copy link Article
    var copy_link = $("#copy_link_article");
    copy_link.click(function () {
        const link = this.dataset.link;
        console.log(link);
        const textarea = document.createElement("textarea");
        textarea.value = link;
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand("copy");
        document.body.removeChild(textarea);
        ShowSuccess("Copy link bài viết thành công");
    });

    var content = $("#hide_content");
    var printclick = $("#print_click");
    printclick.click(function () {
        var contenthtml = content.html();
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`<html><head><title>Print Link</title></head><body><h1>${contenthtml}</h1></body></html>`);
        printWindow.document.close();
        printWindow.print();
    });
    var saveClick = $("#save_click");
    saveClick.click(function () {

        if (userLoginId === 'nologin') {
            ShowError("Ban chưa đang nhập. Nên không thể lưu bài viết.");
        } else {
            let urlUpdateSaveArticle = `https://localhost:8080/api/SaveArticles/RemoveOrAddSaveArticle?userId=${userLoginId}&articleId=${articleIdSave}`
            $.ajax({
                url: urlUpdateSaveArticle,
                type: 'POST',
                contentType: "application/json",
                success: function (result, status, xhr) {
                    if (result === true) {
                        ShowSuccess("Đã lưu thành công bài báo");
                        saveClick.removeClass('text-dark');
                        saveClick.addClass('text-primary');
                    } else {
                        ShowSuccess("Đã xóa thành công bài báo");

                        saveClick.removeClass('text-primary');
                        saveClick.addClass('text-dark');
                    }

                    console.log("Success:", result);
                },
                error: function (xhr, status, error) {
                    let text = xhr.responseText;
                    ShowError("Đã xảy ra lỗi hãy thử lại.");
                }
            });
        }
    });
    // drop emotion 
    $('.click_emotion').each(function () {
        $(this).click(function () {
            var emotionId = $(this).attr('data-emotionId');
            var urlAddOrRemoveDropEmotion = `https://localhost:8080/api/DropEmotions/AddOrRemoveDropEmotion?userId=${userLoginId}&articleId=${articleIdSave}&emotionId=${emotionId}`;
            console.log(urlAddOrRemoveDropEmotion);
            var divClick = $(this);
            if (userLoginId == 'nologin') {
                ShowError("Ban chưa đăng nhập không thể thả cảm xúc");
            } else {
                $.ajax({
                    url: urlAddOrRemoveDropEmotion,
                    type: 'POST',
                    contentType: "application/json",
                    success: function (result, status, xhr) {
                        if (result === true) {
                            var span = divClick.find('span.numberDropEmotion');
                            span.addClass('text-primary');
                            var numberDropEmotion = parseInt(span.html(), 10) + 1;
                            span.html(numberDropEmotion);
                            ShowSuccess("Đã thả emotion");

                        } else {
                            var span = divClick.find('span.numberDropEmotion:first');
                            span.removeClass('text-primary');
                            var numberDropEmotion = parseInt(span.html(), 10) - 1;
                            span.html(numberDropEmotion);

                            ShowSuccess("Đã xóa emotion");

                        }

                        console.log("Success:", result);
                    },
                    error: function (xhr, status, error) {
                        var text = xhr.responseText;
                        showError(text);
                    }
                });
                // do something with the emotionId
                console.log('Emotion ID:', emotionId);
            }



        });
    });




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










