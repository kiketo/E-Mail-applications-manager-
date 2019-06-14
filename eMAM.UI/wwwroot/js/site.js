//show body of email in preview
$('.email-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = $this.attr('data-url');

    //disable modal from closing when clicked outside the modal window
    //$(messageId).modal({
    //    backdrop: 'static',
    //    keyboard: false
    //})

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            __RequestVerificationToken: token,
            messageId: messageRequestData
        },
        success: function (response) {
            $(messageId).find('.mail-body').html(response);
        },
        error: function (res) {
            toastr.error("Ups, something went wrong");
        }
    });

    //$.post(url, { messageId: messageRequestData }, function (response) {
    //    $(messageId).find('.mail-body').html(response);
    //});
    //ev.stopPropagation();
});

//loading indicator while requesting body mail for preview
$('.emailButton')
    .ajaxStop(function () {
        $('#loading-indicator').hide();
    });

//validate e-mail & close
$('.validation-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            messageId: messageId
        },
        success: function () {
            toastr.success("Mail Validated");
            var status = $.find(".status-" + messageId);
            status[0].innerHTML = "New";
            //remove the preview button if not a Manager

            //var selector = '.email-button[data-target="#mails-' + messageId + '"]';
            //previewButton[0].childNodes[1].outerHTML = "";
            //console.log(previewButton);
            //var selector = '.email-button[data-target="#mails-' + messageId + '"]';
            var previewButton = $('.email-button[data-target="#mails-' + messageId + '"]');
            var isManager = previewButton.data('isManager');
            if (!isManager) {
                previewButton.remove();
            }
            //todo refresh data properly
            location.reload();
            //previewButton[0].childNodes[1].outerHTML = "";
            console.log(previewButton);
        },
        error: function (err) {
            toastr.error(err.responseText);
        }
    });
    //change the status in the DOM


});

//not valid e-mail
$('.notValid-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            messageId: messageId
        },
        success: function (res, as, okijjg) {
            toastr.warning("Mail Not Valid");
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
    //change the status in the DOM
    var status = $.find(".status-" + messageId);
    status[0].innerHTML = "Invalid Application";
    //remove the preview button if not a Manager

    //var selector = '.email-button[data-target="#mails-' + messageId + '"]';
    var previewButton = $('.email-button[data-target="#mails-' + messageId + '"]');
    var isManager = previewButton.data('isManager');
    if (!isManager) {
        previewButton.remove();
    }
    //previewButton[0].childNodes[1].outerHTML = "";
    // console.log(previewButton);
});

//back to not previewed e-mail
$('.close-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-target');

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: "/home/notpreviewed",
        data: {
            __RequestVerificationToken: token,
            id: messageId
        },
        success: function (res, as, okijjg) {
            toastr.warning("Mail Not Previewed");
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
});

//toggle roles User<->Manager
function userManagerToggle(button) {
    var clickedButton = $(button);
    var userId = clickedButton.attr('data-userId');
    var clickedButtonClass = clickedButton.attr('class');
    var otherButtonClass = ".btn.btn-success.disabled." + userId;
    var otherButton = $(otherButtonClass);
    var otherButtonClass = otherButton.attr('class');
    clickedButton.removeClass(clickedButtonClass).addClass(otherButtonClass);
    otherButton.removeClass(otherButtonClass).addClass(clickedButtonClass);
    var onClickEvent = clickedButton.attr('onclick');
    otherButton.attr('onclick', onClickEvent);
    console.log(otherButton.attr('onclick'));
    clickedButton.removeAttr(onClickEvent);

    var userId = clickedButton.attr('data-userId');

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    $.ajax({
        type: "POST",
        url: "superadmin/ChangeUserRole",
        data: {
            __RequestVerificationToken: token,
            userId: userId
        },
        success: function () {
            toastr.success("Role changed");
        },
        error: function (res) {
            toastr.error("Ups, something went wrong ;-)");
        }
    });

}

