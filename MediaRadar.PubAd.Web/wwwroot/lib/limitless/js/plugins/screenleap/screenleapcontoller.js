var isSharing = false;
var accountId = 'CondeNast';
var authToken = 'iwWJESdpsY';
var isMobile = false;

window.onload = function () {
    screenleap.onScreenShareStart = function () {
        screenShareStartedUIChange();
    };
    screenleap.onPresenterConnect = function () {
        showUserMessage('The presenter app has connected to the app server.');
    };
    screenleap.onViewerConnect = function (participantId, externalId) {
        showUserMessage('Conde Nast support team is now connected.');
    };
    screenleap.onViewerDisconnect = function (participantId, externalId) {
        showUserMessage('Conde Nast support team is now disconnected.');
    };
    screenleap.onScreenShareEnd = function (message) {
        isSharing = false;
        if (message && message == 'CONFLICT') {
            showErrorMessage('Another screen share is already running.');
        } else {
            showUserMessage('Screen share ended' + (message ? ' (' + message + ')' : '') + '.');
        }

        Cookies.set('screenShareCode', '');
        screenShareStoppedUIChange();
    };
    screenleap.error = function (action, errorMessage, xhr) {
        var msg = action + ': ' + errorMessage;
        if (xhr) {
            msg += ' (' + xhr.status + ')';
        }
        showErrorMessage('Error in ' + msg);
    };
};


$(document).ready(function () {
    if (window.location.href.indexOf('referer=/install-api-extension') != -1) {
        startScreenShare();
    }
    else if (Cookies.get('screenShareCode') && Cookies.get('screenShareCode') != '') {
        $.ajax({
            url: 'https://api.screenleap.com/v2/screen-shares/' + Cookies.get('screenShareCode'),
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('accountId', accountId);
                xhr.setRequestHeader('authtoken', authToken);
            },
            success: function (screenShareData) {
                if (screenShareData && screenShareData.endTime == undefined) {
                    screenShareStartedUIChange();
                }
                else {
                    $("#btnSharingContainer").show();
                }
            }
        });
    }
    else {
        $("#btnSharingContainer").show();
    }

    $(".stopshare").click(function () {
        stopSharing();
    });
    $(".startshare").click(function () {
        screenleap.checkIsExtensionEnabled(function () {
            startScreenShare();
        }, function () {
            screenleap.checkIsExtensionInstalled(function () {
                showErrorMessage("Screenleap extension is not enabled!");
            }, function () {
                window.location.href = 'https://api.screenleap.com/install-api-extension?redirectUrl=' + window.location.href;
            });
        });
        return false;
    });
});

function screenShareStoppedUIChange() {
    isSharing = false;
    $('#btnSharingContainer').show();
    $('#beingSharedContainer').hide();
    $("#screensharecode").text("");

}
function screenShareStartedUIChange() {
    isSharing = true;
    $('#beingSharedContainer').show();
    $('#btnSharingContainer').hide();
    $("#screensharecode").text(Cookies.get('screenShareCode'));
}
function createScreenShareCallback(screenShareData) {
    Cookies.set('screenShareCode', screenShareData['screenShareCode']);
    showUserMessage('Starting screen share with support team.');
    screenleap.startSharing(
        "IN_BROWSER",
        screenShareData,
        {
            screenShareStarting: onScreenShareStarting,
            appConnectionFailed: onAppConnectionFailed,
            screenShareStartError: onScreenShareStartError
        });
}
function createScreenShare(alreadyDownloaded, successCallback) {
    showUserMessage('Requesting a new screen share code.');
    $.ajax({
        url: 'https://api.screenleap.com/v2/screen-shares?presenterAppType=presenterCountryCode=US&IN_BROWSER&useV2=true&title=ePayables',
        type: 'POST',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('accountId', accountId);
            xhr.setRequestHeader('authtoken', authToken);
        },
        success: function (screenShareData) {
            if (typeof screenShareData == 'string') {
                screenShareData = JSON.parse(screenShareData);
            }
            successCallback(screenShareData, alreadyDownloaded);
        },
        global: false, // do not fire the global error handling
        error: function (xhr) {
            var errorJson = null;
            if (parseInt(xhr.status / 100) != 5) {
                errorJson = xhr.responseText ? JSON.parse(xhr.responseText) : '';
                if (errorJson && errorJson.errorMessage) {
                    alert('Error: ' + errorJson.errorMessage);
                } else {
                    alert('Unable to start screen sharing: ' + xhr.status);
                }
            }
            else {
                alert('We are experiencing problems connecting to the Screenleap site. Please try again in a few minutes.');
            }
        }
    });
}
function startScreenShare() {
    createScreenShare(false, createScreenShareCallback);
    return false;
}
function stopSharing() {

    $.ajax({
        url: 'https://api.screenleap.com/v2/screen-shares/' + Cookies.get('screenShareCode') + '/stop',
        type: 'POST',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('accountId', accountId);
            xhr.setRequestHeader('authtoken', authToken);
        },
        success: function (screenShareData) {
            isSharing = false;
            Cookies.set('screenShareCode', '');
            window.location.href = window.location.href;
        }
    });

    return false;
}
function onScreenShareStarting() {

}
function onAppConnectionFailed() {
    showErrorMessage("Connection failed!");
    screenleap.onScreenShareEnd();
}
function onScreenShareStartError() {
    showErrorMessage("Error Occured!");
}
function showUserMessage(message) {
    console.log(message);
}
function showErrorMessage(message) {
    console.log(message);
}