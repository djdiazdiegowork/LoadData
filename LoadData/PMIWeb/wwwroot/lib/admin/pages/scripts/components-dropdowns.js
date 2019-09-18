var ComponentsDropdowns = function () {

    var handleSelect2 = function () {

        $('#select2_sample1').select2({
            placeholder: "Select an option",
            allowClear: true
        });

        $('#select2_sample2').select2({
            placeholder: "Select a State",
            allowClear: true
        });

        $("#select2_sample3").select2({
            placeholder: "Select...",
            allowClear: true,
            minimumInputLength: 1,
            query: function (query) {
                var data = {
                    results: []
                }, i, j, s;
                for (i = 1; i < 5; i++) {
                    s = "";
                    for (j = 0; j < i; j++) {
                        s = s + query.term;
                    }
                    data.results.push({
                        id: query.term + i,
                        text: s
                    });
                }
                query.callback(data);
            }
        });

        function format(state) {
            if (!state.id) return state.text; // optgroup
            return "<img class='flag' src='" + Metronic.getGlobalImgPath() + "flags/" + state.id.toLowerCase() + ".png'/>&nbsp;&nbsp;" + state.text;
        }
        $("#select2_sample4").select2({
            placeholder: "Select a Country",
            allowClear: true,
            formatResult: format,
            formatSelection: format,
            escapeMarkup: function (m) {
                return m;
            }
        });

        $("#select2_sample5").select2({
            tags: ["red", "green", "blue", "yellow", "pink"]
        });

        $('.app-multiselct').select2(
            {
                placeholder: "Seleccione los grupos de permisos",
                allowClear: true,
            }
        );

        $('.app-multiselct-ft').select2(
            {
                placeholder: "Seleccione la ficha técnica",
                allowClear: true,
            }
        );

        $('.app-multiselct-at').select2(
            {
                placeholder: "Seleccione los autores",
                allowClear: true,
            }
        );

        $('.app-multiselct-gn').select2(
            {
                placeholder: "Seleccione el género",
                allowClear: true,
            }
        );

        $('.app-multiselct-gr').select2(
            {
                placeholder: "Seleccione el grupo de televisión",
                allowClear: true,
            }
        );

        $('.app-multiselct-per').select2(
            {
                placeholder: "Seleccione la persona",
                allowClear: true,
            }
        );

        $('.app-multiselct-integr').select2(
            {
                placeholder: "Seleccione los integrantes",
                allowClear: true,
            }
        );

        $('.app-multiselct-aspPosit').select2(
            {
            }
        );

        $('.app-multiselct-aspNeg').select2(
            {

            }
        );

        $('.app-multiselct-aspVal').select2(
            {

            }
        );

        $('.app-multiselct-aspSug').select2(
            {

            }
        );

        $('.app-multiselct-aspCon').select2(
            {

            }
        );

        $('.app-multiselct-usuarperm').select2(
            {
                placeholder: "Seleccione los usuarios",
                allowClear: true,
            }
        );

        $('.app-multiselct-escr').select2(
            {
                placeholder: "Seleccione los escritores",
                allowClear: true,
            }
        );

        $('.app-multiselct-ases').select2(
            {
                placeholder: "Seleccione el asesor",
                allowClear: true,
            }
        );

        $('.app-multiselct-direcProd').select2(
            {
                placeholder: "Seleccione el director-productor",
                allowClear: true,
            }
        );

        $('.app-multiselct-pres').select2(
            {
                placeholder: "Seleccione el presidente de la comisión",
                allowClear: true,
            }
        );

        $('.app-multiselct-prod').select2(
            {
                placeholder: "Seleccione el Productor",
                allowClear: true,
            }
        );

        $('.app-multiselct-direc').select2(
            {
                placeholder: "Seleccione el Director",
                allowClear: true,
            }
        );

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
                        tempTable: PROGRAMTB_PRGMNAME,
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
                data: function(term /*, page*/) {
                    return {
                        q: term, // search term
                        tempTable: PRGMSUBTYPETB_SUBTYPENAME,
                        //page_limit: 10,
                        //apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                    };
                },
                results: function(data, page) { // parse the results into the format expected by Select2.
                    // since we are using custom formatting functions we do not need to alter remote JSON data
                    return {
                        results: $.map(data.items,
                            function(item) {
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

        function movieFormatResult(movie) {
            var markup = "<table class='movie-result'><tr>";
            if (movie.posters !== undefined && movie.posters.thumbnail !== undefined) {
                markup += "<td valign='top'><img src='" + movie.posters.thumbnail + "'/></td>";
            }
            markup += "<td valign='top'><h5>" + movie.title + "</h5>";
            if (movie.critics_consensus !== undefined) {
                markup += "<div class='movie-synopsis'>" + movie.critics_consensus + "</div>";
            } else if (movie.synopsis !== undefined) {
                markup += "<div class='movie-synopsis'>" + movie.synopsis + "</div>";
            }
            markup += "</td></tr></table>"
            return markup;
        }

        function movieFormatSelection(movie) {
            return movie.title;
        }

        $("#select2_sample6").select2({
            placeholder: "Search for a movie",
            minimumInputLength: 1,
            ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
                url: "http://api.rottentomatoes.com/api/public/v1.0/movies.json",
                dataType: 'jsonp',
                data: function (term, page) {
                    return {
                        q: term, // search term
                        page_limit: 10,
                        apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                    };
                },
                results: function (data, page) { // parse the results into the format expected by Select2.
                    // since we are using custom formatting functions we do not need to alter remote JSON data
                    return {
                        results: data.movies
                    };
                }
            },
            initSelection: function (element, callback) {
                // the input tag has a value attribute preloaded that points to a preselected movie's id
                // this function resolves that id attribute to an object that select2 can render
                // using its formatResult renderer - that way the movie name is shown preselected
                var id = $(element).val();
                if (id !== "") {
                    $.ajax("http://api.rottentomatoes.com/api/public/v1.0/movies/" + id + ".json", {
                        data: {
                            apikey: "ju6z9mjyajq2djue3gbvv26t"
                        },
                        dataType: "jsonp"
                    }).done(function (data) {
                        callback(data);
                    });
                }
            },
            formatResult: movieFormatResult, // omitted for brevity, see the source of this page
            formatSelection: movieFormatSelection, // omitted for brevity, see the source of this page
            dropdownCssClass: "bigdrop", // apply css that makes the dropdown taller
            escapeMarkup: function (m) {
                return m;
            } // we do not want to escape markup since we are displaying html in results
        });
    }

    var handleSelect2Modal = function () {

        $('#select2_sample_modal_1').select2({
            placeholder: "Select an option",
            allowClear: true
        });

        $('#select2_sample_modal_2').select2({
            placeholder: "Select a State",
            allowClear: true
        });

        $("#select2_sample_modal_3").select2({
            allowClear: true,
            minimumInputLength: 1,
            query: function (query) {
                var data = {
                    results: []
                }, i, j, s;
                for (i = 1; i < 5; i++) {
                    s = "";
                    for (j = 0; j < i; j++) {
                        s = s + query.term;
                    }
                    data.results.push({
                        id: query.term + i,
                        text: s
                    });
                }
                query.callback(data);
            }
        });

        function format(state) {
            if (!state.id) return state.text; // optgroup
            return "<img class='flag' src='" + Metronic.getGlobalImgPath() + "flags/" + state.id.toLowerCase() + ".png'/>&nbsp;&nbsp;" + state.text;
        }
        $("#select2_sample_modal_4").select2({
            allowClear: true,
            formatResult: format,
            formatSelection: format,
            escapeMarkup: function (m) {
                return m;
            }
        });

        $("#select2_sample_modal_5").select2({
            tags: ["red", "green", "blue", "yellow", "pink"]
        });


        function movieFormatResult(movie) {
            var markup = "<table class='movie-result'><tr>";
            if (movie.posters !== undefined && movie.posters.thumbnail !== undefined) {
                markup += "<td valign='top'><img src='" + movie.posters.thumbnail + "'/></td>";
            }
            markup += "<td valign='top'><h5>" + movie.title + "</h5>";
            if (movie.critics_consensus !== undefined) {
                markup += "<div class='movie-synopsis'>" + movie.critics_consensus + "</div>";
            } else if (movie.synopsis !== undefined) {
                markup += "<div class='movie-synopsis'>" + movie.synopsis + "</div>";
            }
            markup += "</td></tr></table>"
            return markup;
        }

        function movieFormatSelection(movie) {
            return movie.title;
        }

        $("#select2_sample_modal_6").select2({
            placeholder: "Search for a movie",
            minimumInputLength: 1,
            ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
                url: "http://api.rottentomatoes.com/api/public/v1.0/movies.json",
                dataType: 'jsonp',
                data: function (term, page) {
                    return {
                        q: term, // search term
                        page_limit: 10,
                        apikey: "ju6z9mjyajq2djue3gbvv26t" // please do not use so this example keeps working
                    };
                },
                results: function (data, page) { // parse the results into the format expected by Select2.
                    // since we are using custom formatting functions we do not need to alter remote JSON data
                    return {
                        results: data.movies
                    };
                }
            },
            initSelection: function (element, callback) {
                // the input tag has a value attribute preloaded that points to a preselected movie's id
                // this function resolves that id attribute to an object that select2 can render
                // using its formatResult renderer - that way the movie name is shown preselected
                var id = $(element).val();
                if (id !== "") {
                    $.ajax("http://api.rottentomatoes.com/api/public/v1.0/movies/" + id + ".json", {
                        data: {
                            apikey: "ju6z9mjyajq2djue3gbvv26t"
                        },
                        dataType: "jsonp"
                    }).done(function (data) {
                        callback(data);
                    });
                }
            },
            formatResult: movieFormatResult, // omitted for brevity, see the source of this page
            formatSelection: movieFormatSelection, // omitted for brevity, see the source of this page
            dropdownCssClass: "bigdrop", // apply css that makes the dropdown taller
            escapeMarkup: function (m) {
                return m;
            } // we do not want to escape markup since we are displaying html in results
        });
    }

    var handleBootstrapSelect = function() {
        $('.bs-select').selectpicker({
            iconBase: 'fa',
            tickIcon: 'fa-check'
        });
    }

    var handleMultiSelect = function () {
        $('#my_multi_select1').multiSelect();
        $('#my_multi_select2').multiSelect({
            selectableOptgroup: true
        });
    }

    return {
        //main function to initiate the module
        init: function () {            
            handleSelect2();
            handleSelect2Modal();
            handleMultiSelect();
            handleBootstrapSelect();
        }
    };

}();