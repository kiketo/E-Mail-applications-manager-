$('.email-button').click(function (ev) {
    var $this = $(this);
    var messageId = $this.attr('data-target');
    var messageRequestData = messageId.replace("#mails-", "");
    var url = $this.attr('data-url');
    $.post(url, { messageId: messageRequestData }, function (response) {
        $(messageId).find('.mail-body').html(response);
    });
});
$('.emailButton')
    .ajaxStop(function () {
        $('#loading-indicator').hide();
    });

$('.validation-button').click(function (ev) {
    var $this = $(this);
    debugger;
    var messageId = $('.email-button').attr('data-target');
    debugger;
    console.log(messageId);
});
