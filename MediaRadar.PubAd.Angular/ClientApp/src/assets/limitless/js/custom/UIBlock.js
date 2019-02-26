var UIBlock = {
    blockFullPage: function () {
        $.blockUI({
            message: '<i class="icon-spinner4 spinner"></i>',
            overlayCSS: {
                backgroundColor: '#1b2024',
                opacity: 0.8,
                cursor: 'wait'
            },
            css: {
                border: 0,
                color: '#fff',
                padding: 0,
                backgroundColor: 'transparent'
            }
        });
    },
    unblockFullPage: function () {
        $.unblockUI()
    },
    blockControl: function (selector) {
        this.blockControlElement($(selector));
    },
    blockControlElement: function (ctrl) {
        $(ctrl).block({
            message: '<i class="icon-spinner4 spinner"></i>',
            overlayCSS: {
                backgroundColor: '#fff',
                opacity: 0.8,
                cursor: 'wait'
            },
            css: {
                border: 0,
                padding: 0,
                backgroundColor: 'transparent'
            }
        });
    },
    unblockControl: function (selector) {
        $(selector).unblock();
    },
    unblockControlElement: function (ctrl) {
        ctrl.unblock();
    }
};