$(function () {
    
    $('input[name="daterange"]').daterangepicker({
                //opens: 'center',
            },
            function(start, end, label) {
                var $this = $(this);
                var myId = $this.get(0).element.attr('id');
                var parent = $('#' + myId).parent();
                var inputAux = parent.find('input.input_aux');
                inputAux.get(0).value = start.format('YYYY-MM-DD');
                inputAux.get(1).value = end.format('YYYY-MM-DD');
                //console.log("A new date selection was made: " + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD'));
            });

    $('.datetimepicker').datetimepicker();
    $('.datepicker').datepicker(
        {
            minViewMode : "months",
        });
    $('.datepickeryear').datepicker(
        {
            minViewMode: "years",
        });

    //// Opcion trimestral para el datepicker
    $.fn.datepicker.dates['qtrs'] = {
        days: ["Sunday", "Moonday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
        daysShort: ["Sun", "Moon", "Tue", "Wed", "Thu", "Fri", "Sat"],
        daysMin: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
        months: ["Enero-Marzo", "Abril-Junio", "Julio-Septiembre", "Octubre-Diciembre", "", "", "", "", "", "", "", ""],
        monthsShort: ["Ene&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Feb&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Mar", "Abr&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;May&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Jun", "Jul&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Ago&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Sep", "Oct&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Nov&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dec", "", "", "", "", "", "", "", ""],
        today: "Today",
        clear: "Clear",
        format: "mm/dd/yyyy",
        //titleFormat: "MM yyyy",
        /* Leverages same syntax as 'format' */
        weekStart: 0
    };

    //// Opcion de verano para el datepicker
    $.fn.datepicker.dates['summer'] = {
        days: ["Sunday", "Moonday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
        daysShort: ["Sun", "Moon", "Tue", "Wed", "Thu", "Fri", "Sat"],
        daysMin: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
        months: ["Julio y Agosto", "", "", "", "", "", "", "", "", "", "", ""],
        monthsShort: ["Julio&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Agosto&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp", "", "", "", "", "", "", "", "", "", "", ""],
        today: "Today",
        clear: "Clear",
        format: "mm/dd/yyyy",
        //titleFormat: "MM yyyy",
        /* Leverages same syntax as 'format' */
        weekStart: 0
    };


    $('.datepicker_quarter').datepicker({
        format: "MM yyyy",
        minViewMode: 1,
        autoclose: true,
        language: 'qtrs',
        forceParse: false
    }).on("show", function (event) {
        $(".datepicker-months").find(".month").each(function (index, element) {
            if (index > 3) $(element).hide();
            else $(element).css('width' , '100%');
        });
        });

    $('.datepicker_summer').datepicker({
        format: "MM yyyy",
        minViewMode: 1,
        autoclose: true,
        language: 'summer',
        forceParse: false
    }).on("show", function (event) {
        $(".datepicker-months").find(".month").each(function (index, element) {
            if (index > 0) $(element).hide();
            else $(element).css('width', '100%');
        });
        });
   

    $.ajax({
        url: "/Finder/GetDataFrom",
        method: "POST",
        dataType: "json",
        data: {
            q: "", 
            tempTable: PROGRAMTB_DIRECTORY,
            treeView: true,
        },
        success: function(data) {
            //// Creando el control treeview
            //// con el plugin de bootstrap 
            $('#DIRECTORY').treeview({
                data: JSON.stringify(data.items),
                // custom icons
                expandIcon: 'glyphicon glyphicon-folder-close',
                collapseIcon: 'glyphicon glyphicon-folder-open',
                emptyIcon: 'glyphicon',
                nodeIcon: '',
                selectedIcon: '',
                checkedIcon: 'glyphicon glyphicon-check',
                uncheckedIcon: 'glyphicon glyphicon-unchecked',
            });
        }
    });

    //// Select2
    //PMI
    //PRGMTYPENAME
    $("#PRGMTYPENAME").select2({
        placeholder: "Seleccione un PRGMTYPENAME",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Finder/GetDataFrom",
            dataType: 'json',
            type: "POST",
            data: function (term/*, page*/) {
                return {
                    q: term, // search term
                    tempTable: PRGMTYPETB_PRGMTYPENAME,
                    //page_limit: 10,
                    //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                };
            },
            results: function (data, page) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: $.map(data.items, function (item) {
                        return {
                            text: item.text,
                            id: item.id
                        }
                    })
                };
            }
        },
    });


    //SUBTYPENAME
    $("#SUBTYPENAME").select2({
        placeholder: "Seleccione un SUBTYPENAME",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Finder/GetDataFrom",
            dataType: 'json',
            type: "POST",
            data: function (term /*, page*/) {
                return {
                    q: term, // search term
                    tempTable: PRGMSUBTYPETB_SUBTYPENAME,
                    //page_limit: 10,
                    //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                };
            },
            results: function (data, page) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: $.map(data.items,
                        function (item) {
                            return {
                                text: item.text,
                                id: item.id
                            }
                        })
                };
            }
        },
    });

    //VIDEOFILENAME
    $("#VIDEOFILENAME").select2({
        placeholder: "Seleccione un VIDEOFILENAME",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Finder/GetDataFrom",
            dataType: 'json',
            type: "POST",
            data: function (term /*, page*/) {
                return {
                    q: term, // search term
                    tempTable: PROGRAMTB_VIDEOFILENAME,
                    //page_limit: 10,
                    //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                };
            },
            results: function (data, page) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: $.map(data.items,
                        function (item) {
                            return {
                                text: item.text,
                                id: item.id
                            }
                        })
                };
            }
        },
    });

    //PRGMNAME
    $("#PRGMNAME").select2({
        placeholder: "Seleccione un PRGMNAME",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Finder/GetDataFrom",
            dataType: 'json',
            type: "POST",
            data: function (term /*, page*/) {
                return {
                    q: term, // search term
                    tempTable: PROGRAMTB_PRGMNAME,
                    //page_limit: 10,
                    //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                };
            },
            results: function (data, page) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: $.map(data.items,
                        function (item) {
                            return {
                                text: item.text,
                                id: item.id
                            }
                        })
                };
            }
        },
    });

    //CREATOR
    $("#CREATOR").select2({
        placeholder: "Seleccione un CREATOR",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Finder/GetDataFrom",
            dataType: 'json',
            type: "POST",
            data: function (term /*, page*/) {
                return {
                    q: term, // search term
                    tempTable: PROGRAMTB_CREATOR,
                    //page_limit: 10,
                    //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                };
            },
            results: function (data, page) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: $.map(data.items,
                        function (item) {
                            return {
                                text: item.text,
                                id: item.id
                            }
                        })
                };
            }
        },
    });

    //EDITOR
    $("#EDITOR").select2({
        placeholder: "Seleccione un EDITOR",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Finder/GetDataFrom",
            dataType: 'json',
            type: "POST",
            data: function (term /*, page*/) {
                return {
                    q: term, // search term
                    tempTable: PROGRAMTB_EDITOR,
                    //page_limit: 10,
                    //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                };
            },
            results: function (data, page) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: $.map(data.items,
                        function (item) {
                            return {
                                text: item.text,
                                id: item.id
                            }
                        })
                };
            }
        },
    });

    //CHECKOR
    $("#CHECKOR").select2({
        placeholder: "Seleccione un CHECKOR",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Finder/GetDataFrom",
            dataType: 'json',
            type: "POST",
            data: function (term /*, page*/) {
                return {
                    q: term, // search term
                    tempTable: PROGRAMTB_CHECKOR,
                    //page_limit: 10,
                    //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                };
            },
            results: function (data, page) { // parse the results into the format expected by Select2.
                // since we are using custom formatting functions we do not need to alter remote JSON data
                return {
                    results: $.map(data.items,
                        function (item) {
                            return {
                                text: item.text,
                                id: item.id
                            }
                        })
                };
            }
        },
    });

    $('.durationpicker').durationPicker();
    
    function getCompTypeFromTabControl(id) {
        var mainControl = $('#' + id).find('#' + id + "_COMP_UL");
        var liActive = mainControl.find('li.active');
        var comp = liActive.first().attr('name');
        return comp;
    }

    function getRealValueFromTabControl(id, findInSelect = false) {
        var result = [];
        var mainControl = $('#' + id).find('#' + id + "_VAL_DIV");
        var tab_control_active = mainControl.find('div.tab-pane.active');
        var input = tab_control_active.find('input');
        if (input.length > 1) result = [input.get(0).value, input.get(1).value, selectedVal];
        else result = [input.get(0).value];
        if (findInSelect) {
            var selectedVal = tab_control_active.find('select').children('option:selected').val();
            result.push(selectedVal);
        }
        return result;
    }

    function getRealValueFromTabControl2(id) {
        var mainControl = $('#' + id).find('#' + id + "_VAL_DIV");
        var tab_pane = mainControl.find('div.tab-pane.active');
        var input = tab_pane.find('input');
        var tab_name = tab_pane.attr('name');
        if (input.get(0).name == 'daterange') {
            var start = input.get(1).value;
            var end = input.get(2).value;
            return [ tab_name, start, end];
        }
        var dateValue = input.val();
        return [tab_name, dateValue];
    }

    function getValueFromCustomSelect2(id) {
        var span = $('#s2id_' + id).find('span.select2-chosen');
        return span.html();
    }

    function getValueFromCustomTreeView() {
        var valueDIRECTORY = '';
        var selectedNodeId = $('#DIRECTORY').treeview('getSelected');
        if (selectedNodeId != undefined && selectedNodeId.length > 0) {
            var valueDIRECTORY = selectedNodeId[0].text + "\\";

            var parentTemp = $('#DIRECTORY').treeview('getParent', selectedNodeId);
            while (parentTemp.nodes != undefined) {
                valueDIRECTORY = parentTemp.text + "\\" + valueDIRECTORY;
                //alert(valueDIRECTORY);
                parentTemp = $('#DIRECTORY').treeview('getParent', parentTemp);
            }
        }
        return valueDIRECTORY != undefined ? valueDIRECTORY : "undefined";
    }

    function getValueFromCustomRadio(id) {
        var control = $('#' + id);
        var radioChecked = control.find('input[type=radio]:checked');
        return radioChecked.val();
    }

    function setValueToInput(idInput, val) {
        $('#' + idInput).val(val);
    }

    $('body').on('submit', '#formFinder', function (e) {

        e.preventDefault();
        //******Tipos numericos****
        //PRGMID
        var compTypePrgmId = getCompTypeFromTabControl('PRGMID_CONTROL');
        var valPrgmId = getRealValueFromTabControl('PRGMID_CONTROL');
        setValueToInput('PRGMID_COMP', compTypePrgmId);
        setValueToInput('PRGMID_VAL_1', valPrgmId[0]);
        if (valPrgmId.length > 1) setValueToInput('PRGMID_VAL_2', valPrgmId[1]);

        //DURATIONMS
        var compTypeDurationms = getCompTypeFromTabControl('DURATIONMS_CONTROL');
        var valDurationms = getRealValueFromTabControl('DURATIONMS_CONTROL');
        setValueToInput('DURATIONMS_COMP', compTypeDurationms);
        setValueToInput('DURATIONMS_VAL_1', valDurationms[0]);
        if (valDurationms.length > 1) setValueToInput('DURATIONMS_VAL_2', valDurationms[1]);

        //BITRATE
        var compTypeBitrate = getCompTypeFromTabControl('BITRATE_CONTROL');
        var valBitrate = getRealValueFromTabControl('BITRATE_CONTROL', true);
        setValueToInput('BITRATE_COMP', compTypeBitrate);
        setValueToInput('BITRATE_VAL_1', valBitrate[0]);
        if (valBitrate.length > 2) setValueToInput('BITRATE_VAL_2', valBitrate[1]);
        setValueToInput('BITRATE_TYPE', valBitrate[valBitrate.length - 1]);

        //FILESIZE
        var compTypeFilesize = getCompTypeFromTabControl('FILESIZE_CONTROL');
        var valFilesize = getRealValueFromTabControl('FILESIZE_CONTROL', true);
        setValueToInput('FILESIZE_COMP', compTypeFilesize);
        setValueToInput('FILESIZE_VAL_1', valFilesize[0]);
        if (valFilesize.length > 2) setValueToInput('FILESIZE_VAL_2', valFilesize[1]);
        setValueToInput('FILESIZE_TYPE', valFilesize[valFilesize.length - 1]);
        //******Tipos numericos FIN****

        //****Tipos fechas*******
        //CREATETIME
        var compTypeValCreateTime = getRealValueFromTabControl2('CREATETIME_CONTROL');
        setValueToInput('CREATETIME_COMP', compTypeValCreateTime[0]);
        setValueToInput('CREATETIME_VAL_1', compTypeValCreateTime[1]);
        if (compTypeValCreateTime.length > 2) setValueToInput('CREATETIME_VAL_2', compTypeValCreateTime[2]);

        //EDITTIME
        var compTypeValEditTime = getRealValueFromTabControl2('EDITTIME_CONTROL');
        setValueToInput('EDITTIME_COMP', compTypeValEditTime[0]);
        setValueToInput('EDITTIME_VAL_1', compTypeValEditTime[1]);
        if (compTypeValEditTime.length > 2) setValueToInput('EDITTIME_VAL_2', compTypeValEditTime[2]);

        //CHECKTIME
        var compTypeValCheckTime = getRealValueFromTabControl2('CHECKTIME_CONTROL');
        setValueToInput('CHECKTIME_COMP', compTypeValCheckTime[0]);
        setValueToInput('CHECKTIME_VAL_1', compTypeValCheckTime[1]);
        if (compTypeValEditTime.length > 2) setValueToInput('CHECKTIME_VAL_2', compTypeValCheckTime[2]);
        //****FIN Tipos fechas*******

        //****Select2*****
        //PRGMTYPENAME
        var valuePRGMTYPENAME = getValueFromCustomSelect2('PRGMTYPENAME');
        setValueToInput('PRGMTYPENAME_VAL', valuePRGMTYPENAME);

        //SUBTYPENAME
        var valueSUBTYPENAME = getValueFromCustomSelect2('SUBTYPENAME');
        setValueToInput('SUBTYPENAME_VAL', valueSUBTYPENAME);

        //VIDEOFILENAME
        //var valueVIDEOFILENAME = getValueFromCustomSelect2('VIDEOFILENAME');
        //setValueToInput('VIDEOFILENAME_VAL', valueVIDEOFILENAME);

        //PRGMNAME
        var valuePRGMNAME = getValueFromCustomSelect2('PRGMNAME');
        setValueToInput('PRGMNAME_VAL', valuePRGMNAME);

        //CREATOR
        var valueCREATOR = getValueFromCustomSelect2('CREATOR');
        setValueToInput('CREATOR_VAL', valueCREATOR);

        //EDITOR
        var valueEDITOR = getValueFromCustomSelect2('EDITOR');
        setValueToInput('EDITOR_VAL', valueEDITOR);

        //CHECKOR
        var valueCHECKOR = getValueFromCustomSelect2('CHECKOR');
        setValueToInput('CHECKOR_VAL', valueCHECKOR);
        //****Fin Select2****

        //DIRECTORY
        var valueDIRECTORY = getValueFromCustomTreeView();
        setValueToInput('DIRECTORY_VAL', valueDIRECTORY);

        //****Campos booleanos****
        //LOCKED
        var valueSelectedOptionRadioLOCKED = getValueFromCustomRadio('LOCKED_CONTROL');
        setValueToInput('LOCKED_VAL', valueSelectedOptionRadioLOCKED);

        //CHECKUP
        var valueSelectedOptionRadioCHECKUP = getValueFromCustomRadio('CHECKUP_CONTROL');
        setValueToInput('CHECKUP_VAL', valueSelectedOptionRadioCHECKUP);

        //****FIN Campos booleanos***

        /**
         * Implementacion del posteo del formulario
         * mediante ajax.
         */
        
        var formData = $('#formFinder').serialize();
        $.ajax({
            url: $('#formFinder').attr("action"), //recommended
            type: "POST",
            data: formData,
        }).done(function (result) {
            // do something with the result now
            console.log(result);
            if (result.status === "success" && result.data != null) {
                var data = result.data;
                modelData = data;
                var modalBodyResult = $('#modalBodyResult');
                var tableFirstPart =
                    '<table id="example" class="table table-striped table-bordered"> <thead ><tr><th>PID</th><th>PRGMNAME</th><th>PRGMTYPEID</th><th>PRGMTYPETB</th><th>DIRECTORY</th><th>DURATIONMS</th><th>CREATOR</th><th>CREATETIME</th><th>EDITOR</th><th>EDITTIME</th><th>SUBTYPEID</th><th>PRGMSUBTYPETB</th><th>BITRATE</th><th>FILESIZE</th><th>LOCKED</th><th>CHECKOR</th><th>CHECKTIME</th><th>CHECKUP</th></tr></thead ><tbody id="tableBodyResult">';
                var tableSecondPart =
                    '</tbody><tfoot ><tr><th>PID</th><th>PRGMNAME</th><th>PRGMTYPEID</th><th>PRGMTYPETB</th><th>DIRECTORY</th><th>DURATIONMS</th><th>CREATOR</th><th>CREATETIME</th><th>EDITOR</th><th>EDITTIME</th><th>SUBTYPEID</th><th>PRGMSUBTYPETB</th><th>BITRATE</th><th>FILESIZE</th><th>LOCKED</th><th>CHECKOR</th><th>CHECKTIME</th><th>CHECKUP</th></tr></tfoot ></table >';
                var rows = '';
                for (var i = 0; i < data.length; i++) {
                    var prgtb = data[i];
                    var prgtypename = prgtb.prgmtypetb != undefined ? prgtb.prgmtypetb.prgmtypename : '-';
                    var subtypename = prgtb.prgmsubtypetb != undefined ? prgtb.prgmsubtypetb.subtypename : '-';
                    var row =
                        '<tr> <td>' + prgtb.prgmid + '</td >' +
                            '<td>' + prgtb.prgmname + '</td>' +
                            '<td>' + prgtb.prgmtypeid + '</td>' +
                            '<td>' + prgtypename + '</td>' +
                            //'<td>' + prgtb.videofilename + '</td>' +
                            '<td>' + prgtb.directory + '</td>' +
                            '<td>' + prgtb.durationms + '</td>' +
                            '<td>' + prgtb.creator + '</td>' +
                            '<td>' + prgtb.createtime + '</td>' +
                            '<td>' + prgtb.editor + '</td>' +
                            '<td>' + prgtb.edittime + '</td>' +
                            '<td>' + prgtb.subtypeid + '</td>' +
                            '<td>' + subtypename + '</td>' +
                            '<td>' + prgtb.bitrate + '</td>' +
                            '<td>' + prgtb.filezise + '</td>' +
                            '<td>' + prgtb.locked + '</td>' +
                            '<td>' + prgtb.checkor + '</td>' +
                            '<td>' + prgtb.checktime + '</td>' +
                            '<td>' + prgtb.checkup + '</td>' +
                            '</tr >';
                    rows += row;
                }

                var htmlResult = tableFirstPart + rows + tableSecondPart;
                modalBodyResult.html(htmlResult);
                var dataTable = $('#example').DataTable({
                    "scrollX": true
                });
                $('#modalResult').modal().show();
                dataTable.columns.adjust();

            } else {
                alert("No se encontraron resultados");
            }
        });

    });
});

