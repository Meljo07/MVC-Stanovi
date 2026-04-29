function dodajStan() {
    alert("radi");


    let slika = document.getElementById("slikaInput").value;
    let lokacija = document.getElementById("lokacijaInput").value;
    let povrsina = document.getElementById("povrsinaInput").value;
    let cena = document.getElementById("cenaInput").value;
    let adresa = document.getElementById("adresaInput").value;

    let maliDiv = document.createElement("div");
    maliDiv.className = "stan";

    let img = document.createElement("img");
    img.src = slika;
    img.style.width = "250px";

    let tekst = document.createElement("p");
    tekst.innerText =
        "Lokacija: " + lokacija +
        "\nPovršina: " + povrsina +
        "\nCena: " + cena +
        "\nAdresa: " + adresa;

    maliDiv.appendChild(img);
    maliDiv.appendChild(tekst);

    document.getElementById("velikiDiv").appendChild(maliDiv);