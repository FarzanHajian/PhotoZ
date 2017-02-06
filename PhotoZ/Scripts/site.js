function initializeModal() {
    var modal = document.getElementById('myModal');
    var modalImg = document.getElementById("imgModal");
    var captionText = document.getElementById("caption");

    var thumbnails = document.querySelectorAll('.photoz-thumbnail');
    for (index in thumbnails) {
        thumbnails[index].onclick = function () {
            modal.style.display = "block";
            modalImg.src = this.src;
            captionText.innerHTML = this.alt;
        };
    }

    var span = document.getElementsByClassName("close")[0];
    span.onclick = function () {
        modal.style.display = "none";
    }
}

function initializeBackToTopButton() {
    var amountScrolled = 300;

    $(window).scroll(function () {
        if ($(window).scrollTop() > amountScrolled) {
            $('a.back-to-top').fadeIn('slow');
        } else {
            $('a.back-to-top').fadeOut('slow');
        }
    });

    $('a.back-to-top').click(function () {
        $('html, body').animate({
            scrollTop: 0
        }, 700);
        return false;
    });
}