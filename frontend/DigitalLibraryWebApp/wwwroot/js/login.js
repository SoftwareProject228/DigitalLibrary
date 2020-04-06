/*!
 * jQuery CLI
 * Simulating a command line interface with jQuery
 */
var destination_password;
var destination_login;

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
            t.init = function(e) {
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
$(document).ready(function() {
    $("#submit_bt").prop("disabled", true);
    $(".login_text").hide();
    $(".password_text").hide();
    $("#login_page").textTyper({
        speed: 20,
        afterAnimation: function() {

            $(".login_text").fadeIn(); // .fadeIn();//element .command appear through 400 ms
            $("#login_text")
                .focus(); //The focused element is the element which will receive keyboard and similar events by default
            $("#login_text").keyup(function(e) {
                //if enter was pressed
                if (e.which == 13) {
                    $("#submit_bt").prop("disabled", false);
                    $(".password_text").fadeIn(); // .fadeIn();//element .command appear through 400 ms
                    $("#password_text")
                        .focus(); //The focused element is the element which will receive keyboard and similar events by default
                }
            });

        }
    });

});