﻿//show body of email in preview
function previewMailButton(button) {
    var $this = $(button);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = $this.attr('data-url');

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
            $('.mail-body-' + messageRequestData).html(response);
        },
        error: function () {
            toastr.error("Ups, something went wrong");
        }
    });
};

//loading indicator while requesting body mail for preview
$('.emailButton')
    .ajaxStop(function () {
        $('#loading-indicator').hide();
    });

//validate e-mail & close
function validateEmail(button) {
    var $this = $(button);
    debugger;
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: url,
        dataType: "html",
        cache: true,
        data: {
            __RequestVerificationToken: token,
            messageId: messageId
        },
        success: function (data) {
            toastr.success("Mail Validated");
            //change the status in the DOM
            $('.row-' + messageId).html(data);
        },
        error: function (err) {
            toastr.error(err.responseText);
        }
    });
};

//not valid e-mail
//$('.notValid-button').click(
function invalidateEmail(button) {
    var $this = $(button);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: url,
        dataType: "html",
        data: {
            __RequestVerificationToken: token,
            messageId: messageId
        },
        success: function (data) {
            toastr.warning("Mail Not Valid");
            //change the status in the DOM
            $('.row-' + messageId).html(data);
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
};

//back to not reviewed e-mail status
//$('.close-button').click(
function backToNotPreviewed(button) {
    debugger;
    var $this = $(button);
    var messageId = $this.attr('data-target');
    var status = $this.attr('data-status');
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    if (status == 'Invalid Application') {
        $.ajax({
            type: "POST",
            dataType: "html",
            url: "/home/notpreviewed",
            data: {
                __RequestVerificationToken: token,
                id: messageId
            },
            success: function (data) {
                toastr.warning("Mail back to Not Reviewed status");
                //change the status in the DOM
                $('.row-' + messageId).html(data);//TODO
            },
            error: function (res) {
                toastr.error(res.responseText);
            }
        });
    } else {
        toastr.warning("Mail Not Reviewed");
    }
};

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

//toggle roles User<->Operator
function userOperatorToggle(button) {
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
        url: "manager/ChangeUserRole",
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


//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------


//open email new->open
//$('.process-button').click(
function processMailButton(button) {
    debugger;
    var $this = $(button);
    console.log($this);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = '/home/getbodydb';
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    var status = $this.attr('data-status');

    if (status == "New") {
        var url1 = '/home/changestatustoopen';

        //read the body from DB
        $.ajax({
            type: "POST",
            url: url,
            data: {
                __RequestVerificationToken: token,
                messageId: messageRequestData
            },
            success: function (response) {
                debugger;
                //render the body
                $(messageId).find('.mail-bodyDB').html(response);
                //change status to open
                $.ajax({
                    type: "POST",
                    url: url1,
                    dataType: "html",
                    data: {
                        __RequestVerificationToken: token,
                        messageId: messageRequestData
                    },
                    success: function (data) {
                        data1 = data;
                        debugger;
                        //change the status in the DOM
                        var status = $.find('.status-' + messageRequestData);
                        status[0].innerHTML = "Open";
                    },
                    error: function (res) {
                        toastr.error(res.responseText);
                    }
                });
                toastr.success("Mail Opened");
                debugger;
            },
            error: function () {
                toastr.error("Ups, e-mail didn't load");
            }
        });
    } else {
        //read the body from DB
        $.ajax({
            type: "POST",
            url: url,
            data: {
                __RequestVerificationToken: token,
                messageId: messageRequestData
            },
            success: function (response) {
                debugger;
                //render the body
                $(messageId).find('.mail-bodyDB').html(response);
            },
            error: function () {
                toastr.error("Ups, e-mail didn't load");
                debugger;
            }
        });
    }
};

//submit and approve form
//$('.close-application-aprove').submit(
function approveApplicationButton(button) {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var gmail = $(button).attr('data-gmailIdNumber');
    var egn = $(button).parent().find('#egnid-' + gmail)[0].value;
    var number = $(button).parent().find('#phoneid-' + gmail)[0].value;
    debugger;
    $.ajax({
        type: "POST",
        url: "/home/submitncloseapplicationaproved",
        data: {
            __RequestVerificationToken: token,
            GmailIdNumber: gmail,
            CustomerEGN: egn,
            CustomerPhoneNumber: number
        },
        dataType: "html",
        success: function (response) {
            //change the status in the DOM
            toastr.success("Application was Aproved");
            $('.row-' + gmail).html(response);
            debugger;
        },
        error: function (res) {
            toastr.error(res.responseText);
            debugger;
        }
    })
};

//reject application
//$('.rejected-button').click(
function rejectApplicationButton(button) {
    var $this = $(button);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');
    var gmail = $(button).attr('data-messageId');
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: url,
        data: {
            __RequestVerificationToken: token,
            GmailIdNumber: messageId
        },
        dataType: "html",
        success: function (response) {
            toastr.warning("Application Rejected");
            //change the status in the DOM
            $('.row-' + gmail).html(response);
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
    debugger;
};

//back to new from closed status
function backToNewApplicationButton(button) {
    var $this = $(button);
    debugger;
    var messageId = $this.attr('data-messageId');
    var url = "/home/backtonewstatus";

    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    /////////////////////

    var url1 = "/home/validatemail";

    //download e-mail body to DB
    $.ajax({
        type: "POST",
        url: url1,
        //dataType: "html",
        //cache: true,
        data: {
            __RequestVerificationToken: token,
            messageId: messageId
        },
        success: function (data) {
            //toastr.success("Mail Body in DB");
            //change the status in the DOM
           // $('.row-' + messageId).html(data);
        },
        error: function (err) {
            toastr.error(err.responseText);
        }
    });

    /////////////////


    $.ajax({
        type: "POST",
        url: url,
        dataType: "html",
        //cache: true,
        data: {
            __RequestVerificationToken: token,
            id: messageId
        },
        success: function (data) {
            toastr.success("Mail back to New");
            //change the status in the DOM
            $('.row-' + messageId).html(data);
        },
        error: function (err) {
            toastr.error(err.responseText);
        }
    });
};








////preview open aplication
//$('.preview-open-manager-button').click(function (ev) {
//    var $this = $(this);
//    var messageId = $this.attr('data-messageId'); //??
//    var url = '/home/getbodydb';
//    //disable modal from closing when clicked outside the modal window?does not work?
//    $(messageId).modal({
//        backdrop: 'static',
//        keyboard: false
//    })
//    var form = $('#__AjaxAntiForgeryForm');
//    var token = $('input[name="__RequestVerificationToken"]', form).val();



//    $.ajax({
//        type: "POST",
//        url: url,
//        data: {
//            __RequestVerificationToken: token,
//            messageId: messageRequestData
//        },
//        success: function (response) {
//            $(messageId).find('.mail-bodyDB').html(response);
//            toastr.success("Preview");

//        },
//        error: function (res) {
//            toastr.error(res.responseText);
//        }
//    });
//    debugger;
//});