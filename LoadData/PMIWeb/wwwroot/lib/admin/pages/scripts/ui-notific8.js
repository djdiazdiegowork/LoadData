var UINotific8 = function () {

    return {
        //main function to initiate the module
        init: function () {

            
                    $('#notific8_show').click(function(event) {
                        var settings = {
                                theme: $('#notific8_theme').val(),
                                sticky: $('#notific8_sticky').is(':checked'),
                                horizontalEdge: $('#notific8_pos_hor').val(),
                                verticalEdge: $('#notific8_pos_ver').val()
                            },
                            $button = $(this);
                        
                        if ($.trim($('#notific8_heading').val()) != '') {
                            settings.heading = $.trim($('#notific8_heading').val());
                        }
                        
                        if (!settings.sticky) {
                            settings.life = $('#notific8_life').val();
                        }

                        $.notific8('zindex', 11500);
                        $.notific8($.trim($('#notific8_text').val()), settings);
                        
                        $button.attr('disabled', 'disabled');
                        
                        setTimeout(function() {
                            $button.removeAttr('disabled');
                        }, 1000);

                    });


            $('body').on('submit', 'form', function (e) {
                var select = $('select')[0];
                if(select.selectedOptions.length == 0) {

                    var settings = {
                            theme: "ruby",
                            sticky: false,
                            horizontalEdge: "top",
                            verticalEdge: "right"
                        },
                        $button = $(this);

                    if ($.trim($('#notific8_heading').val()) != '') {
                        settings.heading = $.trim($('#notific8_heading').val());
                    }

                    if (!settings.sticky) {
                        settings.life = 3000;
                    }

                    $.notific8('zindex', 11500);
                    $.notific8($.trim('Seleccione los usuarios que veran sus comentarios'), settings);

                    $button.attr('disabled', 'disabled');

                    setTimeout(function() {
                        $button.removeAttr('disabled');
                    }, 1000);


                    return false;
                }

            });


        }

    };

}();