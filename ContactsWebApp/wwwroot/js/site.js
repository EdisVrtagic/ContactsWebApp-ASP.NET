
function FillCities(listCountryCtrl, listCityId) {
    var listCities = $("#" + listCityId);
    listCities.empty();

    var selectedCountry = listCountryCtrl.options[listCountryCtrl.selectedIndex].value;

    if (selectedCountry != null && selectedCountry != '') {
        $.getJSON("/home/GetCitiesByCountry", { countryId: selectedCountry }, function (cities) {
            if (cities != null && !jQuery.isEmptyObject(cities)) {
                $.each(cities, function (index, city) {
                    listCities.append($('<option/>',
                        {
                            value: city.value,
                            text: city.text
                        }));
                });
            };
        });
    }

    return;
}
function CountryChanged() {
    var listCountryCtrl = document.getElementById("CountryId");
    var listCityId = "CityId";
    FillCities(listCountryCtrl, listCityId);
}
