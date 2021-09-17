var wnd_addPrinter;
var wnd_addUser;
wnd_addPrinter = $("#wnd_addPrinter").kendoWindow({
    title: "Data Asset Printer",
    width: "1200px",
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
                    url: $("#urlPath").val() + "/PrinterMonitoring/AjaxReadPrinter",
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

            { command: [{ text: "Pilih", click: selects }], title: "Pilih Printer", width: "100px" },
            {
                field: "ASSET_NO", title: "ASSET NO", width: "90px"
            }, {
                field: "DSTRCT_CODE", title: "DSTRCT", width: "80px"
            }, {
                field: "ASSET_CLASSIF", title: "ASSET_CLASSIF", width: "120px"
            }, {
                field: "ASSET_DESC", title: "ASSET_DESC", width: "200px"
            }, {
                field: "ASSET_LOCAT", title: "ASSET_LOCAT", width: "120px"
            }, {
                field: "SERIAL_NO", title: "SERIAL_NO", width: "100px"
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
    $("#kodep").val(dataItem.ASSET_NO);
    $("#namap").val(dataItem.ASSET_DESC);
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
                field: "DIV_CODE", title: "Divisi", width: "80px"
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

    $("#namapic").val(dataItem.NAME);
    $("#nrppic").val(dataItem.NRP);
    $("#divpic").val(dataItem.DIV_CODE);
    wnd_addUser.center().close();

}
$(document).ready(function () {
    LoadNotif();
    loadgrid();
    dd_div();
});
function loadgrid() {
    $("#grid").empty();
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/PrinterMonitoring/Read",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },
                create: {
                    url: $("#urlPath").val() + "/PrinterMonitoring/Insert",
                    contentType: "application/json",
                    type: "POST",
                    complete: function (data) {
                        alert(data.responseJSON.remarks);
                    }
                },
                update: {
                    url: $("#urlPath").val() + "/PrinterMonitoring/Update",
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
                    url: $("#urlPath").val() + "/PrinterMonitoring/Delete",
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
                    id: "ID_PRINTER",
                    fields: {
                        KODE_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        NAMA_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        DETAIL_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        UMUR_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        JENIS_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        STATUS_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        IP_PRINTER: { type: "string", filterable: true, sortable: true, editable: true },
                        LOKASI: { type: "string", filterable: true, sortable: true, editable: true }
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
        //{
        //    template: '<button type="button" id="btn_add" class="k-button k-button-icontext" onclick="open_wnd_add()"><span class="glyphicon glyphicon-plus"></span> Create</button>'
        //},
         {
             name: "excel",
             imageClass: '<button type="button" button id="btn_export" class="btn btn-info"><span class="glyphicon glyphicon-export"></span> Export To Excel</button>'
         }, ],

        excel: {
            fileName: "DataPrinter.xlsx",
            AllPages: true,
            filterable: true
        },
        detailInit: detailInitPeminjam,
        columns: [
            {
                command: [
                //{
                //    template: '<button type="button" id="btn_addpinjam" class="k-button k-button-icontext" onclick="Addpeminjam()"><span class="glyphicon glyphicon-plus"></span>ADD PIC</button>'
                //},
                { name: "data-pinjam", text: "ADD PIC", click: Addpeminjam, iconClass: "glyphicon glyphicon-edit spasi-kanan" },
                { name: "update-data-printer", text: "Edit Printer", click: EditPrinter, iconClass: "glyphicon glyphicon-edit spasi-kanan" },
                 "destroy"], title: "ACTION", width: 120
            },
           { field: "KODE_PRINTER", title: "Kode Printer", width: 150 },
           { field: "NAMA_PRINTER", title: "Nama Printer", width: 170 },
           { field: "DETAIL_PRINTER", title: "Detail Printer", width: 170 },
           { field: "JENIS_PRINTER", title: "Jenis Printer", width: 170 },
           { field: "UMUR_PRINTER", title: "Umur Printer", width: 170 },
           { field: "IP_PRINTER", title: "IP Printer", width: 130 },
           { field: "STATUS_PRINTER", title: "Status", width: 130 },
           { field: "LOKASI", title: "Lokasi Printer", width: 170 }

        ],
        dataBound: function (e) {
            var grid = $("#grid").data("kendoGrid");
            var gridData = grid.dataSource.view();
            //var currentUid = senderr._data[i].uid
            var iadd_header = $("#add_header").val(); //1 maka tampil,//0 maka hilang
            //for (var i = 0; i < e.sender.dataSource._pristineData.length; i++) {
            var gp = $("#gp").val();
            //console.log(gp);
            if (gp!=0 && gp!=13)
            {
                //var currenRow = grid.table.find("tr[data-uid='" + e.sender._data[i].uid + "']");
                //var req = $(currenRow).find(".k-grid-req-data");
                //req.hide();
                //e.sender.columns[1].hide();
                //$("btn_add").hide();
                btn_add.style.display = 'none';
                btn_addasset.style.display = 'none';
                var columnAction = $("#grid").data("kendoGrid");
                columnAction.hideColumn(0);
                }
            //}
        },
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });

    $("#jenis").kendoDropDownList({
        dataSource: [
         { text: "Color", value: "Color" },
        { text: "Black", value: "Black" }
        ],
        dataTextField: "text",
        dataValueField: "value",
        optionLabel: "Pilih...",
        filter: "contains",
        popup: {
            appendTo: $("#container")
        }
    });
    $("#status").kendoDropDownList({
        dataSource: [
         { text: "Tersedia", value: "Tersedia" },
        { text: "Dipinjam", value: "Dipinjam" },
        { text: "Rusak", value: "Rusak" },
        { text: "Service", value: "Service" },
        { text: "Scarpt", value: "Scarpt" }
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
function dd_div() {
    var ds_district = new kendo.data.DataSource({
        type: "json",
        transport: {
            read: {
                url: $("#urlPath").val() + "/PeminjamanPrinter/ReadDiv",
                contentType: "application/json",
                type: "POST",
                cache: false
            }
        },
        schema: {
            data: "Data",
            total: "Total"
        }
    });

    $("#lokasi").kendoDropDownList({
        autoBind: true,
        dataSource: ds_district,
        dataTextField: "DIV_CODE",
        dataValueField: "DIV_CODE",
        optionLabel: "Pilih...",
        filter: "contains",
        change: function (e) {
            var dataItem = this.dataItem(e.sender._oldIndex);
        }
    }).data("kendoDropDownList");
}
function open_asset_add() {
    $("#noasset").val("");
    $("#distrik").val("");
    $("#classif").val("");
    $("#desc").val("");
    $("#locat").val("");
    $("#noserial").val("");

    save_method = 'add';
    $('#myModalAsset').modal({
        show: true,
        keyboard: false,
        backdrop: 'static'
    });
}
$("#btn_save").click(function () {
    var url = "";
        url_ = $("#urlPath").val() + "/PrinterMonitoring/InsertAsset";
    var stbl_m_driver = {
        ASSET_NO: $("#noasset").val(),
        DSTRCT_CODE: $("#distrik").val(),
        ASSET_CLASSIF: $("#classif").val(),
        ASSET_DESC: $("#desc").val(),
        ASSET_LOCAT: $("#locat").val(),
        SERIAL_NO: $("#noserial").val(),
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        url: url_,
        data: JSON.stringify(stbl_m_driver),
        success: function (response) {
            alert(response.remarks)
            LoadNotif();
            $('#myModalAsset').modal('toggle');
            //$("#grid").data("kendoGrid").dataSource.read();
            $("#grid").data("kendoGrid").refresh();
        }
    });
});

function open_wnd_add() {
    $("#idp").prop('disabled', true).val("");
    $("#kodep").prop('disabled', true).val("");
    $("#namap").prop('disabled', true).val("");
    $("#detail").val("");
    $("#jenis").data("kendoDropDownList").value(-1);
    $("#umur").val("");
    $("#ip").val("");
    $("#status").data("kendoDropDownList").value(-1);
    $("#lokasi").data("kendoDropDownList").value(-1);

    save_method = 'add';
    $('#myModalPrinter').modal({
        show: true,
        keyboard: false,
        backdrop: 'static'
    });
}
function EditPrinter(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $("#idp").prop('disabled', true).val("");
    $("#kodep").prop('disabled', true).val("");
    $("#namap").prop('disabled', true).val("");
    $("#detail").val("");
    $("#jenis").data("kendoDropDownList").value(-1);
    $("#umur").val("");
    $("#ip").val("");
    $("#status").data("kendoDropDownList").value(-1);
    $("#lokasi").data("kendoDropDownList").value(-1);

    $("#idp").val(dataItem.ID_PRINTER);
    $("#kodep").val(dataItem.KODE_PRINTER);
    $("#namap").val(dataItem.NAMA_PRINTER);
    $("#jenis").data("kendoDropDownList").value(dataItem.JENIS_PRINTER);
    $("#detail").val(dataItem.DETAIL_PRINTER);
    $("#umur").val(dataItem.UMUR_PRINTER);
    $("#ip").val(dataItem.IP_PRINTER);
    $("#status").data("kendoDropDownList").value(dataItem.STATUS_PRINTER);
    $("#lokasi").data("kendoDropDownList").value(dataItem.LOKASI);


    save_method = "update";
    $('#myModalPrinter').modal({
        show: true,
        keyboard: false,
        backdrop: 'static'
    });
}
$("#btn_simpan").click(function () {
    var url = "";
    if (save_method == "update") {
        url_ = $("#urlPath").val() + "/PrinterMonitoring/Update";
    } else {
        url_ = $("#urlPath").val() + "/PrinterMonitoring/Insert";
    }
    console.log($("#status").data("kendoDropDownList").value());
    console.log($("#jenis").data("kendoDropDownList").value());
    var stbl_m_driver = {
        ID_PRINTER: $("#idp").val(),
        KODE_PRINTER: $("#kodep").val(),
        NAMA_PRINTER: $("#namap").val(),
        DETAIL_PRINTER: $("#detail").val(),
        UMUR_PRINTER: $("#umur").val(),
        JENIS_PRINTER: $("#jenis").data("kendoDropDownList").value(),
        IP_PRINTER: $("#ip").val(),
        STATUS_PRINTER: $("#status").data("kendoDropDownList").value(),
        LOKASI: $("#lokasi").data("kendoDropDownList").value(),
    }
    $.ajax({
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        url: url_,
        data: JSON.stringify(stbl_m_driver),
        success: function (response) {
            alert(response.remarks)
            LoadNotif();
            $('#myModalPrinter').modal('toggle');
            $("#grid").data("kendoGrid").dataSource.read();
            $("#grid").data("kendoGrid").refresh();
              }
    });
});

function detailInitPeminjam(e) {
    console.log(e);
    $("<div/>").appendTo(e.detailCell).kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/PrinterMonitoring/ReadPeminjam",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                },

                update: {
                    url: $("#urlPath").val() + "/PrinterMonitoring/Update_peminjaman",
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
                            $("#gridpinjam").data("kendoGrid").dataSource.read()
                        }
                    }
                },
                destroy: {
                    url: $("#urlPath").val() + "/PrinterMonitoring/Delete_peminjaman",
                    contentType: "application/json",
                    type: "POST",
                    cache: false,
                    complete: function (data) {
                        //alert($("urlPath").val() + "/PrinterMonitoring/Delete_peminjaman");
                        alert(data.responseJSON.remarks);
                        console.log(data);
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
            schema: {
                data: "Data",
                total: "Total",
                model: {
                    id: "ID_PINJAM",
                    fields: {
                        //KODE_PRINTER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        //NAMA_PRINTER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        //JENIS_PRINTER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        TGL_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        NAMA_USER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        NRP_USER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        DIV_USER_PINJAM: { type: "string", filterable: true, sortable: true, editable: true },
                        SELESAI_PINJAM: { type: "string", filterable: true, sortable: true, editable: true }
                    }
                }
            },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            pageSize: 10,
            filter: { field: "KODE_PRINTER_PINJAM", operator: "eq", value: e.data.KODE_PRINTER }
        },
        scrollable: false,
        sortable: true,
        pageable: true,
        columns: [
             {
                 command: [
                     { name: "update-data-pinjam", text: "Edit", click: EditPinjam, iconClass: "glyphicon glyphicon-edit spasi-kanan" }, "destroy"], title: "ACTION", width: 100
             },
           //{ field: "KODE_PRINTER_PINJAM", title: "Kode Printer", width: 120 },
           // { field: "NAMA_PRINTER_PINJAM", title: "Nama Printer", width: 160 },
            //{ field: "JENIS_PRINTER_PINJAM", title: "Jenis Printer", width: 150 },
            
            { field: "NAMA_USER_PINJAM", title: "Nama User", width: 150 },
            { field: "NRP_USER_PINJAM", title: "NRP User", width: 100 },
            { field: "DIV_USER_PINJAM", title: "Divisi", width: 80 },
              { field: "TGL_PINJAM", title: "Tanggal Pinjam", width: 120 },
            { field: "SELESAI_PINJAM", title: "Selesai Pinjam", width: 120 },
        ],
         dataBound: function (e) {
             var grid1 = $("#grid").data("kendoGrid");
             //console.log(grid1);
             var gridData = grid1.dataSource.view();
             //var currentUid = senderr._data[i].uid
             var iadd_header = $("#add_header").val(); //1 maka tampil,//0 maka hilang
             for (var i = 0; i < e.sender.dataSource._pristineData.length; i++) {
                 var gp = $("#gp").val();
                 //console.log(gp);
                 if (gp != 0 && gp != 13) {
                     var currenRow = grid1.table.find("tr[data-uid='" + e.sender._data[i].uid + "']");
                     var req = $(currenRow).find(".k-grid-update-data-pinjam");
                     var req1 = $(currenRow).find(".k-grid-delete");
                     req.hide();
                     req1.hide();
                 }
             }
         },
         dataBinding: function () {
             window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
         }
    });

}
function Addpeminjam(e) {
    //var grid = $("#grid").data("kendoGrid");
    //var gridData = grid.dataSource.view();

    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $("#id").prop('disabled', true).val("");
    $("#kodepr").prop('disabled', true).val("");
    $("#printer").prop('disabled', true).val("");
    $("#jenispr").prop('disabled', true).val("");
    $("#namapic").prop('disabled', true).val("");
    $("#tgl").val("");
    $("#divpic").prop('disabled', true).val("");
    $("#tglkembali").val("");
    $("#nrppic").prop('disabled', true).val("");

    $("#kodepr").val(dataItem.KODE_PRINTER);
    $("#printer").val(dataItem.NAMA_PRINTER);
    $("#jenispr").val(dataItem.JENIS_PRINTER);
    
    save_method = 'add';
    $('#myModal1').modal({
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

    var start = $("#tglkembali").kendoDatePicker({
        format: "yyyy-MM-dd",
        dateInput: false,
        change: function () {
            var start = $("#tglkembali").val();
        }
    }).data("kendoDatePicker");
}
function EditPinjam(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    $("#id").prop('disabled', true).val("");
    $("#kodepr").prop('disabled', true).val("");
    $("#printer").prop('disabled', true).val("");
    $("#jenispr").prop('disabled', true).val("");
    $("#namapic").prop('disabled', true).val("");
    $("#tgl").val("");
    $("#divpic").prop('disabled', true).val("");
    $("#tglkembali").val("");
    $("#nrppic").prop('disabled', true).val("");

    $("#id").val(dataItem.ID_PINJAM);
    $("#kodepr").val(dataItem.KODE_PRINTER_PINJAM);
    $("#printer").val(dataItem.NAMA_PRINTER_PINJAM);
    $("#jenispr").val(dataItem.JENIS_PRINTER_PINJAM);
    $("#namapic").val(dataItem.NAMA_USER_PINJAM);
    $("#tgl").val(dataItem.TGL_PINJAM);
    $("#nrppic").val(dataItem.NRP_USER_PINJAM);
    $("#divpic").val(dataItem.DIV_USER_PINJAM);
    $("#tglkembali").val(dataItem.SELESAI_PINJAM);

    save_method = "updatepic";
    $('#myModal1').modal({
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

    var start = $("#tglkembali").kendoDatePicker({
        format: "yyyy-MM-dd",
        dateInput: false,
        change: function () {
            var start = $("#tglkembali").val();
        }
    }).data("kendoDatePicker");
}

$("#btn_simpanpinjam").click(function () {
    var url = "";
    if (save_method == "updatepic") {
        url_ = $("#urlPath").val() + "/PrinterMonitoring/Update_peminjaman";
    } else {
        url_ = $("#urlPath").val() + "/PrinterMonitoring/Insert_peminjaman";
    }

    var stbl_m_driver = {
        ID_PINJAM: $("#id").val(),
        KODE_PRINTER_PINJAM: $("#kodepr").val(),
        NAMA_PRINTER_PINJAM: $("#printer").val(),
        JENIS_PRINTER_PINJAM: $("#jenispr").val(),
        NAMA_USER_PINJAM: $("#namapic").val(),
        NRP_USER_PINJAM: $("#nrppic").val(),
        TGL_PINJAM: $("#tgl").val(),
        SELESAI_PINJAM: $("#tglkembali").val(),
        DIV_USER_PINJAM: $("#divpic").val()
    }
    var datepicker = $("#tgl").data("kendoDatePicker");
    var value = datepicker.value();
    var req_date = kendo.toString(value, "yyyy-MM-dd");

    var datepicker1 = $("#tglkembali").data("kendoDatePicker");
    var value = datepicker1.value();
    var req_date = kendo.toString(value, "yyyy-MM-dd");
    $.ajax({
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        url: url_,
        data: JSON.stringify(stbl_m_driver),
        success: function (response) {
            alert(response.remarks)
            $('#myModal1').modal('toggle');
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

function btn_rusak() {
    window.location.href = $("#urlPath").val() + "/ServicePrinter";
}
function btn_service() {
    window.location.href = $("#urlPath").val() + "/ServiceUpdatePrinter";
}
function btn_tersedia() {
    //debugger;
    window.location.href = $("#urlPath").val() + "/PrinterMonitoring";
}
//function btn_pinjam() {
//    //debugger;
//    window.location.href = $("#urlPath").val() + "/PeminjamanPrinter";
//}
