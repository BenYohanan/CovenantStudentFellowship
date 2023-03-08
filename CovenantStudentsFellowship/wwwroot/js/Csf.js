function userRegistration()
{
    debugger; 
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastName').val();
    data.SchoolId = $('#schoolId').val();
    data.MiddleName = $('#middleName').val();
    data.Email = $('#email').val();
    data.Password = $('#password').val();
    data.ConfirmPassword = $('#confirmpassword').val();
    data.PhoneNumber = $('#phoneNumber').val();
    data.GenderId = $('#genderId').val();
    data.DateOfBirth = $('#dateOfBirth').val();
    if (data.Email != "" && data.Password != "" && data.PhoneNumber != "") {
        let applicationViewModel = JSON.stringify(data);
        $.ajax({
            type: 'POST',
            dataType: 'Json',
            url: '/Account/UserRegistration',
            data:
            {
                applicationViewModel: applicationViewModel
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/Account/Login';
                    successAlertWithRedirect(result.msg, url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
    else
    {
        errorAlert("Please,Enter Email,Password & Confirm Password To Continue");
    }
}

function CompleteRegistration()
{
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var file = document.getElementById("profilePicture").files;
    var data = {};
    data.StateId = $('#stateId').val();
    data.ReligionId = $('#religionId').val();
    data.NationalityId = $('#countryId').val();
    data.Department = $('#department').val();
    data.HomeSynagogue = $('#homeSynagogue').val();
    data.HomeAddress = $('#homeAddress').val();
    data.ContactPhoneNumber = $('#emergencyContactNumber').val();
    data.ContactRelationship = $('#contactRelationship').val();
    data.ContactName = $('#contactName').val();
    data.SchoolAddress = $('#schoolAddress').val();
    let userDetails = JSON.stringify(data);
    if (file[0] == null) {
        $.ajax({
            type: 'POST',
            URL: '/User/CompleteRegistration',
            dataType: 'JSON',
            data:
            {
                applicationUserViewModel: userDetails
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    debugger;
                    var url = result.dashboard;
                    newSuccessAlert(result.msg, url);
                } else {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            }
        });
    }
    else {
        const reader = new FileReader();
        reader.readAsDataURL(file[0]);
        var base64;
        reader.onload = function () {
            base64 = reader.result;
            debugger;
            $.ajax({
                type: 'POST',
                dataType: 'Json',
                url: '/User/CompleteRegistration',
                data:
                {
                    base64: base64,
                    applicationUserViewModel: userDetails
                },
                success: function (result) {
                    debugger;
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    if (!result.isError) {
                        var url = result.dashboard;
                        successAlertWithRedirect(result.msg, url);
                    }
                    else {
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        errorAlert(result.msg);
                    }
                },
                error: function (ex) {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert("Error Occured,try again.");
                }
            });
        }
    }
}
   
function Login() {
    debugger;
    var email = $("#emailAddress").val();
    var password = $("#password").val();
    $.ajax({
        type: 'POST',
        url: '/Account/Login',
        data:
        {
            email: email,
            password: password
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                location.href = result.dashboard;
            }
            else if (result.emailNotConfirmed) {
                var yesUrl = "/Account/ResendEmail";
                var noUrl = "/Account/ChangeEmail";
                SweeTest(yesUrl, noUrl)

                    Swal.fire({
                        title: "Unverified E-mail Address",
                        text: "Check your inbox Or Resend email",
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonColor: "#FE9E76",
                        confirmButtonText: "Resend email",
                        cancelButtonText: "Change email",
                        closeOnConfirm: false,
                        closeOnCancel: false
                    })
                        .then((result) => {
                            debugger
                            if (result.isConfirmed) {
                                ResendEmail();
                            }
                            else {
                                location.href = noUrl
                            }

                        });
            }
            else {
                $('#loader').show(3000);
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        },
        Error: function (ex)
        {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            errorAlert(ex);
        }
    });
}

function ResetPassword() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var resetPassword = {};
    resetPassword.Email = $("#passwordResetEmail").val();
    let applicationViewModel = JSON.stringify(resetPassword);
    $.ajax({
        type: 'POST',
        url: '/Account/ForgotPassword/',
        data:
        {
            applicationViewModel: applicationViewModel
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                var url = '/Account/Login';
                newSuccessAlert(result.msg, url);
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        },
        Error: function (ex)
        {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            errorAlert(ex);
        }

    });
}

function SetNewPassword()
{
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var newLogInPassword = {};
    newLogInPassword.NewPassword = $('#newPassword').val();
    newLogInPassword.ConfirmNewPassword = $('#comfirmNewPassword').val();
    newLogInPassword.Token = $('#token').val();
    let passwordViewModel = JSON.stringify(newLogInPassword);
    $.ajax({
        type: 'POST',
        url: '/Account/ResetPassword/',
        data:
        {
            viewmodel: passwordViewModel
        },
        success: function (result)
        {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError)
            {
                var url = "/Account/Login";
                newSuccessAlert(result.msg, url);
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                newErrorAlert(result.msg, url);
            }
        },
        Error: function (ex)
        {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            errorAlert(ex);
        }
    });
}

function ChangeEmail() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.Email = $("#emailAddress").val();
    data.ConfirmEmail = $("#ConfirmEmail").val();
    let changeEmailData = JSON.stringify(data);
    $.ajax({
        type: 'POST',
        url: '/Account/ChangeEmail',
        data:
        {
            changeEmailViewModel: changeEmailData
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                var url = '/Account/login';
                newSuccessAlert(result.msg, url);
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        },
        Error: function (ex)
        {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            errorAlert(ex);
        }
    });
}

function UpdatePassword() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var PasswordDetails = {};
    PasswordDetails.OldPassword = $('#oldPassword').val();
    PasswordDetails.NewPassword = $('#newPassword').val();
    PasswordDetails.ConfirmPassword = $('#confirmNewPassword').val();
    if (PasswordDetails.NewPassword != "" && PasswordDetails.NewPassword != "") {
        let newPassword = JSON.stringify(PasswordDetails);
        $.ajax({
            type: 'POST',
            URL: '/Account/ChangePassword',
            dataType: 'JSON',
            data:
            {
                ViewModel: newPassword
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    debugger;
                    var url = "/Account/Login";
                    newSuccessAlert(result.msg, url);

                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            }
        });
    }
}

function ResendEmail() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.Email = $("#emailAddress").val();
    let resendEmail = JSON.stringify(data);
    $.ajax({
        type: 'POST',
        url: '/Account/ResendEmail',
        data:
        {
            applicationViewModel: resendEmail
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                var url = '/Account/Login';
                newSuccessAlert(result.msg, url);
            }
            else {
                errorAlert(result.msg);
            }
        },
        Error: function (ex) {
            errorAlert(ex);
        }
    });
}

