//Skripta koja konvertuje broj telefona u zadani format npr XXX/XXX-XXX i ne dopusta unos teksta i znakova, nista osim brojeva
$(function () {
    $('#PhoneNumber').on('input', function () {
        let phoneNumber = $(this).val();
        phoneNumber = phoneNumber.replace(/\D/g, '');
        if (phoneNumber.length >= 3) {
            phoneNumber = phoneNumber.replace(/(\d{3})(\d{0,3})(\d{0,3})/, '$1/$2-$3');
        }
        $(this).val(phoneNumber);
    });
});
