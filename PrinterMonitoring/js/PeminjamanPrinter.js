var wnd_addPrinter;
var wnd_addUser;
wnd_addPrinter = $("#wnd_addPrinter").kendoWindow({
    title: "Data Printer",
    width: "1000px",
    modal: true,
    visible: false,
    draggable: true,
    actions: [
        "Minimize",
        "Close"
    ],
    pinned: true
}).data("kendoWindow");
function doShowforData() {
    wnd_addPrinter.center().open();
    var grid = $("#gridPemilihan").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/PeminjamanPrinter/AjaxReadPrinter",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            pageSize: 15,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "Data",
                total: "Total"
            }
        },
        height: "480px",
        filterable: {
            extra: false
        },
        sortable: true,
        pageable: true,
        editable: "popup",
        pageable: {
            refresh: true,
            buttonCount: 10,
            input: true,
            pageSizes: [10, 20, 50, 100, 1000, 100000],
            info: true,
            messages: {
            }
        },
        columns: [

            { command: [{ text: "Pilih", click: selects }], title: "Pilih Printer", width: "70px" },
            {
                field: "KODE_PRINTER", title: "Kode", width: "60px"
            }, {
                field: "NAMA_PRINTER", title: "Nama Printer", width: "120px"
            }, {
                field: "JENIS_PRINTER", title: "Jenis Printer", width: "90px"
            }, {
                field: "IP_PRINTER", title: "IP", width: "70px"
            }, {
                field: "STATUS_PRINTER", title: "Status", width: "70px"
            }, {
                field: "LOKASI", title: "Lokasi", width: "70px"
            }
        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });

}

function selects(e) {

    e.preventDefault();
    console.log(this.dataItem($(e.currentTarget).closest("tr")));
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $("#kode").val(dataItem.KODE_PRINTER);
    $("#printer").val(dataItem.NAMA_PRINTER);
    $("#jenis").val(dataItem.JENIS_PRINTER);
    wnd_addPrinter.center().close();

}

wnd_addUser = $("#wnd_addUser").kendoWindow({
    title: "Data Karyawan",
    width: "900px",
    modal: true,
    visible: false,
    draggable: true,
    actions: [
        "Minimize",
        "Close"
    ],
    pinned: true
}).data("kendoWindow");

function doShowformEmployee() {
    wnd_addUser.center().open();
    var grid = $("#gridPemilihanKaryawan").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/AddUser/AjaxReadEmployeeQty",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },
                parameterMap: function (data, operation) {
                    return kendo.stringify(data)
                }
            },
            pageSize: 15,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "Data",
                total: "Total"
            }
        },
        height: "480px",
        filterable: {
            extra: false
        },
        sortable: true,
        pageable: true,
        editable: "popup",
        pageable: {
            refresh: true,
            buttonCount: 10,
            input: true,
            pageSizes: [10, 20, 50, 100, 1000, 100000],
            info: true,
            messages: {
            }
        },
        columns: [

            { command: [{ text: "Pilih", click: pilih }], title: "Pilih User", width: "80px" },
            {
                field: "NRP", title: "NRP", width: "100px"
            }, {
                field: "NAME", title: "Nama", width: "150px"
            }, {
                field: "DIV_CODE", title: "Department", width: "80px"
            }
        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });

}

function pilih(e) {

    e.preventDefault();
    console.log(this.dataItem($(e.currentTarget).closest("tr")));
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    $("#nama").val(dataItem.NAME);
    $("#nrp").val(dataItem.NRP);
    $("#div").val(dataItem.DIV_CODE);
    wnd_addUser.center().close();

}