function CreateBlog() {
    debugger;
    var blogViewModel = {};
    blogViewModel.Title = $('#title').val();
    blogViewModel.BlogCategoryId = $('#blogCategoryId').val();
    blogViewModel.BlogContent = $('#blogContent').val();
    blogViewModel.AboutMe = $('#aboutMe').val();
    let blogModel = JSON.stringify(blogViewModel);
    $.ajax({
        type: 'Post',
        dataType: 'Json',
        url: '/Blog/CreateBlog',
        data: {
            blogViewModel: blogModel
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                var url = '/User/UserIndex';
                successAlertWithRedirect(result.msg, url);
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            errorAlert("Error Occured,try again.");
        }
    });
}

function editBlog(blogId) {
    debugger;

    $.ajax({
        type: 'GET',
        url: '/Blog/EditBlog',
        dataType: 'json',
        data:
        {
            blogId: blogId
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                $("#editTitle").val(result.title);
                $("#editedBlogId").val(result.id);
                $("#editBlogContent").val(result.blogContent);
                $("#editPicture").val(result.image);
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function submitEditedBlog() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var blogDetails = {};
    blogDetails.Id = $("#editedBlogId").val();
    blogDetails.Title = $("#editTitle").val();
    blogDetails.BlogContent = $("#editBlogContent").val();
    blogDetails.Image = $("#editPicture").val();
    if (blogDetails.AuthorsName != "" && blogDetails.Title != "" && blogDetails.BlogContent != "") {
        let blogViewModel = JSON.stringify(blogDetails);
        $.ajax({
            type: 'POST',
            url: '/Blog/EditBlog',
            dataType: 'json',
            data:
            {
                blogViewModel: blogViewModel,
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/Admin/AllBlogs';
                    successAlertWithRedirect(result.msg, url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
}

function deleteBlog(id) {
    debugger;
    $("#deletedBlog").val(id);
}

function blogDelete() {
    debugger;
    var blogDetails = {};
    blogDetails = $("#deletedBlog").val();
    $.ajax({
        type: 'Post',
        url: "/Blog/DeleteBlog",
        dataType: 'Json',
        data:
        {
            blogId: blogDetails
        },
        success: function (result)
        {
            debugger;
            if (!result.isError)
            {
                debugger;
                var url = "/Admin/AllBlogs";
                newSuccessAlert(result.msg, url);

            } else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function AddDropdown() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();

    var commonDropdown = {};
    commonDropdown.DropdownKey = $("#dropDownKeyId").val();
    commonDropdown.Name = $("#dropDownName").val();
    if (commonDropdown.DropdownKey > 0 && commonDropdown.DropdownKey != "") {
        let dropdown = JSON.stringify(commonDropdown);
        $.ajax({
            type: 'POST',
            url: '/SuperAdmin/AddDropdown',
            dataType: 'Json',
            data:
            {
                commonDropdowns: dropdown
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/SuperAdmin/Dropdown';
                    successAlertWithRedirect(result.msg, url);
                }
                else {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
}

function editDropdown(id) {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    $.ajax({
        type: 'GET',
        url: '/SuperAdmin/EditDropdown',
        dataType: 'json',
        data: { dropdownId: id },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                $.each(result.drop, function (i, getdropdown) {
                    debugger;
                    $("#editDropDownKeyId").append('<option value="' + getdropdown.id + '">' + getdropdown.name + '</option>');
                });

                $("#editdropDownName").val(result.data.name);
                $("#editedDropDownId").val(result.data.id);

            }
            else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }

        },
        error: function (ex)
        {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            errorAlert('Failed to retrieve dropdowns' + ex)
        }

    });
}

function submitEditDropDown() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();

    var dropdownDetails = {};
    dropdownDetails.Id = $("#editedDropDownId").val();
    dropdownDetails.Name = $("#editdropDownName").val();
    if (dropdownDetails.Name != "") {
        $.ajax({
            type: 'POST',
            url: '/SuperAdmin/EditDropdown',
            dataType: 'json',
            data:
            {
                commonDropdown: dropdownDetails,
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/SuperAdmin/Dropdown';
                    successAlertWithRedirect(result.msg, url);
                }
                else {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
}

function deleteDropdown(id) {
    debugger;
    $("#deletedDropDownId").val(id);
}

function dropdownDelete() {
    debugger;
    var dropdownDetail = {};
    dropdownDetail = $("#deletedDropDownId").val();
    $.ajax({
        type: 'POST',
        url: "/SuperAdmin/DeleteDropdown",
        dataType: 'json',
        data:
        {
            dropdownId: dropdownDetail
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var url = "/SuperAdmin/Dropdown";
                newSuccessAlert(result.msg, url);

            } else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function AddRole() {
    debugger;

    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    var User = {};
    var Role = {};
    User.Email = $('#email').val();
    Role.Id = $('#roleId').val();
    data.User = User; data.Role = Role;
    if (data.User.Email != "" && data.Role.Id != "") {
        debugger;
        let roleViewModel = JSON.stringify(data);
        $.ajax({
            type: 'Post',
            dataType: 'Json',
            url: '/SuperAdmin/AddUserRole',
            data:
            {
                roleViewModel: roleViewModel
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/SuperAdmin/Roles';
                    newSuccessAlert(result.msg, url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            Error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
}

function removeRole(id, userId) {
    debugger;
    $("#deletedRoleId").val(id);
    $("#userId").val(userId);
}

function roleRemove() {
    debugger;
    var roleId = $("#deletedRoleId").val();
    var userId = $("#userId").val();
    if (roleId != "" && userId != "") {
        $.ajax({
            type: 'POST',
            url: "/SuperAdmin/DeleteRole",
            dataType: 'json',
            data:
            {
                roleId: roleId,
                userId: userId,
            },
            success: function (result) {
                debugger;
                if (!result.isError) {
                    debugger;
                    var url = "/SuperAdmin/Roles";
                    newSuccessAlert(result.msg, url);

                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            }
        });
    }
}

function editUserProfile(userId)
{
    debugger;
    $.ajax({
        type: 'Get',
        url: '/User/EditUserProfile',
        dataType: 'Json',
        data:
        {
            userId: userId,
        },
        success: function (data)
        {
            debugger;
            if (!data.isError)
            {
                debugger;
                $("#userId").val(data.id);
                $("firstName").val(data.firstName);
                $("#lastName").val(data.lastName);
                $("#middleName").val(data.middleName);
                $("#department").val(data.department);
                $("#email").val(data.email);
                $("#gender").val(data.genderId);
                $("#phoneNumber").val(data.phoneNumber);
                $("#schoolAddress").val(data.schoolAddress);
                $("#country").val(data.nationalityId);
                $("#StateId").val(data.stateId);
                $("#religion").val(data.religionId);
                $("#dateOfBirth").val(data.dateOfBirth);
                $("#homeAddress").val(data.homeAddress);
                $("#school").val(data.schoolId);
                $("#homeSynagogue").val(data.homeSynagogue);
                $('#emergencyContact').val(data.contactPhoneNumber);
                $('#contactRelationship').val(data.contactRelationship);
                $('#contactName').val(data.contactName);
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(data.msg);
            }
        }
    });
}

function submitUserProfile()
{
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var UserDetails = {};
    UserDetails.Id = $("#userId").val();
    UserDetails.FirstName = $("#firstName").val();
    UserDetails.LastName = $("#lastName").val();
    UserDetails.MiddleName = $("#middleName").val();
    UserDetails.Department = $("#department").val();
    UserDetails.Email = $("#email").val();
    UserDetails.GenderId = $("#gender").val();
    UserDetails.PhoneNumber = $("#phoneNumber").val();
    UserDetails.SchoolAddress = $("#schoolAddress").val();
    UserDetails.NationalityId = $("#country").val();
    UserDetails.StateId = $("#StateId").val();
    UserDetails.ReligionId = $("#religion").val();
    UserDetails.DateOfBirth = $("#dateOfBirth").val();
    UserDetails.HomeAddress = $("#homeAddress").val();
    UserDetails.SchoolId = $("#school").val();
    UserDetails.HomeSynagogue = $("#homeSynagogue").val();
    UserDetails.ContactPhoneNumber = $('#emergencyContact').val();
    UserDetails.ContactRelationship = $('#contactRelationship').val();
    UserDetails.ContactName = $('#contactName').val();
    if (UserDetails.Id != "" && UserDetails.Name != "")
    {
        let editedUserProfile = JSON.stringify(UserDetails);
        debugger;
        $.ajax({
            type: 'POST',
            url: '/User/SubmitUserProfile',
            dataType: 'json',
            data:
            {
                applicationViewModel: editedUserProfile,
            },
            success: function (result)
            {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError)
                {
                    successAlertWithRedirect(result.msg, result.url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
    else
    {
        $('#loader').show();
        $('#loader-wrapper').fadeOut(3000);
        errorAlert("Id & Name is null,put in data to continue.");
    }
}

function deactivateUser(id) {
    debugger;
    $("#deleteUserId").val(id);
}

function deactivatedeUser() {
    debugger;
    var userDetails = {};
    userDetails = $("#deleteUserId").val();
    $.ajax({
        type: 'Post',
        url: "/Admin/DeactivatedUser",
        dataType: 'Json',
        data:
        {
            userId: userDetails
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var url = "/Admin/AllUsers";
                newSuccessAlert(result.msg, url);

            } else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function reactivateUser(id) {
    debugger;
    $("#reactivateUserId").val(id);
}

function reactivatedUserId() {
    debugger;
    var userDetails = {};
    userDetails = $("#reactivateUserId").val();
    $.ajax({
        type: 'Post',
        url: "/Admin/ReactivateUser",
        dataType: 'Json',
        data:
        {
            userId: userDetails
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var url = "/Admin/AllUsers";
                newSuccessAlert(result.msg, url);

            } else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function SaveProfilePicture()
{
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var newImage = document.getElementById("newlogo").files;
    var userId = $("#userId").val();
    if (newImage != "") {
        const reader = new FileReader();
        reader.readAsDataURL(newImage[0]);
        var base64;
        reader.onload = function ()
        {
            base64 = reader.result;
            debugger;
            if (base64 != "")
            {
                $.ajax({
                    type: 'POST',
                    dataType: 'Json',
                    url: '/User/EditUserProfilePicture',
                    data:
                    {
                        base64: base64,
                        userId: userId
                    },
                    success: function (result)
                    {
                        debugger;
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        if (!result.isError)
                        {
                            successAlertWithRedirect(result.msg, result.url);
                        }
                        else
                        {
                            $('#loader').show();
                            $('#loader-wrapper').fadeOut(3000);
                            errorAlert(result.msg);
                        }
                    },
                    error: function (ex)
                    {
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        errorAlert("Error Occured,try again.");
                    }
                });
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert("Name & Address Field is Required.");
            }
        }
    }
}

function AddEvent() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.Title = $('#title').val();
    data.Note = $('#note').val();
    data.Date = $('#date').val();
    if (data.Title != "" && data.Note != "" && data.Date != "") {
        let eventsDetails = JSON.stringify(data);
        $.ajax({
            type: 'Post',
            url: '/Admin/AddEvent',
            dataType: 'Json',
            data:
            {
                eventViewModel: eventsDetails
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    debugger;
                    var url = "/User/AllEvent";
                    newSuccessAlert(result.msg, url);

                } else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            }
        });
    }
}

function editEvent(id) {
    debugger;
    $.ajax({
        type: 'GET',
        url: '/Admin/EditEvent',
        dataType: 'json',
        data:
        {
            eventId: id
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                $("#editedEventTitle").val(result.title);
                $("#eventId").val(result.id);
                $("#editedNote").val(result.note);
                $("#editedDate").val(result.date);
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function submitEditEvent() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var eventDetails = {};
    eventDetails.Id = $("#eventId").val();
    eventDetails.Note = $("#editedNote").val();
    eventDetails.Title = $("#editedEventTitle").val();
    eventDetails.Date = $("#editedDate").val();
    if (eventDetails.Date != "" && eventDetails.Title != "" && eventDetails.Note != "") {
        let eventViewModel = JSON.stringify(eventDetails);
        $.ajax({
            type: 'POST',
            url: '/Admin/EditEvent',
            dataType: 'json',
            data:
            {
                eventViewModel: eventViewModel,
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/User/AllEvent';
                    successAlertWithRedirect(result.msg, url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
}

function deleteEvent(id) {
    debugger;
    $("#deleteEventId").val(id);
}

function eventDelete() {
    debugger;
    var eventDetails = {};
    eventDetails = $("#deleteEventId").val();
    $.ajax({
        type: 'Post',
        url: "/Admin/DeleteEvent",
        dataType: 'Json',
        data:
        {
            eventId: eventDetails
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var url = "/User/AllEvent";
                newSuccessAlert(result.msg, url);

            } else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function uploadPicture() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var file = document.getElementById("uploadedPicture").files;
    var data = {};
    data.Title = $('#pictureTitle').val();
    data.Note = $('#pictureDescription').val();
    let photoDetails = JSON.stringify(data);
    debugger;
    if (file[0] != null) {
        const reader = new FileReader();
        reader.readAsDataURL(file[0]);
        var base64;
        reader.onload = function () {
            base64 = reader.result;
            debugger;  
            $.ajax({
                type: 'POST',
                dataType: 'Json',
                url: '/Admin/UploadPicture',
                data:
                {
                    fileUrl: base64,
                    galleryViewModel: photoDetails
                },
                success: function (result) {
                    debugger;
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    if (!result.isError) {
                        var url = '/Admin/UploadedPictures';
                        successAlertWithRedirect(result.msg, url);

                    } else
                    {
                        errorAlert(result.msg);
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                    }
                },
                error: function (ex) {
                    errorAlert("Error Occured,try again.");
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                }
            });
        }
    }
}

function UploadedPictureToBeDeleted(id) {
    debugger;
    $("#pictureId").val(id);
}

function deleteUploadedPicture() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var imageDetails = {};
    imageDetails = $("#pictureId").val();
    $.ajax({
        type: 'Post',
        url: "/Admin/DeleteUploadedPicture",
        dataType: 'Json',
        data:
        {
            photoId: imageDetails
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                debugger;
                var url = "/Admin/UploadedPictures";
                newSuccessAlert(result.msg, url);
            } else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function addHomePagePicture() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var file = document.getElementById("picture").files;
    var pictureType= $('#typeId').val();
    debugger;
    if (file[0] != null) {
        const reader = new FileReader();
        reader.readAsDataURL(file[0]);
        var base64;
        reader.onload = function () {
            base64 = reader.result;
            debugger;
            if (base64 != "" || base64 != 0 && pictureType != "" ) {
                $.ajax({
                    type: 'POST',
                    dataType: 'Json',
                    url: '/SuperAdmin/AddHomePagePicture',
                    data:
                    {
                        base64: base64,
                        pictureType: pictureType
                    },
                    success: function (result) {
                        debugger;
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        if (!result.isError) {
                            var url = '/SuperAdmin/HomepageImages';
                            successAlertWithRedirect(result.msg, url);
                        }
                        else
                        {
                            $('#loader').show();
                            $('#loader-wrapper').fadeOut(3000);
                            errorAlert(result.msg);
                        }
                    },
                    error: function (ex)
                    {
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        errorAlert("Error Occured,try again.");
                    }
                });
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert("Please Enter Details");
            }
        }
    }
}

function HomepagePictureToBeDeleted(id) {
    debugger;
    $("#deleteHompagePicture").val(id);
}

function DeleteHomepagePicture() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var homepageImageDetails = {};
    homepageImageDetails = $("#deleteHompagePicture").val();
    $.ajax({
        type: 'Post',
        url: "/SuperAdmin/DeleteHomePagePicture",
        dataType: 'Json',
        data:
        {
            homepageImageId: homepageImageDetails
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                debugger;
                var url = "/SuperAdmin/HomepageImages";
                newSuccessAlert(result.msg, url);
            } else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function HomepagePictureToBeEdited(imageId) {
    debugger;
    $.ajax({
        type: 'GET',
        url: '/SuperAdmin/EditHomepagePicture',
        dataType: 'JSON',
        data:
        {
            imageId: imageId
        },
        success: function (result)
        {
            debugger;
            if (!result.isError) {
                debugger;
                $.each(result.data.homepagePictures, function (name, dropdown) {
                    debugger;
                    $("#homePictureTypeId").append('<option value="' + dropdown.id + '">' + dropdown.name + '</option>');
                });

                /*$("#homePictureTypeId").val(result.data.homePageImage.name);*/
                $("#editHomePagePictureId").val(result.data.id);
                /*$("#editHomePagePicture").val(result.data.imageUrl);*/
            }
            else {
                errorAlert(data.msg);
            }
        }
    });
}

function editHomePagePicture() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var file = document.getElementById("editHomePagePicture").files;
    var homePagePictureDetails = {};
    homePagePictureDetails.Id = $("#editHomePagePictureId").val();
    homePagePictureDetails.HomePagePicture = $("#homePictureTypeId").val();
    debugger;
    if (file[0] != null) {
        const reader = new FileReader();
        reader.readAsDataURL(file[0]);
        var base64;
        reader.onload = function () {
            base64 = reader.result;
            debugger;
            if (homePagePictureDetails.ImageUrl != "") {
                debugger;
                let homepagePictures = JSON.stringify(homePagePictureDetails);
                $.ajax({
                    type: 'POST',
                    url: '/SuperAdmin/EditHomepagePicture',
                    dataType: 'json',
                    data:
                    {
                        base64: base64,
                        homePagePicutreViewModel: homepagePictures,
                    },
                    success: function (result) {
                        debugger;
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        if (!result.isError) {
                            var url = '/SuperAdmin/HomepageImages';
                            successAlertWithRedirect(result.msg, url);
                        }
                        else
                        {
                            $('#loader').show();
                            $('#loader-wrapper').fadeOut(3000);
                            errorAlert(result.msg);
                        }
                    },
                    error: function (ex)
                    {
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        errorAlert(ex);
                    }
                });
            }

        }
    }
}

function addDocument() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var file = document.getElementById("fileUrl").files;
    var data = {};
    data.DocumentList = $('#documentTypeId').val();
    data.Name = $('#documentName').val();
    debugger;
    if (file[0] != null) {
        const reader = new FileReader();
        reader.readAsDataURL(file[0]);
        var base64;
        reader.onload = function () {
            base64 = reader.result;
            debugger;
            if (base64 != "" || base64 != 0 && data.Name != "" && data.DocumentList != "") {
                let documentDetails = JSON.stringify(data);
                $.ajax({
                    type: 'Post',
                    dataType: 'Json',
                    url: '/Admin/AddDocument',
                    data:
                    {
                        base64: base64,
                        documentViewModel: documentDetails
                    },
                    success: function (result) {
                        debugger;
                        $('#loader').show();
                        $('#loader-wrapper').fadeOut(3000);
                        if (!result.isError) {
                            var url = '/User/Document';
                            successAlertWithRedirect(result.msg, url);
                        }
                        else
                        {
                            $('#loader').show();
                            $('#loader-wrapper').fadeOut(3000);
                            errorAlert(result.msg);
                        }
                    },
                });
            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert("Please Enter Details");
            }
        }
    }
}

function Downloadpdf(documentId) {
    debugger;
    $("#" + documentId + "").empty();
    $("#" + documentId + "").append('<i class="fa fa-spinner fa-spin" style="font-size:24px"></i> Downloading...');
    $.ajax({
        type: 'POST',
        dataType: 'Json',
        url: '/User/DownloadPdf',
        data:
        {
            documentId: documentId
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var link = document.createElement('a');
                link.href = result.data.fileUrl;
                link.download = result.data.name;
                link.dispatchEvent(new MouseEvent('click'));
                $("#" + documentId + "").empty();
                $("#" + documentId + "").append('<span><i class="fa fa-download fa-spin"></i></span> &nbsp; Download');
                successAlert(result.msg);
            }
            else
            {
                errorAlert(result.msg);
            }
        },
        Error: function (ex)
        {
            errorAlert(ex);
        }
    });
}

function documentDelete(documentId) {
    debugger;
    $("#deleteFileId").val(documentId);
}

function DeleteDocument() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data = $("#deleteFileId").val();
    $.ajax({
        type: 'Post',
        url: "/Admin/DeleteDocument",
        dataType: 'Json',
        data:
        {
            documentId: data
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                debugger;
                var url = "/User/Document";
                newSuccessAlert(result.msg, url);

            } else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function comment(id) {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.FullName = $('#name').val();
    data.Email = $('#email').val();
    data.Comment = $('#comment').val();
    debugger;
    if (data.Email != "" && data.FullName != "" && data.Comment != "") {
        let myComment = JSON.stringify(data);
        $.ajax({
            type: 'Post',
            dataType: 'Json',
            url: '/Blog/AddComment',
            data:
            {
                blogId: id,
                blogCommentViewModel: myComment
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/Blog/SingleBlog/' + result.data.blogId;
                    successAlertWithRedirect(result.msg, url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
        });
    }
    else
    {
        $('#loader').show();
        $('#loader-wrapper').fadeOut(3000);
        errorAlert("Please Enter Details");
    }
}

function subscribe() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.Email = $("#subscriptionEmail").val();
    if (data.Email != "") {
        let subscriberEmail = JSON.stringify(data);
        $.ajax({
            type: 'POST',
            datatype: 'Json',
            url: '/User/AddSubscribersEmail',
            data:
            {
                blogUpdate: subscriberEmail
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/Home/Index';
                    newSuccessAlert(result.msg, url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            Error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
}

function AddDues()
{
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.Name = $('#duesName').val();
    data.Amount = $('#duesAmount').val();
    if (data.Name != "" && data.Amount != "")
    {
        let duesDetails = JSON.stringify(data);
        $.ajax({
            type: 'Post',
            url: '/Admin/AddDues',
            dataType: 'Json',
            data:
            {
                duesViewModel: duesDetails
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    debugger;
                    var url = "/User/AllDues";
                    newSuccessAlert(result.msg, url);

                } else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            }
        });
    }
}

function editDues(editDuesId)
{
    debugger;
    $.ajax({
        type: 'GET',
        url: '/Admin/EditDues',
        dataType: 'json',
        data:
        {
            duesId: editDuesId
        },
        success: function (result) {
            debugger;
            if (!result.isError)
            {
                debugger;
                $("#editDuesName").val(result.name);
                $("#editDuesId").val(result.id);
                $("#editDuesAmount").val(result.amount);
            }
            else {
                errorAlert(result.msg);
            }
        }
    });
}

function editedDues()
{
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var duesDetails = {};
    duesDetails.Id = $("#editDuesId").val();
    duesDetails.Name = $("#editDuesName").val();
    duesDetails.Amount = $("#editDuesAmount").val();
    if (duesDetails.Name != "" && duesDetails.Amount != "")
    {
        let duesViewModel = JSON.stringify(duesDetails);
        $.ajax({
            type: 'POST',
            url: '/Admin/EditDues',
            dataType: 'json',
            data:
            {
                duesViewModel: duesViewModel,
            },
            success: function (result) {
                debugger;
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                if (!result.isError) {
                    var url = '/User/AllDues';
                    successAlertWithRedirect(result.msg, url);
                }
                else
                {
                    $('#loader').show();
                    $('#loader-wrapper').fadeOut(3000);
                    errorAlert(result.msg);
                }
            },
            error: function (ex)
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(ex);
            }
        });
    }
}

function deleteDues(id) {
    debugger;
    $("#deleteDuesId").val(id);
} 

function DeletedDues() {
    debugger;
    var duesDetails = {};
    duesDetails = $("#deleteDuesId").val();
    $.ajax({
        type: 'Post',
        url: "/Admin/DeleteDues",
        dataType: 'Json',
        data:
        {
            duesId: duesDetails
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var url = "/User/AllDues";
                newSuccessAlert(result.msg, url);

            }
            else
            {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

$('.myFirst_display').change(function ()
{
    debugger;
    var FirstName = $('#firstname').val();
    var LastName = $('#lastName').val();
    var GenderId = $('#genderId').val();
    var Email = $('#email').val();
    var Password = $('#password').val();
    var ConfirmPassword = $('#confirmpassword').val();
    var DateOfBirth = $('#dateOfBirth').val();
    var PhoneNumber = $('#phoneNumber').val();
    if (FirstName != "" && LastName != "" && GenderId != "" && Email != "" && Password != "" && ConfirmPassword != "" && DateOfBirth != "" && PhoneNumber != "")
    {
        $("#firstButtton").removeClass('disabled');
    }
    else
    {
        $("#firstButtton").hasClass('disabled');
    }
});

function SortBlog() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var SearchBlog = {};
    SearchBlog.Name = $('#authorsName').val();
    $.ajax({
        type: 'POST',
        dataType: 'Json',
        url: '/Admin/FilterBlog',
        data:
        {
            sortingModel: SearchBlog
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                if (result.data != null) {
                    $.each(result.data, function (i, comp) {
                        debugger;
                    })
                }
            }
            else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                $("#loader").hide();
                var url = "/Admin/Index";
                newErrorAlert("Blog Author Not Found,try again.", url);
            }
        }
    });
}

function addSchool() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.SchoolName = $('#schoolName').val();
    data.SchoolCodeName = $('#schoolCodeName').val();
    data.StateId = $('#stateId').val();
    data.Address = $('#schoolAddress').val();
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastName').val();
    data.MiddleName = $('#middleName').val();
    data.Email = $('#email').val();
    data.SchoolAdinPhoneNumber = $('#phoneNumber').val();
    data.GenderId = $('#genderId').val();
    data.DateOfBirth = $('#dateOfBirth').val();
    let schoolDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/SuperAdmin/AddSchool',
        dataType: 'Json',
        data:
        {
            schoolViewModel: schoolDetails
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                debugger;
                var url = "/SuperAdmin/RegisteredSchools";
                newSuccessAlert(result.msg, url);

            } else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function editSchoolDetails(schoolId) {
    debugger;
    $.ajax({
        type: 'GET',
        url: '/SuperAdmin/EditRegisteredSchool',
        dataType: 'json',
        data:
        {
            schoolId: schoolId
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                $("#editSchoolName").val(result.schoolName);
                $("#editSchoolCodeName").val(result.schoolCodeName);
                $("#editStateId").val(result.stateId);
                $("#editAddress").val(result.address);
            }
            else {
                errorAlert(result.msg);
            }
        }
    });
}

function editedSchool() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var schoolDetails = {};
    schoolDetails.Id = $("#schoolId").val();
    schoolDetails.SchoolName = $("#editSchoolName").val();
    schoolDetails.SchoolCodeName = $("#editSchoolCodeName").val();
    schoolDetails.StateId = $("#editStateId").val();
    schoolDetails.Address = $("#editAddress").val();
    schoolDetails.SchoolAdminId = $("#editSchoolAdminId").val();
    let schoolViewModel = JSON.stringify(schoolDetails);
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/EditRegisteredSchool',
        dataType: 'json',
        data:
        {
            schoolViewModel: schoolViewModel,
        },
        success: function (result) {
            debugger;
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!result.isError) {
                var url = '/SuperAdmin/AllRegisteredSchool';
                successAlertWithRedirect(result.msg, url);
            }
            else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            errorAlert(ex);
        }
    });
}

function deleteSchool(id) {
    debugger;
    $("#deleteSchoolId").val(id);
}

function DeletedSchool() {
    debugger;
    var schoolDetails = {};
    duesDetails = $("#deleteSchoolId").val();
    $.ajax({
        type: 'Post',
        url: "/SuperAdmin/DeleteSchool",
        dataType: 'Json',
        data:
        {
            schoolId: schoolDetails
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var url = "/SuperAdmin/AllRegisteredSchool";
                newSuccessAlert(result.msg, url);

            }
            else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function makeDonation() {
    debugger;
    $('#loader').show();
    $('#loader-wrapper').show();
    var data = {};
    data.CategoryId = $('#categoryId').val();
    data.FullName = $('#fullName').val();
    data.StateId = $('#stateId').val();
    data.CurrencyId = $('#currencyId').val();
    data.Email = $('#email').val();
    data.Amount = $('#amount').val();
    let paymentDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/User/MakePayment',
        dataType: 'Json',
        data:
        {
            paymentDetails: paymentDetails
        },
        success: function (response) {
            $('#loader').show();
            $('#loader-wrapper').fadeOut(3000);
            if (!response.isError) {
                debugger;
                Swal.fire
                    ({
                        title: "Success",
                        text: "You're good to go,Let's make the payment now",
                        icon: "success",
                        timer: "30000",
                        overlay: "background - color: rgba(43, 165, 137, 0.45)",
                    })
                    .then(function () {
                        location.href = response;
                    })
            }
            else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

function setupManual() {
    debugger;
    var data = {};
    data.Name = $('#topic').val();
    data.BibleVerse = $('#bibleText').val();
    data.SpeakerId = $('#speakerId').val();
    data.CordinatorId = $('#cordinatorId').val();
    data.Date = $('#date').val();
    let manualDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/Admin/AddManual',
        dataType: 'Json',
        data: {
            manualDetails: manualDetails
        },
        success: function (result) {
            debugger;
            if (!result.isError) {
                debugger;
                var url = "/User/SemesterManual";
                newSuccessAlert(result.msg, url);
            }
            else {
                $('#loader').show();
                $('#loader-wrapper').fadeOut(3000);
                errorAlert(result.msg);
            }
        }
    });
}

$(function () {
    $("#genderId").select2();
    $("#stateId").select2();
    $("#cordinatorId").select2();
    $("#speakerId").select2();
});

$('.blogDisplay').change(function () {
    debugger;
    var blogCategory = $('#blogCategoryId').val();
    var title = $('#title').val();
    var aboutMe = $('#aboutMe').val();
    if (blogCategory != "" && title != "" && aboutMe != "") {
        $('#primaryInfo').prop("disabled", false);
    }
    else {
        $('#primaryInfo').prop("disabled", true);
    }
});

function hidePrimaryInfo() {
    debugger;
    $('#blogMainContent').show();
    $('#primaryInfo').hide();
}

function showPrimaryInfo() {
    debugger;
    $('#blogMainContent').hide();
    $('#primaryInfo').show();
}