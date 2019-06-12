//show body of email in preview
$('.email-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = $this.attr('data-url');

    //disable modal from closing when clicked outside the modal window
    $(messageId).modal({
        backdrop: 'static',
        keyboard: false
    })

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

$('.applicationEmail').click(function (ev) {
    //ev.stopPropagation();
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






//class="applicationEmail" data-target="#mails-@mail.GmailIdNumber" data-url="/home/openapplication"
//<div class="mail-bodyDB" style="width: 100%; height:300px; overflow-y:scroll;">