$(document).ready(function () {
    loadgrid_peminjaman();
    LoadNotif();
});
function loadgrid_peminjaman() {
    $("#grid").empty();
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/PeminjamanPrinter/Read",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },

                create: {
                    url: $("#urlPath").val() + "/PeminjamanPrinter/Insert_peminjaman",
                    contentType: "application/json",
                    type: "POST",
                    complete: function (data) {
                        alert(data.responseJSON.remarks);
                    }
                },

                update: {
                    url: $("#urlPath").val() + "/PeminjamanPrinter/Update_peminjaman",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                    complete: function (data) {

                        if (data.status) {
                            alert(data.responseJSON.remarks);
                            alert(data.remarks)
                            $("#grid").data("kendoGrid").dataSource.read()
                        } else {
                            alert(data.error);
                            $("#grid").data("kendoGrid").dataSource.read()
                        }
                    }
                },

                destroy: {
                    url: $("#urlPath").val() + "/PeminjamanPrinter/Delete_peminjaman",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                    complete: function (data) {
                        alert(data.responseJSON.remarks);
                        if (data.status) {
                            alert(data.remarks)
                            $("#grid").data("kendoGrid").dataSource.read()
                        } else {
                            alert(data.error);
                            $("#grid").data("kendoGrid").dataSource.read()
                        }
                    }
                },

                parameterMap: function (data, operation) {

                    return kendo.stringify(data)
                }
            },
            pageSize: 1000,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true,
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    id: "ID_PINJAM",
                    fields: {
                        KODE_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        NAMA_PRINTER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        JENIS_PRINTER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        TGL_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        NAMA_USER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        NRP_USER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        DIV_USER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        ESTIMASI_PINJAM: { type: "string", filterable: true, sortable: true, editable: true }
                    }
                }
            }
        },
        height: 500,
        filterable: true,
        sortable: true,
        pageable: true,
        resizable: true,
        editable: {
            mode: "popup"
        },
        toolbar: [
         {
             template: '<button type="button" id="btn_add" class="k-button k-button-icontext" onclick="open_wnd_add()"><span class="glyphicon glyphicon-plus"></span> Create</button>'
         },
         {
             name: "excel",
             imageClass: '<button type="button" button id="btn_export" class="btn btn-info"><span class="glyphicon glyphicon-export"></span> Export To Excel</button>'
         }, ],

        excel: {
            fileName: "DataPeminjamanPrinter.xlsx",
            AllPages: true,
            filterable: true
        },
        columns: [
             {
                 command: [{ name: "update-data", text: "Edit", click: Edit, iconClass: "glyphicon glyphicon-edit spasi-kanan" }, "destroy"], title: "ACTION", width: 180
             },
            { field: "KODE_PRINTER", title: "Kode Printer", width: 160 },
            { field: "NAMA_PRINTER_PINJAM", title: "Nama Printer", width: 160 },
            { field: "JENIS_PRINTER_PINJAM", title: "Jenis Printer", width: 150 },
            { field: "TGL_PINJAM", title: "Tanggal Pinjam", width: 170 },
            { field: "NAMA_USER_PINJAM", title: "Nama User", width: 150 },
            { field: "NRP_USER_PINJAM", title: "NRP User", width: 130 },
            { field: "DIV_USER_PINJAM", title: "Divisi", width: 100 },
            { field: "ESTIMASI_PINJAM", title: "Estimasi Peminjaman", width: 210 }

        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });
    //$("#jenis").kendoDropDownList({
    //    dataSource: [
    //     { text: "Color", value: "Color" },
    //    { text: "Black", value: "Black" }
    //    ],
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    optionLabel: "Pilih...",
    //    filter: "contains",
    //    popup: {
    //        appendTo: $("#container")
    //    }
    //});
    $(document).ready(function () {
        // create DatePicker from input HTML element
        $("#tgl").kendoDatePicker();
    });

}
//function dd_div() {
//    var ds_district = new kendo.data.DataSource({
//        type: "json",
//        transport: {
//            read: {
//                url: $("#urlPath").val() + "/PeminjamanPrinter/ReadDiv",
//                contentType: "application/json",
//                type: "POST",
//                cache: false
//            }
//        },
//        schema: {
//            data: "Data",
//            total: "Total"
//        }
//    });


