//-----------------------------------------------------
//-----------------------------------------------------
//--PREVIEW MODAL--------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------

//show body of email in preview
function previewMailButton(button) {
    var $this = $(button);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = $this.attr('data-url');
    debugger;
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
            debugger;
        },
        error: function () {
            toastr.error("Ups, something went wrong");
            debugger;
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
    console.log(parent);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

    var row = $('.row-' + messageId)
    var rowShouldBeEdited = row.attr('data-rowShouldEdit');

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
            //edit or delete the row in the DOM
            if (rowShouldBeEdited == "True") {
                $('.row-' + messageId).html(data);
                debugger;
            } else {
                $('.row-' + messageId).remove();
                debugger;
            }
        },
        error: function (err) {
            toastr.error(err.responseText);
        }
    });
};

//not valid e-mail
function invalidateEmail(button) {
    var $this = $(button);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');

    var row = $('.row-' + messageId)
    var rowShouldBeEdited = row.attr('data-rowShouldEdit');

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
            //edit or delete the row in the DOM
            if (rowShouldBeEdited == "True") {
                $('.row-' + messageId).html(data);
                debugger;
            } else {
                $('.row-' + messageId).remove();
                debugger;
            }
        },
        error: function (res, as, okijjg) {
            toastr.error(res.responseText);
        }
    });
};

//back to not reviewed e-mail status
function backToNotPreviewed(button) {
    debugger;
    var $this = $(button);
    var messageId = $this.attr('data-target');
    var status = $this.attr('data-status');
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var row = $('.row-' + messageId)
    var rowShouldBeEdited = row.attr('data-rowShouldEdit');

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
                //edit or delete the row in the DOM
                if (rowShouldBeEdited == "True") {
                    $('.row-' + messageId).html(data);
                    debugger;
                } else {
                    $('.row-' + messageId).remove();
                    debugger;
                }
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
//--PROCESS MODAL--------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------
//-----------------------------------------------------


//open email new->open
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
        if (status == "Approved") {
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
        } else {
            //read the body from gmail
            $.ajax({
                url: "/home/previewmail",
                type: 'POST',
                data: {
                    __RequestVerificationToken: token,
                    messageId: messageRequestData
                },
                success: function (response) {
                    $(messageId).find('.mail-bodyDB').html(response);
                    debugger;
                },
                error: function () {
                    toastr.error("Ups, something went wrong");
                    debugger;
                }
            });
        }
    }
};

//submit and approve form
function approveApplicationButton(button) {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var gmail = $(button).attr('data-gmailIdNumber');
    var egn = $(button).parent().find('#egnid-' + gmail)[0].value;
    var number = $(button).parent().find('#phoneid-' + gmail)[0].value;

    var row = $('.row-' + gmail)
    var rowShouldBeEdited = row.attr('data-rowShouldEdit');

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
            toastr.success("Application was Aproved");
            //edit or delete the row in the DOM
            if (rowShouldBeEdited == "True") {
                $('.row-' + gmail).html(response);
                debugger;
            } else {
                $('.row-' + gmail).remove();
                debugger;
            }
            debugger;
        },
        error: function (res) {
            toastr.error("Please enter valid details");
            debugger;
        }
    })
};

//reject application
function rejectApplicationButton(button) {
    var $this = $(button);
    var messageId = $this.attr('data-messageId');
    var url = $this.attr('data-url');
    //var gmail = $(button).attr('data-messageId');
    var form = $('#__AjaxAntiForgeryForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    var row = $('.row-' + messageId)
    var rowShouldBeEdited = row.attr('data-rowShouldEdit');

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
            //edit or delete the row in the DOM
            if (rowShouldBeEdited == "True") {
                $('.row-' + messageId).html(response);
                debugger;
            } else {
                $('.row-' + messageId).remove();
                debugger;
            }
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

    var row = $('.row-' + messageId)
    var rowShouldBeEdited = row.attr('data-rowShouldEdit');

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
            //edit or delete the row in the DOM
            if (rowShouldBeEdited == "True") {
                $('.row-' + messageId).html(data);
                debugger;
            } else {
                $('.row-' + messageId).remove();
                debugger;
            }
        },
        error: function (err) {
            toastr.error(err.responseText);
        }
    });
};







