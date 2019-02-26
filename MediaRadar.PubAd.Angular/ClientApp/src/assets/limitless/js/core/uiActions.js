hideSideBar = function () {
  if (!$("body").hasClass("sidebar-xs")) {
    $("body").addClass("sidebar-xs");
  }
}

reRegisterPlugins = function () {
  $('[data-popup="tooltip"]').tooltip();
}
registerDatatables = function () {
  $('.datatable-fixed-header').DataTable({
    responsive: {
      details: false
    },
    dom: '<"datatable-header"fl><"datatable-wrap"t><"datatable-footer"ip>',
    searching: false,
    fixedHeader: true,
    sPaginationType: "full_numbers",
    aLengthMenu: [[50, 100, 500, -1], ["50", "100", "500", "All"]],
    aaSorting: []
  });
}

module.exports = exports = {
  hideSideBar,
  registerDatatables,
  reRegisterPlugins
}
