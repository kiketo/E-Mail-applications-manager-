//show body of email in preview
$('.email-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = $this.attr('data-url');


    $.post(url, { messageId: messageRequestData }, function (response) {
        $(messageId).find('.mail-body').html(response);
    });
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

    $.ajax({
        type: "POST",
        url: url,
        data: { messageId: messageId },
        success: function (res, as, okijjg) {
            toastr.success("Mail Validated");
        },
        error: function (res, as, okijjg) {
            toastr.error(err.responseText);
        }
    });
});

//nto valid e-mail
$('.notValid-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

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


//get modal open/inworkingprogress
$('.applicationEmail').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = '/home/getbodydb';
    $.ajax({
        type: "GET",
        url: url,
        data: { messageId: messageRequestData },
        success: function (response) {
            $(messageId).find('.mail-bodyDB').html(response)
        }
    })

});



//rq for form validation
$('.openApplicationForm').validate({
    debug: true,
    rules: {
        FirstName: {
            required: true,
            minlength: 2
        },
        LastName: {
            required: true,
            minlength: 2
        },
        CustomerEGN: {
            required: true
        },
        CustomerPhoneNumber: {
            required: false,
            minlength: 10,
            number: true
        },
        Emails: {
            required: true
            
        },
    },
    messages: {
        FirstName: {
            required: "First name is required",
            minlength: jQuery.validator.format("At least {2} characters required!")
        },
        LastName: {
            required: "Last name is required",
            minlength: jQuery.validator.format("At least {2} characters required!")
        },
        CustomerEGN: {
            required: "Identification number is required e.g EGN, Passport Number"
        },
        CustomerPhoneNumber: {
            required: false,
            minlength: jQuery.validator.format("Enter valid pone number"),
            number: "Only digits are allowed"
        },
        Emails: {
            required: true

        },
    },
    onkeyup: false,
    onblur: true,
    focusCleanup: true
    
});
