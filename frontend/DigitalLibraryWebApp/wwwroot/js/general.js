// Output to Console
function output(print) {
  var cmd = $('.console-input').val();
  console.log(cmd);
  if(cmd==""){cmd="<span style='opacity:0;'>...</span>";}
  $("#outputs").append("<span class='output-cmd-pre'>User:/$</span><span class='output-cmd'>" + cmd + "</span>");

  $.each(print, function(index, value) {
    cmd = "Vasya";
    cmd += ":/$";
    if (value == "") {
      value = "&nbsp;";
    }
    $("#outputs").append("<span class='output-text-pre'>" + cmd + "</span><span class='output-text'>" + value + "</span>");
  });

  $('.console-input').val("");
  //$('.console-input').focus();
  $("html, body").animate({
    scrollTop: $(document).height()
  }, 300);
}

function add(value, cmd) {
  cmd += ":/$";
  if (value == "") {
    value = "&nbsp;";
  }
  $("#outputs").append("<span class='output-text-pre'>" + cmd + "</span><span class='output-text'>" + value + "</span>");
}

function uploading(print) {
  var cmd = $('.console-input').val();
  console.log(cmd);
  if(cmd==""){cmd="<span style='opacity:0;'>...</span>";}
  $("#outputs").append("<span class='output-cmd-pre'>User:/$</span><span class='output-cmd'>" + cmd + "</span>");

  add(print[0], "Vasya");
  add(print[1], "Name");
  add(print[2], "Upload");

  $('.console-input').val("");
  //$('.console-input').focus();
  $("html, body").animate({
    scrollTop: $(document).height()
  }, 300);
}

// Break Value
var newLine = "<br/> &nbsp;";

// User Commands
var cmds = {

  "reset": function() {
    window.location.replace(location.href);
  },

  "clear": function() {
    $("#outputs").html("");
  },

  "help": function() {

    var print = ["Commands:"];
    print = $.merge(print, Object.keys(cmds));

    output(print);
  },

  "about": function() {
    var print = ["This is the command line of the experiment 'Vasilisa Premudraya'. With a lot of luck, in this digital library you can find the books you need to learn."];
    output(print);
  },

// Form to submit to server
  "upload": function() {
    var print = ["You have an unique opportunity to leave a mark on history and upload your material to this library.",
      "<form action=\"\"><input type=\"text\" class=\"form-input\" placeholder=\"type name of the file...\" required/>",
      "<input name=\"myFile\" type=\"file\" id=\"file\" class=\"inputfile\" required><label for=\"file\" id=\"fileLabel\">Choose file</label><input type=\"submit\" id=\"fileSubmit\" value=\"Submit\"></form>"
    ];
    uploading(print);
    let input = $('#file');
    input.on('change', (e) => {
      if( this.files && this.files.length > 1 )
        fileName = ( this.getAttribute( 'data-multiple-caption' ) || '' ).replace( '{count}', this.files.length );
      else
        fileName = e.target.value.split('\\').pop();

      if( fileName )
        $( '#fileLabel' ).html(fileName);

    });
  },

  "library": function() {
    var print = ["<p class=\"book_list\">NAME FROM SERVER <a href=\"LINK FROM SERVER\">Download</a> </p>",
      "<p class=\"book_list\">NAME FROM SERVER <a href=\"LINK FROM SERVER\">Download</a> </p>"];
    output(print);
  },

  "delete": function() {
    var print = ["<p class=\"book_list\">NAME FROM SERVER <a class=\"delete_button\">Delete</a> </p>",
      "<p class=\"book_list\">NAME FROM SERVER <a class=\"delete_button\">Delete</a> </p>"];
    output(print);
    deleted = $('.delete_button');
    deleted.on('click', (e) =>{
      console.log($(e.target).parent());
      parent = $($($(e.target).parent()).parent());
      parent.prev().detach();
      parent.detach();
      // $($(e.target).parent()).detach();
    });
  },

};

// Starting message
$('.console-input').val("start");
  var print = ["Welcome to the digital library 'Vasilisa Premudraya'. Type 'help' + ENTER -- for available commands."];
  output(print);

// Get User Command
$('.console-input').on('keypress', function(event) {
  if (event.which === 13) {
    var str = $(this).val();
    var data = str.split(' '); data.shift(); data = data.join(' ');
    var cmd = str.split(' ')[0];

    if (typeof cmds[cmd] == 'function') {
      if(cmds[cmd].length > 0) {
        cmds[cmd](data);
      } else {
        cmds[cmd]();
      }
    } else {
      output(["Command not found: '" + cmd + "'", "Use '/help' for list of commands."]);
    }
    $(this).val("");
  }
});