//$('.btn-outline-success').click(function (ev) {
//    console.log(ev.target);
//    debugger;
//    var clickedButton = $(this);
//    var userId = clickedButton.attr('data-userId');

//    //var form = $('#__AjaxAntiForgeryForm');
//    //var token = $('input[name="__RequestVerificationToken"]', form).val();
//    //$.ajax({
//    //    type: "POST",
//    //    url: "superadmin/ChangeUserRole",
//    //    data: {
//    //        __RequestVerificationToken: token,
//    //        userId: userId
//    //    },
//    //    success: function () {
//    //        toastr.warning("Role changed");
//    //    },
//    //    error: function (res) {
//    //        toastr.error(res.responseText);
//    //    }
//    //});
//    //$(document).ready(function () {
//    //console.log(clickedButton);

//    //change the type of buttons
//   // debugger;
//    var clickedButtonClass = clickedButton.attr('class');
//    var otherButtonClass = ".btn.btn-success.disabled." + userId;
//    var otherButton = $(otherButtonClass);
//    var otherButtonClass = otherButton.attr('class');
//    clickedButton.removeClass(clickedButtonClass).addClass(otherButtonClass);
//    otherButton.removeClass(otherButtonClass).addClass(clickedButtonClass);

//});

//open email new->open
$('.applicationEmail').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = '/home/getbodydb';
    //disable modal from closing when clicked outside the modal window?does not work?
    $(messageId).modal({
        backdrop: 'static',
        keyboard: false
    })
    debugger;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            messageId: messageRequestData
        },
        success: function (response) {
            $(messageId).find('.mail-bodyDB').html(response)
        },
        error: function (res) {
            toastr.error("Ups, something went wrong");
        }
    });

});


$('.close-application').click(function (ev) {
    $.ajax({
        type: "POST",
        url: "/home/submitncloseapplication",
        data: {
            GmailIdNumber: $("#gmailid").val(),
            CustomerEGN: $("#egn").val(),
            CustomerPhoneNumber: $("#phone").val()
        },
        dataType: "json"

    })
        .done(function (res, as, okijjg) {
            toastr.success("Application was successfully closed!");
            location.pathname("/home/index");
        })
        .fail(function (jqxhr, status, error) {
            console.log("Something went wrong")
        })
});



//show body of email in open
//$('.applicationEmail').click(function (ev) {
//    var $this = $(this);
//    var messageId = $this.attr('data-target').replace("#mails-", "");
//    var url = $this.attr('data-url');

//    $.post(url, { messageId: messageId }, function (response) {
//        $(messageId).find('.mail-bodyDB').html(response);
//    });

//    $.ajax({
//        type: "GET",
//        url: url,
//        data: { messageId: messageRequestData },
//        success: $.post(url, { emailId: messageRequestData }, function (response) {
//            $(emailId).find('.mail-bodyDB').html(response)
//        })
//    });
//});

//rq for form validation
//$('.openApplicationForm').validate({
//    debug: true,
//    rules: {
//        FirstName: {
//            required: true,
//            minlength: 2
//        },
//        LastName: {
//            required: true,
//            minlength: 2
//        },
//        CustomerEGN: {
//            required: true
//        },
//        CustomerPhoneNumber: {
//            required: false,
//            minlength: 10,
//            number: true
//        },
//        Emails: {
//            required: true

//        },
//    },
//    messages: {
//        FirstName: {
//            required: "First name is required",
//        },
//        LastName: {
//            required: "Last name is required",
//        },
//        CustomerEGN: {
//            required: "Identification number is required e.g EGN, Passport Number"
//        },
//        CustomerPhoneNumber: {
//            required: false,
//            number: "Only digits are allowed"
//        },
//        Emails: {
//            required: true

//        },
//    },
//    onkeyup: false,
//    onblur: true,
//    focusCleanup: true

//});