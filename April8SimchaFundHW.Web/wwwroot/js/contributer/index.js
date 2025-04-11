$(() => {
    const modal = new bootstrap.Modal($(".new-contrib")[0]);
    const depositModal = new bootstrap.Modal($(".deposit")[0]);
    const editModal = new bootstrap.Modal($(".edit")[0]);

    $("#new-contributor").on('click', function(){
        modal.show();
    })
    $(".table-responsive").on('click', '.deposit-button', function () {
        
        depositModal.show();

        const button = $(this);
        const id = button.attr('value')

        document.getElementById('contributorid').value = id;

            
    })
    $(".table-responsive").on('click', '.edit-contrib', function () {
        editModal.show();

        const button = $(this);
        const firstName = button.attr('data-first-name')
        const lastName = button.attr('data-last-name')
        const cell = button.attr('data-cell')
        const id = button.attr('data-id')
        const alwayseInclude = button.attr('data-always-include')
        document.getElementById('econtributor_first_name').value = firstName;
        document.getElementById('econtributor_last_name').value = lastName;
        document.getElementById('econtributor_cell_number').value = cell;
        document.getElementById('contributor-id').value = id;

       
        $('.form-check-input').prop('checked', alwayseInclude !== 'False')
       
        

    })
   
   
})