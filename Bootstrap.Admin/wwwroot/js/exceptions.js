﻿$(function () {
    var $dialog = $('#dialogNew');
    var $dataForm = $('#dataForm');
    var $dataFormDetail = $('#dataFormDetail');
    var $errorList = $('#errorList');
    var $errorDetail = $('#errorDetail');
    var $errorDetailTitle = $('#myDetailModalLabel');

    $('table').smartTable({
        url: Exceptions.url,
        sortName: 'LogTime',
        sortOrder: 'desc',
        queryParams: function (params) { return $.extend(params, { StartTime: $("#txt_operate_start").val(), EndTime: $("#txt_operate_end").val() }); },
        columns: [
            { title: "请求网址", field: "ErrorPage", sortable: true },
            { title: "用户名", field: "UserId", sortable: true },
            { title: "IP", field: "UserIp", sortable: true },
            { title: "异常类型", field: "ExceptionType", sortable: false },
            { title: "异常描述", field: "Message", sortable: false },
            { title: "记录时间", field: "LogTime", sortable: true }
        ]
    });

    $('.date').datetimepicker({
        language: 'zh-CN',
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        format: 'yyyy-mm-dd',
        pickerPosition: 'bottom-left',
        fontAwesome: true
    });

    $('#btn_view').on('click', function (row) {
        $.bc({
            url: Exceptions.url, method: "post",
            callback: function (result) {
                var html = result.map(function (ele) {
                    return $.format('<div class="form-group col-lg-3 col-md-3 col-sm-4 col-6"><a class="logfile" data-toggle="tooltip" title="{0}" href="#"><i class="fa fa-file-text-o"></i><span>{0}</span></a></div>', ele);
                }).join('');
                $dataForm.children('div').html(html).find('[data-toggle="tooltip"]').tooltip();
                $dialog.modal('show');
            }
        });
    });

    $dialog.on('click', 'a', function () {
        var fileName = $(this).tooltip('hide').find('span').text();
        $errorDetailTitle.text(fileName);
        $errorList.hide();
        $errorDetail.show();
        $dataFormDetail.html('<div class="text-center"><i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i></div>');
        $.bc({
            url: Exceptions.url, method: "put", data: { FileName: fileName },
            callback: function (result) {
                $dataFormDetail.html(result);
            }
        });
    });

    $errorDetail.on('click', 'button', function () {
        $errorDetail.hide();
        $errorList.show();
    });
});