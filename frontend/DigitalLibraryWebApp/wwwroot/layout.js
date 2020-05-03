/*!
 * jQuery CLI
 * Simulating a command line interface with jQuery
 */
(function(e){
  "use strict";
  e.fn.textTyper=function(t){
    var n={typingClass:"typing",
    beforeAnimation:function(){},
    afterAnimation:function(){},
    speed:10,
    nextLineDelay:400,
    startsFrom:0,
    repeatAnimation:false,
    repeatDelay:4e3,
    repeatTimes:1,
    cursorHtml:'<span class="cursor">|</span>'
  },
  r=e.extend({},n,t);
  this.each(function(){
    var t=e(this),
    n=1,
    i="typingCursor";
    var s=t,
    o=s.length,
    u=[];
    while(o--){
      u[o]=e.trim(e(s[o]).html());
      e(s[o]).html("")
    }
    t.init=function(e){
      var n=r.beforeAnimation;
      if(n)n();
      t.animate(0)
    };
    t.animate=function(o){
      var a=s[o],
      f=r.typingClass,
      l=r.startsFrom;
      e(a).addClass(f);
      var c=setInterval(function(){
        var f=r.cursorHtml;
        f=e("<div>").append(e(f).addClass(i)).html();
        e(a).html(u[o].substr(0,l)+f);
        l++;
        if(u[o].length<l){
          clearInterval(c);
          o++;
          if(s[o]){
            setTimeout(function(){
              e(a).html(u[o-1]);
              t.animate(o)
            },
            r.nextLineDelay)
          }else{
            e(a).find("."+i).remove();
            if(r.repeatAnimation&&(r.repeatTimes==0||n<r.repeatTimes)){
              setTimeout(function(){
                t.animate(0);
                n++
              },
              r.repeatDelay)
            }else{
              var h=r.afterAnimation;
              if(h)h()
            }
          }
        }
      },r.speed)};
    t.init()
  });
  return this}})(jQuery)

// Handler for .ready() - event will be triggered only after the entire page is formed
$(document).ready(function() {

  // initially we should hide all fields
  //registration fields
  $('.email').hide();
  $('.name').hide();
  $('.login').hide();
  $('.password').hide();
  //login fields
  $('.login_text').hide();
  $('.password_text').hide();
  //library
  $('.books_list').hide();
  //upload
  $('.name_prefix').hide();
  $('.upload_prefix').hide();
  $('.filupp-file-name').hide();
  $('.button_to_upload').hide();
  //all
  $('.command').hide();

  // our first action is to show intro section with animation
  $('.intro').textTyper({
        speed:20,
        afterAnimation:function(){
          //login-animation part
          $('.login_text').fadeIn();// .fadeIn();//element .command appear through 400 ms
          $('#login_text').focus();//The focused element is the element which will receive keyboard and similar events by default
          $('#login_text').keydown(function(e){
            //if enter was pressed
            if(e.which == 13){
              $('.password_text').fadeIn();// .fadeIn();//element .command appear through 400 ms
              $('#password_text').focus();//The focused element is the element which will receive keyboard and similar events by default
            }
          });

          //registraton-animation part
          $('.email').fadeIn();// .fadeIn();//element .command appear through 400 ms
          $('#email').focus();//The focused element is the element which will receive keyboard and similar events by default
          $('#email').keyup(function(e){
            //if enter was pressed
            if(e.which == 13){
              $('.name').fadeIn();// .fadeIn();//element .command appear through 400 ms
              $('#name').focus();//The focused element is the element which will receive keyboard and similar events by default
              $('#name').keyup(function(e){
                //if second enter was pressed
                if(e.which == 13){
                  $('.login').fadeIn();// .fadeIn();//element .command appear through 400 ms
                  $('#login').focus();//The focused element is the element which will receive keyboard and similar events by default
                  $('#login').keyup(function(e){
                    //if second enter was pressed
                    if(e.which == 13){
                      $('.password').fadeIn();// .fadeIn();//element .command appear through 400 ms
                      $('#password').focus();//The focused element is the element which will receive keyboard and similar events by default
                    }
                  });

                }
              });

            }
          });

          $('.books_list').fadeIn();//this is for general, delete and library html
          $('.courses_list').fadeIn();//this is for courses html
          $('.name_prefix').fadeIn();//this is for upload cursor html

          //now we can show the last field for the navigation command
          $('.command').fadeIn();//element .command appear through 400 ms
          $('input[type="text"]').focus();//The focused element is the element which will receive keyboard and similar events by default
          $('input[type="text"]').val('');//Gets the value of the value attribute

          //this part is for upload html
          $('.upload_prefix').fadeIn();// .fadeIn();//element .command appear through 400 ms
          $('.filupp-file-name').fadeIn();// .fadeIn();//element .command appear through 400 ms
          $('input[type="file"]').change(function(){
              var value = $("input[type='file']").val();
              $('.js-value').fadeOut(0).text(value).fadeIn(400, ()=>{
                $('.button_to_upload').fadeIn(400);
              });
          });

        }
    });

});
