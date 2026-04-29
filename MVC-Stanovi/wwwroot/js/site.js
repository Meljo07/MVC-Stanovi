function dodajStan() {
    let url = document.getElementById("slikaInput").value;

    let maliDiv = document.createElement("div");
    maliDiv.className = "stan";

    let img = document.createElement("img");
    img.src = url;
    img.style.width = "100px";

    maliDiv.appendChild(img);

    document.getElementById("velikiDiv").appendChild(maliDiv);
}