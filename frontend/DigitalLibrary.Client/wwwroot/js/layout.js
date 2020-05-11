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
  $('.books_list').hide();
  $('.courses_list').hide();
  $('.name_text').hide();
  $('.upload_text').hide();
  $('.filupp-file-name').hide();
  $('.button_to_upload').hide();
  $('.command').hide();

  // our first action is to show intro section with animation
  $('.sect').addClass('open');
  $('.sect').textTyper({
        speed:20,
        afterAnimation:function(){
          $('.books_list').fadeIn();//this is for general, delete and library html
          $('.courses_list').fadeIn();//this is for courses html
          $('.name_text').fadeIn();//this is for upload cursor html

          //this part is for delete html
          deleted = $('.delete_button');
          deleted.on('click', (e) =>{
            $($(e.target).parent()).detach();
          });

          //now we can show the last field for the navigation command
          $('.command').fadeIn();//element .command appear through 400 ms
          $('input[type="text"]').focus();//The focused element is the element which will receive keyboard and similar events by default
          $('input[type="text"]').val('');//Gets the value of the value attribute

          //this part is for upload html
          $('.upload_text').fadeIn();// .fadeIn();//element .command appear through 400 ms
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
function focusElement(id) {
    const element = document.getElementById(id);
    element.focus();
}