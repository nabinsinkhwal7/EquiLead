// next prev

//show active step
function showActiveStep()
{
    if ($('#step1').is(':visible'))
    {
        $(".step-bar .move").css('width', '25%');
        $(".step-rate span").html("25%");
    }
    else if ($('#step2').is(':visible'))
    {
        $(".step-bar .move").css('width', '50%');
        $(".step-rate span").html("50%");
    }
    else if ($('#step3').is(':visible'))
    {
        $(".step-bar .move").css('width', '75%');
        $(".step-rate span").html("75%");
    }
    else
    {
        $(".step-bar .move").css('width', '100%');
        $(".step-rate span").html("100%");
    }
}


// next prev
var divs = $('.show-section section');
var now = 0; // currently shown div
divs.hide().first().show(); // hide all divs except first

function next()
{
    divs.eq(now).hide();
    now = (now + 1 < divs.length) ? now + 1 : 0;
    divs.eq(now).show(); // show next
    showActiveStep();
}

$(".prev").click(function() {
    divs.eq(now).hide();
    now = (now > 0) ? now - 1 : divs.length - 1;
    divs.eq(now).show(); // show previous
    showActiveStep();
});

var allstars = document.querySelectorAll(".star");

allstars.forEach((star, i) => 
{
    star.onclick = function()
    {
        let current_level_star = i + 1;
        allstars.forEach((star, j) =>
        {
            if(current_level_star >= j+1)
            {
                star.classList.add('fa-solid');
                star.classList.remove('fa-regular');
            }
            else
            {
                star.classList.add('fa-regular');
                star.classList.remove('fa-solid');
            }
            $('#starselection').html(current_level_star);
        })   
    }
      
});





// quiz validation
var checkedradio = false;

function radiovalidate(stepnumber)
{
    var checkradio = $("#step"+stepnumber+" input").map(function()
    {
    if($(this).is(':checked'))
    {
        return true;
    }
    else
    {
        return false;
    }
    }).get();

    checkedradio = checkradio.some(Boolean);
}



$(document).ready(function()
{
  setTimeout(function()
  {
    $("#step1 .radio-field").each(function()
    {
      $(this).addClass('revealfield');
    })
  }, 1000)

  setTimeout(function()
  {
    $("#step2 .radio-field-2").each(function()
    {
      $(this).addClass('revealfield');
    })
  }, 1000)

})




// disable on enter
$('form').on('keyup keypress', function(e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) { 
      e.preventDefault();
      return false;
    }
  });
  
  



// form validiation













