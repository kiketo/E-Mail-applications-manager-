$('.email-button').click(function (ev) {
    var currentLink = $(ev.target);
    console.log('catch event');

    var gmailId = currentLink.data('target');//remove "#mails-"

});