$('.email-button').click(function (ev) {
    var $this = $(this);

    var messageId = $this.attr('data-target');
    messageId = messageId.replace("#mails-", "");
    var url = $this.attr('data-url');

    //console.log(messageId);
    //console.log(url);

    $.post(url, { messageId: messageId }, function (response) {
        console.log(response);
        $('#bodyMail').append(response);
        debugger;
    });
});