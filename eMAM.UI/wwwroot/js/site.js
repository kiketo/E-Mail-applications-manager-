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
//revert-invalid to not reviewed

$('.revert-not-reviewed-button').click(function (ev) {
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
            location.reload();
            var status = $.find(".status-" + messageId);
            status[0].innerHTML = "Invalid Application";
            toastr.success("Mail Status Reverted");
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
});





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
    debugger;
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
            $(messageId).find('.mail-bodyDB').html(response);
            toastr.success("Application is Open");
        },
        error: function (res) {
            toastr.error("Ups, something went wrong");
        }
    });
    debugger;
});

//submit and approve form
$('.close-application-aprove').submit(function (ev) {
    // ev.preventDefault;
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var data = $(this).serialize() + "&__RequestVerificationToken=" + token;
   
    
    debugger;
    $.ajax({
        type: "POST",
        url: "/home/submitncloseapplicationaproved",
        data: {
            __RequestVerificationToken : token,
            data: data
        },
        dataType: "json",
        success: function (response) {
            toastr.success("Application was Aproved");
            //change the status in the DOM
            var status = $.find(".open-status-" + messageId);
            status[0].innerHTML = "Aproved";
            location.reload();
        },
        error: function (res) {
            toastr.error(res.responseText);
        }
    })
    debugger;
});

//revert back to not reviewed
$('.notreviewed-button').click(function (ev) {
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
            GmailIdNumber: messageId
        },
        success: function () {
            //change the status in the DOM
            var status = $.find(".open-status-" + messageId);
            status[0].innerHTML = "Not Reviewed";
            location.reload();

            toastr.warning("Application Reverted To Not Reviewed");
            
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
    debugger;
});


//reject application
$('.rejected-button').click(function (ev) {
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
            GmailIdNumber: messageId
        },
        success: function () {
            toastr.warning("Application Rejected");
            //change the status in the DOM
            var status = $.find(".open-status-" + messageId);
            status[0].innerHTML = "Rejected";
            location.reload();
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
    debugger;
});

