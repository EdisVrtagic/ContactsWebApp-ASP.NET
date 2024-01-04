//Skripta za zatvaranje bootstrap modalnog prozora ( dodano da se zatvara u slucaju izlaska sa modala na X, button Cancel, ESC na tastaturi, i nakon kreiranja kontakta)
$(function () {
    $('#createModal').modal('show');
    $('#cancelButton, #closeButton').on('click', function () {
        closeCreateModal();
    });

    $(document).on('keyup', function (e) {
        if (e.key === "Escape") {
            closeCreateModal();
        }
    });

    $(document).on('mouseup', function (e) {
        var modal = $('#createModal');
        if (!modal.is(e.target) && modal.has(e.target).length === 0) {
            closeCreateModal();
        }
    });

    function closeCreateModal() {
        $('#createModal').modal('hide');
        window.location.href = "/Home/Index";
    }

    $('#createModal').on('hidden.bs.modal', function () {
        closeCreateModal();
    });
});
