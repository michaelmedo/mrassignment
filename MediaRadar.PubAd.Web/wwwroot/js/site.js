$(document).ready(function () {
    $('.datatable-fixed-header').DataTable({
        responsive: {
            details: false
        },
        dom: '<"datatable-header"fl><"datatable-wrap"t><"datatable-footer"ip>',
        searching: true,
        fixedHeader: true,
        sPaginationType: "full_numbers",
        aLengthMenu: [[50, 100, 500, -1], ["50", "100", "500", "All"]],
        aaSorting: []
    });
})