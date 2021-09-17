$(document).ready(function () {
    var grid = $("#grid").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: $("#urlPath").val() + "/MappingUser/AjaxRead",
                    contentType: "application/json",
                    type: "POST",
                    cache: false
                },
                destroy: {
                    url: $("#urlPath").val() + "/MappingUser/AjaxDelete",
                    contentType: "application/json",
                    type: "POST",
                    complete: function (data) {
                        $("#grid").data("kendoGrid").dataSource.read();
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
                    id: "ID",
                    fields: {
                        GP: { type: "int", filterable: true, sortable: true, editable: true },
                        NRP: { type: "string", filterable: true, sortable: true, editable: true },
                        Deskripsi: { type: "string", filterable: true, sortable: true, editable: true },
                        Deskripsi_ID: { type: "string", filterable: true, sortable: true, editable: true },
                        ID: { type: "number", filterable: true, sortable: true, editable: true },
                        DISTRIK: { type: "string", filterable: true, sortable: true, editable: true },
                        NAMA: { type: "string", filterable: true, sortable: true, editable: true },
                    }
                }
            }
        },
        height: 450,
        filterable: true,
        resizable: true,
        sortable: true,
        pageable: true,
        groupable: true,
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
            {
                command: ["destroy"], title: "", width: "50px"
            }, {
                field: "NRP", title: "NRP", width: "50px"
            }, {
                field: "NAMA", title: "NAMA", width: "200px"
            }, {
                field: "DISTRIK", title: "DISTRIK", width: "50px"
            }, {
                field: "Deskripsi", title: "Deskripsi", width: "100px"
            }
        ],
        dataBinding: function () {
            window.rowNo = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });
});