function open_wnd_add() {
    $("#id").prop('disabled', true).val("");
    $("#kode").prop('disabled', true).val("");
    $("#printer").prop('disabled', true).val("");
    $("#jenis").prop('disabled', true).val("");
    $("#nama").prop('disabled', true).val("");
    $("#tgl").val("");
    $("#div").prop('disabled', true).val("");
    $("#pinjam").val("");
    $("#nrp").prop('disabled', true).val("");

    save_method = 'add';
    $('#myModal').modal({
        show: true,
        keyboard: false,
        backdrop: 'static'
    });
    var start = $("#tgl").kendoDatePicker({
        format: "yyyy-MM-dd",
        dateInput: false,
        change: function () {
            var start = $("#tgl").val();
        }
    }).data("kendoDatePicker");
}
function Edit(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $("#id").prop('disabled', true).val("");
    $("#kode").prop('disabled', true).val("");
    $("#printer").prop('disabled', true).val("");
    $("#jenis").prop('disabled', true).val("");
    $("#nama").prop('disabled', true).val("");
    $("#tgl").val("");
    $("#div").prop('disabled', true).val("");
    $("#pinjam").val("");
    $("#nrp").prop('disabled', true).val("");

    $("#id").val(dataItem.ID_PINJAM);
    $("#kode").val(dataItem.KODE_PRINTER);
    $("#printer").val(dataItem.NAMA_PRINTER_PINJAM);
    $("#jenis").val(dataItem.JENIS_PRINTER_PINJAM);
    $("#nama").val(dataItem.NAMA_USER_PINJAM);
    $("#tgl").val(dataItem.TGL_PINJAM);
    $("#nrp").val(dataItem.NRP_USER_PINJAM);
    $("#div").val(dataItem.DIV_USER_PINJAM);
    $("#pinjam").val(dataItem.ESTIMASI_PINJAM);

    save_method = "update";
    $('#myModal').modal({
        show: true,
        keyboard: false,
        backdrop: 'static'
    });
    var start = $("#tgl").kendoDatePicker({
        format: "yyyy-MM-dd",
        dateInput: false,
        change: function () {
            var start = $("#tgl").val();
        }
    }).data("kendoDatePicker");
}
$("#btn_simpan").click(function () {
    var url = "";
    if (save_method == "update") {
        url_ = $("#urlPath").val() + "/PeminjamanPrinter/Update_peminjaman";
    } else {
        url_ = $("#urlPath").val() + "/PeminjamanPrinter/Insert_peminjaman";
    }
  
    
    var stbl_m_driver = {
        ID_PINJAM: $("#id").val(),
        KODE_PRINTER: $("#kode").val(),
        NAMA_PRINTER_PINJAM: $("#printer").val(),
        JENIS_PRINTER_PINJAM: $("#jenis").val(),
        NAMA_USER_PINJAM: $("#nama").val(),
        NRP_USER_PINJAM: $("#nrp").val(),
        TGL_PINJAM: $("#tgl").val(),
        ESTIMASI_PINJAM: $("#pinjam").val(),
        DIV_USER_PINJAM: $("#div").val()
    }
    var datepicker = $("#tgl").data("kendoDatePicker");
    var value = datepicker.value();
    var req_date = kendo.toString(value, "yyyy-MM-dd");
    $.ajax({
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        url: url_,
        data: JSON.stringify(stbl_m_driver),
        success: function (response) {
            alert(response.remarks)
            $('#myModal').modal('toggle');
            $("#grid").data("kendoGrid").dataSource.read();
            $("#grid").data("kendoGrid").refresh();
        }
    });
});

function LoadNotif() {
    $.ajax({
        type: "GET",
        url: $('#urlPath').val() + "/PrinterMonitoring/getNotifCount",
        success: function (response) {
            if (response.ID_PRINTER == true) {
                document.getElementById("tersedia").innerHTML = response.tersedia;
                document.getElementById("dipinjam").innerHTML = response.dipinjam;
                document.getElementById("service").innerHTML = response.service;
                document.getElementById("rusak").innerHTML = response.rusak;
            } else {
                document.getElementById("tersedia").innerHTML = response.tersedia;
                document.getElementById("dipinjam").innerHTML = response.dipinjam;
                document.getElementById("service").innerHTML = response.service;
                document.getElementById("rusak").innerHTML = response.rusak;
            }
        }
    });
}
function btn_rusak() {
    //debugger;
    window.location.href = $("#urlPath").val() + "/ServicePrinter";
}
function btn_service() {
    // debugger;
    window.location.href = $("#urlPath").val() + "/ServiceUpdatePrinter";
}
function btn_tersedia() {
    // debugger;
    window.location.href = $("#urlPath").val() + "/PrinterMonitoring";
}

