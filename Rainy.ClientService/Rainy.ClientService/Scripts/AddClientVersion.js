
function SaveForm() {
    var url = $("#mainForm").attr("action");

    var filesTodelete = [];
    $("#FilesToRemove option").each(function (item, i) {
        filesTodelete.push(item.value);
    });

    var formData = {
        VersionName: $("#VersionName").val().trim(),
        Description: $("#Description").text(),
        IsLastVersion: $("#AsLastestVersion").prop("checked"),
        FileToDelete: filesTodelete,
        FilesToUpgrade: $('#functionsTree').jstree("get_selected")
    };
    $("#btnSave").attr("disabled");

    $.ajax({
        url: url,
        data: formData,
        type: "post",
        success: function (result) {
            if (result.IsSuccess) {
                window.top.location.href = "/SoftwareVersion/Detail?version=" + $("#VersionName").val().trim();
            } else {
                alert("保存出错："+result.Message);
            }
        },
        complete: function () {
            $("#btnSave").removeAttr("disabled");
        }
    });
}

/*jslint unparam: true, regexp: true */
/*global window, $ */
$(function () {
    'use strict';
    // Change this to the location of your server-side upload handler:
    var url = '/SoftwareVersion/UploadFile/';

    $("#btnSave").click(SaveForm);

    $(".otherGroup").hide();

    $('#fileupload').fileupload({
        url: url,
        dataType: 'json',
        add: function (e, data) {
            var that = this;
            data.formData = { versionName: $("#VersionName").val().trim() }; // e.g. {id: 123}
            $.blueimp.fileupload.prototype
                       .options.add.call(that, e, data);
        },
        //  limitMultiFileUploads:1,
        done: function (e, data) {

            if (data.result.isvalid)
                $('<p/>').text(data.result.FileName).appendTo('#files');
            else
                alert(data.result.message);

            $('#functionsTree').jstree({
                'plugins': ["checkbox", "types"],
                'core': {
                    'data': data.result.fileRecords
                },
                "checkbox": {
                    // "keep_selected_style" : false,
                    "cascade": "down",
                    three_state: false
                },
                "types": {
                    "default": {
                        "icon": "fa fa-folder icon-state-warning icon-lg"
                    },
                    "file": {
                        "icon": "fa fa-file icon-state-warning icon-lg"
                    }
                }
            });
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .progress-bar').css(
                'width',
                progress + '%'
            );
        },
        change: function (e, data) {

            try {
                $('#functionsTree').jstree("destroy");
            } catch (e) {

            };

            $('#progress .progress-bar').css(
                'width',
               "0"
            );
        }
    }).prop('disabled', !$.support.fileInput)
         .parent().addClass($.support.fileInput ? undefined : 'disabled');
});

function ConfirmVersioNO() {

    $.ajax({
        url: "/SoftwareVersion/ConfirmVersion",
        data: { VersionName: $("#VersionName").val().trim() },
        type: "post",
        success: function (result) {
            if (result.CanUse) {

                $(".otherGroup").show();
                $(".resetNO").show();
                $(".confirmNO").hide();
                $("#VersionName").attr("readonly", "readonly");
            } else {
                alert(result.Message);
            }
        }

    });

}

function RenameVersionNO() {
    $(".otherGroup").hide();
    $(".resetNO").hide();
    $(".confirmNO").show();
    $("#VersionName").removeAttr("readonly", "readonly");

    try {
        $('#functionsTree').jstree("destroy");
    } catch (e) {

    }
}

function ClearFile() {
    var $fileNameELE = $("#FileNameToRemove");
    var fileName = $fileNameELE.val().trim();

    if ($fileNameELE.val()) {
        var selectTag = document.getElementById("FilesToRemove"); //获取select标记

        var colls = selectTag.options; //获取引用
        clearOptions(colls);

        function clearOptions(colls) {
            var length = colls.length;
            for (var i = length - 1; i >= 0; i--) {
                colls.remove(i);
            }
        }
    }
}

function AddRemoveFile() {
    var $fileNameELE = $("#FileNameToRemove");
    var fileName = $fileNameELE.val().trim();

    if ($fileNameELE.val()) {
        var selectTag = document.getElementById("FilesToRemove"); //获取select标记

        var colls = selectTag.options; //获取引用

        var repeated = false;

        for (var i = 0; i < colls.length; i++) {
            if (colls[i].value == fileName) {
                repeated = true;
                break;
            }
        }

        if (!repeated) {
            var item = new Option(fileName, fileName); //通过Option()构造函数创建option对象
            colls.add(item); //添加到options集合中
        }
    }
}