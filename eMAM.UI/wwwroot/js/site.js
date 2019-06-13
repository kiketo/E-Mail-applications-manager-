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
        success: function (res, as, okijjg) {
            toastr.success("Mail Validated");
        },
        error: function (res, as, okijjg) {
            toastr.error(err.responseText);
        }
    });
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


    $.ajax({
        type: "POST",
        url: url,
        data: { messageId: messageId },
        success: function (res, as, okijjg) {
            toastr.warning("Mail Not Valid");
        },
        error: function (res, as, okijjg) {
            toastr.error(err.responseText);
        }
    });


});

//open email new->open
$('.applicationEmail').click(function (ev) {
    ev.stopPropagation();
    //ev.preventDefault();
    // ev.stopImmediatePropagation();
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
    ev.preventDefault;
    debugger;
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
        })
        .fail(function (jqxhr, status, error) {
            console.log("Something went wrong")
        })
});

$('.close-button-open').click(function (ev) {
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