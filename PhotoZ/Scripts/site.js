function initializeModal() {
    var modal = document.getElementById('myModal');
    var modalImg = document.getElementById("imgModal");
    var captionText = document.getElementById("caption");

    var thumbnails = document.querySelectorAll('[thumbnail]');
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