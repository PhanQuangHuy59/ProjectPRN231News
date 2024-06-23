$(document).ready(function () {

    //show và tắt password
    const eyeoldPassword = document.getElementById('eyeoldPassword');
    const oldPassword = document.getElementById('oldPassword');

    eyeoldPassword.addEventListener('click', () => {
        if (oldPassword.type === 'password') {
            oldPassword.type = 'text';
        } else {
            oldPassword.type = 'password';
        }
    });

    const eyenewPassword = document.getElementById('eyenewPassword');
    const newPassword = document.getElementById('newPassword');

    eyenewPassword.addEventListener('click', () => {s
        if (newPassword.type === 'password') {
            newPassword.type = 'text';
        } else {
            newPassword.type = 'password';
        }
    });

    const eyeconfirmPassword = document.getElementById('eyeconfirmPassword');
    const confirmPassword = document.getElementById('confirmPassword');

    eyeconfirmPassword.addEventListener('click', () => {
        if (confirmPassword.type === 'password') {
            confirmPassword.type = 'text';
        } else {
            confirmPassword.type = 'password';
        }
    });



    function showSuccess(message){
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

    function showError(message){
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

    

    $("#buttonEnable").click(function () {
        $("#displayName").prop("disabled", function (i, val) {
            return !val;
        });
    });

    // khi form displayname được submit
    const form = document.querySelector("#formUpdateDisplayName");
    let displayName = document.querySelector("#displayName");
    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const formData = new FormData(form);
        // lấy dữ liệu từ form
        const userId = formData.get("userId");
        const displayName = formData.get("displayName");

        // làm gì đó với dữ liệu
        console.log(userId, displayName);

        // hoặc gửi dữ liệu đến server sử dụng fetch

        let urlUpdateDisplayName = `https://localhost:7251/api/Users/UpdateDisplayName?userId=${userId}&displayName=${displayName}`

        $.ajax({
            url: urlUpdateDisplayName,
            type: 'PUT',
            contentType: "application/json",
            success: function (result, status, xhr) {
                //location.reload
                displayName.value = result.DisplayName;
                showSuccess('Đã thay đổi tên thành công.');
                
                console.log("Success:", result);
            },
            error: function (xhr, status, error) {
                var text = xhr.responseText;
                showError(text);
                console.error("Error: " + text);
            }
        });
    });

    //Xử lý form thông tin cơ bản
    const formbasic = document.getElementById("formBasicInformation");

    formbasic.addEventListener("submit", (e) => {
        e.preventDefault(); // Prevent the form from submitting

        const phoneNumber = document.getElementById("phonenumber").value;
        const ngaysinh = document.getElementById("ngaysinh").value;
        const gender = getSelectedRadioValue("gender");
        const diachi = document.getElementById("diachi").value;
        const userId = document.getElementById("userId").value

        console.log({
            phoneNumber,
            ngaysinh,
            gender,
            diachi,
            userId
        });


        let urlUpdateDisplayName = `https://localhost:7251/api/Users/UpdateInformationBasic?userId=${userId}&phoneNumber=${phoneNumber}&dateOfBirth=${ngaysinh}&gioiTinh=${gender}&address=${diachi}`
        console.log(urlUpdateDisplayName);
        $.ajax({
            url: urlUpdateDisplayName,
            type: 'PUT',
            contentType: "application/json",
            success: function (result, status, xhr) {
                //location.reload
                displayName.value = result.DisplayName;
                showSuccess(('Update Thành Công Thông Tin');
               
                console.log("Success:", result);
            },
            error: function (xhr, status, error) {
                var text = xhr.responseText;
                showError(text);
                console.error("Error: " + text);
            }
        });

        // Xử lý dữ liệu tại đây
    });

    function getSelectedRadioValue(name) {
        const radios = document.getElementsByName(name);
        for (let i = 0; i < radios.length; i++) {
            if (radios[i].checked) {
                return radios[i].value;
            }
        }
        return null;
    }
    // xử lý form change password

    const formChangePassword = document.getElementById("formChangePasswordMain");

    formChangePassword.addEventListener("submit", (e) => {
        e.preventDefault(); // Prevent the form from submitting
        const userId = $("#userId").val();
        let oldPassword = $("#oldPassword").val();
        let newPassword = $("#newPassword").val();
        let confirmPassword = $("#confirmPassword").val();

        var checkIsValid = false;
        if (newPassword === confirmPassword) {
            $("#newPassword").removeClass("is-invalid").addClass("is-valid");
            $("#confirmPassword").removeClass("is-invalid").addClass("is-valid");
            checkIsValid = true;
        } else {
            $("#newPassword").removeClass("is-valid").addClass("is-invalid");
            $("#confirmPassword").removeClass("is-valid").addClass("is-invalid");
            showError("Mật Khẩu không khớp nhau");
            checkIsValid = false;
        }

        if (checkIsValid) {
            let urlUpdateDisplayName = `https://localhost:7251/api/Users/ChangePassword?userId=${userId}&oldPassword=${oldPassword}&newPassword=${newPassword}`;
            console.log(urlUpdateDisplayName);
            $.ajax({
                url: urlUpdateDisplayName,
                type: 'PUT',
                contentType: "application/json",
                success: function (result, status, xhr) {
                    //location.reload
                    var message = 'Thay đổi mật khẩu thành công.';
                    showSuccess(message);
                    newPassword.val('');
                    oldPassword.val('');
                    confirmPassword.val('');
                    console.log("Success:", result);
                },
                error: function (xhr, status, error) {
                    var text = xhr.responseText;
                    showError(text);
                    if (xhr.status == 405) {
                        $("#oldPassword").removeClass("is-valid").addClass("is-invalid");
                        $("#newPassword").removeClass("is-invalid").addClass("is-valid");
                        $("#confirmPassword").removeClass("is-invalid").addClass("is-valid");
                    } else if (xhr.status == 406) {
                        $("#newPassword").removeClass("is-valid").addClass("is-invalid");
                        $("#confirmPassword").removeClass("is-valid").addClass("is-invalid");
                        $("#oldPassword").removeClass("is-invalid").addClass("is-valid");
                    }


                    console.error("Error: " + text);
                }
            });
        }

    });
      

});