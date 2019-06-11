﻿//show body of email in preview
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
//validate e-mail
$('.validation-button').click(function (ev) {
    var $this = $(this);
    //debugger;
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

    //$.post(url, { messageId: messageId }, function (res) {
    //    console.log('?')
    //    toastr.success("Mail Validated!");
    //})

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