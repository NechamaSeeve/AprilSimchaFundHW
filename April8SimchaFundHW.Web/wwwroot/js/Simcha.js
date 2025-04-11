$(() => {
    const modal = new bootstrap.Modal($(".modal")[0]);
    $("#new-simcha").on('click', function () {
        modal.show();
    })

    
})