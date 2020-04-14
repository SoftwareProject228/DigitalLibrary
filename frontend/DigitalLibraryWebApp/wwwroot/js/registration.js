/*!
 * jQuery CLI
 * Simulating a command line interface with jQuery
 */
(function(e) {
    "use strict";
    e.fn.textTyper = function(t) {
        var n = {
                typingClass: "typing",
                beforeAnimation: function() {},
                afterAnimation: function() {},
                speed: 10,
                nextLineDelay: 400,
                startsFrom: 0,
                repeatAnimation: false,
                repeatDelay: 4e3,
                repeatTimes: 1,
                cursorHtml: '<span class="cursor">|</span>'
            },
            r = e.extend({}, n, t);
        this.each(function() {
            var t = e(this),
                n = 1,
                i = "typingCursor";
            var s = t,
                o = s.length,
                u = [];
            while (o--) {
                u[o] = e.trim(e(s[o]).html());
                e(s[o]).html("");
            }
            t.init = function() {
                var n = r.beforeAnimation;
                if (n) n();
                t.animate(0);
            };
            t.animate = function(o) {
                var a = s[o],
                    f = r.typingClass,
                    l = r.startsFrom;
                e(a).addClass(f);
                var c = setInterval(function() {
                        var f = r.cursorHtml;
                        f = e("<div>").append(e(f).addClass(i)).html();
                        e(a).html(u[o].substr(0, l) + f);
                        l++;
                        if (u[o].length < l) {
                            clearInterval(c);
                            o++;
                            if (s[o]) {
                                setTimeout(function() {
                                        e(a).html(u[o - 1]);
                                        t.animate(o);
                                    },
                                    r.nextLineDelay);
                            } else {
                                e(a).find("." + i).remove();
                                if (r.repeatAnimation && (r.repeatTimes == 0 || n < r.repeatTimes)) {
                                    setTimeout(function() {
                                            t.animate(0);
                                            n++;
                                        },
                                        r.repeatDelay);
                                } else {
                                    var h = r.afterAnimation;
                                    if (h) h();
                                }
                            }
                        }
                    },
                    r.speed);
            };
            t.init();
        });
        return this;
    }
})(jQuery);

// Handler for .ready() - event will be triggered only after the entire page is formed
$(document).ready(function () {
    $("#submit_bt").prop("disabled", true);
    $(".email").hide();
    $(".name").hide();
    $(".login").hide();
    $(".password").hide();
    $("#registration_page").textTyper({
        speed: 20,
        afterAnimation: function() {

            $(".email").fadeIn(); // .fadeIn();//element .command appear through 400 ms
            $("#email").focus(); //The focused element is the element which will receive keyboard and similar events by default
            $("#email").keyup(function(e) {
                //if enter was pressed
                if (e.which == 13) {
                    var email = $('#email').val(); //Gets the value of the value attribute
                    console.log(email);

                    $(".name").fadeIn(); // .fadeIn();//element .command appear through 400 ms
                    $("#name")
                        .focus(); //The focused element is the element which will receive keyboard and similar events by default
                    $("#name").keyup(function(e) {
                        //if second enter was pressed
                        if (e.which == 13) {
                            var name = $("#name").val(); //Gets the value of the value attribute
                            console.log(name);

                            $(".login").fadeIn(); // .fadeIn();//element .command appear through 400 ms
                            $("#login")
                                .focus(); //The focused element is the element which will receive keyboard and similar events by default
                            $("#login").keyup(function(e) {
                                //if second enter was pressed
                                if (e.which == 13) {
                                    var login = $("#login").val(); //Gets the value of the value attribute
                                    console.log(login);

                                    $(".password").fadeIn(); // .fadeIn();//element .command appear through 400 ms
                                    $("#password")
                                        .focus(); //The focused element is the element which will receive keyboard and similar events by default
                                    $("#password");
                                    $("#submit_bt").prop("disabled", false);
                                }
                            });

                        }
                    });

                }
            });

        }
    });
});
