 
/*jslint unparam: true, regexp: true */
/*global window, $ */
$(function () {
    'use strict';

    $.ajax({
        url: "/SoftwareVersion/GetUpgradeFileNodes",
        data: { versionName: window.VersionName },
        type: "post",
        success: function (result) {
            $('#functionsTree').jstree({
                'plugins': ["types"],
                'core': {
                    'data': result
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
        complete: function () {
 
        }
    });
  
});

 