var wnd_addPrinter;
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
    $("#nama").val(dataItem.NAMA_PRINTER);
    $("#jenis").val(dataItem.JENIS_PRINTER);
    wnd_addPrinter.center().close();

}

$(document).ready(function () {
    LoadNotif();
    loadgrid();
});
function loadgrid() {
    $("#grid").empty();
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/ServicePrinter/Read",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },
                create: {
                    url: $("#urlPath").val() + "/ServicePrinter/Insert",
                    contentType: "application/json",
                    type: "POST",
                    complete: function (data) {
                        alert(data.responseJSON.remarks);
                    }
                },
                update: {
                    url: $("#urlPath").val() + "/ServicePrinter/Update",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                    complete: function (data) {

                        if (data.status) {
                            alert(data.responseJSON.remarks);
                            //alert(data.remarks)
                            $("#grid").data("kendoGrid").dataSource.read()
                        } else {
                            alert(data.error);
                            $("#grid").data("kendoGrid").dataSource.read()
                        }
                    }
                },
                destroy: {
                    url: $("#urlPath").val() + "/ServicePrinter/Delete",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                    complete: function (data) {
                        alert(data.responseJSON.remarks);
                        if (data.status) {
                            //alert(data.remarks)
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
                    id: "ID_SERVICE",
                    fields: {
                        NO_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        NAMA_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        JENIS_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        KETERANGAN: { type: "string", filterable: true, sortable: true, editable: true },
                        TGL_RUSAK: { type: "string", filterable: true, sortable: true, editable: true },
                        EST_PERBAIKAN: { type: "string", filterable: true, sortable: true, editable: true },
                        BIAYA: { type: "number", filterable: true, sortable: true, editable: true, validation: { min: 0, required: true } },
                        STATUS: { type: "string", filterable: true, sortable: true, editable: true }
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
             name: "excel",
             imageClass: '<button type="button" button id="btn_export" class="btn btn-info"><span class="glyphicon glyphicon-export"></span> Export To Excel</button>'
         }, ],

        excel: {
            fileName: "DataServicePrinter.xlsx",
            AllPages: true,
            filterable: true
        },
        columns: [
            {
                command: [{ name: "update-data", text: "Edit", click: Edit, iconClass: "glyphicon glyphicon-edit spasi-kanan" }, "destroy"], title: "ACTION", width: 180
            },
           { field: "NO_PRINTER", title: "Kode Printer", width: 150 },
           { field: "NAMA_PRINTER", title: "Nama Printer", width: 170 },
           { field: "JENIS_PRINTER", title: "Jenis Printer", width: 150 },
           { field: "KETERANGAN", title: "Keterangan", width: 150 },
           { field: "TGL_RUSAK", title: "Tanggal rusak", width: 170 },
           { field: "EST_PERBAIKAN", title: "Estimasi perbaikan(hari)", width: 230 },
           { field: "BIAYA", title: "Estimasi Biaya", format: "{0:n}", width: 170 },
           { field: "STATUS", title: "Status", width: 150 }

        ],
        dataBound: function (e) {
            var grid = $("#grid").data("kendoGrid");
            var gridData = grid.dataSource.view();
            //var currentUid = senderr._data[i].uid
            var iadd_header = $("#add_header").val(); //1 maka tampil,//0 maka hilang
            //for (var i = 0; i < e.sender.dataSource._pristineData.length; i++) {
            var gp = $("#gp").val();
            console.log(gp);
            if (gp != 0 && gp != 13) {
                //var currenRow = grid.table.find("tr[data-uid='" + e.sender._data[i].uid + "']");
                //var req = $(currenRow).find(".k-grid-req-data");
                //req.hide();
                //e.sender.columns[1].hide();
                btn_add.style.display = 'none';
                var columnAction = $("#grid").data("kendoGrid");
                columnAction.hideColumn(0);
            }
            //}
        },
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
    $("#status").kendoDropDownList({
        dataSource: [
         { text: "Menunggu Pickup", value: "Menunggu Pickup" }
        ],
        dataTextField: "text",
        dataValueField: "value",
        optionLabel: "Pilih...",
        filter: "contains",
        popup: {
            appendTo: $("#container")
        }
    });
}

function open_wnd_add() {
    $("#id").prop('disabled', true).val("");
    $("#kode").prop('disabled', true).val("");
    $("#nama").prop('disabled', true).val("");
    $("#jenis").prop('disabled', true).val("");
    $("#ket").val("");
    $("#tgl").val("");
    $("#est").val("");
    $("#biaya").val("");
    $("#status").data("kendoDropDownList").value(-1);

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
    $("#nama").prop('disabled', true).val("");
    $("#jenis").prop('disabled', true).val("");
    $("#ket").val("");
    $("#tgl").val("");
    $("#est").val("");
    $("#biaya").val("");
    $("#status").data("kendoDropDownList").value(-1);

    $("#id").val(dataItem.ID_SERVICE);
    $("#kode").val(dataItem.NO_PRINTER);
    $("#nama").val(dataItem.NAMA_PRINTER);
    $("#jenis").val(dataItem.JENIS_PRINTER);
    $("#ket").val(dataItem.KETERANGAN);
    $("#tgl").val(dataItem.TGL_RUSAK);
    $("#est").val(dataItem.EST_PERBAIKAN);
    $("#biaya").val(dataItem.BIAYA);
    $("#status").data("kendoDropDownList").value(dataItem.STATUS);

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
        url_ = $("#urlPath").val() + "/ServicePrinter/Update";
    } else {
        url_ = $("#urlPath").val() + "/ServicePrinter/Insert";
    }
    var stbl_m_driver = {
        ID_SERVICE: $("#id").val(),
        NO_PRINTER: $("#kode").val(),
        NAMA_PRINTER: $("#nama").val(),
        JENIS_PRINTER: $("#jenis").val(),
        KETERANGAN: $("#ket").val(),
        TGL_RUSAK: $("#tgl").val(),
        EST_PERBAIKAN: $("#est").val(),
        BIAYA: $("#biaya").val(),
        STATUS: $("#status").data("kendoDropDownList").value()
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
            LoadNotif();
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
            console.log(response);
            if (response.ID_PRINTER == true) {
                document.getElementById("tersedia").innerHTML = response.tersedia;
                document.getElementById("dipinjam").innerHTML = response.dipinjam;
                document.getElementById("service").innerHTML = response.service;
                document.getElementById("rusak").innerHTML = response.rusak;
                document.getElementById("scarpt").innerHTML = response.scarpt;
            } else {
                document.getElementById("tersedia").innerHTML = response.tersedia;
                document.getElementById("dipinjam").innerHTML = response.dipinjam;
                document.getElementById("service").innerHTML = response.service;
                document.getElementById("rusak").innerHTML = response.rusak;
                document.getElementById("scarpt").innerHTML = response.scarpt;
            }
        }
    });
}
function btn_tersedia() {
    window.location.href = $("#urlPath").val() + "/PrinterMonitoring";
}
function btn_service() {
    window.location.href = $("#urlPath").val() + "/ServiceUpdatePrinter";
}
//function btn_pinjam() {
//    window.location.href = $("#urlPath").val() + "/PeminjamanPrinter";
//}